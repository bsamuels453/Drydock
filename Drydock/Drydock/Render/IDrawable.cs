using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Drydock.Render {
    interface IDrawable {
        int X { get; set; }
        int Y { get; set; }
        Rectangle BoundingBox { get;}
    }
}
