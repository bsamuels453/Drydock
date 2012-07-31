using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI{
    internal interface IUIInteractiveElement : IUIElement{
        new Rectangle BoundingBox { get; }

        List<OnMouseAction> OnLeftButtonClick { get; }
        List<OnMouseAction> OnLeftButtonDown { get; }
        List<OnMouseAction> OnLeftButtonUp { get; }
        List<OnMouseAction> OnMouseEntry { get; }
        List<OnMouseAction> OnMouseExit { get; }
        List<OnMouseAction> OnMouseHover { get; }
        List<OnMouseAction> OnMouseMovement { get; }
        bool MouseMovementHandler(MouseState state);
        bool MouseClickHandler(MouseState state);
        bool MouseEntryHandler(MouseState state);
        bool MouseExitHandler(MouseState state);
    }
}