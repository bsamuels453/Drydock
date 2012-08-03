using Microsoft.Xna.Framework;

namespace Drydock.Render{
    internal interface IDrawable{
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        Rectangle BoundingBox { get; } //move somewhere else?

    }
}