using System;
using System.Collections;
using System.Collections.Generic;
using Drydock.Control;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI{
    internal delegate bool OnMouseAction(MouseState state);

    internal static class UIContext{
        private static List<IUIElement> _elements;
        private static UISortedList _layerSortedElements;
        private static MouseState _prevMouseState;

        #region ctor

        public static void Init() {
            _elements = new List<IUIElement>();
            _layerSortedElements = new UISortedList();

            _prevMouseState = Mouse.GetState();

            MouseHandler.ClickSubscriptions.Add(OnMouseButtonEvent);
            MouseHandler.MovementSubscriptions.Add(OnMouseMovementEvent);
        }

        #endregion

        #region ui element addition methods

        static public TElement Add<TElement>(IUIElement elementToAdd) {
            _elements.Add(elementToAdd);
            return (TElement) elementToAdd;
        }

        static public TElement Add<TElement>(IUIInteractiveElement elementToAdd) {
            _elements.Add(elementToAdd);

            _layerSortedElements.Add(elementToAdd.LayerDepth, elementToAdd);

            return (TElement) elementToAdd;
        }

        #endregion

        static public void Update() {
            foreach (IUIElement element in _elements){
                element.Update();
            }
        }

        #region event handlers

        static private bool OnMouseButtonEvent(MouseState state) {
            bool retval = false;
            bool forceListCleanup = false;
            for (int i = 0; i < _layerSortedElements.Count; i++) {
                if (_layerSortedElements[i] != null){
                    if (_layerSortedElements[i].MouseClickHandler(state)) {
                        retval = true;
                        break;
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

        static private bool OnMouseMovementEvent(MouseState state) {
            for (int i = 0; i < _layerSortedElements.Count; i++){
                _layerSortedElements[i].MouseMovementHandler(state);
                if (_layerSortedElements[i].BoundingBox.Contains(state.X, state.Y) && !_layerSortedElements[i].BoundingBox.Contains(_prevMouseState.X, _prevMouseState.Y)) {
                    //dispatch event for mouse entering the bounding box of the element
                    _layerSortedElements[i].MouseEntryHandler(state);
                }
                else {
                    if (!_layerSortedElements[i].BoundingBox.Contains(state.X, state.Y) && _layerSortedElements[i].BoundingBox.Contains(_prevMouseState.X, _prevMouseState.Y)) {
                        //dispatch event for mouse exiting the bounding box of the element
                        _layerSortedElements[i].MouseExitHandler(state);
                    }
                }
            }
            _prevMouseState = state;
            return false;
        }

        #endregion
    }


    internal class UISortedList{
        private readonly List<float> _depthList;
        private readonly List<IUIInteractiveElement> _objList;

        public UISortedList(){
            _depthList = new List<float>();
            _objList = new List<IUIInteractiveElement>();
        }

        public void Add(float depth, IUIInteractiveElement element){
            
        }

        public void Clear(){
            _depthList.Clear();
            _objList.Clear();
        }

        public int Count{
            get { return _depthList.Count; }
        }


        public void RemoveAt(int index){
            _depthList.RemoveAt(index);
            _objList.RemoveAt(index);
        }

        public IUIInteractiveElement this[int index] {
            get { return _objList[index]; }
        }
    }
}