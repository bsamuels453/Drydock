using Drydock.Render;

namespace Drydock.Logic{
    internal class CurveHandle : IDraggable{
        private const string _handleTexture = "box";
        private readonly CDraggable _dragComponent;
        private readonly Sprite2D _elementSprite;
        private readonly int _id;
        private readonly CurveController _parentController;

        #region properties

        public int CentX{ //this refers to the center of the sprite, rather than the position of its origin
            get { return _dragComponent.CentX; }
        }

        public int CentY{
            get { return _dragComponent.CentY; }
        }

        public int X{ //the position of the sprite is the grand reference for where the object actually is
            get { return _elementSprite.X; }
            set { _elementSprite.X = value; }
        }

        public int Y{
            get { return _elementSprite.Y; }
            set { _elementSprite.Y = value; }
        }

        #endregion

        public CurveHandle(int x, int y, int id, CurveController parent){
            _elementSprite = new Sprite2D(_handleTexture, x, y);
            _dragComponent = new CDraggable(this, x, y, 8, 8);
            _id = id;
            _parentController = parent;
        }

        public void ManualTranslation(int dx, int dy){
            _dragComponent.ManualTranslation(dx, dy);
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