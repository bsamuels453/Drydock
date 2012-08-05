#region

using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Control {
    internal enum InterruptState{
        AllowOtherEvents,
        InterruptEventDispatch
    }

    internal delegate InterruptState OnMouseAction(MouseState state);
    internal delegate InterruptState OnKeyboardAction(KeyboardState state);

    static class InputEventDispatcher {
        public static List<OnMouseAction> OnMMovementDispatcher;
        public static List<OnMouseAction> OnMButtonDispatcher;
        public static List<OnKeyboardAction> OnKeyboardDispatcher;

        private static KeyboardState _prevKeyboardState;
        private static MouseState _prevMouseState;

        static InputEventDispatcher(){
            OnMMovementDispatcher = new List<OnMouseAction>();
            OnMButtonDispatcher = new List<OnMouseAction>();
            OnKeyboardDispatcher = new List<OnKeyboardAction>();
            _prevKeyboardState = Keyboard.GetState();
            _prevMouseState = Mouse.GetState();
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
                    foreach (var t in OnMButtonDispatcher) {
                    if (t(newState) == InterruptState.InterruptEventDispatch){
                        break;
                    }
                }
            }

            if (newState.X != _prevMouseState.X ||
                newState.Y != _prevMouseState.Y){
                    foreach (OnMouseAction t in OnMMovementDispatcher) {
                    if (t(newState) == InterruptState.InterruptEventDispatch){
                        break;
                    }
                }
            }
            _prevMouseState = newState;
        }

        private static void UpdateKeyboard(){
            var state = Keyboard.GetState();
            if (state != _prevKeyboardState) {
                foreach (var actionHandler in OnKeyboardDispatcher) {
                    if (actionHandler(state) == InterruptState.InterruptEventDispatch) {
                        break;
                    }
                }
            }
            _prevKeyboardState = state;
        }
    }
}
