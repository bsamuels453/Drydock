#region

using System;
using System.Collections.Generic;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    internal class HullGeometryHandler : ATargetingCamera, IInputUpdatable {
        int _visibleDecks;
        readonly ShipGeometryBuffer[] _deckWallBuffers;
        readonly ShipGeometryBuffer[] _deckFloorBuffers;
        readonly WireframeBuffer _selectionBuff;
        readonly BoundingBox[][] _deckFloorBoundingBoxes;
        readonly int _numDecks;
        readonly Vector3 _centerPoint;

        public HullGeometryHandler(HullGeometryInfo geometryInfo){
            //CullMode.CullClockwiseFace
            _deckWallBuffers = geometryInfo.DeckWallBuffers;
            _deckFloorBuffers = geometryInfo.DeckFloorBuffers;
            _centerPoint = geometryInfo.CenterPoint;
            _numDecks = geometryInfo.NumDecks;
            _visibleDecks = _numDecks;
            _deckFloorBoundingBoxes = geometryInfo.DeckFloorBoundingBoxes;

            _selectionBuff = new WireframeBuffer(12, 12, 6);
            _selectionBuff.Indexbuffer.SetData(new []{ 0, 1, 2, 3,4,5,6,7,8,9,10,11 });

            SetCameraTarget(_centerPoint);

            //override default lighting
            foreach (var buffer in _deckFloorBuffers){
                buffer.DiffuseDirection = new Vector3(0, 1, 0);
            }
            foreach (var buffer in _deckWallBuffers){
                buffer.DiffuseDirection = new Vector3(0, -1, 1);
                buffer.CullMode = CullMode.None;
            }
                
        }


        public void AddVisibleLevel(){
            if (_visibleDecks != _numDecks){
                //todo: linq this
                var tempFloorBuff = _deckFloorBuffers.Reverse().ToArray();
                var tempWallBuff = _deckWallBuffers.Reverse().ToArray();

                for (int i = 0; i < tempFloorBuff.Count(); i++){
                    if (tempFloorBuff[i].IsEnabled == false){
                        _visibleDecks++;
                        tempFloorBuff[i].IsEnabled = true;
                        tempWallBuff[i].CullMode = CullMode.None;
                        break;
                    }
                }
            }
        }

        public void RemoveVisibleLevel() {
            if (_visibleDecks != 0){
                for (int i = 0; i < _deckFloorBuffers.Count(); i++) {
                    if (_deckFloorBuffers[i].IsEnabled == true) {
                        _visibleDecks--;
                        _deckFloorBuffers[i].IsEnabled = false;
                        _deckWallBuffers[i].CullMode = CullMode.CullClockwiseFace;
                        break;
                    }
                }
            }
        }

        public override void Update(){
            UpdateCamera(ref InputEventDispatcher.CurrentControlState);
        }


        public void InputUpdate(ref ControlState state){
            //first we generate the mouse's ray
            var nearMouse = new Vector3(state.MousePos.X, state.MousePos.Y, 0);
            var farMouse = new Vector3(state.MousePos.X, state.MousePos.Y, 1);
            //var nearMouse = new Vector3(1, 1, 1);

            //transform the mouse into world space
            var nearPoint = Singleton.Device.Viewport.Unproject(
                nearMouse,
                Singleton.ProjectionMatrix,
                state.ViewMatrix,
                Matrix.Identity
                );

            var farPoint = Singleton.Device.Viewport.Unproject(
                farMouse,
                Singleton.ProjectionMatrix,
                state.ViewMatrix,
                Matrix.Identity
                );

            var direction = farPoint - nearPoint;
            direction.Normalize();
            var ray = new Ray(nearPoint, direction);


            int deckIndex = _numDecks - _visibleDecks;
            for (int i = 0; i < _deckFloorBoundingBoxes[deckIndex].Length; i++) {
                if (ray.Intersects(_deckFloorBoundingBoxes[deckIndex][i]) != null){

                    Vector3 v1, v2, v3, v4;
                    //v4  v3
                    //
                    //v1  v2
                    v1 = _deckFloorBoundingBoxes[deckIndex][i].Min;
                    v2 = new Vector3(_deckFloorBoundingBoxes[deckIndex][i].Min.X, _deckFloorBoundingBoxes[deckIndex][i].Min.Y, _deckFloorBoundingBoxes[deckIndex][i].Max.Z);
                    v3 = _deckFloorBoundingBoxes[deckIndex][i].Max;
                    v4 = new Vector3(_deckFloorBoundingBoxes[deckIndex][i].Max.X, _deckFloorBoundingBoxes[deckIndex][i].Min.Y, _deckFloorBoundingBoxes[deckIndex][i].Min.Z);

                    v1.Y += 0.1f;
                    v2.Y += 0.1f;
                    v3.Y += 0.1f;
                    v4.Y += 0.1f;

                    var verts = new VertexPositionColor[12];
                    verts[0] = new VertexPositionColor(v1, Color.White);
                    verts[1] = new VertexPositionColor(v2, Color.White);
                    verts[2] = new VertexPositionColor(v2, Color.White);
                    verts[3] = new VertexPositionColor(v3, Color.White);

                    verts[4] = new VertexPositionColor(v3, Color.White);
                    verts[5] = new VertexPositionColor(v4, Color.White);
                    verts[6] = new VertexPositionColor(v4, Color.White);
                    verts[7] = new VertexPositionColor(v1, Color.White);

                    verts[8] = new VertexPositionColor(v1, Color.White);
                    verts[9] = new VertexPositionColor(v3, Color.White);
                    verts[10] = new VertexPositionColor(v2, Color.White);
                    verts[11] = new VertexPositionColor(v4, Color.White);

                    _selectionBuff.Vertexbuffer.SetData(verts);

                    break;
                }
            }
        }
    }
}