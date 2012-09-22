#region

using System;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic{
    delegate void SpecialKeyboardRec(KeyboardState state);
    internal static class GamestateManager{
        static IGameState _currentState;

        public static void Init(){
            InputEventDispatcher.SpecialKeyboardDispatcher = SpecialKeyboardRec;
            _currentState = null;
        }
        public static void ClearGameState(){
            RenderPanel.Clear();
            UIElementCollection.Clear();
            _currentState = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void SetGameState(IGameState newState){
            if (_currentState != null){
                throw new Exception("gamestate has to be cleared before setting a new state");
            }
            _currentState = newState;
        }


        public static void Update(){
            if (_currentState != null){
                _currentState.InputUpdate(ref InputEventDispatcher.CurrentControlState);
                _currentState.Update();
            }
        }

        public static void SpecialKeyboardRec(KeyboardState state){

        }
    }

    internal interface IGameState{
        void Update();
        void InputUpdate(ref ControlState state);
    }
}