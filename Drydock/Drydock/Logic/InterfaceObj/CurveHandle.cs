using System.Diagnostics;
using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic.InterfaceObj{
    internal class CurveHandle{
        private const string _handleTexture = "box";
        //private readonly CDraggable _dragComponent;
        private readonly Sprite2D _elementSprite;
        private readonly int _id;
        private readonly CurveController _parentController;
        private readonly Stopwatch _selectionTimer;
        private Rectangle _boundingBox;
        private Vector2 _centPosition;
        private bool _didHandleMoveSinceTimerStart;
        private bool _isSelected;

        #region properties

        public Vector2 CentPosition{
            get { return _centPosition; }
        }

        public int CentX{ //this refers to the center of the sprite, rather than the position of its origin
            get { return BoundingBox.X + BoundingBox.Width/2; }
        }

        public int CentY{
            get { return BoundingBox.Y + BoundingBox.Height/2; }
        }

        public bool IsSelected{
            get { return _isSelected; }
            set{
                if (value){
                    _isSelected = true;
                    _elementSprite.ChangeTexture("bigbox");
                    _boundingBox.Width = 20;
                    _boundingBox.Height = 20;
                }
                else{
                    _isSelected = false;
                    _elementSprite.ChangeTexture("box");
                    _boundingBox.Width = 9;
                    _boundingBox.Height = 9;
                }
            }
        }

        public int X{
            get { return _boundingBox.X; }
            set{
                _boundingBox.X = value;
                _centPosition.X = BoundingBox.X + BoundingBox.Width/2;
            }
        }

        public int Y{
            get { return _boundingBox.Y; }
            set{
                _boundingBox.Y = value;
                _centPosition.Y = BoundingBox.Y + BoundingBox.Height/2;
            }
        }

        public Rectangle BoundingBox{
            get { return _boundingBox; }
        }

        #endregion

        public CurveHandle(int x, int y, int id, CurveController parent){
            _centPosition = new Vector2();
            _boundingBox = new Rectangle(x, y, 9, 9);
            _centPosition.X = BoundingBox.X + BoundingBox.Width/2;
            _centPosition.Y = BoundingBox.Y + BoundingBox.Height/2;
            //_dragComponent = new CDraggable(this);
            //_elementSprite = new Sprite2D(_handleTexture, this, 1, 1);
            _id = id;
            _parentController = parent;
            _isSelected = false;
            _selectionTimer = new Stopwatch();
            MouseHandler.ClickSubscriptions.Add(HandleMouseClick);
        }

        #region IDraggable Members

        public void HandleObjectMovement(int dx, int dy){
            //_parentController.BalanceHandleMovement(CentX, CentY, dx, dy, _id);
            if (dx != 0 || dy != 0){
                _didHandleMoveSinceTimerStart = true;
            }
        }

        public void ClampDraggedPosition(ref int x, ref int y){
        }

        #endregion

        private bool HandleMouseClick(MouseState state){
            if (_boundingBox.Contains(state.X, state.Y)){
                if (!_selectionTimer.IsRunning && state.LeftButton == ButtonState.Pressed){
                    _selectionTimer.Start();
                    _didHandleMoveSinceTimerStart = false;
                }
                else{
                    if (state.LeftButton == ButtonState.Released){
                        if (_selectionTimer.ElapsedMilliseconds < 100 && !_didHandleMoveSinceTimerStart){
                            _selectionTimer.Stop();
                            IsSelected = true;
                            return true;
                        }
                        else{
                            _selectionTimer.Stop();
                        }
                    }
                }
            }

            return false;
        }

        public void ManualTranslation(int dx, int dy){
            X += dx;
            Y += dy;
        }
    }
}