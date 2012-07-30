using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Control;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI {
    internal delegate bool OnMouseAction(MouseState state);
    class UIContext {
        private readonly List<IUIElement> _elements;
        private readonly SortedList<float, IUIInteractiveElement> _layerSortedElements;
        private readonly MouseState _prevMouseState;

        #region ctor
        public UIContext(){
            _elements = new List<IUIElement>();
            _layerSortedElements = new SortedList<float, IUIInteractiveElement>();

            _prevMouseState = Mouse.GetState();

            MouseHandler.ClickSubscriptions.Add(OnMouseButtonEvent);
            MouseHandler.MovementSubscriptions.Add(OnMouseMovementEvent);
        }
        #endregion

        #region ui element addition methods
        public TElement Add<TElement>(IUIElement elementToAdd){
            _elements.Add(elementToAdd);
            return (TElement)elementToAdd;
        }

        public TElement Add<TElement>(IUIInteractiveElement elementToAdd) {
            _elements.Add(elementToAdd);
            _layerSortedElements.Add(elementToAdd.LayerDepth, elementToAdd);

            return (TElement)elementToAdd;
        }
        #endregion

        public void Update(){
            foreach (var element in _elements){
                element.Update();
            }
        }

        #region event handlers
        private bool OnMouseButtonEvent(MouseState state){
            bool retval = false;
            bool forceListCleanup = false;
            foreach (var element in _layerSortedElements){
                if (element.Value != null) {
                    if (element.Value.BoundingBox.Contains(state.X, state.Y)) {
                        if (element.Value.MouseClickHandler(state)){
                            retval = true;
                            break;
                        }
                    }
                }
                else{
                    forceListCleanup = true;
                }
            }
            if (forceListCleanup){
                for (int i = 0; i < _layerSortedElements.Count; i++){
                    if (_layerSortedElements[i] == null){
                        _layerSortedElements.RemoveAt(i);
                    }
                }
            }

            return retval;
        }

        private bool OnMouseMovementEvent(MouseState state){
            foreach (var element in _layerSortedElements){

                //dispatch event for mouse movement
                element.Value.MouseMovementHandler(state);

                if (element.Value.BoundingBox.Contains(state.X, state.Y) && !element.Value.BoundingBox.Contains(_prevMouseState.X, _prevMouseState.Y)) {
                    //dispatch event for mouse entering the bounding box of the element
                    element.Value.MouseEntryHandler(state);
                }
                else{
                    if (!element.Value.BoundingBox.Contains(state.X, state.Y) && element.Value.BoundingBox.Contains(_prevMouseState.X, _prevMouseState.Y)){
                        //dispatch event for mouse exiting the bounding box of the element
                        element.Value.MouseExitHandler(state);
                    }
                }
            }

            return false;
        }
        #endregion

    }
}
