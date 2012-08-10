using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic {
    class HullEditor : ICanReceiveInputEvents {
        HullEditorPanel _sidepanel;


        public HullEditor(){

            _sidepanel = new HullEditorPanel(0, 0, ScreenData.GetScreenValueX(0.5f), ScreenData.GetScreenValueY(0.5f), "side.xml");

            _sidepanel.SaveCurves("side.xml");
            InputEventDispatcher.EventSubscribers.Add(this);
        
        }

        public void Update(){
            _sidepanel.Update();
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
                return InterruptState.InterruptEventDispatch;
            }
            return InterruptState.AllowOtherEvents;
        }
    }
}
