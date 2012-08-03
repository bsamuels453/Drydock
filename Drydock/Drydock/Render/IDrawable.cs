using Drydock.Utilities;
using Microsoft.Xna.Framework;

namespace Drydock.Render{
    internal interface IDrawable{
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float  Height { get; set; }
        FloatingRectangle BoundingBox { get; } //move somewhere else?

    }
}