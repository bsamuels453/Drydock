#region

using System;
using System.Collections.Generic;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    internal class HullGeometryHandler : ATargetingCamera{
        readonly ShipGeometryBuffer _auxHullBuffer; //used for filling in around portholes
        readonly ShipGeometryBuffer[] _deckBuffers;
        readonly List<List<List<Vector3>>> _deckVertexes; // deck->levels of deck vertexes->vertexes for each level 
        readonly List<List<Vector3>> _layerVerts;

        readonly int _numDecks;
        int _visibleDecks;

        public HullGeometryHandler(List<List<Vector3>> geometry, int deckPrimitiveHeight, int numDecks){
            _deckBuffers = new ShipGeometryBuffer[numDecks + 1];
            _visibleDecks = numDecks;
            _numDecks = numDecks;

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
            _deckVertexes = new List<List<List<Vector3>>>(numDecks + 1);
            for (int i = 0; i < numDecks; i++){
                _deckVertexes.Add(new List<List<Vector3>>(deckPrimitiveHeight));

                for (int level = 0; level < deckPrimitiveHeight + 1; level++){
                    _deckVertexes[i].Add(_layerVerts[i*(deckPrimitiveHeight) + level]);
                }
            }
            //edge case for the final deck, the "bottom"
            _deckVertexes.Add(new List<List<Vector3>>());
            for (int level = numDecks*deckPrimitiveHeight; level < _layerVerts.Count; level++){
                _deckVertexes[_deckVertexes.Count - 1].Add(_layerVerts[level]);
            }

            //generate a normals array for the entire ship, rather than per-deck
            var totalMesh = new Vector3[_layerVerts.Count,_layerVerts[0].Count];
            var totalNormals = new Vector3[_layerVerts.Count,_layerVerts[0].Count];
            MeshHelper.Encode2DListIntoArray(_layerVerts.Count, _layerVerts[0].Count, ref totalMesh, _layerVerts);
            MeshHelper.GenerateMeshNormals(totalMesh, ref totalNormals);

            //now generate the floor for each deck
            var deckMesh = new List<Vector3[,]>();
            var deckNormals = new Vector3[3, _layerVerts[0].Count / 2];
            for (int x = 0; x < 3; x++){
                for (int z = 0; z < _layerVerts[0].Count / 2; z++){
                    deckNormals[x, z] = Vector3.Up;
                }
            }
            for (int deck = 0; deck < numDecks; deck ++) {
                deckMesh.Add(new Vector3[3, _layerVerts[0].Count / 2]);
                for (int vert = 0; vert < _layerVerts[0].Count / 2; vert++){
                    deckMesh.Last()[0, vert] = _layerVerts[deck *deckPrimitiveHeight][_layerVerts[0].Count / 2 + vert];

                    deckMesh.Last()[1, vert] = _layerVerts[deck * deckPrimitiveHeight][_layerVerts[0].Count / 2 + vert];
                    deckMesh.Last()[2, vert] = _layerVerts[deck * deckPrimitiveHeight][_layerVerts[0].Count / 2 - vert - 1];
                    deckMesh.Last()[1, vert].Z = 0;

                    deckMesh.Last()[2, vert] = _layerVerts[deck * deckPrimitiveHeight][_layerVerts[0].Count / 2 - vert -1];
                }
            }

            //now set up the display buffer for each deck
            for (int i = 0; i < _deckVertexes.Count; i++){
                var mesh = new Vector3[deckPrimitiveHeight + 1,_deckVertexes[0][0].Count];
                var normals = new Vector3[deckPrimitiveHeight + 1,_deckVertexes[0][0].Count];
                int[] hullIndicies = MeshHelper.CreateIndiceArray((deckPrimitiveHeight + 1)*_deckVertexes[0][0].Count);
                VertexPositionNormalTexture[] hullVerticies = MeshHelper.CreateTexcoordedVertexList((deckPrimitiveHeight + 1)*_deckVertexes[0][0].Count);

                for (int x = 0; x < deckPrimitiveHeight + 1; x++){
                    for (int z = 0; z < _deckVertexes[0][0].Count; z++){
                        normals[x, z] = totalNormals[i*deckPrimitiveHeight + x, z];
                    }
                }
                MeshHelper.Encode2DListIntoArray(deckPrimitiveHeight + 1, _deckVertexes[0][0].Count, ref mesh, _deckVertexes[i]);
                MeshHelper.ConvertMeshToVertList(mesh, normals, ref hullVerticies);

                //now generate the deck geometry<
                var deckVerticies = new VertexPositionNormalTexture[0];
                var deckIndicies = new int[0];
                if (i != _deckVertexes.Count - 1) {//exception based on how the very very bottom section does not have a designated "deck"
                    deckVerticies = MeshHelper.CreateTexcoordedVertexList(4 * _layerVerts[0].Count / 2);
                    deckIndicies = MeshHelper.CreateIndiceArray(4 * _layerVerts[0].Count / 2);


                    for (int si = 0; si < deckIndicies.Count(); si++) { //offset the deck indicies to be after the hull indicies
                        deckIndicies[si] += hullVerticies.Length;
                    }
                    MeshHelper.ConvertMeshToVertList(deckMesh[i], deckNormals, ref deckVerticies);
                }

                var totalVerticies = new VertexPositionNormalTexture[hullVerticies.Length + deckVerticies.Length];
                hullVerticies.CopyTo(totalVerticies, 0);
                deckVerticies.CopyTo(totalVerticies, hullVerticies.Length);

                var totalIndicies = new int[hullIndicies.Length + deckIndicies.Length];
                hullIndicies.CopyTo(totalIndicies, 0);
                deckIndicies.CopyTo(totalIndicies, hullIndicies.Length);

                _deckBuffers[i] = new ShipGeometryBuffer(totalIndicies.Length, totalVerticies.Length, totalIndicies.Length/3, "whiteborder");
                _deckBuffers[i].Indexbuffer.SetData(totalIndicies);
                _deckBuffers[i].Vertexbuffer.SetData(totalVerticies);
            }

            //get center point
            var p = new Vector3();

            p += totalMesh[0, 0];
            p += totalMesh[totalMesh.GetLength(0) - 1, totalMesh.GetLength(1) - 1];
            p /= 4;

            SetCameraTarget(p);
        }


        public void AddVisibleLevel(){
            if (_visibleDecks != _numDecks){
                foreach (var buffer in _deckBuffers.Reverse().Where(buffer => buffer.IsEnabled == false)) {
                    buffer.IsEnabled = true;
                    _visibleDecks++;
                    break;
                }

            }
        }

        public void RemoveVisibleLevel() {
            if (_visibleDecks != 0){
                foreach (var buffer in _deckBuffers) {
                    if (buffer.IsEnabled) {
                        buffer.IsEnabled = false;
                        _visibleDecks--;
                        break;
                    }
                }
            }
        }

        public override void Update(){
            UpdateCamera(ref InputEventDispatcher.CurrentControlState);
        }
    }
}