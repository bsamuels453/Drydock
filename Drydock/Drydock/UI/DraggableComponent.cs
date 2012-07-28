using Drydock.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI{
    internal delegate void DraggableObjectClamp(Button owner, ref int x, ref int y);
    internal delegate void ReactToDragMovement(Button owner, int dx, int dy);

    internal class DraggableComponent : IButtonComponent{
        private readonly DraggableObjectClamp _clampNewPosition;
        private readonly ReactToDragMovement _alertToDragMovement;
        private bool _isEnabled;
        private bool _isMoving;
        private Button _owner;
        private Vector2 _mouseOffset;

        #region properties

        public Button Owner{ //this function acts as kind of a pseudo-constructor
            set{
                _owner = value;
                _isEnabled = true;
                _isMoving = false;
                //_owner.OnLeftDownDelegates.Add(MouseClickHandler);
                _owner.OnLeftButtonDown.Add(OnLeftButtonDown);
                _owner.OnLeftButtonUp.Add(OnLeftButtonUp);
                _owner.OnMouseMovement.Add(OnMouseMovement);
            }
        }

        #endregion

        public DraggableComponent(DraggableObjectClamp clampFunction=null, ReactToDragMovement alertToDragMovement=null){
            _clampNewPosition = clampFunction;
            _alertToDragMovement = alertToDragMovement;
            _mouseOffset = new Vector2();
        }

        #region IButtonComponent Members

        public bool IsEnabled{
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public void Update(){
        }

        #endregion

        #region event handlers
        private bool OnLeftButtonDown(MouseState state){
            if (!_isMoving){
                if (_owner.BoundingBox.Contains(state.X, state.Y)){
                    _isMoving = true;
                    _mouseOffset.X = _owner.X - state.X;
                    _mouseOffset.Y = _owner.Y - state.Y;
                    return true;
                }
            }
        return false;
        }

        private bool OnLeftButtonUp(MouseState state){
            if (_isMoving) {
                if (state.LeftButton == ButtonState.Released) {
                    _isMoving = false;
                }
            }
            return false;
        }

        private bool OnMouseMovement(MouseState state){
            if (_isMoving){
                int oldX = _owner.X;
                int oldY = _owner.Y;
                int x = (int)(state.X +  _mouseOffset.X);
                int y = (int)(state.Y +  _mouseOffset.Y);
                if (_clampNewPosition != null){
                    _clampNewPosition(_owner, ref x, ref y);
                }
                _owner.X = x;
                _owner.Y = y;

                if (_alertToDragMovement != null){
                    _alertToDragMovement(_owner, x - oldX, y - oldY);
                }
            }
            return false;
        }
        #endregion
    }
}