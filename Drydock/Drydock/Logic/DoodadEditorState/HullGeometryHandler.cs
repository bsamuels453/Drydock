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
        ShipGeometryBuffer[] _deckWallBuffers;
        ShipGeometryBuffer[] _deckFloorBuffers;
        int _numDecks;
        readonly Vector3 _centerPoint;

        public HullGeometryHandler(HullGeometryInfo geometryInfo){
            _deckWallBuffers = geometryInfo.DeckWallBuffers;
            _deckFloorBuffers = geometryInfo.DeckFloorBuffers;
            _centerPoint = geometryInfo.CenterPoint;
            _numDecks = geometryInfo.NumDecks;
            _visibleDecks = _numDecks - 1;
            SetCameraTarget(_centerPoint);
        }


        public void AddVisibleLevel(){
            if (_visibleDecks != _numDecks){
                foreach (var buffer in _deckFloorBuffers.Reverse().Where(buffer => buffer.IsEnabled == false)) {
                    buffer.IsEnabled = true;
                    _visibleDecks++;
                    break;
                }

            }
        }

        public void RemoveVisibleLevel() {
            if (_visibleDecks != 0){
                foreach (var buffer in _deckFloorBuffers) {
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