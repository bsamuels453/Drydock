#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Drydock.Control;
using Drydock.Logic.Drydock.Logic;

#endregion

namespace Drydock.Logic {
    internal enum SharedStateData {
        PlayerPosition,
        CameraTarget
    }
    static internal class GamestateManager {
        static readonly InputHandler _inputHandler;

        static readonly List<IGameState> _activeStates;
        static readonly Dictionary<SharedStateData, object> _sharedData;

        static GamestateManager() {
            _activeStates = new List<IGameState>();
            _inputHandler = new InputHandler();
            _sharedData = new Dictionary<SharedStateData, object>();//todo-optimize: might be able to make this into a list instead
        }

        static public void ClearAllStates() {
            foreach (var state in _activeStates) {
                state.Dispose();
            }
            _sharedData.Clear();
            _activeStates.Clear();
        }

        static public void Draw(){
            foreach (var state in _activeStates){
                state.Draw();
            }
        }

        static public void ClearState(IGameState state) {
            _activeStates.Remove(state);
            state.Dispose();
        }

        static public object QuerySharedData(SharedStateData identifier) {
            return _sharedData[identifier];
        }

        static public void AddSharedData(SharedStateData identifier, object data) {
            _sharedData.Add(identifier, data);
        }

        static public void ModifySharedData(SharedStateData identifier, object data) {
            _sharedData[identifier] = data;
        }

        static public void DeleteSharedData(SharedStateData identifier) {
            _sharedData.Remove(identifier);
        }

        static public void AddGameState(IGameState newState) {
            _activeStates.Add(newState);
        }

        static public void Update(){
            _inputHandler.Update();
            for (int i = 0; i < _activeStates.Count; i++){
                _activeStates[i].Update(_inputHandler.CurrentInputState, 0);
            }
        }
    }
}