using Microsoft.Xna.Framework;

namespace Drydock.Render{
    internal interface IDrawable{
        int X { get; }
        int Y { get; }
        Rectangle BoundingBox { get; }
    }
}