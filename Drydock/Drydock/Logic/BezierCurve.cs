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

        public BezierCurve PrevCurveReference{
            set { 
                _prevCurve = value;
                if (_prevLines == null) {
                    _prevLines = new List<Line>(_linesPerSide);
                    for (int i = 0; i < _linesPerSide; i++) {
                        _prevLines.Add(new Line(Vector2.Zero, Vector2.Zero, 1.0f));
                    }
                }
            }
        }
        public BezierCurve NextCurveReference {
            set { 
                _nextCurve = value;
                if (_nextLines == null) {
                    _nextLines = new List<Line>(_linesPerSide);
                    for (int i = 0; i < _linesPerSide; i++) {
                        _nextLines.Add(new Line(Vector2.Zero, Vector2.Zero, 1.0f));
                    }
                }
                }
        }

        public void InsertBetweenCurves(BezierCurve prevCurve, BezierCurve nextCurve, float t){

        }

        public void SetPrevCurve(BezierCurve val, float angle, float handleLength){
            _prevCurve = val;
            _prevCurve.NextCurveReference = this;
            if (_prevLines == null) {
                _prevLines = new List<Line>(_linesPerSide);

                for (int i = 0; i < _linesPerSide; i++) {
                    _prevLines.Add(new Line(Vector2.Zero, Vector2.Zero, 1.0f));
                }
            }
            else {
                for (int i = 0; i < _linesPerSide; i++) {
                    _prevLines[i].Dispose();
                    _prevLines[i] = new Line(Vector2.Zero, Vector2.Zero, 1.0f);
                }
            }
            _controller.Angle = angle;
            _controller.PrevHandleLength = handleLength;
            Update();
        }

        public void SetNextCurve(BezierCurve val, float angle, float handleLength) {
            _nextCurve = val;
            _nextCurve.PrevCurveReference = this;
            if (_nextLines == null) {
                _nextLines = new List<Line>(_linesPerSide);
                for (int i = 0; i < _linesPerSide; i++) {
                    _nextLines.Add(new Line(Vector2.Zero, Vector2.Zero, 1.0f));
                }
            }
            else {
                for (int i = 0; i < _linesPerSide; i++) {
                    _nextLines[i].Dispose();
                    _nextLines[i] = new Line(Vector2.Zero, Vector2.Zero, 1.0f);
                }
            }
            _controller.Angle = angle;
            _controller.NextHandleLength = handleLength;
            Update();
        }


        #endregion

        /// <summary>
        /// usually used to add new curves between two points
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="linesPerSide"> </param>
        public BezierCurve(int x, int y, int linesPerSide=20){
            _controller = new CurveController(x, y, 100, 100,1.5f);
            _linesPerSide = linesPerSide;
            _nextLines = null;
            _nextCurve = null;
            _prevCurve = null;
            _prevLines = null;
            /*if (prevController == null) {
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

            //now set the controller angle
            //it doesnt matter which set of control points we use to determine the tangent angle because of ~=LIMITS=~
            if (prevController != null) {
                Vector2 pt1;
                Vector2 pt2;
                //Bezier.GetBezierValue(out pt1, _prevCurve.NextHandlePos, _prevCurve.NextHandlePos, 

            }
            else{

            }


            Update();*/
        }

        public void Dispose(){
            throw new NotImplementedException();
        }

        public Point PrevContains(MouseState state){
            var mousePoint = new Vector2(state.X,state.Y);
            const int width = 5;

            if (_prevLines != null){
                foreach (var line in _prevLines){
                    if (Vector2.Distance(line.DestPoint, mousePoint) < width){
                        return new Point((int) line.DestPoint.X, (int) line.DestPoint.Y);
                    }
                }
            }

            return Point.Zero;
        }

        public Point NextContains(MouseState state){
            var mousePoint = new Vector2(state.X, state.Y);
            const int width = 5;
            if (_nextLines != null) {
                foreach (var line in _nextLines) {
                    if (Vector2.Distance(line.DestPoint, mousePoint) < width) {
                        return new Point((int)line.DestPoint.X, (int)line.DestPoint.Y);
                    }
                }
            }
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
