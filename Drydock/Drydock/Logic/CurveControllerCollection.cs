using System;
using System.Collections.Generic;
using Drydock.Logic.InterfaceObj;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic{
    internal class CurveControllerCollection{
        private const int _segmentsBetweenControllers = 40;
        private readonly List<CurveController> _curveControllers;
        private readonly List<Line2D> _segments;
        private int _numControllers = 4;

        public CurveControllerCollection(){
            _curveControllers = new List<CurveController>();
            _segments = new List<Line2D>(_curveControllers.Count*_segmentsBetweenControllers);
            //mouseHandler.ClickSubscriptions.Add(this);

            //_segments.Add(new Line2D());
            int x = 200;
            int y = 100;
            int dx = 100;
            for (int i = 0; i < _numControllers; i++){
                _curveControllers.Add(new CurveController(x, y, 100, 100, 1.5f));
                x += dx;
            }
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

                    _segments.Add(new Line2D(firstPos, secondPos, 0.5f));
                }
            }
        }

        public bool HandleMouseClickEvent(MouseState state){
            throw new NotImplementedException();
        }

        public void UpdateCurves(){
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

                    //_segments.Add(new Line2D((int)firstPos.X, (int)firstPos.Y, (int)secondPos.X, (int)secondPos.Y));
                    _segments[i*_segmentsBetweenControllers + si].OriginPoint = new Vector2((int) firstPos.X, (int) firstPos.Y);
                    _segments[i*_segmentsBetweenControllers + si].DestPoint = new Vector2((int) secondPos.X, (int) secondPos.Y);
                }
            }
        }
    }
}