using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Drydock.UI {
    interface IUIPrimitive {
        Rectangle BoundingBox { get; }
    }
}
