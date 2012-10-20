#region

using Drydock.Control;

#endregion

namespace Drydock.UI.Widgets{
    internal interface IToolbarTool : IInputUpdates, ILogicUpdates{
        void Enable();
        void Disable();
    }
}