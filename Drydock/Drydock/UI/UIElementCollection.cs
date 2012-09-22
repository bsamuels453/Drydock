#region

using System;
using System.Collections.Generic;
using Drydock.Control;
using Drydock.UI.Components;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.UI{
    /// <summary>
    ///   This class serves as a container for UI elements. Its purpose is to update said elements through the UIContext, and to provide collection-wide modification methods for use by external classes.
    /// </summary>
    internal class UIElementCollection : IUpdatable, IInputUpdatable{
        readonly List<IUIElement> _elements;
        readonly UISortedList _layerSortedIElements;
        public bool DisableEntryHandlers;

        #region ctor

        public UIElementCollection(DepthLevel depth = DepthLevel.Medium){
            _elements = new List<IUIElement>();
            _layerSortedIElements = new UISortedList();
            DisableEntryHandlers = false;
            //InputEventDispatcher.EventSubscribers.Add((float) depth/10, this);
            Add(this);
        }

        #endregion

        #region ui element addition methods

        private void AddElementToCollection(IUIElement elementToAdd){
            _elements.Add(elementToAdd);
        }

        private void AddElementToCollection(IUIInteractiveElement elementToAdd) {
            _layerSortedIElements.Add(elementToAdd.Depth, elementToAdd);
        }

        #endregion

        public void Update(double timeDelta){
            foreach (IUIElement element in _elements){
                element.Update(timeDelta);
            }
        }

        public void InputUpdate(ref ControlState state){
            foreach (IUIElement element in _elements) {
                //element.InputUpdate(ref state);
            }
        }

        #region event dispatchers

        //i'll rip your balls off if you LINQ these foreach loops
        /*public override InterruptState OnMouseMovement(MouseState state, MouseState? prevState = null){
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                foreach (var mouseEvent in _layerSortedIElements[i].OnMouseMovement){
                    if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch){
                        return InterruptState.InterruptEventDispatch;
                    }
                }

                if (_layerSortedIElements[i].BoundingBox.Contains(state.X, state.Y)
                    && !_layerSortedIElements[i].ContainsMouse){
                    //dispatch event for mouse entering the bounding box of the element
                    if (!DisableEntryHandlers){
                        _layerSortedIElements[i].ContainsMouse = true;
                        foreach (var mouseEvent in _layerSortedIElements[i].OnMouseEntry){
                            if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch){
                                return InterruptState.InterruptEventDispatch;
                            }
                        }
                    }
                }

                else{
                    if (!_layerSortedIElements[i].BoundingBox.Contains(state.X, state.Y)
                        && _layerSortedIElements[i].ContainsMouse){
                        //dispatch event for mouse exiting the bounding box of the element
                        _layerSortedIElements[i].ContainsMouse = false;
                        foreach (var mouseEvent in _layerSortedIElements[i].OnMouseExit){
                            if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch){
                                return InterruptState.InterruptEventDispatch;
                            }
                        }
                    }
                }
            }
            return InterruptState.AllowOtherEvents;
        }
        
        public override InterruptState OnLeftButtonClick(MouseState state, MouseState? prevState = null){
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                if (_layerSortedIElements[i].ContainsMouse){

                    foreach (var mouseEvent in _layerSortedIElements[i].OnLeftButtonClick){
                        if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch){
                            return InterruptState.InterruptEventDispatch;
                        }
                    }
                }
                return InterruptState.AllowOtherEvents;
            }
            return InterruptState.AllowOtherEvents;
        }

        public override InterruptState OnLeftButtonPress(MouseState state, MouseState? prevState = null){
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                foreach (var mouseEvent in _layerSortedIElements[i].OnLeftButtonPress){
                    if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch){
                        return InterruptState.InterruptEventDispatch;
                    }
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public override InterruptState OnLeftButtonRelease(MouseState state, MouseState? prevState = null){
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                foreach (var mouseEvent in _layerSortedIElements[i].OnLeftButtonRelease){
                    if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch){
                        return InterruptState.InterruptEventDispatch;
                    }
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public override InterruptState OnMouseScroll(MouseState state, MouseState? prevState = null){
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                foreach (var mouseEvent in _layerSortedIElements[i].OnMouseScroll){
                    if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch){
                        return InterruptState.InterruptEventDispatch;
                    }
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public override InterruptState OnKeyboardEvent(KeyboardState state){
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                foreach (var keyboardEvent in _layerSortedIElements[i].OnKeyboardEvent){
                    keyboardEvent(state);
                }
            }
            return InterruptState.AllowOtherEvents;
        }
        */
        #endregion

        #region collection modification methods

        public void EnableComponents<TComponent>(){
            foreach (var element in _elements){
                if (element.DoesComponentExist<TComponent>()){
                    ((IUIComponent) (element.GetComponent<TComponent>())).IsEnabled = true; //()()()()()((()))
                }
            }
        }

        public void DisableComponents<TComponent>(){
            foreach (var element in _elements){
                if (element.DoesComponentExist<TComponent>()){
                    ((IUIComponent) (element.GetComponent<TComponent>())).IsEnabled = false;
                }
            }
        }

        public void SelectAllElements(){
            foreach (var element in _elements){
                if (element.DoesComponentExist<SelectableComponent>()){
                    element.GetComponent<SelectableComponent>().SelectThis();
                }
            }
        }

        public void DeselectAllElements(){
            foreach (var element in _elements){
                if (element.DoesComponentExist<SelectableComponent>()){
                    element.GetComponent<SelectableComponent>().DeselectThis();
                }
            }
        }

        public void FadeInAllElements(){
            foreach (var element in _elements){
                if (element.DoesComponentExist<FadeComponent>()){
                    element.GetComponent<FadeComponent>().ForceFadein(Mouse.GetState());
                }
            }
        }

        public void FadeOutAllElements(){
            foreach (var element in _elements){
                if (element.DoesComponentExist<FadeComponent>()){
                    element.GetComponent<FadeComponent>().ForceFadeout(Mouse.GetState());
                }
            }
        }

        public void AddFadeCallback(FadeStateChange deleg){
            foreach (var element in _elements){
                if (element.DoesComponentExist<FadeComponent>()){
                    element.GetComponent<FadeComponent>().FadeStateChangeDispatcher += deleg;
                }
            }
        }

        public void AddSelectionCallback(ReactToSelection deleg){
            foreach (var element in _elements){
                if (element.DoesComponentExist<SelectableComponent>()){
                    element.GetComponent<SelectableComponent>().ReactToSelectionDispatcher += deleg;
                }
            }
        }

        public void AddDragCallback(OnComponentDrag deleg){
            foreach (var element in _elements){
                if (element.DoesComponentExist<DraggableComponent>()){
                    element.GetComponent<DraggableComponent>().DragMovementDispatcher += deleg;
                }
            }
        }

        public void AddDragConstraintCallback(DraggableObjectClamp deleg){
            foreach (var element in _elements){
                if (element.DoesComponentExist<DraggableComponent>()){
                    element.GetComponent<DraggableComponent>().DragMovementClamp += deleg;
                }
            }
        }

        #endregion

        #region static stuff
        static readonly List<UIElementCollection> _uiElementCollections;
        static UIElementCollection _curElementCollection;

        static UIElementCollection(){
            _uiElementCollections = new List<UIElementCollection>();
            _curElementCollection = null;
        }

        private static void Add(UIElementCollection collection){
            _uiElementCollections.Add(collection);
        }

        public static void Clear(){
            _uiElementCollections.Clear();
            _curElementCollection = null;
        }

        public static void AddElement(IUIElement elementToAdd) {
            if (_curElementCollection != null) {
                if (elementToAdd is IUIInteractiveElement) {
                    _curElementCollection.AddElementToCollection(elementToAdd as IUIInteractiveElement);
                }
                else{
                    _curElementCollection.AddElementToCollection(elementToAdd);
                }
            }
            else{
                throw new Exception("no uielementcollection bound");
            }
        }

        public static void BindCollection(UIElementCollection collection){
            if (_curElementCollection != null) {
                throw new Exception("the previous bound collection needs to be cleared before a new one is set");
            }
            _curElementCollection = collection;
        }

        public static void UnbindCollection(){
            _curElementCollection = null;
        }

        public static UIElementCollection Collection{
            get { return _curElementCollection; }
        }

        #endregion
    }

    #region uisortedlist

    internal class UISortedList{
        readonly List<float> _depthList;
        readonly List<IUIInteractiveElement> _objList;

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