using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Logic.HullEditorState;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities;

namespace Drydock.Logic.DoodadEditorState {
    class DoodadEditor : IGameState {
        readonly RenderPanel _renderTarget;
        readonly UIElementCollection _elementCollection;

        public DoodadEditor(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo){
            _elementCollection = new UIElementCollection(DepthLevel.Medium);
            _renderTarget = new RenderPanel(0, 0, ScreenData.ScreenWidth, ScreenData.ScreenHeight, DepthLevel.Medium);
            RenderPanel.SetRenderPanel(_renderTarget);

            var geometryGenerator = new HullGeometryGenerator(backCurveInfo, sideCurveInfo, topCurveInfo);
        }

        public void Update(){
            throw new NotImplementedException();
        }

        public void Dispose(){
            throw new NotImplementedException();
        }
    }
}
