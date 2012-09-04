#region

using System.Collections.Generic;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    internal class DoodadEditor : IGameState{
        readonly RenderPanel _renderTarget;
        readonly ShipRenderer _shipRenderer;

        public DoodadEditor(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo){
            _renderTarget = new RenderPanel(0, 0, ScreenData.ScreenWidth, ScreenData.ScreenHeight);
            RenderPanel.SetRenderPanel(_renderTarget);

            var geomGen = new HullGeometryGenerator(backCurveInfo, sideCurveInfo, topCurveInfo);
            _shipRenderer = new ShipRenderer(geomGen.GetGeometrySlices());
            int f = 5;
        }

        #region IGameState Members

        public void Update(){
            //throw new NotImplementedException();
        }

        public void Dispose(){
        }

        #endregion
    }
}