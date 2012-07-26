using Drydock.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic{
    internal class CDraggable : IClickSubbable, IMouseMoveSubbable{
        private static MouseHandler _mouseHandler;
        private readonly IDraggable _parent;
        private bool _isMoving;

        public CDraggable(IDraggable parent){
            _parent = parent;
            _mouseHandler.ClickSubscriptions.Add(this);
            _mouseHandler.MovementSubscriptions.Add(this);
            _isMoving = false;
        }

        public void ManualTranslation(int dx, int dy){
            _parent.X += dx;
            _parent.Y += dy;
        }

        #region properties

        #endregion

        #region IClickSubbable Members

        public bool HandleMouseClickEvent(MouseState state){
            if (!_isMoving){
                if (state.LeftButton == ButtonState.Pressed && _parent.BoundingBox.Contains(state.X, state.Y)){
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

        #endregion

        #region IMouseMoveSubbable Members

        public bool HandleMouseMovementEvent(MouseState state){
            if (_isMoving){
                int oldX = _parent.X;
                int oldY = _parent.Y;
                int x = state.X - _parent.BoundingBox.Width / 2;
                int y = state.Y - _parent.BoundingBox.Height/ 2;
                _parent.ClampDraggedPosition(ref x, ref y);
                _parent.X = x;
                _parent.Y = y;
                _parent.HandleObjectMovement(x - oldX, y - oldY);
            }
            return false;
        }

        #endregion

        #region static initalizer

        public static void Init(MouseHandler mouseHandler){
            _mouseHandler = mouseHandler;
        }

        #endregion
    }

    #region IDraggable

    internal interface IDraggable{
        int X { get; set; }
        int Y { get; set; }
        Rectangle BoundingBox { get; }
        void ClampDraggedPosition(ref int x, ref int y);
        void HandleObjectMovement(int dx, int dy);
    }

    #endregion
}