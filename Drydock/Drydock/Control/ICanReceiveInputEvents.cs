#region

using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Control {
    interface ICanReceiveInputEvents {
        InterruptState OnMouseMovement(MouseState state);
        InterruptState OnLeftButtonClick(MouseState state);
        InterruptState OnLeftButtonPress(MouseState state);
        InterruptState OnLeftButtonRelease(MouseState state);
        InterruptState OnKeyboardEvent(KeyboardState state);
    }
}
