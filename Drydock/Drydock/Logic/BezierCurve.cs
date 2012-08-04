#region

using System;
using System.Collections.Generic;
using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic {
    class BezierCurve {
        #region fields and properties
        private BezierCurve _nextCurve;
        private List<Line> _nextLines;
        private  BezierCurve _prevCurve;
        private List<Line> _prevLines;
        private readonly CurveController _controller;
        private readonly int _linesPerSide;

        public Vector2 PrevHandlePos {
            get { return _controller.PrevHandlePos; }
        }
        public Vector2 HandlePos{
            get { return _controller.CenterHandlePos; }
        }
        public Vector2 NextHandlePos {
            get { return _controller.NextHandlePos; }
        }

        public float PrevHandleLength { get; set; }//the empire strikes back
        public float NextHandleLength { get; set; }

        public BezierCurve PrevCurve{
            set{
                _prevCurve = value;
                if (_prevLines == null) {
                    _prevLines = new List<Line>(_linesPerSide);

                    for (int i = 0; i < _linesPerSide; i++) {
                        _prevLines.Add(new Line(Vector2.Zero, Vector2.Zero, 1.0f));
                    }
                }
                else{
                    for (int i = 0; i < _linesPerSide; i++) {
                        _prevLines[i].Dispose();
                        _prevLines[i] = new Line(Vector2.Zero, Vector2.Zero, 1.0f);
                    }
                }
                Update();
            }
        }
        public BezierCurve NextCurve {
            set{
                _nextCurve = value;
                if (_nextLines == null) {
                    _nextLines = new List<Line>(_linesPerSide);
                    for (int i = 0; i < _linesPerSide; i++) {
                        _nextLines.Add(new Line(Vector2.Zero, Vector2.Zero, 1.0f));
                    }
                }
                else{
                    for (int i = 0; i < _linesPerSide; i++) {
                        _nextLines[i].Dispose();
                        _nextLines[i] = new Line(Vector2.Zero, Vector2.Zero, 1.0f);
                    }
                }
                Update();
            }
        }

        #endregion

        public BezierCurve(int x, int y, int linesPerSide, BezierCurve prevController=null, BezierCurve nextController=null){
            _controller = new CurveController(x, y, 100, 100, 1.5f);
            _linesPerSide = linesPerSide;
            if (prevController == null) {
                _prevCurve = null;
                _prevLines = null;
            }
            else{
                _prevCurve = prevController;
                _prevCurve._nextCurve = this;
                if (_prevLines == null){
                    _prevLines = new List<Line>(linesPerSide);
                    for (int i = 0; i < linesPerSide; i++){
                        _prevLines.Add(new Line(Vector2.Zero, Vector2.Zero, 1.0f));
                    }
                }

            }
            if (nextController == null) {
                _nextCurve = null;
                _nextLines = null;
            }
            else{
                _nextCurve = nextController;
                _nextCurve.PrevCurve = this;
                if (_nextLines == null){
                    _nextLines = new List<Line>(linesPerSide);
                    for (int i = 0; i < linesPerSide; i++){
                        _nextLines.Add(new Line(Vector2.Zero, Vector2.Zero, 1.0f));
                    }
                }
            }

            if (prevController == null && nextController == null){
                //we have nothing to work on, so just return
                return;
            }
            Update();
        }

        public void Dispose(){
            throw new NotImplementedException();
        }

        public Point Contains(MouseState state){
            return Point.Zero;
        }

        public void Update(){
            float t, dt;
            Vector2 firstPos, secondPos;
            if (_prevCurve != null){
                t = 0.5f;
                dt = 1 / (float)(_linesPerSide * 2);
                foreach (var line in _prevLines){
                    Bezier.GetBezierValue(
                        out firstPos,
                        _prevCurve.HandlePos,
                        _prevCurve.NextHandlePos,
                        PrevHandlePos,
                        HandlePos,
                        t
                    );

                    Bezier.GetBezierValue(
                        out secondPos,
                        _prevCurve.HandlePos,
                        _prevCurve.NextHandlePos,
                        PrevHandlePos,
                        HandlePos,
                        t+dt
                    );
                    line.OriginPoint = firstPos;
                    line.DestPoint = secondPos;
                    t += dt;
                }
            }
            if (_nextCurve != null) {
                t = 0;
                dt = 1 / (float)(_linesPerSide * 2);
                foreach (var line in _nextLines) {
                    Bezier.GetBezierValue(
                        out firstPos,
                        HandlePos,
                        NextHandlePos,
                        _nextCurve.PrevHandlePos,
                        _nextCurve.HandlePos,
                        t
                    );

                    Bezier.GetBezierValue(
                        out secondPos,
                        HandlePos,
                        NextHandlePos,
                        _nextCurve.PrevHandlePos,
                        _nextCurve.HandlePos,
                        t + dt
                    );
                    line.OriginPoint = firstPos;
                    line.DestPoint = secondPos;
                    t += dt;
                }
            }
        }
    }
}
