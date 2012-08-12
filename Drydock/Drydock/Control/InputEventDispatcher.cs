#region

using System.Collections.Generic;
using System.Diagnostics;
using Drydock.Render;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Control {
    internal enum InterruptState{
        AllowOtherEvents,
        InterruptEventDispatch
    }

    internal delegate InterruptState OnMouseEvent(MouseState state);
    internal delegate InterruptState OnKeyboardEvent(KeyboardState state);

    //these two delegates are to be used in literal events
    internal delegate void EOnMouseEvent(MouseState state);
    internal delegate void EOnKeyboardEvent(KeyboardState state);

    internal static class InputEventDispatcher{
        public static List<ICanReceiveInputEvents> EventSubscribers;

        private static KeyboardState _prevKeyboardState;
        private static MouseState _prevMouseState;
        private static readonly Stopwatch _clickTimer;

        private static ScreenText _mousePos;

        static InputEventDispatcher(){
            EventSubscribers = new List<ICanReceiveInputEvents>();

            _prevKeyboardState = Keyboard.GetState();
            _prevMouseState = Mouse.GetState();
            _clickTimer = new Stopwatch();
            _mousePos = new ScreenText(0, 0, "not init");
        }

        public static void Update(){
            UpdateMouse();
            UpdateKeyboard();
        }

        private static void UpdateMouse(){
            MouseState newState = Mouse.GetState();
            if (newState.LeftButton != _prevMouseState.LeftButton ||
                newState.RightButton != _prevMouseState.RightButton ||
                newState.MiddleButton != _prevMouseState.MiddleButton){

                if (newState.LeftButton == ButtonState.Released){
                    //dispatch onbuttonreleased
                    foreach (var subscriber in EventSubscribers) {
                        if (subscriber.OnLeftButtonRelease(newState) == InterruptState.InterruptEventDispatch){
                            break;
                        }
                    }

                    
                    if (_clickTimer.ElapsedMilliseconds < 200){
                        //dispatch onclick
                        foreach (var subscriber in EventSubscribers) {
                            if (subscriber.OnLeftButtonClick(newState) == InterruptState.InterruptEventDispatch){
                                break;
                            }
                        }
                    }
                    _clickTimer.Reset();
                }

                if (newState.LeftButton == ButtonState.Pressed) {
                    _clickTimer.Start();
                    //dispatch onbuttonpressed
                    foreach (var subscriber in EventSubscribers) {
                        if (subscriber.OnLeftButtonPress(newState) == InterruptState.InterruptEventDispatch) {
                            break;
                        }
                    }
                }
            }

            if (newState.X != _prevMouseState.X ||
                newState.Y != _prevMouseState.Y){
                    _mousePos.EditText("X:" + newState.X + " Y:" + newState.Y);
                //dispatch onmovement
                    foreach (var subscriber in EventSubscribers) {
                    if (subscriber.OnMouseMovement(newState) == InterruptState.InterruptEventDispatch){
                        break;
                    }
                }
            }
            _prevMouseState = newState;
        }

        private static void UpdateKeyboard(){
            var state = Keyboard.GetState();
            if (state != _prevKeyboardState){
                foreach (var subscriber in EventSubscribers) {
                    if (subscriber.OnKeyboardEvent(state) == InterruptState.InterruptEventDispatch){
                        break;
                    }
                }
            }
            _prevKeyboardState = state;
        }
    }
}
