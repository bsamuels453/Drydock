#region

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Control {
    internal enum InterruptState{
        AllowOtherEvents,
        InterruptEventDispatch
    }

    internal delegate InterruptState OnMouseAction(MouseState state);
    internal delegate InterruptState OnKeyboardAction(KeyboardState state);

    internal static class InputEventDispatcher{
        public static List<OnMouseAction> OnMMovementDispatcher;
        public static List<OnMouseAction> OnMButtonDispatcher;
        public static List<OnMouseAction> OnMClickDispatcher;
        public static List<OnKeyboardAction> OnKeyboardDispatcher;

        private static KeyboardState _prevKeyboardState;
        private static MouseState _prevMouseState;
        private static readonly Stopwatch _clickTimer;

        static InputEventDispatcher(){
            OnMMovementDispatcher = new List<OnMouseAction>();
            OnMButtonDispatcher = new List<OnMouseAction>();
            OnMClickDispatcher = new List<OnMouseAction>();
            OnKeyboardDispatcher = new List<OnKeyboardAction>();
            _prevKeyboardState = Keyboard.GetState();
            _prevMouseState = Mouse.GetState();
            _clickTimer = new Stopwatch();
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



                foreach (var action in OnMButtonDispatcher){
                    if (action(newState) == InterruptState.InterruptEventDispatch){
                        break;
                    }
                }
                if (newState.LeftButton == ButtonState.Pressed){
                    _clickTimer.Start();
                }
                if (newState.LeftButton == ButtonState.Released){
                    _clickTimer.Reset();
                    if (_clickTimer.ElapsedMilliseconds < 200){
                        foreach (var action in OnMClickDispatcher){
                            if (action(newState) == InterruptState.InterruptEventDispatch){
                                break;
                            }
                        }
                    }
                }

                //}
            }

            if (newState.X != _prevMouseState.X ||
                newState.Y != _prevMouseState.Y){
                foreach (OnMouseAction action in OnMMovementDispatcher){
                    if (action(newState) == InterruptState.InterruptEventDispatch){
                        break;
                    }
                }
            }
            _prevMouseState = newState;
        }

        private static void UpdateKeyboard(){
            var state = Keyboard.GetState();
            if (state != _prevKeyboardState){
                foreach (var actionHandler in OnKeyboardDispatcher){
                    if (actionHandler(state) == InterruptState.InterruptEventDispatch){
                        break;
                    }
                }
            }
            _prevKeyboardState = state;
        }
    }
}
