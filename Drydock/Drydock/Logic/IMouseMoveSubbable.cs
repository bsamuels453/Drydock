using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic {
    interface IMouseMoveSubbable {
        void HandleMouseMovementEvent(MouseState state);
    }
}
