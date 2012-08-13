using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Render;
using Drydock.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Logic {
    class PreviewRenderer {
        private readonly Vector3[,] _mesh;
        private readonly VertexPositionNormalTexture[] _verticies;
        private readonly int[] _indicies;
        private const int _meshPrimitiveWidth = 20;//this is in primitives
        private readonly int _bufferId;
        private readonly CurveControllerCollection _sideCurves;
        private readonly CurveControllerCollection _topCurves;
        private readonly Button[] _buttons;
        private readonly UIElementCollection _coll;

        public PreviewRenderer(CurveControllerCollection sideCurves, CurveControllerCollection topCurves){
            _verticies = new VertexPositionNormalTexture[_meshPrimitiveWidth * _meshPrimitiveWidth * 4];
            _indicies = new int[_meshPrimitiveWidth * _meshPrimitiveWidth * 6];// 6 indicies make up 2 triangles, can make this into triangle strip in future if have optimization boner
            _bufferId = AuxBufferManager.AddVbo(_verticies.Count(), _indicies.Count(), (_meshPrimitiveWidth ) * (_meshPrimitiveWidth ) * 2, "brown");
            _mesh = new Vector3[_meshPrimitiveWidth+1,_meshPrimitiveWidth+1];

            _sideCurves = sideCurves;
            _topCurves = topCurves;

            //construct indice list
            //remember the clockwise-fu
            //+1-----+2
            //|     /
            //|   /    
            //| /     
            //+0
            //       +2
            //      / |
            //    /   | 
            //  /     |
            //+0-----+3
            int curVertex = 0;
            for (int i = 0; i < _indicies.Count(); i += 6) {
                _indicies[i] = curVertex;
                _indicies[i+1] = curVertex+1;
                _indicies[i+2] = curVertex+2;

                _indicies[i+3] = curVertex;
                _indicies[i+4] = curVertex+2;
                _indicies[i+5] = curVertex+3;

                curVertex += 4;
            }

            _topCurves.GetParameterizedPoint(0, true);
            var topPts = new Vector2[_meshPrimitiveWidth];
            for (float i = 0; i < _meshPrimitiveWidth; i++){
                float t = i / (_meshPrimitiveWidth-1);
                topPts[(int)i] = _topCurves.GetParameterizedPoint(t);
            }


            _coll = new UIElementCollection();
            _buttons = new Button[_meshPrimitiveWidth];
            for (int i = 0; i < _buttons.Count(); i++){
                _buttons[i] = _coll.Add<Button>(
                    new Button(topPts[i].X, topPts[i].Y, 9, 9, DepthLevel.High, _coll, "box"));
            }

            AuxBufferManager.SetIndicies(_bufferId, _indicies);
        }

        public void Update() {
        }

    }
}
