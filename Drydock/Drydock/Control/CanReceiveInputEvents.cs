#region

using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Control{
    internal abstract class CanReceiveInputEvents{
        public virtual InterruptState OnMouseMovement(MouseState state, MouseState? prevState = null){
            return InterruptState.AllowOtherEvents;
        }

        public virtual InterruptState OnLeftButtonClick(MouseState state, MouseState? prevState = null){
            return InterruptState.AllowOtherEvents;
        }

        public virtual InterruptState OnLeftButtonPress(MouseState state, MouseState? prevState = null){
            return InterruptState.AllowOtherEvents;
        }

        public virtual InterruptState OnLeftButtonRelease(MouseState state, MouseState? prevState = null){
            return InterruptState.AllowOtherEvents;
        }

        public virtual InterruptState OnMouseScroll(MouseState state, MouseState? prevState = null){
            return InterruptState.AllowOtherEvents;
        }

        public virtual InterruptState OnKeyboardEvent(KeyboardState state){
            return InterruptState.AllowOtherEvents;
        }
    }
}