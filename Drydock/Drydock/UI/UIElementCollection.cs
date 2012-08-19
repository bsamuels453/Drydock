#region

using System.Collections.Generic;
using Drydock.Control;
using Drydock.UI.Components;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.UI{
    /// <summary>
    ///   This class serves as a container for UI elements. Its purpose is to update said elements through the UIContext, and to provide collection-wide modification methods for use by external classes.
    /// </summary>
    internal class UIElementCollection : CanReceiveInputEvents{
        private readonly List<UIElementCollection> _childCollections;
        private readonly List<IUIElement> _elements;
        private readonly UISortedList _layerSortedIElements;
        public bool DisableEntryHandlers;

        #region ctor

        public UIElementCollection(DepthLevel depth){
            _elements = new List<IUIElement>();
            _layerSortedIElements = new UISortedList();
            _childCollections = new List<UIElementCollection>();
            UIContext.Add(this);
            DisableEntryHandlers = false;

            InputEventDispatcher.EventSubscribers.Add((float)depth/10, this);
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

        public UIElementCollection Add(UIElementCollection collectionToAdd){
            _childCollections.Add(collectionToAdd);
            //collectionToAdd.DepthManager.Depth = DepthManager.Depth + 1;
            return collectionToAdd;
        }

        #endregion

        #region disposal methods

        public void DisposeElement(IUIElement element){
            if (element is IUIInteractiveElement){
                _layerSortedIElements.Remove(element);
            }
            _elements.Remove(element);
        }

        public void DisposeCollection(UIElementCollection collection){
            _childCollections.Remove(collection);
        }

        #endregion

        public void Update(){
            foreach (IUIElement element in _elements){
                element.Update();
            }
            foreach (var collection in _childCollections){
                collection.Update();
            }
        }

        #region event dispatchers

        //i'll rip your balls off if you LINQ these foreach loops
        public override InterruptState OnMouseMovement(MouseState state, MouseState? prevState = null) {
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                foreach (var mouseEvent in _layerSortedIElements[i].OnMouseMovement){
                    if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch){
                        return InterruptState.InterruptEventDispatch;
                    }
                }

                if (_layerSortedIElements[i].BoundingBox.Contains(state.X, state.Y)
                    && !_layerSortedIElements[i].ContainsMouse) {
                    //dispatch event for mouse entering the bounding box of the element
                    if (!DisableEntryHandlers) {
                        _layerSortedIElements[i].ContainsMouse = true;
                        foreach (var mouseEvent in _layerSortedIElements[i].OnMouseEntry) {
                            if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch) {
                                return InterruptState.InterruptEventDispatch;
                            }
                        }
                    }
                }

                else{
                    if (!_layerSortedIElements[i].BoundingBox.Contains(state.X, state.Y)
                        && _layerSortedIElements[i].ContainsMouse) {
                        
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

            if (!DisableEntryHandlers){ //disabling entry handlers disables movement dispatch for child collections
                foreach (var collection in _childCollections){
                    if (collection.OnMouseMovement(state, prevState) == InterruptState.InterruptEventDispatch){
                        return InterruptState.InterruptEventDispatch;
                        ;
                    }
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public override InterruptState OnLeftButtonClick(MouseState state, MouseState? prevState = null) {
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                foreach (var mouseEvent in _layerSortedIElements[i].OnLeftButtonClick){
                    if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch){
                        return InterruptState.InterruptEventDispatch;
                    }
                }
            }
            foreach (var collection in _childCollections){
                if (collection.OnLeftButtonClick(state, prevState) == InterruptState.InterruptEventDispatch) {
                    return InterruptState.InterruptEventDispatch;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public override InterruptState OnLeftButtonPress(MouseState state, MouseState? prevState = null) {
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                foreach (var mouseEvent in _layerSortedIElements[i].OnLeftButtonPress){
                    if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch) {
                        return InterruptState.InterruptEventDispatch;
                    }
                }
            }
            foreach (var collection in _childCollections){
                if (collection.OnLeftButtonPress(state, prevState) == InterruptState.InterruptEventDispatch) {
                    return InterruptState.InterruptEventDispatch;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public override InterruptState OnLeftButtonRelease(MouseState state, MouseState? prevState = null) {
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                foreach (var mouseEvent in _layerSortedIElements[i].OnLeftButtonRelease){
                    if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch) {
                        return InterruptState.InterruptEventDispatch;
                    }
                }
            }
            foreach (var collection in _childCollections){
                if (collection.OnLeftButtonRelease(state, prevState) == InterruptState.InterruptEventDispatch) {
                    return InterruptState.InterruptEventDispatch;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public override InterruptState OnMouseScroll(MouseState state, MouseState? prevState = null) {
            for (int i = 0; i < _layerSortedIElements.Count; i++) {
                foreach (var mouseEvent in _layerSortedIElements[i].OnMouseScroll) {
                    if (mouseEvent(state, prevState) == InterruptState.InterruptEventDispatch) {
                        return InterruptState.InterruptEventDispatch;
                    }
                }
            }
            foreach (var collection in _childCollections) {
                if (collection.OnMouseScroll(state, prevState) == InterruptState.InterruptEventDispatch) {
                    return InterruptState.InterruptEventDispatch;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public override InterruptState OnKeyboardEvent(KeyboardState state) {
            for (int i = 0; i < _layerSortedIElements.Count; i++){
                foreach (var keyboardEvent in _layerSortedIElements[i].OnKeyboardEvent){
                    keyboardEvent(state);
                }
            }
            foreach (var collection in _childCollections){
                collection.OnKeyboardEvent(state);
            }
            return InterruptState.AllowOtherEvents;
        }

        #endregion

        #region collection modification methods

        public void EnableComponents<TComponent>(){
            foreach (var element in _elements){
                if (element.DoesComponentExist<TComponent>()){
                    ((IUIComponent) (element.GetComponent<TComponent>())).IsEnabled = true; //()()()()()((()))
                }
            }
            //propogate changes to children
            foreach (var collection in _childCollections){
                collection.EnableComponents<TComponent>();
            }
        }

        public void DisableComponents<TComponent>(){
            foreach (var element in _elements){
                if (element.DoesComponentExist<TComponent>()){
                    ((IUIComponent) (element.GetComponent<TComponent>())).IsEnabled = false;
                }
            }
            foreach (var collection in _childCollections){
                collection.DisableComponents<TComponent>();
            }
        }

        public void SelectAllElements(){
            foreach (var element in _elements){
                if (element.DoesComponentExist<SelectableComponent>()){
                    element.GetComponent<SelectableComponent>().SelectThis();
                }
            }
            foreach (var collection in _childCollections){
                collection.SelectAllElements();
            }
        }

        public void DeselectAllElements(){
            foreach (var element in _elements){
                if (element.DoesComponentExist<SelectableComponent>()){
                    element.GetComponent<SelectableComponent>().DeselectThis();
                }
            }
            foreach (var collection in _childCollections){
                collection.DeselectAllElements();
            }
        }

        public void FadeInAllElements(){
            foreach (var element in _elements){
                if (element.DoesComponentExist<FadeComponent>()){
                    element.GetComponent<FadeComponent>().ForceFadein(Mouse.GetState());
                }
            }
            foreach (var collection in _childCollections){
                collection.FadeInAllElements();
            }
        }

        public void FadeOutAllElements(){
            foreach (var element in _elements){
                if (element.DoesComponentExist<FadeComponent>()){
                    element.GetComponent<FadeComponent>().ForceFadeout(Mouse.GetState());
                }
            }
            foreach (var collection in _childCollections){
                collection.FadeOutAllElements();
            }
        }

        public void AddFadeCallback(FadeStateChange deleg){
            foreach (var element in _elements){
                if (element.DoesComponentExist<FadeComponent>()){
                    element.GetComponent<FadeComponent>().FadeStateChangeDispatcher += deleg;
                }
            }
            foreach (var collection in _childCollections){
                collection.AddFadeCallback(deleg);
            }
        }

        public void AddSelectionCallback(ReactToSelection deleg){
            foreach (var element in _elements){
                if (element.DoesComponentExist<SelectableComponent>()){
                    element.GetComponent<SelectableComponent>().ReactToSelectionDispatcher += deleg;
                }
            }
            foreach (var collection in _childCollections){
                collection.AddSelectionCallback(deleg);
            }
        }

        public void AddDragCallback(OnComponentDrag deleg){
            foreach (var element in _elements){
                if (element.DoesComponentExist<DraggableComponent>()){
                    element.GetComponent<DraggableComponent>().DragMovementDispatcher += deleg;
                }
            }
            foreach (var collection in _childCollections){
                collection.AddDragCallback(deleg);
            }
        }

        public void AddDragConstraintCallback(DraggableObjectClamp deleg){
            foreach (var element in _elements){
                if (element.DoesComponentExist<DraggableComponent>()){
                    element.GetComponent<DraggableComponent>().DragMovementClamp += deleg;
                }
            }
            foreach (var collection in _childCollections){
                collection.AddDragConstraintCallback(deleg);
            }
        }

        #endregion
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