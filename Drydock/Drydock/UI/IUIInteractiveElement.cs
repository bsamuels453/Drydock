using System.Collections.Generic;
using Drydock.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI{
    internal interface IUIInteractiveElement : IUIElement{

        //These lists will dispatch contained delegates when their event trigger is fulfilled
        #region event dispatch lists
        List<OnMouseEvent> OnLeftButtonClickDispatch { get; }//procs when left mouse button is pressed and released within a time interval (global)
        List<OnMouseEvent> OnLeftButtonPressDispatch { get; }//procs when left mouse button is pressed (global)
        List<OnMouseEvent> OnLeftButtonReleaseDispatch { get; }//procs when left mouse button is released (global)
        List<OnMouseEvent> OnMouseEntryDispatch { get; }//procs when mouse enters the bounding box of the element
        List<OnMouseEvent> OnMouseExitDispatch { get; }//procs when the mouse exits the bounding box of the element
        List<OnMouseEvent> OnMouseHoverDispatch { get; }//procs when mouse has been within the bounding box of the element for a certain period of time
        List<OnMouseEvent> OnMouseMovementDispatch { get; }//procs on global mouse movement
        List<OnKeyboardEvent> OnKeyboardEventDispatch { get; }

        #endregion

        //These event handlers are called by the UIElementCollection, they should dispatch events as reflected in the above lists
        #region event handlers
        InterruptState OnMouseMovement(MouseState state);
        InterruptState OnLeftButtonClick(MouseState state);
        InterruptState OnLeftButtonPress(MouseState state);
        InterruptState OnLeftButtonRelease(MouseState state);
        InterruptState OnKeyboardEvent(KeyboardState state);
        InterruptState OnMouseEntry(MouseState state);
        InterruptState OnMouseExit(MouseState state);
        InterruptState OnMouseHover(MouseState state);
        #endregion
    }
}