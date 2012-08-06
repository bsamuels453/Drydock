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
        public static List<OnMouseAction> OnMMovementEvent;
        public static List<OnMouseAction> OnMButtonEvent;
        public static List<OnMouseAction> OnMClickEvent;
        public static List<OnKeyboardAction> OnKeyboardEvent;

        private static KeyboardState _prevKeyboardState;
        private static MouseState _prevMouseState;
        private static readonly Stopwatch _clickTimer;

        static InputEventDispatcher(){
            OnMMovementEvent = new List<OnMouseAction>();
            OnMButtonEvent = new List<OnMouseAction>();
            OnMClickEvent = new List<OnMouseAction>();
            OnKeyboardEvent = new List<OnKeyboardAction>();
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

                foreach (var action in OnMButtonEvent){
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
                        foreach (var action in OnMClickEvent){
                            if (action(newState) == InterruptState.InterruptEventDispatch){
                                break;
                            }
                        }
                    }
                }
            }

            if (newState.X != _prevMouseState.X ||
                newState.Y != _prevMouseState.Y){
                foreach (OnMouseAction action in OnMMovementEvent){
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
                foreach (var actionHandler in OnKeyboardEvent){
                    if (actionHandler(state) == InterruptState.InterruptEventDispatch){
                        break;
                    }
                }
            }
            _prevKeyboardState = state;
        }
    }
}
