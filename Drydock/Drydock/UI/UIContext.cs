﻿using System.Collections.Generic;
using Drydock.Control;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI{

    internal static class UIContext{
        private static List<IUIElement> _elements;
        private static UISortedList _layerSortedIElements;
        private static MouseState _prevMouseState;
        public static bool DisableEntryHandlers;

        #region ctor

        public static void Init(){
            _elements = new List<IUIElement>();
            _layerSortedIElements = new UISortedList();

            _prevMouseState = Mouse.GetState();
            DisableEntryHandlers = false;

            InputEventDispatcher.OnKeyboardDispatcher.Add(OnKeyboardAction);
            InputEventDispatcher.OnMMovementDispatcher.Add(OnMouseMovementEvent);
            InputEventDispatcher.OnMButtonDispatcher.Add(OnMouseButtonEvent);
        }

        #endregion

        #region ui element addition methods

        public static TElement Add<TElement>(IUIElement elementToAdd){
            _elements.Add(elementToAdd);
            return (TElement) elementToAdd;
        }

        public static TElement Add<TElement>(IUIInteractiveElement elementToAdd){
            _elements.Add(elementToAdd);

            _layerSortedIElements.Add(elementToAdd.Depth, elementToAdd);

            return (TElement) elementToAdd;
        }

        #endregion

        public static void Update(){
            foreach (IUIElement element in _elements){
                element.Update();
            }
        }

        public static void DisposeElement(IUIElement element){
            if (IsElementInteractive(element)){
                _layerSortedIElements.Remove(element);
            }
        }

        #region event handlers

        private static InterruptState OnMouseButtonEvent(MouseState state) {
            InterruptState retval = InterruptState.AllowOtherEvents;
            bool forceListCleanup = false;
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                if (_layerSortedIElements[i] != null){
                    if (_layerSortedIElements[i].MouseClickHandler(state)){
                        retval = InterruptState.AllowOtherEvents; ;
                        break;
                    }
                }
                else{
                    forceListCleanup = true;
                }
            }
            if (forceListCleanup){
                for (int i = 0; i < _layerSortedIElements.Count; i++){
                    if (_layerSortedIElements[i] == null){
                        _layerSortedIElements.RemoveAt(i);
                    }
                }
            }

            return retval;
        }

        private static InterruptState OnMouseMovementEvent(MouseState state) {
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                _layerSortedIElements[i].MouseMovementHandler(state);


                if (_layerSortedIElements[i].BoundingBox.Contains(state.X, state.Y) && !_layerSortedIElements[i].BoundingBox.Contains(_prevMouseState.X, _prevMouseState.Y)){
                    //dispatch event for mouse entering the bounding box of the element

                    if (!DisableEntryHandlers){
                        if (_layerSortedIElements[i].MouseEntryHandler(state)){
                            break;
                        }
                    }
                }

                else{
                    if (!_layerSortedIElements[i].BoundingBox.Contains(state.X, state.Y) && _layerSortedIElements[i].BoundingBox.Contains(_prevMouseState.X, _prevMouseState.Y)){
                        //dispatch event for mouse exiting the bounding box of the element
                        _layerSortedIElements[i].MouseExitHandler(state);
                    }
                }
            }
            _prevMouseState = state;
            return InterruptState.AllowOtherEvents;
        }

        private static InterruptState OnKeyboardAction(KeyboardState state) {
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                if (_layerSortedIElements[i].KeyboardActionHandler(state)){
                    break;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        #endregion

        public static bool IsElementInteractive(IUIElement element){
            return element is IUIInteractiveElement;
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