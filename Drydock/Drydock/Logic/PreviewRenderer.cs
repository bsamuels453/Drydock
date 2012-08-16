using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Logic {
    class PreviewRenderer {
        private readonly Vector3[,] _mesh;
        private readonly VertexPositionNormalTexture[] _verticies;
        private readonly int[] _indicies;
        private const int _meshVertexWidth = 32;//this is in primitives
        private readonly int _bufferId;
        private readonly BezierCurveCollection _sideCurves;
        private readonly BezierCurveCollection _topCurves;
        private readonly BezierCurveCollection _backCurves;

        public PreviewRenderer(BezierCurveCollection sideCurves, BezierCurveCollection topCurves, BezierCurveCollection backCurves){
            _verticies = new VertexPositionNormalTexture[_meshVertexWidth * _meshVertexWidth * 4];
            _indicies = new int[_meshVertexWidth * _meshVertexWidth * 6];// 6 indicies make up 2 triangles, can make this into triangle strip in future if have optimization boner
            _bufferId = AuxBufferManager.AddVbo(_verticies.Count(), _indicies.Count(), (_meshVertexWidth ) * (_meshVertexWidth ) * 2, "brown");
            _mesh = new Vector3[_meshVertexWidth,_meshVertexWidth];

            _sideCurves = sideCurves;
            _topCurves = topCurves;
            _backCurves = backCurves;

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

            for(int i=0; i<_verticies.Count(); i++){
                _verticies[i] = new VertexPositionNormalTexture();
            }

            var sw = new Stopwatch();
            sw.Start();
            _topCurves.GetParameterizedPoint(0, true);
            var topPts = new Vector2[_meshVertexWidth];
            for (double i = 0; i < _meshVertexWidth; i++) {
                double t = i / ((_meshVertexWidth-1)*2);
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (i == _meshVertexWidth - 1){//special exception to force a pointed tip
                    t = 0.5d;
                }
                // ReSharper restore CompareOfFloatsByEqualityOperator
                topPts[(int)i] = _topCurves.GetParameterizedPoint(t);
            }

            topPts = topPts.Reverse().ToArray();
            float reflectionPoint = topPts[0].Y;
            var yDelta = new float[_meshVertexWidth];

            for (int i = 0; i < _meshVertexWidth; i++){
                yDelta[i] = Math.Abs(topPts[i].Y - reflectionPoint) * 2 / _meshVertexWidth;
            }


            sideCurves.GetParameterizedPoint(0, true);//this refreshes internal fields
            //orient controllers correctly for the bezierintersect
            var li = _sideCurves.Select(bezierCurve => new BezierInfo(
                _sideCurves.Normalize(bezierCurve.CenterHandlePos),
                _sideCurves.Normalize(bezierCurve.PrevHandlePos),
                _sideCurves.Normalize(bezierCurve.NextHandlePos))).ToList();

            

            var sideIntersectGenerator = new BezierIntersect(li);

            var sideIntersectionCache = new float[_meshVertexWidth];

            for (int x = 0; x < _meshVertexWidth; x++){

                float y = sideIntersectGenerator.GetIntersectionFromX(topPts[x].X).Y;
                sideIntersectionCache[x] = y;
                /*for (int z = 0; z < _meshVertexWidth; z++){
                    _mesh[x, z] = new Vector3(topPts[x].X, y, topPts[x].Y + yDelta[x]*z);
                }*/
            }

            var maxY = (float)_backCurves.NormalizeY(_backCurves.MaxY);
            var backCurvesMaxWidth = (float)(_backCurves.NormalizeX(_backCurves[_backCurves.Count - 1].CenterHandlePos.X) - _backCurves.NormalizeX(_backCurves[0].CenterHandlePos.X));
            //topPts = topPts.Reverse().ToArray();

            for (int x = 0; x < _meshVertexWidth; x++){

                float scaleX = ((reflectionPoint - topPts[x].Y) * 2) / backCurvesMaxWidth;
                float scaleY = sideIntersectionCache[x]/ maxY;
                
                var bezierInfo = _backCurves.GetControllerInfo(scaleX, scaleY);
                var crossIntersectGenerator = new BezierIntersect(bezierInfo);

                for (int z = 0; z < _meshVertexWidth; z++) {
                    if (z > _meshVertexWidth / 2 && x != 0){
                        int g = 5;
                    }
                    Vector2 pos = crossIntersectGenerator.GetIntersectionFromX(yDelta[x] * (z));
                    _mesh[x, z] = new Vector3(topPts[x].X, pos.Y, topPts[x].Y + yDelta[x] * (z));
                    //_mesh[x, _meshVertexWidth-1-z] = new Vector3(topPts[x].X, pos.Y, topPts[x].Y + yDelta[x] * (z));
                }

            }



            //convert from 2d array to 1d
            int index = 0;
            for(int x=0; x<_meshVertexWidth-1; x++){
                for (int z = 0; z < _meshVertexWidth-1; z++){
                    _verticies[index].Normal = Vector3.Zero;
                    _verticies[index].Position = -_mesh[x, z];
                    _verticies[index].TextureCoordinate = new Vector2(0, 0);//make  this assigned during init

                    _verticies[index+1].Normal = Vector3.Zero;
                    _verticies[index+1].Position = -_mesh[x, z+1];
                    _verticies[index+1].TextureCoordinate = new Vector2(0, 1);

                    _verticies[index+2].Normal = Vector3.Zero;
                    _verticies[index+2].Position = -_mesh[x+1, z+1];
                    _verticies[index+2].TextureCoordinate = new Vector2(1, 1);

                    _verticies[index+3].Normal = Vector3.Zero;
                    _verticies[index+3].Position = -_mesh[x+1, z];
                    _verticies[index+3].TextureCoordinate = new Vector2(1, 0);

                    index+=4;
                }
            }
            sw.Stop();
            System.Console.WriteLine("time:" + sw.Elapsed.TotalMilliseconds * 1000000);
            AuxBufferManager.SetIndicies(_bufferId, _indicies);
            AuxBufferManager.SetVerticies(_bufferId, _verticies);


        }

        public void Update() {
        }

    }
}
