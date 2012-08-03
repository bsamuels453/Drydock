using System;
using System.Collections.Generic;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic{
    internal class CurveControllerCollection{
        private const int _segmentsBetweenControllers = 40;
        private readonly List<CurveController> _curveControllers;
        private readonly List<Line> _segments;
        private int _numControllers = 4;

        private BezierCurve curve1;
        private BezierCurve curve2;

        public CurveControllerCollection(){
            curve1 = new BezierCurve(200, 200, 20);
            curve2 = new BezierCurve(300, 200, 20);
            curve1.NextCurve = curve2;
            curve2.PrevCurve = curve1;
            /*_curveControllers = new List<CurveController>();
            _segments = new List<Line>(_curveControllers.Count*_segmentsBetweenControllers);

            int x = 200;
            int y = 200;
            int dx = 100;
            //for (int i = 0; i < _numControllers; i++){
                _curveControllers.Add(new CurveController(x, y, 100, 100, 1.5f));
                x += dx;
                _curveControllers.Add(new CurveController(x, y, 100, 50,  1.5f));
                x += dx/2;
                _curveControllers.Add(new CurveController(x, y, 50, 50,-1.0f));
                x +=(int)( dx-dx*0.5f);
                _curveControllers.Add(new CurveController(x, y, 50, 100, 1.5f));
                x += dx;

           // }
            for (int i = 0; i < _numControllers - 1; i++){
                for (int si = 0; si < _segmentsBetweenControllers; si++){
                    float t = (float) si/_segmentsBetweenControllers;
                    Vector2 firstPos, secondPos;

                    Bezier.GetBezierValue(
                        out firstPos,
                        _curveControllers[i].CenterHandlePos,
                        _curveControllers[i].NextHandlePos,
                        _curveControllers[i + 1].PrevHandlePos,
                        _curveControllers[i + 1].CenterHandlePos,
                        t
                        );

                    t += 1f/_segmentsBetweenControllers;
                    Bezier.GetBezierValue(
                        out secondPos,
                        _curveControllers[i].CenterHandlePos,
                        _curveControllers[i].NextHandlePos,
                        _curveControllers[i + 1].PrevHandlePos,
                        _curveControllers[i + 1].CenterHandlePos,
                        t
                        );

                    _segments.Add(new Line(firstPos, secondPos, 1.0f));
                }
            }*/
        }

        /// <summary>
        /// insert a new curve controller handle at the specified line segment 
        /// </summary>
        /// <param name="lineSegIndex"></param>
        private void AddController(int lineSegIndex){
            int controllerInsertIndex = 0;
            while (lineSegIndex - _segmentsBetweenControllers > 0){
                controllerInsertIndex++;
                lineSegIndex -= _segmentsBetweenControllers;
            }

        }

        public void UpdateCurves(){
            curve1.Update();
            curve2.Update();
            /*
            for (int i = 0; i < _numControllers - 1; i++){
                for (int si = 0; si < _segmentsBetweenControllers; si++){
                    float t = (float) si/_segmentsBetweenControllers;
                    Vector2 firstPos, secondPos;

                    Bezier.GetBezierValue(
                        out firstPos,
                        _curveControllers[i].CenterHandlePos,
                        _curveControllers[i].NextHandlePos,
                        _curveControllers[i + 1].PrevHandlePos,
                        _curveControllers[i + 1].CenterHandlePos,
                        t
                        );

                    t += 1f/_segmentsBetweenControllers;
                    Bezier.GetBezierValue(
                        out secondPos,
                        _curveControllers[i].CenterHandlePos,
                        _curveControllers[i].NextHandlePos,
                        _curveControllers[i + 1].PrevHandlePos,
                        _curveControllers[i + 1].CenterHandlePos,
                        t
                        );

                    _segments[i*_segmentsBetweenControllers + si].OriginPoint = new Vector2((int) firstPos.X, (int) firstPos.Y);
                    _segments[i*_segmentsBetweenControllers + si].DestPoint = new Vector2((int) secondPos.X, (int) secondPos.Y);
                }
            }*/
        }
    }
}