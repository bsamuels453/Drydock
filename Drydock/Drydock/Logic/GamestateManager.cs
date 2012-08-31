#region

using Drydock.Control;
using Drydock.UI;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic{
    delegate void SpecialKeyboardRec(KeyboardState state);
    internal static class GamestateManager{
        static IGameState _currentState;

        public static void Init(){
            InputEventDispatcher.SpecialKeyboardDispatcher = SpecialKeyboardRec;
        }

        public static void SetGameState(IGameState newState){
            if (_currentState != null){
                _currentState.Dispose();
            }
            _currentState = newState;
        }


        public static void Update(){
            UIContext.Update();
            if (_currentState != null){
                _currentState.Update();
            }
        }

        public static void SpecialKeyboardRec(KeyboardState state){

        }
    }

    internal interface IGameState{
        void Update();
        void Dispose();
        //void Draw();
    }
}