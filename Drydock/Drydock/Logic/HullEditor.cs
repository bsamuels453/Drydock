using System.Diagnostics;
using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic {
    class HullEditor : ICanReceiveInputEvents {
        readonly SideEditorPanel _sidepanel;
        readonly TopEditorPanel _toppanel;
        readonly BackEditorPanel _backpanel;

        readonly PreviewRenderer _previewRenderer;
        

        public HullEditor(){

            _sidepanel = new SideEditorPanel(0, 0, ScreenData.GetScreenValueX(0.5f), ScreenData.GetScreenValueY(0.5f), "side.xml");
            _toppanel = new TopEditorPanel(0, ScreenData.GetScreenValueY(0.5f), ScreenData.GetScreenValueX(0.5f), ScreenData.GetScreenValueY(0.5f), "top.xml");
            _backpanel = new BackEditorPanel(ScreenData.GetScreenValueX(0.5f), 0, ScreenData.GetScreenValueX(0.25f), ScreenData.GetScreenValueY(0.5f), "back.xml");

            _sidepanel.BackPanelModifier = _backpanel.ModifyHandlePosition;
            _sidepanel.TopPanelModifier = _toppanel.ModifyHandlePosition;

            _toppanel.BackPanelModifier = _backpanel.ModifyHandlePosition;
            _toppanel.SidePanelModifier = _sidepanel.ModifyHandlePosition;

            _backpanel.TopPanelModifier = _toppanel.ModifyHandlePosition;
            _backpanel.SidePanelModifier = _sidepanel.ModifyHandlePosition;

            _sidepanel.Update();
            _toppanel.Update();
            _backpanel.Update();

            _previewRenderer = new PreviewRenderer(_sidepanel.Curves, _toppanel.Curves, _backpanel.Curves);

            InputEventDispatcher.EventSubscribers.Add(this);
        
        }

        public void TranslateLinkedHandleMovement(HullEditorPanel caller, float dx, float dy){

        }

        public void Update(){
            _sidepanel.Update();
            _toppanel.Update();
            _backpanel.Update();
            _previewRenderer.Update();
        }

        public InterruptState OnMouseMovement(MouseState state){
            return InterruptState.AllowOtherEvents;
        }

        public InterruptState OnLeftButtonClick(MouseState state){
            return InterruptState.AllowOtherEvents;
        }

        public InterruptState OnLeftButtonPress(MouseState state){
            return InterruptState.AllowOtherEvents;
        }

        public InterruptState OnLeftButtonRelease(MouseState state){
            return InterruptState.AllowOtherEvents;
        }

        public InterruptState OnKeyboardEvent(KeyboardState state){
            if (state.IsKeyDown(Keys.LeftControl) && state.IsKeyDown(Keys.S)){
                _sidepanel.SaveCurves("side.xml");
                _toppanel.SaveCurves("top.xml");
                _backpanel.SaveCurves("back.xml");
                return InterruptState.InterruptEventDispatch;
            }

            return InterruptState.AllowOtherEvents;
        }
    }
}
