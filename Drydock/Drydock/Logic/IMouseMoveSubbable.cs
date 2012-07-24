using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic{
    internal interface IMouseMoveSubbable{
        void HandleMouseMovementEvent(MouseState state);
    }
}