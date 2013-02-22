﻿#region

using System.Collections.Generic;
using Drydock.Control;
using Drydock.Logic.DoodadEditorState.Tools;
using Drydock.Render;
using Drydock.UI;
using Drydock.UI.Widgets;
using Drydock.Utilities;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    internal class DoodadEditor : IGameState{
        const int _primsPerDeck = 3;

        readonly BodyCenteredCamera _cameraController;
        readonly DoodadUI _doodadUI;
        readonly HullGeometryInfo _hullInfo;
        readonly RenderPanel _renderTarget;
        readonly Toolbar _toolBar;
        readonly UIElementCollection _uiElementCollection;

        public DoodadEditor(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo){
            _renderTarget = new RenderPanel(0, 0, ScreenData.ScreenWidth, ScreenData.ScreenHeight);
            _uiElementCollection = new UIElementCollection();
            _cameraController = new BodyCenteredCamera();

            #region construct UI and any UI-related tools

            RenderPanel.BindRenderTarget(_renderTarget);
            UIElementCollection.BindCollection(_uiElementCollection);


            _hullInfo = HullGeometryGenerator.GenerateShip(backCurveInfo, sideCurveInfo, topCurveInfo, _primsPerDeck);
           
            _doodadUI = new DoodadUI(_hullInfo);

            #region construct toolbar

            _toolBar = new Toolbar("Templates/DoodadToolbar.json");
            _toolBar.BindButtonToTool(0, new WallMenuTool(
                                             _hullInfo,
                                             _doodadUI.VisibleDecks,
                                             _doodadUI.WallBuffers,
                                             _doodadUI.WallPositions)
                );

            _toolBar.BindButtonToTool(1, new LadderBuildTool(
                                             _hullInfo,
                                             _doodadUI.VisibleDecks
                                             ));

            #endregion

            RenderPanel.UnbindRenderTarget();
            UIElementCollection.UnbindCollection();

            #endregion

            _cameraController.SetCameraTarget(_hullInfo.CenterPoint);
        }

        #region IGameState Members

        public void Update(ref ControlState state, double timeDelta){
            UIElementCollection.BindCollection(_uiElementCollection);

            #region update input

            UIElementCollection.Collection.UpdateInput(ref state);
            _toolBar.UpdateInput(ref state);
            _cameraController.UpdateInput(ref state);

            #endregion

            #region update logic

            UIElementCollection.Collection.UpdateLogic(timeDelta);
            _toolBar.UpdateLogic(timeDelta);

            #endregion

            UIElementCollection.UnbindCollection();
        }

        #endregion
    }
}