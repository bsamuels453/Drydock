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
        static bool _isGStateCleared;

        public static void Init(){
            InputEventDispatcher.SpecialKeyboardDispatcher = SpecialKeyboardRec;
            _isGStateCleared = true;
        }
        public static void ClearGameState(){
            _isGStateCleared = true;
            InputEventDispatcher.EventSubscribers.Clear();
            RenderPanel.Clear();
            UIElementCollection.Clear();
            _currentState = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            int g = 5;
        }

        public static void SetGameState(IGameState newState){
            if (!_isGStateCleared){
                throw new Exception("gamestate has to be cleared before setting a new state");
            }

            _currentState = newState;
            _isGStateCleared = false;
        }


        public static void Update(){
            if (_currentState != null){
                _currentState.Update();
            }
        }

        public static void SpecialKeyboardRec(KeyboardState state){

        }
    }

    internal interface IGameState{
        void Update();
    }
}