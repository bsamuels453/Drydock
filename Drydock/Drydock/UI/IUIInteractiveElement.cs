using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Drydock.UI{
    internal interface IUIInteractiveElement : IUIElement{
        float LayerDepth { get; }
        new Rectangle BoundingBox { get; }
        OnMouseAction MouseMovementHandler { get; }
        OnMouseAction MouseClickHandler { get; }
        OnMouseAction MouseEntryHandler { get; }
        OnMouseAction MouseExitHandler { get; }

        List<OnMouseAction> OnLeftButtonClick { get; }
        List<OnMouseAction> OnLeftButtonDown { get; }
        List<OnMouseAction> OnLeftButtonUp { get; }
        List<OnMouseAction> OnMouseEntry { get; }
        List<OnMouseAction> OnMouseExit { get; }
        List<OnMouseAction> OnMouseHover { get; }
        List<OnMouseAction> OnMouseMovement { get; }
    }
}