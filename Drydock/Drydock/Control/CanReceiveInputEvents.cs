#region

using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Control{
    internal abstract class CanReceiveInputEvents{
        virtual public InterruptState OnMouseMovement(MouseState state, MouseState? prevState = null){
            return InterruptState.AllowOtherEvents;
        }

        virtual public InterruptState OnLeftButtonClick(MouseState state, MouseState? prevState = null) {
            return InterruptState.AllowOtherEvents;
        }

        virtual public InterruptState OnLeftButtonPress(MouseState state, MouseState? prevState = null) {
            return InterruptState.AllowOtherEvents;
        }

        virtual public InterruptState OnLeftButtonRelease(MouseState state, MouseState? prevState = null) {
            return InterruptState.AllowOtherEvents;
        }

        virtual public InterruptState OnMouseScroll(MouseState state, MouseState? prevState = null) {
            return InterruptState.AllowOtherEvents;
        }

        virtual public InterruptState OnKeyboardEvent(KeyboardState state) {
            return InterruptState.AllowOtherEvents;
        }
    }
}