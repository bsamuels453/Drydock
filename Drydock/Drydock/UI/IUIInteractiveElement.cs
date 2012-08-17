#region

using System.Collections.Generic;
using Drydock.Control;

#endregion

namespace Drydock.UI{
    internal interface IUIInteractiveElement : IUIElement{
        #region event dispatch lists
        List<OnMouseEvent> OnLeftButtonClick { get; }//procs when left mouse button is pressed and released within a time interval (global)
        List<OnMouseEvent> OnLeftButtonPress { get; }//procs when left mouse button is pressed (global)
        List<OnMouseEvent> OnLeftButtonRelease { get; }//procs when left mouse button is released (global)
        List<OnMouseEvent> OnMouseEntry { get; }//procs when mouse enters the bounding box of the element
        List<OnMouseEvent> OnMouseExit { get; }//procs when the mouse exits the bounding box of the element
        List<OnMouseEvent> OnMouseMovement { get; }//procs on global mouse movement
        List<EOnKeyboardEvent> OnKeyboardEvent { get; }
        #endregion

        //These lists will be dispatched by uielementcollection when trigger procs
    }
}