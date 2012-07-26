using Drydock.Render;
using Microsoft.Xna.Framework;
using IDrawable = Drydock.Render.IDrawable;

namespace Drydock.Logic.InterfaceObj{
    internal class CurveHandle : IDraggable, IDrawable{
        private const string _handleTexture = "box";
        private readonly CDraggable _dragComponent;
        private readonly Sprite2D _elementSprite;
        private readonly int _id;
        private readonly CurveController _parentController;
        private Rectangle _boundingBox;
        private Vector2 _centPosition;

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
            _boundingBox = new Rectangle(x, y, 8, 8);
            _centPosition.X = BoundingBox.X + BoundingBox.Width/2;
            _centPosition.Y = BoundingBox.Y + BoundingBox.Height/2;
            _dragComponent = new CDraggable(this);
            _elementSprite = new Sprite2D(_handleTexture, this);
            _id = id;
            _parentController = parent;
        }

        #region IDraggable Members

        public void HandleObjectMovement(int dx, int dy){
            _parentController.BalanceHandleMovement(CentX, CentY, dx, dy, _id);
        }

        public void ClampDraggedPosition(ref int x, ref int y){
        }

        #endregion

        public void ManualTranslation(int dx, int dy){
            X += dx;
            Y += dy;
        }
    }
}