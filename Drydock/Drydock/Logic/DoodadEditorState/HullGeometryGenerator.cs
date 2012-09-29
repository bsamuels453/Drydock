#region

using System;
using System.Collections.Generic;
using System.Linq;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    /// <summary>
    /// Generates the geometry for airship hulls. This differs from PreviewRenderer in that this class generates the geometry so that things like windows, portholes, 
    /// or other extremities can be added easily without modifying/removing much of the geometry. In more mathematical terms, it means that the horizontal boundaries between 
    /// adjacent quads are parallel to the XZ plane. This class can also take a few seconds to do its thing because it isnt going to be updating every tick like 
    /// previewrenderer does.
    /// 
    /// This code honestly shoudn't be in a class, but it's really too much to piggyback onto another class and would become much harder to read and maintain.
    /// Since this is essentially just a helper class, the process it performs has been broken down into a set of private functions that should help better describe
    /// what the hell it is doing. 
    /// 
    /// With that in mind, it can be assumed that this class basically acts like one big pure function that runs when it is constructed.
    /// </summary>
    internal class HullGeometryGenerator{
        const float _metersPerDeck = 2.13f;
        const int _numHorizontalPrimitives = 32; //welp
        readonly int _primHeightPerDeck;

        //todo: clean up all these fields, they should be passing between methods, not left here like global garbage
        List<Vector3[,]> _deckFloorMesh;
        Vector3[,] _totalNormals;
        List<List<List<Vector3>>> _deckVertexes;// deck->levels of deck vertexes->vertexes for each level 
        List<List<Vector3>> _layerVerts;
        int _numDecks;

        public HullGeometryInfo Resultant;

        //note: less than 1 deck breaks prolly
        //note that this entire geometry generator runs on the standard curve assumptions
        public HullGeometryGenerator(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo, int primHeightPerDeck){
            _primHeightPerDeck = primHeightPerDeck;
            GenerateHull(backCurveInfo, sideCurveInfo, topCurveInfo);
            GenerateDecks();
            GenerateDeckWallBuffers();
            GenerateDeckFloorBuffers();
        }

        //todo: break up this method into submethods for the sake of cleanliness.
        void GenerateHull(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo){
            float metersPerPrimitive = _metersPerDeck/_primHeightPerDeck;
            var sidePtGen = new BruteBezierGenerator(sideCurveInfo);

            topCurveInfo.RemoveAt(0); //make this curve set pass vertical line test
            backCurveInfo.RemoveAt(0); //make this curve pass the horizontal line test
            var topPtGen = new BruteBezierGenerator(topCurveInfo);
            var geometryYvalues = new List<float>(); //this list contains all the valid Y values for the airship's primitives
            var xzHullIntercepts = new List<List<Vector2>>();

            //get the draft and the berth
            float draft = sideCurveInfo[1].Pos.Y;
            float berth = topCurveInfo[1].Pos.Y;
            float length = sideCurveInfo[2].Pos.X;
            _numDecks = (int) (draft/_metersPerDeck);
            Resultant.NumDecks = _numDecks;
            int numVerticalVertexes = _numDecks*_primHeightPerDeck + _primHeightPerDeck + 1;

            //get the y values for the hull
            for (int i = 0; i < numVerticalVertexes - _primHeightPerDeck; i++){
                geometryYvalues.Add(i*metersPerPrimitive);
            }
            float bottomDeck = geometryYvalues[geometryYvalues.Count - 1];

            //the bottom part of ship (false deck) will not have height of _metersPerDeck so we need to use a different value for metersPerPrimitive
            float bottomPrimHeight = (draft - bottomDeck)/_primHeightPerDeck;
            for (int i = 1; i <= _primHeightPerDeck; i++){
                geometryYvalues.Add(i*bottomPrimHeight + bottomDeck);
            }

            foreach (float t in geometryYvalues){
                xzHullIntercepts.Add(sidePtGen.GetValuesFromDependent(t));
                if (xzHullIntercepts[xzHullIntercepts.Count - 1].Count != 2){
                    if (xzHullIntercepts[xzHullIntercepts.Count - 1].Count == 1){ //this happens at the very bottom of the ship
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
                    float tx = x/(float) (_numHorizontalPrimitives - 1);
                    float xPos = tx*xDiff + xStart;
                    //

                    var keelIntersect = sidePtGen.GetValueFromIndependent(xPos);
                    float profileYScale = keelIntersect.Y/draft;
                    point.X = keelIntersect.X;

                    var topIntersect = topPtGen.GetValueFromIndependent(xPos);
                    float profileXScale = (topIntersect.Y - topCurveInfo[0].Pos.Y)/(berth/2f);
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
                    if (x == _numHorizontalPrimitives - 1 || x == 0){
                        diff = profileIntersect[0].X;
                    }
                    point.Z = profileIntersect[0].X - diff;

                    if (y == numVerticalVertexes - 1){
                        point.Z = 0;
                    }

                    strip.Add(point);
                }
                ySliceVerts.Add(strip);
            }

            foreach (List<Vector3> t in ySliceVerts){ //mystery NaN detecter
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

            var geometry = ySliceVerts;

            //reflect+dupe the geometry across the x axis to complete the opposite side of the ship
            _layerVerts = new List<List<Vector3>>(geometry.Count);
            for (int i = 0; i < geometry.Count; i++){
                _layerVerts.Add(new List<Vector3>(geometry[0].Count*2));

                geometry[i].Reverse();
                for (int si = 0; si < geometry[0].Count; si++){
                    _layerVerts[i].Add(geometry[i][si]);
                }
                geometry[i].Reverse();
                for (int si = 0; si < geometry[0].Count; si++){
                    _layerVerts[i].Add(new Vector3(geometry[i][si].X, geometry[i][si].Y, -geometry[i][si].Z));
                }
            }

            //reflect the layers across the Y axis so that the opening is pointing up
            foreach (var layerVert in _layerVerts){
                for (int i = 0; i < layerVert.Count; i++){
                    layerVert[i] = new Vector3(layerVert[i].X, -layerVert[i].Y, layerVert[i].Z);
                }
            }

            //this fixes the ordering of the lists so that normals generate correctly
            foreach (List<Vector3> t in _layerVerts){
                t.Reverse();
            }

            //now enumerate the ships layer verts into levels for each deck
            _deckVertexes = new List<List<List<Vector3>>>(_numDecks + 1);
            for (int i = 0; i < _numDecks; i++){
                _deckVertexes.Add(new List<List<Vector3>>(_primHeightPerDeck));

                for (int level = 0; level < _primHeightPerDeck + 1; level++){
                    _deckVertexes[i].Add(_layerVerts[i*(_primHeightPerDeck) + level]);
                }
            }
            //edge case for the final deck, the "bottom"
            _deckVertexes.Add(new List<List<Vector3>>());
            for (int level = _numDecks*_primHeightPerDeck; level < _layerVerts.Count; level++){
                _deckVertexes[_deckVertexes.Count - 1].Add(_layerVerts[level]);
            }
            GenerateHullNormals();
        }

        void GenerateHullNormals(){
            //generate a normals array for the entire ship, rather than per-deck
            var totalMesh = new Vector3[_layerVerts.Count, _layerVerts[0].Count];
            _totalNormals = new Vector3[_layerVerts.Count, _layerVerts[0].Count];
            MeshHelper.Encode2DListIntoArray(_layerVerts.Count, _layerVerts[0].Count, ref totalMesh, _layerVerts);
            MeshHelper.GenerateMeshNormals(totalMesh, ref _totalNormals);

            for (int x = 0; x < _layerVerts.Count; x++){
                for (int y = 0; y < _layerVerts[0].Count; y++){
                    _totalNormals[x, y].Y *= -1;
                    //_totalNormals[x, y] = Vector3.Up;
                }
            }


            //since we have generated totalmesh, might as well get the centerpoint now
            GenerateCenterPoint(totalMesh);
        }

        void GenerateDecks(){
            _deckFloorMesh = new List<Vector3[,]>();
            for (int deck = 0; deck < _numDecks+1; deck++){
                _deckFloorMesh.Add(new Vector3[3,_layerVerts[0].Count/2]);
                for (int vert = 0; vert < _layerVerts[0].Count/2; vert++){
                    _deckFloorMesh.Last()[0, vert] = _layerVerts[deck*_primHeightPerDeck][_layerVerts[0].Count/2 + vert];

                    _deckFloorMesh.Last()[1, vert] = _layerVerts[deck*_primHeightPerDeck][_layerVerts[0].Count/2 + vert];
                    _deckFloorMesh.Last()[2, vert] = _layerVerts[deck*_primHeightPerDeck][_layerVerts[0].Count/2 - vert - 1];
                    _deckFloorMesh.Last()[1, vert].Z = 0;

                    _deckFloorMesh.Last()[2, vert] = _layerVerts[deck*_primHeightPerDeck][_layerVerts[0].Count/2 - vert - 1];
                }
            }
        }

        void GenerateDeckWallBuffers(){
            var hullBuffers = new ShipGeometryBuffer[_numDecks + 1];
            //now set up the display buffer for each deck wall, also known as the ships hull
            for (int i = 0; i < _deckVertexes.Count; i++) {
                var hullMesh = new Vector3[_primHeightPerDeck + 1, _deckVertexes[0][0].Count];
                var hullNormals = new Vector3[_primHeightPerDeck + 1, _deckVertexes[0][0].Count];
                int[] hullIndicies = MeshHelper.CreateIndiceArray((_primHeightPerDeck + 1) * _deckVertexes[0][0].Count);
                VertexPositionNormalTexture[] hullVerticies = MeshHelper.CreateTexcoordedVertexList((_primHeightPerDeck + 1) * _deckVertexes[0][0].Count);

                //get the hull normals for this part of the hull from the total normals
                for (int x = 0; x < _primHeightPerDeck + 1; x++) {
                    for (int z = 0; z < _deckVertexes[0][0].Count; z++) {
                        hullNormals[x, z] = _totalNormals[i * _primHeightPerDeck + x, z];
                    }
                }
                //convert the 2d list heightmap into a 2d array heightmap
                MeshHelper.Encode2DListIntoArray(_primHeightPerDeck + 1, _deckVertexes[0][0].Count, ref hullMesh, _deckVertexes[i]);
                //take the 2d array of vertexes and 2d array of normals and stick them in the vertexpositionnormaltexture 
                MeshHelper.ConvertMeshToVertList(hullMesh, hullNormals, ref hullVerticies);

                //now stick it in a buffer
                hullBuffers[i] = new ShipGeometryBuffer(hullIndicies.Length, hullVerticies.Length, hullIndicies.Length / 3, "whiteborder", CullMode.CullClockwiseFace);
                hullBuffers[i].Indexbuffer.SetData(hullIndicies);
                hullBuffers[i].Vertexbuffer.SetData(hullVerticies);
            }
            Resultant.DeckWallBuffers = hullBuffers;
        }

        void GenerateDeckFloorBuffers(){
            var floorNormals = new Vector3[3, _layerVerts[0].Count / 2];
            for (int x = 0; x < 3; x++) {
                for (int z = 0; z < _layerVerts[0].Count / 2; z++) {
                    floorNormals[x, z] = Vector3.Up;
                }
            }

            var deckFloorbuffers = new ShipGeometryBuffer[_deckFloorMesh.Count];

            //now set up the display buffer for each deck floor
            for (int i = 0; i < _deckFloorMesh.Count; i++) {
                VertexPositionNormalTexture[] floorVerticies = MeshHelper.CreateTexcoordedVertexList(4*_layerVerts[0].Count/2);
                int[] foorIndicies = MeshHelper.CreateIndiceArray(4*_layerVerts[0].Count/2);

                MeshHelper.ConvertMeshToVertList(_deckFloorMesh[i], floorNormals, ref floorVerticies);

                deckFloorbuffers[i] = new ShipGeometryBuffer(foorIndicies.Length, floorVerticies.Length, foorIndicies.Length / 3, "whiteborder");
                deckFloorbuffers[i].Indexbuffer.SetData(foorIndicies);
                deckFloorbuffers[i].Vertexbuffer.SetData(floorVerticies);
            }

            Resultant.DeckFloorBuffers = deckFloorbuffers;
        }

        void GenerateCenterPoint(Vector3[,] totalMesh){
            var p = new Vector3();

            p += totalMesh[0, 0];
            p += totalMesh[totalMesh.GetLength(0) - 1, totalMesh.GetLength(1) - 1];
            p /= 4;

            Resultant.CenterPoint = p;
        }
    }

    //this is only used for data transfer between this class and hullgeometryhandler
    internal struct HullGeometryInfo{
        public ShipGeometryBuffer[] DeckWallBuffers;
        public ShipGeometryBuffer[] DeckFloorBuffers;
        public Vector3 CenterPoint;
        public int NumDecks;
    }
}