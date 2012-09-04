using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Logic.DoodadEditorState {
    class ShipRenderer : ATargetingCamera {
        readonly int[] _indicies;
        readonly VertexPositionNormalTexture[] _verticies;
        readonly ShipGeometryBuffer _displayBuffer;
        readonly List<List<Vector3>> _layerVerts; 
        
        public ShipRenderer(List<List<Vector3>> geometry){
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


            //create mesh
            var mesh = new Vector3[_layerVerts.Count, _layerVerts[0].Count];
            var normals = new Vector3[_layerVerts.Count, _layerVerts[0].Count];


            _indicies = MeshHelper.CreateIndiceArray(_layerVerts.Count * _layerVerts[0].Count);
            _verticies = MeshHelper.CreateTexcoordedVertexList(_layerVerts.Count * _layerVerts[0].Count);

            MeshHelper.Encode2DListIntoMesh(_layerVerts.Count, _layerVerts[0].Count, ref mesh, _layerVerts);
            MeshHelper.GenerateMeshNormals(mesh, ref normals);

            for (int i = 0; i < _layerVerts.Count; i++) {
                for (int y = 0; y < _layerVerts[0].Count; y++) {
                    normals[i, y] *= -1;
                    //normals[i, y] = Vector3.Left;
                }
            }

            MeshHelper.ConvertMeshToVertList(mesh, normals, ref _verticies);

            _displayBuffer = new ShipGeometryBuffer(_indicies.Count(), _verticies.Count(), _verticies.Count() / 2, "whiteborder");
            _displayBuffer.Indexbuffer.SetData(_indicies);
            _displayBuffer.Vertexbuffer.SetData(_verticies);

            //get center point
            var p = new Vector3();

            p += mesh[0, 0];
            p += mesh[mesh.GetLength(0) - 1, mesh.GetLength(1) - 1];
            p /= 4;
            p *= -1;

            SetCameraTarget(p);
        }

    }
}
