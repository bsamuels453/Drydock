#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    /// <summary>
    ///   Generates the geometry for airship hulls. This differs from PreviewRenderer in
    /// that this class generates the geometry so that things like windows, portholes, or
    /// other extremities can be added easily without modifying/removing much of the geometry.
    /// In more mathematical terms, it means that the horizontal boundaries between adjacent
    /// quads are parallel to the XZ plane. This class can also take a few seconds to do its
    /// thing because it isnt going to be updating every tick like previewrenderer does.
    /// </summary>
    internal static class HullGeometryGenerator{
        //note: less than 1 deck breaks prolly
        //note that this entire geometry generator runs on the standard curve assumptions
        public static HullGeometryInfo GenerateShip(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo, int primHeightPerDeck){
            const float deckHeight = 2.13f;
            const float bBoxWidth = 0.5f;
            var genResults = GenerateHull(new GenerateHullParams{
                BackCurveInfo = backCurveInfo,
                SideCurveInfo = sideCurveInfo,
                TopCurveInfo = topCurveInfo,
                DeckHeight = deckHeight,
                PrimitivesPerDeck = primHeightPerDeck
            }
                );
            var normalGenResults = GenerateHullNormals(genResults.LayerSilhouetteVerts);
            var deckFloorMesh = GenerateDecks(genResults.LayerSilhouetteVerts, genResults.NumDecks, primHeightPerDeck);
            var hullBuffers = GenerateDeckWallBuffers(genResults.DeckSilhouetteVerts, normalGenResults.NormalMesh, genResults.NumDecks, primHeightPerDeck);
            var deckFloorBuffers = GenerateDeckFloorBuffers(genResults.LayerSilhouetteVerts, deckFloorMesh);
            var boundingBoxResults = GenerateDeckBoundingBoxes(bBoxWidth, deckFloorMesh);

            var resultant = new HullGeometryInfo();
            resultant.CenterPoint = normalGenResults.Centroid;
            resultant.DeckFloorBoundingBoxes = boundingBoxResults.DeckBoundingBoxes;
            resultant.DeckFloorBuffers = deckFloorBuffers;
            resultant.FloorVertexes = boundingBoxResults.DeckVertexes;
            resultant.HullWallTexBuffers = hullBuffers;
            resultant.NumDecks = genResults.NumDecks;
            resultant.WallResolution = bBoxWidth;
            resultant.DeckHeight = deckHeight;
            resultant.MaxBoundingBoxDims = new Vector2((int) (genResults.Length/bBoxWidth), (int) (genResults.Berth/bBoxWidth));
            return resultant;
        }

        //todo: break up this method into submethods for the sake of cleanliness.

        static GenerateHullResults GenerateHull(GenerateHullParams input){
            var sideCurveInfo = input.SideCurveInfo;
            var backCurveInfo = input.BackCurveInfo;
            var topCurveInfo = input.TopCurveInfo;
            float deckHeight = input.DeckHeight;
            int primitivesPerDeck = input.PrimitivesPerDeck;
            var results = new GenerateHullResults();

            int numHorizontalPrimitives = 64;

            float metersPerPrimitive = deckHeight/primitivesPerDeck;
            var sidePtGen = new BruteBezierGenerator(sideCurveInfo);

            topCurveInfo.RemoveAt(0); //make this curve set pass vertical line test
            backCurveInfo.RemoveAt(0); //make this curve pass the horizontal line test
            var topPtGen = new BruteBezierGenerator(topCurveInfo);
            var geometryYvalues = new List<float>(); //this list contains all the valid Y values for the airship's primitives
            var xzHullIntercepts = new List<List<Vector2>>();

            //get the draft and the berth
            float draft = sideCurveInfo[1].Pos.Y;
            results.Berth = topCurveInfo[1].Pos.Y;
            results.Length = sideCurveInfo[2].Pos.X;
            results.NumDecks = (int) (draft/deckHeight);
            int numVerticalVertexes = results.NumDecks*primitivesPerDeck + primitivesPerDeck + 1;

            //get the y values for the hull
            for (int i = 0; i < numVerticalVertexes - primitivesPerDeck; i++){
                geometryYvalues.Add(i*metersPerPrimitive);
            }
            float bottomDeck = geometryYvalues[geometryYvalues.Count - 1];

            //the bottom part of ship (false deck) will not have height of _metersPerDeck so we need to use a different value for metersPerPrimitive
            float bottomPrimHeight = (draft - bottomDeck)/primitivesPerDeck;
            for (int i = 1; i <= primitivesPerDeck; i++){
                geometryYvalues.Add(i*bottomPrimHeight + bottomDeck);
            }

            foreach (float t in geometryYvalues){
                xzHullIntercepts.Add(sidePtGen.GetValuesFromDependent(t));
                if (xzHullIntercepts[xzHullIntercepts.Count - 1].Count != 2){
                    if (xzHullIntercepts[xzHullIntercepts.Count - 1].Count == 1){ //this happens at the very bottom of the ship
                        xzHullIntercepts[xzHullIntercepts.Count - 1].Add(xzHullIntercepts[xzHullIntercepts.Count - 1][0]);
                    }
                    else
                        throw new Exception("more/less than two independent solutions found for a given dependent value");
                }
            }

            var ySliceVerts = new List<List<Vector3>>(); //this list contains slices of the airship which contain all the vertexes for the specific layer of the airship


            //in the future we can parameterize x differently to comphensate for dramatic curves on the keel
            for (int y = 0; y < numVerticalVertexes; y++){
                float xStart = xzHullIntercepts[y][0].X;
                float xEnd = xzHullIntercepts[y][1].X;
                float xDiff = xEnd - xStart;

                var strip = new List<Vector3>();

                for (int x = 0; x < numHorizontalPrimitives; x++){
                    var point = new Vector3();
                    point.Y = geometryYvalues[y];

                    //here is where x is parameterized, and converted into a relative x value
                    float tx = x/(float) (numHorizontalPrimitives - 1);
                    float xPos = tx*xDiff + xStart;
                    //

                    var keelIntersect = sidePtGen.GetValueFromIndependent(xPos);
                    float profileYScale = keelIntersect.Y/draft;
                    point.X = keelIntersect.X;

                    var topIntersect = topPtGen.GetValueFromIndependent(xPos);
                    float profileXScale = (topIntersect.Y - topCurveInfo[0].Pos.Y)/(results.Berth/2f);
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
                    if (x == numHorizontalPrimitives - 1 || x == 0){
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
            results.LayerSilhouetteVerts = new Vector3[geometry.Count][];
            for (int i = 0; i < geometry.Count; i++){
                results.LayerSilhouetteVerts[i] = new Vector3[geometry[0].Count*2];

                geometry[i].Reverse();
                int destIdx = 0;
                for (int si = 0; si < geometry[0].Count; si++){
                    results.LayerSilhouetteVerts[i][destIdx] = geometry[i][si];
                    destIdx++;
                }
                geometry[i].Reverse();
                for (int si = 0; si < geometry[0].Count; si++){
                    results.LayerSilhouetteVerts[i][destIdx] = new Vector3(geometry[i][si].X, geometry[i][si].Y, -geometry[i][si].Z);
                    destIdx++;
                }
            }

            //reflect the layers across the Y axis so that the opening is pointing up
            foreach (var layerVert in results.LayerSilhouetteVerts){
                for (int i = 0; i < layerVert.GetLength(0); i++){
                    layerVert[i] = new Vector3(layerVert[i].X, -layerVert[i].Y, layerVert[i].Z);
                }
            }

            //this fixes the ordering of the lists so that normals generate correctly
            for (int i = 0; i < results.LayerSilhouetteVerts.GetLength(0); i++){
                results.LayerSilhouetteVerts[i] = results.LayerSilhouetteVerts[i].Reverse().ToArray();
            }

            //now enumerate the ships layer verts into levels for each deck
            results.DeckSilhouetteVerts = new Vector3[results.NumDecks + 1][][];
            for (int i = 0; i < results.NumDecks; i++){
                results.DeckSilhouetteVerts[i] = new Vector3[primitivesPerDeck + 1][];

                for (int level = 0; level < primitivesPerDeck + 1; level++){
                    results.DeckSilhouetteVerts[i][level] = results.LayerSilhouetteVerts[i*(primitivesPerDeck) + level];
                }
            }
            //edge case for the final deck, the "bottom"
            results.DeckSilhouetteVerts[results.NumDecks] = new Vector3[primitivesPerDeck + 1][];
            for (int level = 0; level < primitivesPerDeck + 1; level++){
                results.DeckSilhouetteVerts[results.NumDecks][level] = results.LayerSilhouetteVerts[results.NumDecks*(primitivesPerDeck) + level];
            }
            return results;
        }

        static GenerateNormalsResults GenerateHullNormals(Vector3[][] layerSVerts){
            //generate a normals array for the entire ship, rather than per-deck
            var totalMesh = new Vector3[layerSVerts.Length,layerSVerts[0].Length];
            var retMesh = new Vector3[layerSVerts.Length,layerSVerts[0].Length];
            MeshHelper.Encode2DListIntoArray(layerSVerts.Length, layerSVerts[0].Length, ref totalMesh, layerSVerts);
            MeshHelper.GenerateMeshNormals(totalMesh, ref retMesh);

            //since we have generated totalmesh, might as well get the centerpoint now
            var centroid = GenerateCenterPoint(totalMesh);

            var ret = new GenerateNormalsResults();
            ret.NormalMesh = retMesh;
            ret.Centroid = centroid;

            return ret;
        }

        static Vector3[][,] GenerateDecks(Vector3[][] layerSVerts, int numDecks, int primitivesPerDeck){
            var retMesh = new Vector3[4][,];
            int vertsInSilhouette = layerSVerts[0].Length;
            for (int deck = 0; deck < numDecks + 1; deck++){
                retMesh[deck] = new Vector3[3,vertsInSilhouette/2];
                for (int vert = 0; vert < vertsInSilhouette/2; vert++){
                    retMesh[deck][0, vert] = layerSVerts[deck*primitivesPerDeck][vertsInSilhouette/2 + vert];

                    retMesh[deck][1, vert] = layerSVerts[deck*primitivesPerDeck][vertsInSilhouette/2 + vert];
                    retMesh[deck][2, vert] = layerSVerts[deck*primitivesPerDeck][vertsInSilhouette/2 - vert - 1];
                    retMesh[deck][1, vert].Z = 0;

                    retMesh[deck][2, vert] = layerSVerts[deck*primitivesPerDeck][vertsInSilhouette/2 - vert - 1];
                }
            }
            return retMesh;
        }

        static ShipGeometryBuffer[] GenerateDeckWallBuffers(Vector3[][][] deckSVerts, Vector3[,] normalMesh, int numDecks, int primitivesPerDeck){
            int vertsInSilhouette = deckSVerts[0][0].Length;

            var hullBuffers = new ShipGeometryBuffer[numDecks + 1];
            //now set up the display buffer for each deck's wall
            for (int i = 0; i < deckSVerts.Length; i++){
                var hullMesh = new Vector3[primitivesPerDeck + 1,vertsInSilhouette];
                var hullNormals = new Vector3[primitivesPerDeck + 1,vertsInSilhouette];
                int[] hullIndicies = MeshHelper.CreateIndiceArray((primitivesPerDeck + 1)*vertsInSilhouette);
                VertexPositionNormalTexture[] hullVerticies = MeshHelper.CreateTexcoordedVertexList((primitivesPerDeck + 1)*vertsInSilhouette);

                //get the hull normals for this part of the hull from the total normals
                for (int x = 0; x < primitivesPerDeck + 1; x++){
                    for (int z = 0; z < vertsInSilhouette; z++){
                        hullNormals[x, z] = normalMesh[i*primitivesPerDeck + x, z];
                    }
                }
                //convert the 2d list heightmap into a 2d array heightmap
                MeshHelper.Encode2DListIntoArray(primitivesPerDeck + 1, vertsInSilhouette, ref hullMesh, deckSVerts[i]);
                //take the 2d array of vertexes and 2d array of normals and stick them in the vertexpositionnormaltexture 
                MeshHelper.ConvertMeshToVertList(hullMesh, hullNormals, ref hullVerticies);

                //now stick it in a buffer
                hullBuffers[i] = new ShipGeometryBuffer(hullIndicies.Length, hullVerticies.Length, hullIndicies.Length/3, "DoodadEditorHullTex", CullMode.CullClockwiseFace);
                hullBuffers[i].Indexbuffer.SetData(hullIndicies);
                hullBuffers[i].Vertexbuffer.SetData(hullVerticies);
            }
            return hullBuffers;
        }

        static ObjectBuffer<QuadIdentifier>[] GenerateDeckFloorBuffers(Vector3[][] layerSVerts, Vector3[][,] deckFloorMesh){
            int vertsInSilhouette = layerSVerts[0].Length;

            var floorNormals = new Vector3[3,vertsInSilhouette/2];
            for (int x = 0; x < 3; x++){
                for (int z = 0; z < vertsInSilhouette/2; z++){
                    floorNormals[x, z] = Vector3.Up;
                }
            }

            var deckFloorbuffers = new ObjectBuffer<QuadIdentifier>[deckFloorMesh.Length];

            //now set up the display buffer for each deck floor
            for (int i = 0; i < deckFloorMesh.Length; i++){
                VertexPositionNormalTexture[] floorVerticies = MeshHelper.CreateTexcoordedVertexList(4*vertsInSilhouette/2);
                int[] floorIndicies = MeshHelper.CreateIndiceArray(4*vertsInSilhouette/2);

                MeshHelper.ConvertMeshToVertList(deckFloorMesh[i], floorNormals, ref floorVerticies);
                deckFloorbuffers[i] = new ObjectBuffer<QuadIdentifier>(vertsInSilhouette*2, 2, 4, 6, "DoodadEditorFloorTex");

                int vertIndex = 0;
                int indIndex = 0;

                for (int si = 0; si < vertsInSilhouette*2; si++){
                    var indicies = new int[6];
                    var verticies = new VertexPositionNormalTexture[4];

                    for (int vi = 0; vi < 4; vi++){
                        verticies[vi] = floorVerticies[vi + vertIndex];
                    }

                    for (int ii = 0; ii < 6; ii++){
                        indicies[ii] = floorIndicies[ii + indIndex];
                        indicies[ii] -= vertIndex;
                    }

                    var identifier = new QuadIdentifier(floorVerticies[0].Position, floorVerticies[1].Position, floorVerticies[2].Position, floorVerticies[3].Position);

                    vertIndex += 4;
                    indIndex += 6;

                    deckFloorbuffers[i].AddObject(identifier, indicies, verticies);
                }
            }

            return deckFloorbuffers;
        }

        static Vector3 GenerateCenterPoint(Vector3[,] totalMesh){
            var p = new Vector3();

            p += totalMesh[0, 0];
            p += totalMesh[totalMesh.GetLength(0) - 1, totalMesh.GetLength(1) - 1];
            p /= 4;

            return p;
        }

        static BoundingBoxResult GenerateDeckBoundingBoxes(float floorBBoxWidth, Vector3[][,] deckFloorMesh){
            int numHorizontalPrimitives = deckFloorMesh[0].GetLength(1);
            var ret = new BoundingBoxResult();
            var deckBoundingBoxes = new List<BoundingBox>[deckFloorMesh.Length];

            for (int layer = 0; layer < deckFloorMesh.Length; layer++){
                var layerBBoxes = new List<BoundingBox>();
                float yLayer = deckFloorMesh[layer][0, 0].Y;

                float boxCreatorPos = 0;
                while (boxCreatorPos < deckFloorMesh[layer][1, 0].X)
                    boxCreatorPos += floorBBoxWidth;

                while (boxCreatorPos < deckFloorMesh[layer][1, numHorizontalPrimitives - 1].X){
                    int index = -1; //index of the first of the two set of vertexes to use when determining 
                    for (int i = 0; i < numHorizontalPrimitives; i++){
                        if (boxCreatorPos >= deckFloorMesh[layer][1, i].X && boxCreatorPos < deckFloorMesh[layer][1, i + 1].X){
                            index = i;
                            break;
                        }
                    }
                    Debug.Assert(index != -1);

                    float startX = deckFloorMesh[layer][0, index].X;
                    float endX = deckFloorMesh[layer][0, index + 1].X;
                    float startZ = deckFloorMesh[layer][0, index].Z;
                    float endZ = deckFloorMesh[layer][0, index + 1].Z;
                    float zBounding1, zBounding2;

                    var interpolator = new Interpolate(
                        startZ,
                        endZ,
                        endX - startX
                        );

                    if (boxCreatorPos + floorBBoxWidth < endX){ //easy scenario where we only have to take one line into consideration when finding how many boxes wide should be
                        zBounding1 = interpolator.GetLinearValue(boxCreatorPos - startX);
                        zBounding2 = interpolator.GetLinearValue(boxCreatorPos + floorBBoxWidth - startX);
                    }
                    else{
                        zBounding1 = interpolator.GetLinearValue(boxCreatorPos - startX);
                        if (index + 2 != numHorizontalPrimitives){
                            var interpolator2 = new Interpolate(
                                deckFloorMesh[layer][0, index + 1].Z,
                                deckFloorMesh[layer][0, index + 2].Z,
                                deckFloorMesh[layer][0, index + 2].X - deckFloorMesh[layer][0, index + 1].X
                                );

                            zBounding2 = interpolator2.GetLinearValue(boxCreatorPos + floorBBoxWidth - deckFloorMesh[layer][0, index + 1].X);
                        }
                        else{
                            zBounding2 = 0;
                        }
                    }

                    int zBoxes1 = (int) (zBounding1/floorBBoxWidth);
                    int zBoxes2 = (int) (zBounding2/floorBBoxWidth);

                    int numZBoxes;
                    if (zBoxes1 < zBoxes2)
                        numZBoxes = zBoxes1;
                    else
                        numZBoxes = zBoxes2;


                    for (int i = -numZBoxes; i < numZBoxes; i++){
                        layerBBoxes.Add(
                            new BoundingBox(
                                new Vector3(
                                    boxCreatorPos,
                                    yLayer,
                                    i*floorBBoxWidth
                                    ),
                                new Vector3(
                                    boxCreatorPos + floorBBoxWidth,
                                    yLayer,
                                    (i + 1)*floorBBoxWidth
                                    )
                                )
                            );
                    }

                    boxCreatorPos += floorBBoxWidth;
                }
                deckBoundingBoxes[layer] = layerBBoxes;
            }
            ret.DeckBoundingBoxes = deckBoundingBoxes;

            var wallSelectionBoxes = deckBoundingBoxes;
            var wallSelectionPoints = new List<List<Vector3>>();
            //generate vertexes of the bounding boxes

            for (int layer = 0; layer < wallSelectionBoxes.Count(); layer++){
                wallSelectionPoints.Add(new List<Vector3>());
                foreach (var box in wallSelectionBoxes[layer]){
                    wallSelectionPoints.Last().Add(box.Min);
                    wallSelectionPoints.Last().Add(box.Max);
                    wallSelectionPoints.Last().Add(new Vector3(box.Max.X, box.Min.Y, box.Max.Z));
                    wallSelectionPoints.Last().Add(new Vector3(box.Min.X, box.Max.Y, box.Max.Z));
                }

                //now we clear out all of the double entries (stupid time hog optimization)
                /*for (int box = 0; box < wallSelectionPoints[layer].Count(); box++){
                    for (int otherBox = 0; otherBox < wallSelectionPoints[layer].Count(); otherBox++){
                        if (box == otherBox)
                            continue;

                        if (wallSelectionPoints[layer][box] == wallSelectionPoints[layer][otherBox]){
                            wallSelectionPoints[layer].RemoveAt(otherBox);
                        }
                    }
                }*/
            }

            ret.DeckVertexes =
                (
                    from layer in wallSelectionPoints
                    select layer.ToList()
                ).ToArray();
            return ret;
        }

        #region Nested type: BoundingBoxResult

        struct BoundingBoxResult{
            public List<BoundingBox>[] DeckBoundingBoxes;
            public List<Vector3>[] DeckVertexes;
        }

        #endregion

        #region Nested type: GenerateHullParams

        struct GenerateHullParams{
            public List<BezierInfo> BackCurveInfo;
            public float DeckHeight;
            public int PrimitivesPerDeck;
            public List<BezierInfo> SideCurveInfo;
            public List<BezierInfo> TopCurveInfo;
        }

        #endregion

        #region Nested type: GenerateHullResults

        struct GenerateHullResults{
            public float Berth;
            public Vector3[][][] DeckSilhouetteVerts;
            public Vector3[][] LayerSilhouetteVerts;
            public float Length;
            public int NumDecks;
        }

        #endregion

        #region Nested type: GenerateNormalsResults

        struct GenerateNormalsResults{
            public Vector3 Centroid;
            public Vector3[,] NormalMesh;
        }

        #endregion
    }

    internal class HullGeometryInfo{
        public Vector3 CenterPoint;
        public List<BoundingBox>[] DeckFloorBoundingBoxes;
        public ObjectBuffer<QuadIdentifier>[] DeckFloorBuffers;
        public float DeckHeight;
        public List<Vector3>[] FloorVertexes;
        public ShipGeometryBuffer[] HullWallTexBuffers;
        public Vector2 MaxBoundingBoxDims;
        public int NumDecks;
        public float WallResolution;
    }

    internal struct QuadIdentifier : IEquatable<QuadIdentifier>, IEnumerable{
        readonly Vector3[] _points;

        public QuadIdentifier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4){
            _points = new Vector3[4];
            _points[0] = p1;
            _points[1] = p2;
            _points[2] = p3;
            _points[3] = p4;
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator(){
            return _points.GetEnumerator();
        }

        #endregion

        #region IEquatable<QuadIdentifier> Members

        public bool Equals(QuadIdentifier other){
            throw new NotImplementedException();
        }

        #endregion

        public QuadIdentifier CloneWithOffset(Vector3 offset){
            return new QuadIdentifier(_points[0] + offset, _points[1] + offset, _points[2] + offset, _points[3] + offset);
        }

        public BoundingBox GenerateBoundingBox(){
            Debug.Assert(_points[0].X != _points[2].X);
            Debug.Assert(_points[0].Z != _points[2].Z);

            return new BoundingBox(_points[0], _points[2]);
        }
    }
}