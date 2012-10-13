using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Control;
using Drydock.UI.Widgets;
using Drydock.Utilities;
using Drydock.Utilities.ReferenceTypes;
using Microsoft.Xna.Framework;

namespace Drydock.Logic.DoodadEditorState.Tools {
    class WallBuildTool : IToolbarTool{
        HullGeometryInfo _hullInfo;
        readonly IntRef _visibleDecks;

        public WallBuildTool(HullGeometryInfo hullInfo, IntRef visibleDecks){
            _hullInfo = hullInfo;
            _visibleDecks = visibleDecks;
        }

        public void UpdateInput(ref ControlState state){
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


            int deckIndex = _hullInfo.NumDecks - _visibleDecks.Value;
            for (int i = 0; i < _hullInfo.DeckFloorBoundingBoxes[deckIndex].Length; i++){
                if (ray.Intersects(_hullInfo.DeckFloorBoundingBoxes[deckIndex][i]) != null){
                }
            }
        }

        public void UpdateLogic(double timeDelta){

        }

        public void Enable(){
            //throw new NotImplementedException();
        }

        public void Disable(){
            //throw new NotImplementedException();
        }
    }
}
