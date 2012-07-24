using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic{
    internal interface IDraggable{
        Dongle2D ElementDongle { get; set; }
        void ClampDraggedPosition(ref int x, ref int y);
    }

    internal class CDraggable : IClickSubbable, IMouseMoveSubbable{
        private static MouseHandler _mouseHandler;
        private readonly IDraggable _parent;
        private Rectangle _boundingBox;
        private bool _isMoving;

        public CDraggable(IDraggable parent, int initX, int initY, int width, int height){
            _parent = parent;
            _mouseHandler.ClickSubscriptions.Add(this);
            _mouseHandler.MovementSubscriptions.Add(this);
            _boundingBox = new Rectangle(initX, initY, width, height);
            _isMoving = false;
        }

        #region IClickSubbable Members

        public void HandleMouseClickEvent(MouseState state){
            if (!_isMoving){
                if (state.LeftButton == ButtonState.Pressed && _boundingBox.Contains(state.X, state.Y)){
                    _isMoving = true;
                    return;
                }
            }
            if (_isMoving){
                if (state.LeftButton == ButtonState.Released){
                    _isMoving = false;
                    _boundingBox.X = state.X-3;
                    _boundingBox.Y = state.Y-3;
                }
            }
        }

        #endregion

        #region IMouseMoveSubbable Members

        public void HandleMouseMovementEvent(MouseState state){
            if (_isMoving){
                int x = state.X - 3;
                int y = state.Y - 3;
                _parent.ClampDraggedPosition(ref x, ref y);
                _parent.ElementDongle.EditDonglePosition(x, y);
            }
        }

        #endregion

        public static void Init(MouseHandler mouseHandler){
            _mouseHandler = mouseHandler;
        }
    }
}