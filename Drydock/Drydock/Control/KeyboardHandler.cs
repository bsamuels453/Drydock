using System;
using System.Collections.Generic;
using Drydock.Render;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Control{

    delegate bool OnKeyboardAction(KeyboardState state);

    internal static class KeyboardHandler{
        private static Renderer _renderer;
        private static KeyboardState _prevState;

        public static List<OnKeyboardAction> KeyboardSubscriptions;


        public static void Init(Renderer renderer) {
            _renderer = renderer;
            _prevState = Keyboard.GetState();
            KeyboardSubscriptions = new List<OnKeyboardAction>();
        }

        public static void UpdateKeyboard() {
            var state = Keyboard.GetState();
            if (state != _prevState){
                foreach (var actionHandler in KeyboardSubscriptions){
                    if (actionHandler(state)){
                        break;
                    }
                }
            }

            _prevState = state;
        }
    }
}