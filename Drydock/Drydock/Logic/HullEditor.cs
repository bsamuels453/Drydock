#region

using Drydock.Control;
using Drydock.Render;
using Drydock.UI;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic{
    internal class HullEditor : CanReceiveInputEvents{
        readonly BackEditorPanel _backpanel;

        readonly PreviewRenderer _previewRenderer;
        readonly SideEditorPanel _sidepanel;
        readonly TopEditorPanel _toppanel;


        public HullEditor(){
            _sidepanel = new SideEditorPanel(0, 0, ScreenData.GetScreenValueX(0.5f), ScreenData.GetScreenValueY(0.5f), "side.xml");
            _toppanel = new TopEditorPanel(0, ScreenData.GetScreenValueY(0.5f), ScreenData.GetScreenValueX(0.5f), ScreenData.GetScreenValueY(0.5f), "top.xml");
            _backpanel = new BackEditorPanel(ScreenData.GetScreenValueX(0.5f), 0, ScreenData.GetScreenValueX(0.25f), ScreenData.GetScreenValueY(0.5f), "back.xml");

            _sidepanel.BackPanel = _backpanel;
            _sidepanel.TopPanel = _toppanel;

            _toppanel.BackPanel = _backpanel;
            _toppanel.SidePanel = _sidepanel;

            _backpanel.TopPanel = _toppanel;
            _backpanel.SidePanel = _sidepanel;

            _previewRenderer = new PreviewRenderer(_sidepanel.Curves, _toppanel.Curves, _backpanel.Curves);

            InputEventDispatcher.EventSubscribers.Add((float) DepthLevel.Medium/10, this);
        }

        public override InterruptState OnKeyboardEvent(KeyboardState state){
            if (state.IsKeyDown(Keys.LeftControl) && state.IsKeyDown(Keys.S)){
                _sidepanel.SaveCurves("side.xml");
                _toppanel.SaveCurves("top.xml");
                _backpanel.SaveCurves("back.xml");
                return InterruptState.InterruptEventDispatch;
            }

            return InterruptState.AllowOtherEvents;
        }

        public void TranslateLinkedHandleMovement(HullEditorPanel caller, float dx, float dy){
        }

        public void Update(){
            _sidepanel.Update();
            _toppanel.Update();
            _backpanel.Update();
            _previewRenderer.Update();
        }
    }
}