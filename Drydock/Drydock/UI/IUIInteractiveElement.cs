using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI{
    internal interface IUIInteractiveElement : IUIElement{
        //new Rectangle BoundingBox { get; }

        //These lists will dispatch contained delegates when their event trigger is fulfilled
        #region lists
        List<OnMouseAction> OnLeftButtonClick { get; }//procs when left mouse button is pressed and released within a time interval (global)
        List<OnMouseAction> OnLeftButtonDown { get; }//procs when left mouse button is pressed (global)
        List<OnMouseAction> OnLeftButtonUp { get; }//procs when left mouse button is released (global)
        List<OnMouseAction> OnMouseEntry { get; }//procs when mouse enters the bounding box of the element
        List<OnMouseAction> OnMouseExit { get; }//procs when the mouse exits the bounding box of the element
        List<OnMouseAction> OnMouseHover { get; }//procs when mouse has been within the bounding box of the element for a certain period of time
        List<OnMouseAction> OnMouseMovement { get; }//procs on global mouse movement
        #endregion

        //These methods are called by the UIContext, they will dispatch events as reflected in the above lists
        #region methods
        bool MouseMovementHandler(MouseState state);
        bool MouseClickHandler(MouseState state);
        bool MouseEntryHandler(MouseState state);
        bool MouseExitHandler(MouseState state);
        #endregion
    }
}