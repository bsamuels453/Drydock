#region

using System;
using System.Collections.Generic;
using Drydock.Control;
using Drydock.Logic.DoodadEditorState.Tools;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    internal class DoodadEditor : IGameState{
        const int _primsPerDeck = 3;

        readonly RenderPanel _renderTarget;
        readonly UIElementCollection _uiElementCollection;
        readonly HullGeometryHandler _hullGeometryHandler;
        readonly Toolbar _toolBar;
        readonly BodyCenteredCamera _cameraController;
        readonly HullGeometryInfo _hullInfo;

        public DoodadEditor(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo){
            _renderTarget = new RenderPanel(0, 0, ScreenData.ScreenWidth, ScreenData.ScreenHeight);
            _uiElementCollection = new UIElementCollection();
            _cameraController = new BodyCenteredCamera();

            #region construct UI and any UI-related tools
            RenderPanel.BindRenderTarget(_renderTarget);
            UIElementCollection.BindCollection(_uiElementCollection);
            //////////
            var geometryGenerator = new HullGeometryGenerator(backCurveInfo, sideCurveInfo, topCurveInfo, _primsPerDeck);
            _hullInfo = geometryGenerator.Resultant;

            _hullGeometryHandler = new HullGeometryHandler(_hullInfo);
            _toolBar = new Toolbar();
            //////////
            RenderPanel.UnbindRenderTarget();
            UIElementCollection.UnbindCollection();
            #endregion

            _cameraController.SetCameraTarget(_hullInfo.CenterPoint);

            #region set up events for the wrangled user interface stuff

            #endregion
        }

        #region IGameState Members

        public void Update(ref ControlState state, double timeDelta){
            UIElementCollection.BindCollection(_uiElementCollection);
            #region update input
            UIElementCollection.Collection.UpdateInput(ref state);
            _cameraController.UpdateInput(ref state);
            #endregion

            #region update logic
            UIElementCollection.Collection.UpdateLogic(timeDelta);
            #endregion

            UIElementCollection.UnbindCollection();
        }

        #endregion

    }
}