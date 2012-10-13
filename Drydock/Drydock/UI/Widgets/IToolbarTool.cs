using Drydock.Control;

namespace Drydock.UI.Widgets {
    interface IToolbarTool : IInputUpdates, ILogicUpdates {
        void Enable();
        void Disable();
    }
}
