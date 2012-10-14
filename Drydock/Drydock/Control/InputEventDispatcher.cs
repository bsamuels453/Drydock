#region

using System.Diagnostics;
using Drydock.Logic;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Control{
    internal static class InputEventDispatcher{
        public static SpecialKeyboardRec SpecialKeyboardDispatcher;

        static readonly Stopwatch _clickTimer;
        static MouseState _prevMouseState;
        static KeyboardState _prevKeyboardState;
        public static ControlState CurrentControlState;

        static InputEventDispatcher(){
            _prevKeyboardState = Keyboard.GetState();
            _prevMouseState = Mouse.GetState();
            _clickTimer = new Stopwatch();
        }

        public static void Update(){
            //all this crap updates the CurrentControlState to whatever the hell is going on

            var curMouseState = Mouse.GetState();
            var curKeyboardState = Keyboard.GetState();

            var curControlState = new ControlState();

            if (_prevMouseState.X != curMouseState.X || _prevMouseState.Y != curMouseState.Y){
                curControlState.AllowMouseMovementInterpretation = true;
            }
            else{
                curControlState.AllowMouseMovementInterpretation = false;
            }
            //mouse movement stuff needs to be updated every time, regardless of change
            curControlState.MousePos = new Point();
            curControlState.MousePos.X = curMouseState.X;
            curControlState.MousePos.Y = curMouseState.Y;
            curControlState.LeftButtonState = curMouseState.LeftButton;
            curControlState.RightButtonState = curMouseState.RightButton;
            curControlState.MouseScrollChange = curMouseState.ScrollWheelValue - _prevMouseState.ScrollWheelValue;

            if (_prevMouseState.LeftButton != curMouseState.LeftButton){
                curControlState.AllowLeftButtonInterpretation = true;
                if (curMouseState.LeftButton == ButtonState.Released){
                    //check if this qualifies as a click
                    if (_clickTimer.ElapsedMilliseconds < 200){
                        curControlState.LeftButtonClick = true;
                        _clickTimer.Reset();
                    }
                    else{
                        _clickTimer.Reset();
                        curControlState.LeftButtonClick = false;
                    }
                }
                else{ //button was pressed so start the click timer
                    _clickTimer.Start();
                }
            }
            else
                curControlState.AllowLeftButtonInterpretation = false;

            if (_prevMouseState.RightButton != curMouseState.RightButton){
                curControlState.AllowRightButtonInterpretation = true;
            }
            else
                curControlState.AllowRightButtonInterpretation = false;

            if (_prevMouseState.ScrollWheelValue != curMouseState.ScrollWheelValue){
                curControlState.AllowMouseScrollInterpretation = true;
            }
            else
                curControlState.AllowMouseScrollInterpretation = false;

            curControlState.KeyboardState = curKeyboardState;

            _prevKeyboardState = curKeyboardState;
            _prevMouseState = curMouseState;

            curControlState.ViewMatrix = Matrix.CreateLookAt(Renderer.CameraPosition, Renderer.CameraTarget, Vector3.Up);

            curControlState.PrevState = CurrentControlState;
            if (CurrentControlState != null){
                CurrentControlState.PrevState = null;
            }
            CurrentControlState = curControlState;
        }
    }

    internal class ControlState{
        public bool AllowKeyboardInterpretation;
        public bool AllowLeftButtonInterpretation;
        public bool AllowMouseMovementInterpretation;
        public bool AllowMouseScrollInterpretation;
        public bool AllowRightButtonInterpretation;

        public KeyboardState KeyboardState;
        public ButtonState LeftButtonState;
        public bool LeftButtonClick;
        public Point MousePos;
        public int MouseScrollChange;
        public ButtonState RightButtonState;
        public Matrix ViewMatrix;
        public ControlState PrevState;
    }
}