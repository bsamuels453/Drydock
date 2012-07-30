using Drydock.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic.InterfaceObj{
    internal class CDraggable{
        private readonly IDraggable _parent;
        private bool _isMoving;

        public CDraggable(IDraggable parent){
            _parent = parent;
            MouseHandler.ClickSubscriptions.Add(HandleMouseClickEvent);
            MouseHandler.MovementSubscriptions.Add(HandleMouseMovementEvent);
            _isMoving = false;
        }

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

        #region properties

        #endregion

        #region event handlers

        public bool HandleMouseMovementEvent(MouseState state){
            if (_isMoving){
                int oldX = _parent.X;
                int oldY = _parent.Y;
                int x = state.X - _parent.BoundingBox.Width/2;
                int y = state.Y - _parent.BoundingBox.Height/2;
                _parent.ClampDraggedPosition(ref x, ref y);
                _parent.X = x;
                _parent.Y = y;
                _parent.HandleObjectMovement(x - oldX, y - oldY);
            }
            return false;
        }

        #endregion

        #region static initalizer

        public static void Init(){
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