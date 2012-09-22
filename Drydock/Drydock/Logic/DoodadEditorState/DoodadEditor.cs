#region

using System;
using System.Collections.Generic;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    internal class DoodadEditor : IGameState{
        const int _primsPerDeck = 3;
        readonly Button _deckDownButton;
        readonly Button _deckUpButton;
        readonly HullGeometryHandler _hullGeometryHandler;
        readonly RenderPanel _renderTarget;
        readonly UIElementCollection _uiElementCollection;

        public DoodadEditor(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo){
            _renderTarget = new RenderPanel(0, 0, ScreenData.ScreenWidth, ScreenData.ScreenHeight);
            RenderPanel.BindRenderTarget(_renderTarget);
            _uiElementCollection = new UIElementCollection();

            _deckUpButton = new Button(50, 50, 32, 32, DepthLevel.High, "uparrow");
            _deckDownButton = new Button(50, 82, 32, 32, DepthLevel.High, "downarrow");

            var geomGen = new HullGeometryGenerator(backCurveInfo, sideCurveInfo, topCurveInfo, _primsPerDeck);
            _hullGeometryHandler = new HullGeometryHandler(geomGen.GetGeometrySlices(), _primsPerDeck, geomGen.NumDecks);

            //_deckUpButton.OnLeftButtonClick.Add(_hullGeometryHandler.AddVisibleLevel);
            //_deckDownButton.OnLeftButtonClick.Add(_hullGeometryHandler.RemoveVisibleLevel);

            RenderPanel.UnbindRenderTarget();
        }

        #region IGameState Members

        public void Update(ref ControlState state, double timeDelta){
            throw new NotImplementedException();
        }

        #endregion

        public void Update(){
        }

        public void InputUpdate(ref ControlState state){
            throw new NotImplementedException();
        }

        public void Dispose(){
        }
    }
}