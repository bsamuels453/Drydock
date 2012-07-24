using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic {
    interface IClickSubbable {
        void HandleMouseClickEvent(MouseState state);
    }
}
