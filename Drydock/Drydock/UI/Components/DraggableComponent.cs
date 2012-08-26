#region

using System;
using Drydock.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.UI.Components{
    internal delegate void DraggableObjectClamp(IUIInteractiveElement owner, ref int x, ref int y, int oldX, int oldY);

    internal delegate void OnComponentDrag(object caller, int dx, int dy);


    /// <summary>
    ///   allows a UI element to be dragged. Required element to be IUIInteractiveComponent
    /// </summary>
    internal class DraggableComponent : IUIComponent{
        bool _isEnabled;
        bool _isMoving;
        Vector2 _mouseOffset;
        IUIInteractiveElement _owner;

        #region properties

        public IUIElement Owner{ //this function acts as kind of a pseudo-constructor
            set{
                if (!(value is IUIInteractiveElement)){
                    throw new Exception("Invalid element componenet: Unable to set a drag component for a non-interactive element.");
                }
                _owner = (IUIInteractiveElement) value;
                ComponentCtor();
            }
        }

        #endregion

        #region ctor

        public DraggableComponent(){
            _mouseOffset = new Vector2();
        }

        void ComponentCtor(){
            _isEnabled = true;
            _isMoving = false;
            _owner.OnLeftButtonPress.Add(OnLeftButtonDown);
            _owner.OnLeftButtonRelease.Add(OnLeftButtonUp);
            _owner.OnMouseMovement.Add(OnMouseMovement);
        }

        #endregion

        #region IUIComponent Members

        public bool IsEnabled{
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public void Update(){
        }

        #endregion

        #region event handlers

        InterruptState OnLeftButtonDown(MouseState state, MouseState? prevState = null){
            if (!_isMoving && _isEnabled){
                if (_owner.BoundingBox.Contains(state.X, state.Y)){
                    _isMoving = true;
                    _owner.Owner.DisableEntryHandlers = true;
                    _mouseOffset.X = _owner.X - state.X;
                    _mouseOffset.Y = _owner.Y - state.Y;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        InterruptState OnLeftButtonUp(MouseState state, MouseState? prevState = null){
            if (_isMoving){
                _isMoving = false;
                _owner.Owner.DisableEntryHandlers = false;
            }
            return InterruptState.AllowOtherEvents;
        }

        InterruptState OnMouseMovement(MouseState state, MouseState? prevState = null){
            if (_isMoving && _isEnabled){
                var nprevState = (MouseState)prevState;
                var oldX = (int) _owner.X;
                var oldY = (int) _owner.Y;
                var x = (int) (state.X + _mouseOffset.X);
                var y = (int) (state.Y + _mouseOffset.Y);

                //var x = (int)(state.X - nprevState.X +_owner.X);
                //var y = (int)(state.Y - nprevState.Y +_owner.Y);


                if (DragMovementClamp != null){
                    DragMovementClamp(_owner, ref x, ref y, oldX, oldY);
                }

                //this block checks if a drag clamp is preventing the owner from moving, if thats the case then kill the drag
                var tempRect = new Rectangle(x - (int) _owner.BoundingBox.Width*2, y - (int) _owner.BoundingBox.Height*2, (int) _owner.BoundingBox.Width*6, (int) _owner.BoundingBox.Height*6);
                if (!tempRect.Contains(state.X, state.Y)){
                    //_isMoving = false;
                    //_owner.Owner.DisableEntryHandlers = false;
                    //return InterruptState.AllowOtherEvents;
                    //int f = 5;
                }

                _owner.X = x;
                _owner.Y = y;

                if (DragMovementDispatcher != null){
                    DragMovementDispatcher(_owner, x - oldX, y - oldY);
                }
                return InterruptState.InterruptEventDispatch;
            }
            return InterruptState.AllowOtherEvents;
        }

        #endregion

        public event OnComponentDrag DragMovementDispatcher;
        public event DraggableObjectClamp DragMovementClamp;
    }
}