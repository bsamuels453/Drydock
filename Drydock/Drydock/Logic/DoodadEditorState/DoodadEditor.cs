#region

using System.Collections.Generic;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    internal class DoodadEditor : IGameState{
        const int _primsPerDeck = 3;
        readonly RenderPanel _renderTarget;
        readonly HullGeometryHandler _hullGeometryHandler;
        readonly UIElementCollection _uiElementCollection;
        readonly Button _deckUpButton;
        readonly Button _deckDownButton;

        public DoodadEditor(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo){
            _renderTarget = new RenderPanel(0, 0, ScreenData.ScreenWidth, ScreenData.ScreenHeight);
            RenderPanel.BindRenderTarget(_renderTarget);
            _uiElementCollection = new UIElementCollection();
            
            _deckUpButton = _uiElementCollection.Add<Button>(new Button(50,50,32,32, DepthLevel.High, _uiElementCollection, "uparrow"));
            _deckDownButton = _uiElementCollection.Add<Button>(new Button(50, 82, 32, 32, DepthLevel.High, _uiElementCollection, "downarrow"));

            var geomGen = new HullGeometryGenerator(backCurveInfo, sideCurveInfo, topCurveInfo, _primsPerDeck);
            _hullGeometryHandler = new HullGeometryHandler(geomGen.GetGeometrySlices(), _primsPerDeck, geomGen.NumDecks);

            _deckUpButton.OnLeftButtonClick.Add(_hullGeometryHandler.AddVisibleLevel);

            RenderPanel.UnbindRenderTarget();
        }

        #region IGameState Members

        public void Update(){
            //throw new NotImplementedException();
        }

        public void Dispose(){
        }

        #endregion
    }
}