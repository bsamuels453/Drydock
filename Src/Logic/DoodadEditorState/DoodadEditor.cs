#region

using System.Collections.Generic;
using Drydock.Control;
using Drydock.Logic.Drydock.Logic;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    internal class DoodadEditor : IGameState{
        const int _primsPerDeck = 3;

        readonly BodyCenteredCamera _cameraController;
        readonly DoodadUI _doodadUI;
        readonly HullDataManager _hullData;
        readonly RenderTarget _renderTarget;

        readonly UIElementCollection _uiElementCollection;

        public DoodadEditor(GamestateManager mgr, List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo){
            _renderTarget = new RenderTarget(0, 0, ScreenData.ScreenWidth, ScreenData.ScreenHeight);
            _uiElementCollection = new UIElementCollection();
            _cameraController = new BodyCenteredCamera(mgr);

            #region construct UI and any UI-related tools

            UIElementCollection.BindCollection(_uiElementCollection);


            var geometryInfo = HullGeometryGenerator.GenerateShip(backCurveInfo, sideCurveInfo, topCurveInfo, _primsPerDeck);
            _hullData = new HullDataManager(geometryInfo);

            _doodadUI = new DoodadUI(_hullData, _renderTarget, mgr);

            UIElementCollection.UnbindCollection();

            #endregion

            _cameraController.SetCameraTarget(_hullData.CenterPoint);
        }

        public void Dispose(){
            throw new System.NotImplementedException();
        }

        public void Update(InputState state, double timeDelta){
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

        public void Draw(){
            var viewMatrix = Matrix.CreateLookAt(_cameraController.CameraPosition, _cameraController.CameraTarget, Vector3.Up);


            _renderTarget.Bind();
            _doodadUI.Draw(viewMatrix);
            foreach (var buffer in _hullData.DeckBuffers){
                buffer.Draw(viewMatrix);
            }
            foreach (var buffer in _hullData.ObjectBuffers) {
                buffer.Draw(viewMatrix);
            }
            foreach (var buffer in _hullData.HullBuffers) {
                buffer.Draw(viewMatrix);
            }
            foreach (var buffer in _hullData.WallBuffers) {
                buffer.Draw(viewMatrix);
            }
            _renderTarget.Unbind();
        }
    }
}