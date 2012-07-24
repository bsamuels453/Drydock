using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic {
    interface IDraggable {
        Dongle2D ElementDongle { get; set; }
    }

    class CDraggable : IClickSubbable, IMouseMoveSubbable{
        private readonly IDraggable _parent;
        private bool _isMoving;
        private Rectangle _boundingBox;

        public CDraggable(IDraggable parent, int initX, int initY, int width, int height){
            _parent = parent;
            _mouseHandler.ClickSubscriptions.Add(this);
            _mouseHandler.MovementSubscriptions.Add(this);
            _boundingBox = new Rectangle(initX, initY, width, height);
            _isMoving = false;
        }

        public void HandleMouseClickEvent(MouseState state){
            if (!_isMoving) {
                if (state.LeftButton == ButtonState.Pressed && _boundingBox.Contains(state.X, state.Y)) {
                    _isMoving = true;
                    return;
                }
            }
            if(_isMoving){
                if (state.LeftButton == ButtonState.Released){
                    _isMoving = false;
                    _boundingBox.X = state.X;
                    _boundingBox.Y = state.Y;
                }
            }
        }

        public void HandleMouseMovementEvent(MouseState state){
            if (_isMoving){
                _parent.ElementDongle.EditDonglePosition(state.X, state.Y);
            }
        }

        private static MouseHandler _mouseHandler;

        public static void Init(MouseHandler mouseHandler){
            _mouseHandler = mouseHandler;
        }
    }
}
