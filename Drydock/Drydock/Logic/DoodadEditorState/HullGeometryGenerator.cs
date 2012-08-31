using System.Collections.Generic;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Logic.DoodadEditorState {
    class HullGeometryGenerator {
        readonly ShipGeometryBuffer _displayBuffer;
        readonly int[] _indicies;
        readonly Vector3[,] _mesh;
        readonly VertexPositionNormalTexture[] _verticies;
         
        public HullGeometryGenerator(List<BezierInfo> backCurveInfo, List<BezierInfo> sideCurveInfo, List<BezierInfo> topCurveInfo) {

            //_displayBuffer = new ShipGeometryBuffer(1, 1, 1, "whiteborder");
            var v = new BezierIndependentGenerator(sideCurveInfo);

            



        }
    }
}
