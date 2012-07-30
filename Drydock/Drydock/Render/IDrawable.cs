using Microsoft.Xna.Framework;

namespace Drydock.Render{
    internal interface IDrawable{
        int X { get; set; }
        int Y { get; set; }
        Rectangle BoundingBox { get; }
    }
}