using System;
using Drydock.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI.Components{
    internal delegate void DraggableObjectClamp(IUIInteractiveElement owner, ref int x, ref int y, int oldX, int oldY);

    internal delegate void ReactToDragMovement(IUIInteractiveElement owner, int dx, int dy);


    /// <summary>
    /// allows a UI element to be dragged. Required element to be IUIInteractiveComponent
    /// </summary>
    internal class DraggableComponent : IUIElementComponent{
        private readonly ReactToDragMovement _alertToDragMovement;
        private readonly DraggableObjectClamp _clampNewPosition;
        private bool _isEnabled;
        private bool _isMoving;
        private Vector2 _mouseOffset;
        private IUIInteractiveElement _owner;

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

        public DraggableComponent(DraggableObjectClamp clampFunction = null, ReactToDragMovement alertToDragMovement = null){
            _clampNewPosition = clampFunction;
            _alertToDragMovement = alertToDragMovement;
            _mouseOffset = new Vector2();
        }

        private void ComponentCtor(){

            _isEnabled = true;
            _isMoving = false;
            _owner.OnLeftButtonPress.Add(OnLeftButtonDown);
            _owner.OnLeftButtonRelease.Add(OnLeftButtonUp);
            _owner.OnMouseMovement.Add(OnMouseMovement);
        }

        #endregion

        #region IUIElementComponent Members

        public bool IsEnabled{
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public void Update(){
        }

        #endregion

        #region event handlers

        private void OnLeftButtonDown(MouseState state){
            if (!_isMoving){
                if (_owner.BoundingBox.Contains(state.X, state.Y)){
                    _isMoving = true;
                    _owner.Owner.DisableEntryHandlers = true;
                    _mouseOffset.X = _owner.X - state.X;
                    _mouseOffset.Y = _owner.Y - state.Y;
                }
            }
        }

        private void OnLeftButtonUp(MouseState state) {
            if (_isMoving){
                if (state.LeftButton == ButtonState.Released){
                    _isMoving = false;
                    _owner.Owner.DisableEntryHandlers = false;
                }
            }
        }

        private void OnMouseMovement(MouseState state) {
            if (_isMoving){
                var oldX = (int)_owner.X;
                var oldY = (int)_owner.Y;
                var x = (int) (state.X + _mouseOffset.X);
                var y = (int) (state.Y + _mouseOffset.Y);
                if (_clampNewPosition != null){
                    _clampNewPosition(_owner, ref x, ref y, oldX, oldY);
                }
                _owner.X = x;
                _owner.Y = y;

                if (_alertToDragMovement != null){
                    _alertToDragMovement(_owner, x - oldX, y - oldY);
                }
            }
        }

        #endregion
    }
}