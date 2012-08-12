using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic {
    class HullEditor : ICanReceiveInputEvents {
        readonly HullEditorPanel _sidepanel;
        readonly HullEditorPanel _toppanel;
        readonly PreviewRenderer _previewRenderer;
       // readonly HullEditorPanel _backpanel;

        public HullEditor(){

            _sidepanel = new HullEditorPanel(CurveType.Side,  0, 0, ScreenData.GetScreenValueX(0.5f), ScreenData.GetScreenValueY(0.5f), "side.xml");
            _toppanel = new HullEditorPanel(CurveType.Top,  0, ScreenData.GetScreenValueY(0.5f), ScreenData.GetScreenValueX(0.5f), ScreenData.GetScreenValueY(0.5f), "top.xml");
            //_backpanel = new HullEditorPanel(this, ScreenData.GetScreenValueX(0.5f), 0, ScreenData.GetScreenValueX(0.25f), ScreenData.GetScreenValueY(0.5f), "back.xml");

            _sidepanel.ExternalHandleModifier = _toppanel.ModifyHandlePosition;
            _toppanel.ExternalHandleModifier = _sidepanel.ModifyHandlePosition;

            _previewRenderer = new PreviewRenderer(_sidepanel.Curves, _toppanel.Curves);

            InputEventDispatcher.EventSubscribers.Add(this);
        
        }

        public void TranslateLinkedHandleMovement(HullEditorPanel caller, float dx, float dy){

        }

        public void Update(){
            _sidepanel.Update();
            _toppanel.Update();

            Vector2 v;

            v = _toppanel.Curves.GetParameterizedPoint(0.5f);

            int f = 5;
            //_backpanel.Update();
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
                //_backpanel.SaveCurves("back.xml");
                return InterruptState.InterruptEventDispatch;
            }
            return InterruptState.AllowOtherEvents;
        }
    }
}
