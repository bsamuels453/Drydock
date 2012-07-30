using Microsoft.Xna.Framework;

namespace Drydock.UI{
    internal interface IUIInteractiveElement : IUIElement{
        float LayerDepth { get; }
        Rectangle BoundingBox { get; }
        OnMouseAction MouseMovementHandler { get; }
        OnMouseAction MouseClickHandler { get; }
        OnMouseAction MouseEntryHandler { get; }
        OnMouseAction MouseExitHandler { get; }
    }
}