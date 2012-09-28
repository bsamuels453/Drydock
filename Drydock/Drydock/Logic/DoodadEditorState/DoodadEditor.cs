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
            UIElementCollection.BindCollection(_uiElementCollection);

            _deckUpButton = new Button(50, 50, 32, 32, DepthLevel.High, "uparrow");
            _deckDownButton = new Button(50, 82, 32, 32, DepthLevel.High, "downarrow");

            var geometryGenerator = new HullGeometryGenerator(backCurveInfo, sideCurveInfo, topCurveInfo, _primsPerDeck);
            _hullGeometryHandler = new HullGeometryHandler(geometryGenerator.Resultant);

            _deckUpButton.OnLeftClickDispatcher += _hullGeometryHandler.AddVisibleLevel;
            _deckDownButton.OnLeftClickDispatcher += _hullGeometryHandler.RemoveVisibleLevel;

            RenderPanel.UnbindRenderTarget();
            UIElementCollection.UnbindCollection();
        }

        #region IGameState Members

        public void Update(ref ControlState state, double timeDelta){
            UIElementCollection.BindCollection(_uiElementCollection);
            _hullGeometryHandler.Update();

            UIElementCollection.Collection.Update(timeDelta);
            UIElementCollection.Collection.InputUpdate(ref state);
            UIElementCollection.UnbindCollection();
        }

        #endregion
    }
}