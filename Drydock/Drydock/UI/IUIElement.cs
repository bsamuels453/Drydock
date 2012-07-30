using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Drydock.UI {
    interface IUIElement {
        IUIElementComponent[] Components { get; set; }
        TComponent GetComponent<TComponent>();
        void Update();
    }
}
