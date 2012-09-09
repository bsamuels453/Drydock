#region

using System.Collections.Generic;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    internal class DoodadEditor : IGameState{
        const int _primsPerDeck = 3;
        readonly RenderPanel _renderTarget;
        readonly HullGeometryHandler _hullGeometryHandler;

        public DoodadEditor(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo){
            _renderTarget = new RenderPanel(0, 0, ScreenData.ScreenWidth, ScreenData.ScreenHeight);
            RenderPanel.SetRenderPanel(_renderTarget);

            var geomGen = new HullGeometryGenerator(backCurveInfo, sideCurveInfo, topCurveInfo, _primsPerDeck);
            _hullGeometryHandler = new HullGeometryHandler(geomGen.GetGeometrySlices(), _primsPerDeck);
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