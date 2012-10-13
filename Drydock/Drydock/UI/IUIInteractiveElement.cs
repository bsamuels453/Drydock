#region

using System.Collections.Generic;
using Drydock.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.UI{
    internal delegate void OnBasicMouseEvent(int identifier);

    internal interface IUIInteractiveElement : IUIElement, IInputUpdates{
        bool ContainsMouse { get; set; }

        #region event dispatch lists

        List<IAcceptLeftButtonClickEvent> OnLeftButtonClick { get; } //procs when left mouse button is pressed and released within a time interval (global)
        List<IAcceptLeftButtonPressEvent> OnLeftButtonPress { get; } //procs when left mouse button is pressed (global)
        List<IAcceptLeftButtonReleaseEvent> OnLeftButtonRelease { get; } //procs when left mouse button is released (global)
        List<IAcceptMouseEntryEvent> OnMouseEntry { get; } //procs when mouse enters the bounding box of the element
        List<IAcceptMouseExitEvent> OnMouseExit { get; } //procs when the mouse exits the bounding box of the element
        List<IAcceptMouseMovementEvent> OnMouseMovement { get; } //procs on global mouse movement
        List<IAcceptMouseScrollEvent> OnMouseScroll { get; }
        List<IAcceptKeyboardEvent> OnKeyboardEvent { get; }

        #endregion

        event OnBasicMouseEvent OnLeftClickDispatcher;
        event OnBasicMouseEvent OnLeftPressDispatcher;
        event OnBasicMouseEvent OnLeftReleaseDispatcher;
    }

    #region internal event handling interfaces

    //each mouse based event accepts something along the lines of (ref bool allowInterpretation, Vector2 mousePosition, Vector2 mousePosChange)
    internal interface IAcceptLeftButtonClickEvent{
        void OnLeftButtonClick(ref bool allowInterpretation, Point mousePos, Point prevMousePos);
    }

    internal interface IAcceptLeftButtonPressEvent{
        void OnLeftButtonPress(ref bool allowInterpretation, Point mousePos, Point prevMousePos);
    }

    internal interface IAcceptLeftButtonReleaseEvent{
        void OnLeftButtonRelease(ref bool allowInterpretation, Point mousePos, Point prevMousePos);
    }

    internal interface IAcceptMouseEntryEvent{
        void OnMouseEntry(ref bool allowInterpretation, Point mousePos, Point prevMousePos);
    }

    internal interface IAcceptMouseExitEvent{
        void OnMouseExit(ref bool allowInterpretation, Point mousePos, Point prevMousePos);
    }

    internal interface IAcceptMouseMovementEvent{
        void OnMouseMovement(ref bool allowInterpretation, Point mousePos, Point prevMousePos);
    }

    internal interface IAcceptMouseScrollEvent{
        void OnMouseScrollwheel(ref bool allowInterpretation, float wheelChange, Point mousePos);
    }

    internal interface IAcceptKeyboardEvent{
        void OnKeyboardEvent(ref bool allowInterpretation, KeyboardState state);
    }

    #endregion
}