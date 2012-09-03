using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic.DoodadEditorState {
    /// <summary>
    /// Generates the geometry for airship hulls. This differs from PreviewRenderer in that this class generates the geometry so that things like windows, 
    /// portholes, or other extremities can be added easily without modifying a lot of the geometry. In more mathematical terms, it means that the horizontal 
    /// boundaries between adjacent quads are parallel to the XZ plane. This class can also take a few seconds to do its thing because it isnt going to be updating
    /// every tick like previewrenderer does.
    /// </summary>
    class HullGeometryGenerator : ATargetingCamera, IDisposable {
        const float _metersPerDeck = 2.13f;
        const int _numHorizontalPrimitives = 64;//welp
        const int _primitiveHeightPerDeck = 3;
        readonly ShipGeometryBuffer _displayBuffer;

        readonly int[] _indicies;
        readonly VertexPositionNormalTexture[] _verticies;

        //note: less than 1 deck breaks prolly
        //note that this entire geometry generator runs on the standard curve assumptions
        public HullGeometryGenerator(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo) {
            const float metersPerPrimitive = _metersPerDeck / _primitiveHeightPerDeck;
            var sidePtGen = new BruteBezierGenerator(sideCurveInfo);

            topCurveInfo.RemoveAt(0);//make this curve set pass vertical line test
            backCurveInfo.RemoveAt(0);//make this curve pass the horizontal line test
            var topPtGen = new BruteBezierGenerator(topCurveInfo);
            var geometryYvalues = new List<float>();//this list contains all the valid Y values for the airship's primitives
            var xzHullIntercepts = new List<List<Vector2>>();

            //get the draft and the berth
            float draft = sideCurveInfo[1].Pos.Y;
            float berth = topCurveInfo[1].Pos.Y;
            float length = sideCurveInfo[2].Pos.X;
            var numDecks = (int)(draft / _metersPerDeck);
            int numVerticalVertexes = numDecks * _primitiveHeightPerDeck + _primitiveHeightPerDeck+1;

            //get the y values for the hull
            for (int i = 0; i < numVerticalVertexes - _primitiveHeightPerDeck; i++) {
                geometryYvalues.Add(i * metersPerPrimitive);
            }
            float bottomDeck = geometryYvalues[geometryYvalues.Count - 1];

            //the bottom part of ship (false deck) will not have height of _metersPerDeck so we need to use a different value for metersPerPrimitive
            float bottomPrimHeight = (draft - bottomDeck) / _primitiveHeightPerDeck;
            for (int i = 1; i <= _primitiveHeightPerDeck; i++) {
                geometryYvalues.Add(i * bottomPrimHeight + bottomDeck);
            }

            foreach (float t in geometryYvalues){
                xzHullIntercepts.Add(sidePtGen.GetValuesFromDependent(t));
                if (xzHullIntercepts[xzHullIntercepts.Count - 1].Count != 2){
                    if (xzHullIntercepts[xzHullIntercepts.Count - 1].Count == 1) {//this happens at the very bottom of the ship
                        xzHullIntercepts[xzHullIntercepts.Count - 1].Add(xzHullIntercepts[xzHullIntercepts.Count - 1][0]);
                    }
                    else{
                        throw new Exception("more/less than two independent solutions found for a given dependent value");
                    }
                }
                
            }

            var ySliceVerts = new List<List<Vector3>>(); //this list contains slices of the airship which contain all the vertexes for the specific layer of the airship

            //in the future we can parameterize x differently to comphensate for dramatic curves on the keel
            for (int y = 0; y < numVerticalVertexes; y++){
                float xStart = xzHullIntercepts[y][0].X;
                float xEnd = xzHullIntercepts[y][1].X;
                float xDiff = xEnd - xStart;

                var strip = new List<Vector3>(); 

                for (int x = 0; x < _numHorizontalPrimitives; x++){


                    var point = new Vector3();
                    point.Y = geometryYvalues[y];

                    //here is where x is parameterized, and converted into a relative x value
                    float tx = x / (float)(_numHorizontalPrimitives-1);
                    float xPos = tx * xDiff + xStart;
                    //
                    
                    var keelIntersect = sidePtGen.GetValueFromIndependent(xPos);
                    float profileYScale = keelIntersect.Y / draft;
                    point.X = keelIntersect.X;

                    var topIntersect = topPtGen.GetValueFromIndependent(xPos);
                    float profileXScale = (topIntersect.Y-topCurveInfo[0].Pos.Y) / (berth/2f);
                    //float profileXScale = topIntersect.Y  / berth;

                    var scaledProfile = new List<BezierInfo>();

                    foreach (BezierInfo t in backCurveInfo){
                        scaledProfile.Add(t.CreateScaledCopy(profileXScale, profileYScale));
                    }



                    var pointGen = new BruteBezierGenerator(scaledProfile);
                    var profileIntersect = pointGen.GetValuesFromDependent(point.Y);
                    if (profileIntersect.Count != 1){
                        throw new Exception("curve does not pass the horizontal line test");
                    }

                    float diff = scaledProfile[0].Pos.X;
                    if (x == _numHorizontalPrimitives - 1) {
                        diff = profileIntersect[0].X;
                    }
                    point.Z = profileIntersect[0].X - diff;
                    strip.Add(point);

                }
                ySliceVerts.Add(strip);
            }

            foreach (List<Vector3> t in ySliceVerts){//mystery NaN detecter
                if (float.IsNaN(t[0].Z)){
                    throw new Exception("NaN Z coordinate in mesh");
                }
            }

            /*foreach (List<Vector3> t in ySliceVerts){//remove mystery NaNs
                for(int i=0; i<t.Count; i++){
                    if (float.IsNaN(t[i].Z)){
                        t[i] = new Vector3(t[i].X, t[i].Y, 0);
                    }
                }
            }*/

            //create mesh
            var mesh = new Vector3[ySliceVerts.Count, _numHorizontalPrimitives];
            var normals = new Vector3[ySliceVerts.Count, _numHorizontalPrimitives];

            MeshHelper.Encode2DListIntoMesh(ySliceVerts.Count, _numHorizontalPrimitives, ref mesh, ySliceVerts);

            _indicies = MeshHelper.CreateIndiceArray(ySliceVerts.Count * _numHorizontalPrimitives);
            _verticies = MeshHelper.CreateTexcoordedVertexList(ySliceVerts.Count * _numHorizontalPrimitives);

            MeshHelper.GenerateMeshNormals(mesh, ref normals);
            MeshHelper.ConvertMeshToVertList(mesh, normals, ref _verticies);

            _displayBuffer = new ShipGeometryBuffer(_indicies.Count(), _verticies.Count(), _verticies.Count()/2, "whiteborder");
            _displayBuffer.Indexbuffer.SetData(_indicies);
            _displayBuffer.Vertexbuffer.SetData(_verticies);

            var p = new Vector3();
            p += -mesh[0, 0];
            p += -mesh[ySliceVerts.Count - 1, 0];
            p += -mesh[0, _numHorizontalPrimitives - 1];
            p += -mesh[ySliceVerts.Count - 1, _numHorizontalPrimitives - 1];
            p /= 4;
            SetCameraTarget(p);
        }

        public void Dispose(){
        }

    }
}
