using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Control {
    class InputState {
        public bool AllowKeyboardInterpretation;
        public bool AllowLeftButtonInterpretation;
        public bool AllowRightButtonInterpretation;
        public bool AllowMouseMovementInterpretation;
        public bool AllowMouseScrollInterpretation;

        public KeyboardState KeyboardState;

        public ButtonState LeftButtonState;
        public bool LeftButtonChange;
        public bool LeftButtonClick;

        public Point MousePos;
        public int MouseScrollChange;
        public bool MouseMoved;

        public InputState PrevState;

        public ButtonState RightButtonState;
        public bool RightButtonChange;
        public bool RightButtonClick;
    }
}
