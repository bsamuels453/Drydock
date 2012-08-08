using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.UI;
using Drydock.Utilities;

namespace Drydock.Logic {
    class HullEditorPanel {
        readonly FloatingRectangle _boundingBox;
        readonly UIElementCollection _elementCollection;
        readonly CurveControllerCollection _curves;
        readonly Button _background;

        public HullEditorPanel(int x, int y, int width, int  height, string defaultCurveConfiguration){
            _boundingBox = new FloatingRectangle(x, y, width, height);
            _elementCollection = new UIElementCollection();
            _curves = new CurveControllerCollection(defaultCurveConfiguration, _elementCollection);
            _curves.ElementCollection.AddDragConstraintCallback(ClampChildElements);
            _background = _elementCollection.Add<Button>(new Button(x, y, width, height, 1, "panelBG", null, true));

        }

        private void ClampChildElements(IUIInteractiveElement owner, ref int x, ref int y, int oldX, int oldY){
            if (x > _boundingBox.X + _boundingBox.Width || x < _boundingBox.X){
                x = oldX;
            }
            if (y > _boundingBox.Y + _boundingBox.Height || y < _boundingBox.Y) {
                y = oldY;
            }
        }

        public void Update(){
            _curves.Update();
        }

    }
}
