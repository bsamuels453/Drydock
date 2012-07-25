using Drydock.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic{
    internal class CDraggable : IClickSubbable, IMouseMoveSubbable{
        private static MouseHandler _mouseHandler;
        private readonly IDraggable _parent;
        public Rectangle BoundingBox;
        private bool _isMoving;

        public CDraggable(IDraggable parent, int initX, int initY, int width, int height){
            _parent = parent;
            _mouseHandler.ClickSubscriptions.Add(this);
            _mouseHandler.MovementSubscriptions.Add(this);
            BoundingBox = new Rectangle(initX, initY, width, height);
            _isMoving = false;
        }

        #region properties

        public int X{
            get { return BoundingBox.X; }
            set { BoundingBox.X = value; }
        }

        public int Y{
            get { return BoundingBox.Y; }
            set { BoundingBox.Y = value; }
        }

        public int CentX{
            get { return BoundingBox.X + BoundingBox.Width/2; }
        }

        public int CentY{
            get { return BoundingBox.Y + BoundingBox.Height/2; }
        }

        #endregion

        #region IClickSubbable Members

        public bool HandleMouseClickEvent(MouseState state){
            if (!_isMoving){
                if (state.LeftButton == ButtonState.Pressed && BoundingBox.Contains(state.X, state.Y)){
                    _isMoving = true;
                    return true;
                }
            }
            if (_isMoving){
                if (state.LeftButton == ButtonState.Released){
                    _isMoving = false;
                    BoundingBox.X = state.X - BoundingBox.Width/2;
                    BoundingBox.Y = state.Y - BoundingBox.Height/2;
                }
            }
            return false;
        }

        #endregion

        #region IMouseMoveSubbable Members

        public bool HandleMouseMovementEvent(MouseState state){
            if (_isMoving){
                int x = state.X - BoundingBox.Width/2;
                int y = state.Y - BoundingBox.Height/2;
                _parent.ClampDraggedPosition(ref x, ref y);
                _parent.X = x;
                _parent.Y = y;
                BoundingBox.X = x;
                BoundingBox.Y = y;
                _parent.HandleObjectMovement();
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
        void ClampDraggedPosition(ref int x, ref int y);
        void HandleObjectMovement();
    }

    #endregion
}