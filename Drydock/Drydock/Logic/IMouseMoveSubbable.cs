using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic{
    internal interface IMouseMoveSubbable{
        /// <summary>
        /// Handle a fresh mouse movement. Return true if you want all other mouse movement event hooks to be canceled for this update.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        bool HandleMouseMovementEvent(MouseState state);
    }
}