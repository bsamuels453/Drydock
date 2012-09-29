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
        int _visibleDecks;
        readonly ShipGeometryBuffer[] _deckWallBuffers;
        readonly ShipGeometryBuffer[] _deckFloorBuffers;
        readonly int _numDecks;
        readonly Vector3 _centerPoint;

        public HullGeometryHandler(HullGeometryInfo geometryInfo){
            //CullMode.CullClockwiseFace
            _deckWallBuffers = geometryInfo.DeckWallBuffers;
            _deckFloorBuffers = geometryInfo.DeckFloorBuffers;
            _centerPoint = geometryInfo.CenterPoint;
            _numDecks = geometryInfo.NumDecks;
            _visibleDecks = _numDecks;
            SetCameraTarget(_centerPoint);

            //override default lighting
            foreach (var buffer in _deckFloorBuffers){
                buffer.DiffuseDirection = new Vector3(0, 1, 1);
                buffer.AmbientIntensity = 1.60f;}
            foreach (var buffer in _deckWallBuffers){
                //buffer.DiffuseDirection = new Vector3(0, -1, 1);
                buffer.DiffuseDirection = new Vector3(0, 1, 1);
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
    }
}