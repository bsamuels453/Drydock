using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic{
    internal interface IClickSubbable{
        /// <summary>
        /// Handle a fresh mouse click event. Return true if you want all other mouse click event hooks to be canceled for this update.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        bool HandleMouseClickEvent(MouseState state);
    }
}