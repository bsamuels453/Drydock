using Drydock.Control;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI{
    internal delegate void DraggableObjectClamp(ref int x, ref int y);
    internal delegate void ReactToDragMovement(int dx, int dy);

    internal class DraggableComponent : IButtonComponent{
        private readonly DraggableObjectClamp _clampNewPosition;
        private readonly ReactToDragMovement _alertToDragMovement;
        private bool _isEnabled;
        private bool _isMoving;
        private Button _owner;

        #region properties

        public Button Owner{ //this function acts as kind of a pseudo-constructor
            set{
                _owner = value;
                _isEnabled = true;
                _isMoving = false;
                MouseHandler.ClickSubscriptions.Add(MouseClickHandler);
                MouseHandler.MovementSubscriptions.Add(MouseMovementHandler);
            }
        }

        #endregion

        public DraggableComponent(DraggableObjectClamp clampFunction=null, ReactToDragMovement alertToDragMovement=null){
            _clampNewPosition = clampFunction;
            _alertToDragMovement = alertToDragMovement;
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
        private bool MouseClickHandler(MouseState state){
            if (!_isMoving){
                if (state.LeftButton == ButtonState.Pressed && _owner.BoundingBox.Contains(state.X, state.Y)){
                    _isMoving = true;
                    return true;
                }
            }
            if (_isMoving){
                if (state.LeftButton == ButtonState.Released){
                    _isMoving = false;
                }
            }
            return false;
        }

        private bool MouseMovementHandler(MouseState state){
            if (_isMoving){
                int oldX = _owner.X;
                int oldY = _owner.Y;
                int x = state.X - _owner.BoundingBox.Width/2;
                int y = state.Y - _owner.BoundingBox.Height/2;
                if (_clampNewPosition != null){
                    _clampNewPosition(ref x, ref y);
                }
                _owner.X = x;
                _owner.Y = y;

                if (_alertToDragMovement != null){
                    _alertToDragMovement(x - oldX, y - oldY);
                }
            }
            return false;
        }
        #endregion
    }
}