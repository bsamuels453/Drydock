using Drydock.Control;

namespace Drydock.Logic.DoodadEditorState.Tools {
    interface IToolbarTool : IInputUpdates, ILogicUpdates {
        void Enable();
        void Disable();
    }
}
