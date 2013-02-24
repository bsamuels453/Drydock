#region

using Drydock.Control;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.UI.Widgets{
    internal interface IToolbarTool : IInputUpdates, ILogicUpdates{
        bool Enabled { get; set; }
        void Draw(Matrix viewMatrix);
    }
}