using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Control {
    interface ICanReceiveInputEvents {
        InterruptState OnMouseMovement(MouseState state);
        InterruptState OnLeftButtonClick(MouseState state);
        InterruptState OnLeftButtonPress(MouseState state);
        InterruptState OnLeftButtonRelease(MouseState state);
        InterruptState OnKeyboardEvent(KeyboardState state);
    }
}
