using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Render;

namespace Drydock.Logic {
    class CurveHandle : IDraggable{
        private const string _handleTexture = "box";
        private readonly CDraggable _dragComponent;

        public Sprite2D ElementDongle {get;set;}

        public int X{
            get { return _dragComponent.X; }
            set { _dragComponent.X = value; }
        }

        public int Y {
            get { return _dragComponent.Y; }
            set { _dragComponent.Y = value; }
        }

        public CurveHandle(int x, int y){
            ElementDongle = new Sprite2D(_handleTexture, x, y);
            _dragComponent = new CDraggable(this, x, y, 8, 8);
        }

        public void ClampDraggedPosition(ref int x, ref int y){ }
    }
}
