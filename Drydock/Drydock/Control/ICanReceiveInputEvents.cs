#region

using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Control{
    internal interface ICanReceiveInputEvents{
        //it'd be nice to replace this trash
        InterruptState OnMouseMovement(MouseState state);
        InterruptState OnLeftButtonClick(MouseState state);
        InterruptState OnLeftButtonPress(MouseState state);
        InterruptState OnLeftButtonRelease(MouseState state);
        InterruptState OnKeyboardEvent(KeyboardState state);
    }
}