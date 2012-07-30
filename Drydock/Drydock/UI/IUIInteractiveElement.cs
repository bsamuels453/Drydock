using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Drydock.UI {
    interface IUIInteractiveElement : IUIElement {
        float LayerDepth { get; }
        Rectangle BoundingBox { get; }
        OnMouseAction MouseMovementHandler { get; }
        OnMouseAction MouseClickHandler { get; }
        OnMouseAction MouseEntryHandler { get; }
        OnMouseAction MouseExitHandler { get; }
    }
}
