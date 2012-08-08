using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.UI;
using Drydock.Utilities;

namespace Drydock.Logic {
    class HullEditorPanel {
        FloatingRectangle _boundingBox;
        UIElementCollection _elementCollection;
        readonly CurveControllerCollection _curves;
        //UIElementCollection _curveElements;

        public HullEditorPanel(int x, int y, int width, int  height, string defaultCurveConfiguration){
            _boundingBox = new FloatingRectangle(x, y, width, height);
            _elementCollection = new UIElementCollection();
            _curves = new CurveControllerCollection(defaultCurveConfiguration);
            //_curveElements = _curves.ElementCollection;
        }

    }
}
