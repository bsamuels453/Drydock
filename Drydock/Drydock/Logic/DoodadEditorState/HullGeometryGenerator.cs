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
    class HullGeometryGenerator : CanReceiveInputEvents {
        const float _metersPerDeck = 2.13f;
        const int _numHorizontalPrimitives = 16;//welp
        const int _primitiveHeightPerDeck = 2;
        readonly ShipGeometryBuffer _displayBuffer;

        readonly int[] _indicies;
        readonly VertexPositionNormalTexture[] _verticies;
        float _cameraDistance;
        float _cameraPhi;
        float _cameraTheta;
         //note: less than 1 deck breaks prolly
        public HullGeometryGenerator(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo) {
            _cameraPhi = 0.32f;
            _cameraTheta = 0.63f;
            _cameraDistance = 100;
            const float metersPerPrimitive = _metersPerDeck / _primitiveHeightPerDeck;
            InputEventDispatcher.EventSubscribers.Add(1.0f, this);
            var sidePtGen = new BruteBezierGenerator(sideCurveInfo);

            topCurveInfo.RemoveAt(0);//make this curve set pass vertical line test
            backCurveInfo.RemoveAt(0);//make this curve pass the horizontal line test
            var topPtGen = new BruteBezierGenerator(topCurveInfo);
            var geometryYvalues = new List<float>();//this list contains all the valid Y values for the airship's primitives
            var xzHullIntercepts = new List<List<Vector2>>();

            //get the draft and the berth
            float draft = sideCurveInfo[1].Pos.Y;
            //float berth = (topCurveInfo[1].Pos.Y - topCurveInfo[0].Pos.Y) * 2;
            float berth = topCurveInfo[1].Pos.Y;
            float length = sideCurveInfo[2].Pos.X;
            var numDecks = (int)(draft / _metersPerDeck);
            int numVerticalPrimitives = numDecks * _primitiveHeightPerDeck + _primitiveHeightPerDeck;

            //get the y values for the hull
            for (int i = 0; i < numVerticalPrimitives - _primitiveHeightPerDeck; i++) {
                geometryYvalues.Add(i * metersPerPrimitive);
            }
            float bottomDeck = geometryYvalues[geometryYvalues.Count - 1];

            //the bottom part of ship (false deck) will not have height of _metersPerDeck so we need to use a different value for metersPerPrimitive
            float bottomPrimHeight = (draft - bottomDeck) / _primitiveHeightPerDeck;
            for (int i = 0; i < _primitiveHeightPerDeck; i++) {
                geometryYvalues.Add(i * bottomPrimHeight + bottomDeck);
            }

            foreach (float t in geometryYvalues){
                xzHullIntercepts.Add(sidePtGen.GetValuesFromDependent(t));
                if (xzHullIntercepts[xzHullIntercepts.Count - 1].Count != 2){
                    throw new Exception("more/less than two independent solutions found for a given dependent value");
                }
            }

            var ySliceVerts = new List<List<Vector3>>(); //this list contains slices of the airship which contain all the vertexes for the specific layer of the airship

            //in the future we can parameterize x differently to comphensate for dramatic curves on the keel
            for (int y = 0; y < numVerticalPrimitives; y++){
                float xStart = xzHullIntercepts[y][0].X;
                float xEnd = xzHullIntercepts[y][1].X;
                float xDiff = xEnd - xStart;

                var strip = new List<Vector3>(); 

                for (int x = 0; x < _numHorizontalPrimitives; x++){
                    var point = new Vector3();
                    point.Y = geometryYvalues[y];

                    float tx = x / (float)(_numHorizontalPrimitives-1);
                    float xPos = tx * xDiff + xStart;

                    var keelIntersect = sidePtGen.GetValueFromIndependent(xPos);
                    float profileYScale = keelIntersect.Y / draft;
                    point.X = keelIntersect.X;

                    var topIntersect = topPtGen.GetValueFromIndependent(xPos);
                    float profileXScale = topIntersect.X / berth;

                    var scaledProfile = new List<BezierInfo>();

                    foreach (BezierInfo t in backCurveInfo){
                        scaledProfile.Add(t.CreateScaledCopy(profileXScale, profileYScale));
                    }
                    if (x == _numHorizontalPrimitives - 1){
                        int dg = 5;
                    }


                    var pointGen = new BruteBezierGenerator(scaledProfile);
                    var profileIntersect = pointGen.GetValuesFromDependent(point.Y);
                    if (profileIntersect.Count != 1){
                        throw new Exception("curve does not pass the horizontal line test");
                    }
                    point.Z = profileIntersect[0].X -scaledProfile[0].Pos.X;

                    int f = 5;
                    strip.Add(point);
                   // keelIntersect.Y = geometryYvalues[y];

                }
                ySliceVerts.Add(strip);
            }
            ySliceVerts[0][0] = new Vector3(ySliceVerts[0][0].X, ySliceVerts[0][0].Y, 0); //remove mystery NaN

            //treat the slices like a mesh
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
            Renderer.CameraTarget = p;
            //Renderer.CameraTarget = new Vector3(0, 0, 0);
            Renderer.CameraPosition.X = (float)(_cameraDistance * Math.Cos(_cameraPhi) * Math.Sin(_cameraTheta)) + Renderer.CameraTarget.X;
            Renderer.CameraPosition.Z = (float)(_cameraDistance * Math.Cos(_cameraPhi) * Math.Cos(_cameraTheta)) + Renderer.CameraTarget.Z;
            Renderer.CameraPosition.Y = (float)(_cameraDistance * Math.Sin(_cameraPhi)) + Renderer.CameraTarget.Y;

        }
        
        public override InterruptState OnMouseMovement(MouseState state, MouseState? prevState = null) {
            if (prevState != null) {

                if (state.LeftButton == ButtonState.Pressed){
                    int dx = state.X - ((MouseState) prevState).X;
                    int dy = state.Y - ((MouseState) prevState).Y;

                    if (state.LeftButton == ButtonState.Pressed){
                        _cameraPhi += dy*0.01f;
                        _cameraTheta -= dx*0.01f;

                        if (_cameraPhi > 1.56f){
                            _cameraPhi = 1.56f;
                        }
                        if (_cameraPhi < -1.56f){
                            _cameraPhi = -1.56f;
                        }
                        Renderer.CameraPosition.X = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Sin(_cameraTheta)) + Renderer.CameraTarget.X;
                        Renderer.CameraPosition.Z = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Cos(_cameraTheta)) + Renderer.CameraTarget.Z;
                        Renderer.CameraPosition.Y = (float) (_cameraDistance*Math.Sin(_cameraPhi)) + Renderer.CameraTarget.Y;
                    }


                    return InterruptState.InterruptEventDispatch;

                    /*if (state.RightButton == ButtonState.Pressed) {
                        int dx = state.X - ((MouseState)prevState).X;
                        int dy = state.Y - ((MouseState)prevState).Y;

                        _cameraPhi += dy * 0.01f;
                        _cameraTheta += dx * 0.01f;

                        if (_cameraPhi > 1.56f) {
                            _cameraPhi = 1.56f;
                        }
                        if (_cameraPhi < -1.56f) {
                            _cameraPhi = -1.56f;
                        }

                        Renderer.CameraTarget.X = (float)(_cameraDistance * Math.Cos(_cameraPhi + Math.PI) * Math.Sin(_cameraTheta + Math.PI)) - Renderer.CameraPosition.X;
                        Renderer.CameraTarget.Z = (float)(_cameraDistance * Math.Cos(_cameraPhi + Math.PI) * Math.Cos(_cameraTheta + Math.PI)) - Renderer.CameraPosition.Z;
                        Renderer.CameraTarget.Y = (float)(_cameraDistance * Math.Sin(_cameraPhi + Math.PI)) + Renderer.CameraPosition.Y;
                        return InterruptState.InterruptEventDispatch;
                    }*/
        
                    return InterruptState.InterruptEventDispatch;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public override InterruptState OnMouseScroll(MouseState state, MouseState? prevState = null) {
            if (prevState != null) {
                _cameraDistance += (((MouseState)prevState).ScrollWheelValue - state.ScrollWheelValue) / 20f;
                if (_cameraDistance < 5) {
                    _cameraDistance = 5;
                }
                Renderer.CameraPosition.X = (float)(_cameraDistance * Math.Cos(_cameraPhi) * Math.Sin(_cameraTheta)) + Renderer.CameraTarget.X;
                Renderer.CameraPosition.Z = (float)(_cameraDistance * Math.Cos(_cameraPhi) * Math.Cos(_cameraTheta)) + Renderer.CameraTarget.Z;
                Renderer.CameraPosition.Y = (float)(_cameraDistance * Math.Sin(_cameraPhi)) + Renderer.CameraTarget.Y;
            }
            return InterruptState.AllowOtherEvents;
        }
        
    }

}
