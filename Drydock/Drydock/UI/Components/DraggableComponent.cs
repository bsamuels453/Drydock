using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI.Components{
    internal delegate void DraggableObjectClamp(IUIInteractiveElement owner, ref int x, ref int y);

    internal delegate void ReactToDragMovement(IUIInteractiveElement owner, int dx, int dy);

    internal class DraggableComponent : IUIElementComponent{
        private readonly ReactToDragMovement _alertToDragMovement;
        private readonly DraggableObjectClamp _clampNewPosition;
        private bool _isEnabled;
        private bool _isMoving;
        private Vector2 _mouseOffset;
        private IUIInteractiveElement _owner;

        #region properties

        public IUIElement Owner { //this function acts as kind of a pseudo-constructor
            set{
                _owner = (IUIInteractiveElement)value;
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
            _owner.OnLeftButtonDown.Add(OnLeftButtonDown);
            _owner.OnLeftButtonUp.Add(OnLeftButtonUp);
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
            if (_isMoving){
                if (state.LeftButton == ButtonState.Released){
                    _isMoving = false;
                }
            }
            return false;
        }

        private bool OnMouseMovement(MouseState state){
            if (_isMoving){
                int oldX = _owner.X;
                int oldY = _owner.Y;
                var x = (int) (state.X + _mouseOffset.X);
                var y = (int) (state.Y + _mouseOffset.Y);
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