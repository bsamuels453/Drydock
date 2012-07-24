using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic{
    internal interface IDraggable{
        Dongle2D ElementDongle { get; set; }
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
                    _boundingBox.X = state.X;
                    _boundingBox.Y = state.Y;
                }
            }
        }

        #endregion

        #region IMouseMoveSubbable Members

        public void HandleMouseMovementEvent(MouseState state){
            if (_isMoving){
                _parent.ElementDongle.EditDonglePosition(state.X, state.Y);
            }
        }

        #endregion

        public static void Init(MouseHandler mouseHandler){
            _mouseHandler = mouseHandler;
        }
    }
}