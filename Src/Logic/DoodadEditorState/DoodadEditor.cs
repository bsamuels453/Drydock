#region

using System.Collections.Generic;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    internal class DoodadEditor : IGameState{
        const int _primsPerDeck = 3;

        readonly BodyCenteredCamera _cameraController;
        readonly DoodadUI _doodadUI;
        readonly HullDataManager _hullData;
        readonly RenderPanel _renderTarget;

        readonly UIElementCollection _uiElementCollection;

        public DoodadEditor(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo){
            _renderTarget = new RenderPanel(0, 0, ScreenData.ScreenWidth, ScreenData.ScreenHeight);
            _uiElementCollection = new UIElementCollection();
            _cameraController = new BodyCenteredCamera();

            #region construct UI and any UI-related tools

            RenderPanel.BindRenderTarget(_renderTarget);
            UIElementCollection.BindCollection(_uiElementCollection);


            var geometryInfo = HullGeometryGenerator.GenerateShip(backCurveInfo, sideCurveInfo, topCurveInfo, _primsPerDeck);
            _hullData = new HullDataManager(geometryInfo);

            _doodadUI = new DoodadUI(_hullData);

            RenderPanel.UnbindRenderTarget();
            UIElementCollection.UnbindCollection();

            #endregion

            _cameraController.SetCameraTarget(_hullData.CenterPoint);
        }

        #region IGameState Members

        public void Update(ref ControlState state, double timeDelta){
            UIElementCollection.BindCollection(_uiElementCollection);

            #region update input

            UIElementCollection.Collection.UpdateInput(ref state);
            _doodadUI.UpdateInput(ref state);
            _cameraController.UpdateInput(ref state);

            #endregion

            #region update logic

            UIElementCollection.Collection.UpdateLogic(timeDelta);
            _doodadUI.UpdateLogic(timeDelta);

            #endregion

            UIElementCollection.UnbindCollection();
        }

        #endregion
    }
}