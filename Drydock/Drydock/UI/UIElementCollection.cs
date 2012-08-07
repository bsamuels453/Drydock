using System.Collections.Generic;
using System.Diagnostics;
using Drydock.Control;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI{

    internal class UIElementCollection : ICanReceiveInputEvents{
        private readonly List<IUIElement> _elements;
        private readonly UISortedList _layerSortedIElements;
        private readonly List<UIElementCollection> _childCollections;
        private MouseState _prevMouseState;
        public bool DisableEntryHandlers;

        #region ctor

        public UIElementCollection(){
            _elements = new List<IUIElement>();
            _layerSortedIElements = new UISortedList();
            _childCollections = new List<UIElementCollection>();
            ElementCollectionUpdater.Add(this);
            _prevMouseState = Mouse.GetState();
            DisableEntryHandlers = false;

            InputEventDispatcher.EventSubscribers.Add(this);
        }

        #endregion

        #region ui element addition methods

        public TElement Add<TElement>(IUIElement elementToAdd){
            _elements.Add(elementToAdd);
            elementToAdd.Owner = this;

            return (TElement) elementToAdd;
        }

        public TElement Add<TElement>(IUIInteractiveElement elementToAdd){
            _elements.Add(elementToAdd);

            _layerSortedIElements.Add(elementToAdd.Depth, elementToAdd);
            elementToAdd.Owner = this;

            return (TElement) elementToAdd;
        }

        #endregion

        public void Update(){
            foreach (IUIElement element in _elements){
                element.Update();
            }
        }

        public void DisposeElement(IUIElement element){
            if (element is IUIInteractiveElement){
                _layerSortedIElements.Remove(element);
            }
            _elements.Remove(element);
        }

        public InterruptState OnMouseMovement(MouseState state){
            for (int i = 0; i < _layerSortedIElements.Count; i++) {
                if (_layerSortedIElements[i].OnMouseMovement(state) == InterruptState.InterruptEventDispatch){
                    _prevMouseState = state;
                    return InterruptState.InterruptEventDispatch;
                }

                if (_layerSortedIElements[i].BoundingBox.Contains(state.X, state.Y) && !_layerSortedIElements[i].BoundingBox.Contains(_prevMouseState.X, _prevMouseState.Y)) {
                    //dispatch event for mouse entering the bounding box of the element
                    if (!DisableEntryHandlers) {
                        if (_layerSortedIElements[i].OnMouseEntry(state) == InterruptState.InterruptEventDispatch){
                            _prevMouseState = state;
                            return InterruptState.InterruptEventDispatch;
                        }
                    }
                }

                else {
                    if (!_layerSortedIElements[i].BoundingBox.Contains(state.X, state.Y) && _layerSortedIElements[i].BoundingBox.Contains(_prevMouseState.X, _prevMouseState.Y)) {
                        //dispatch event for mouse exiting the bounding box of the element
                        if (_layerSortedIElements[i].OnMouseExit(state) == InterruptState.InterruptEventDispatch){
                            _prevMouseState = state;
                            return InterruptState.InterruptEventDispatch;
                        }
                    }
                }
            }
            _prevMouseState = state;
            return InterruptState.AllowOtherEvents;
        }

        public InterruptState OnLeftButtonClick(MouseState state){
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                if (_layerSortedIElements[i].OnLeftButtonClick(state) == InterruptState.InterruptEventDispatch){
                    return InterruptState.InterruptEventDispatch;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public InterruptState OnLeftButtonPress(MouseState state) {
            for (int i = 0; i < _layerSortedIElements.Count; i++) {
                if (_layerSortedIElements[i].OnLeftButtonPress(state) == InterruptState.InterruptEventDispatch) {
                    return InterruptState.InterruptEventDispatch;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public InterruptState OnLeftButtonRelease(MouseState state){
            for (int i = 0; i < _layerSortedIElements.Count; i++) {
                if (_layerSortedIElements[i].OnLeftButtonRelease(state) == InterruptState.InterruptEventDispatch) {
                    return InterruptState.InterruptEventDispatch;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public InterruptState OnKeyboardEvent(KeyboardState state){
            for (int i = 0; i < _layerSortedIElements.Count; i++) {
                if (_layerSortedIElements[i].OnKeyboardEvent(state) == InterruptState.InterruptEventDispatch) {
                    return InterruptState.InterruptEventDispatch;
                }
            }
            return InterruptState.AllowOtherEvents;
        }
    }

    #region uisortedlist

    internal class UISortedList{
        private readonly List<float> _depthList;
        private readonly List<IUIInteractiveElement> _objList;

        public UISortedList(){
            _depthList = new List<float>();
            _objList = new List<IUIInteractiveElement>();
        }

        public int Count{
            get { return _depthList.Count; }
        }

        public IUIInteractiveElement this[int index]{
            get { return _objList[index]; }
        }

        public void Add(float depth, IUIInteractiveElement element){
            _depthList.Add(depth);
            _objList.Add(element);

            for (int i = _depthList.Count - 1; i < 0; i--){
                if (_depthList[i] < _depthList[i - 1]){
                    _depthList.RemoveAt(i);
                    _objList.RemoveAt(i);
                    _depthList.Insert(i - 2, depth);
                    _objList.Insert(i - 2, element);
                }
                else{
                    break;
                }
            }
        }

        public void Clear(){
            _depthList.Clear();
            _objList.Clear();
        }

        public void RemoveAt(int index){
            _depthList.RemoveAt(index);
            _objList.RemoveAt(index);
        }

        public void Remove(IUIElement element){
            int i = 0;
            while (_objList[i] != element){
                i++;
                if (i == _objList.Count){
                    //return;
                }
            }
            RemoveAt(i);
        }
    }

    #endregion
}