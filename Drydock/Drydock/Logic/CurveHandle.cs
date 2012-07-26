using Drydock.Render;
using Microsoft.Xna.Framework;
using IDrawable = Drydock.Render.IDrawable;

namespace Drydock.Logic{
    internal class CurveHandle : IDraggable, IDrawable{
        private const string _handleTexture = "box";
        private readonly CDraggable _dragComponent;
        private readonly Sprite2D _elementSprite;
        private readonly int _id;
        private readonly CurveController _parentController;
        private Rectangle _boundingBox;


        #region properties

        public int CentX{ //this refers to the center of the sprite, rather than the position of its origin
            get { return BoundingBox.X + BoundingBox.Width / 2; }
        }

        public int CentY{
            get { return BoundingBox.Y + BoundingBox.Height / 2; }
        }

        public int X{
            get { return _boundingBox.X; }
            set { _boundingBox.X = value; }
        }

       public  int Y{
            get { return _boundingBox.Y; }
            set { _boundingBox.Y = value; }
        }

        public Rectangle BoundingBox {
            get { return _boundingBox; }
            set {_boundingBox = value;}
        }

        #endregion

        public CurveHandle(int x, int y, int id, CurveController parent){
            _boundingBox = new Rectangle(x, y, 8, 8);
            _dragComponent = new CDraggable(this);
            _elementSprite = new Sprite2D(_handleTexture, this);
            _id = id;
            _parentController = parent;

        }

        public void ManualTranslation(int dx, int dy){
            X += dx;
            Y += dy;
        }

        #region IDraggable Members

        public void HandleObjectMovement(int dx, int dy){
            _parentController.HandleHandleMovement(CentX, CentY, dx, dy, _id);
        }

        public void ClampDraggedPosition(ref int x, ref int y){
        }

        #endregion


    }
}