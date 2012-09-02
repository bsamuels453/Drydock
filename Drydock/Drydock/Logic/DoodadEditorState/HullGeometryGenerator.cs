using System;
using System.Collections.Generic;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Logic.DoodadEditorState {
    /// <summary>
    /// Generates the geometry for airship hulls. This differs from PreviewRenderer in that this class generates the geometry so that things like windows, 
    /// portholes, or other extremities can be added easily without modifying a lot of the geometry. In more mathematical terms, it means that the horizontal 
    /// boundaries between adjacent quads are parallel to the XZ plane. This class can also take a few seconds to do its thing because it isnt going to be updating
    /// every tick like previewrenderer does.
    /// </summary>
    class HullGeometryGenerator {
        const float _metersPerDeck = 2.13f;
        const int _numHorizontalPrimitives = 16;//welp
        const int _primitiveHeightPerDeck = 2;
        readonly ShipGeometryBuffer _displayBuffer;
        readonly int[] _indicies;
        readonly Vector3[,] _mesh;
        readonly VertexPositionNormalTexture[] _verticies;
         //note: less than 1 deck breaks prolly
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
            //float berth = (topCurveInfo[1].Pos.Y - topCurveInfo[0].Pos.Y) * 2;
            float berth = topCurveInfo[1].Pos.Y;
            float length = sideCurveInfo[2].Pos.X;
            var numDecks = (int)(draft / _metersPerDeck);
            int numVerticalPrimitives = numDecks * _primitiveHeightPerDeck + _primitiveHeightPerDeck;

            //get the y values for the hull
            for (int i = 0; i < numVerticalPrimitives - _primitiveHeightPerDeck; i++) {
                geometryYvalues.Add(i * metersPerPrimitive);
            }

            //the bottom part of ship (false deck) will not have height of _metersPerDeck so we need to use a different value for metersPerPrimitive
            float bottomPrimHeight = (draft - numDecks * _metersPerDeck) / _primitiveHeightPerDeck;
            for (int i = numVerticalPrimitives - _primitiveHeightPerDeck; i < numVerticalPrimitives; i++){
                 geometryYvalues.Add(i * bottomPrimHeight);
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
                    point.Z = profileIntersect[0].X - scaledProfile[0].Pos.X;

                    int f = 5;
                    strip.Add(point);
                   // keelIntersect.Y = geometryYvalues[y];

                }
                ySliceVerts.Add(strip);
            }


            //treat the slices like a mesh

            int g = 5;

        }
    }
}
