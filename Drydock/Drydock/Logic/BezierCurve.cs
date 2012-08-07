#region

using System;
using System.Collections.Generic;
using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic {
    /// <summary>
    /// this class is a fucking mess, here's to hoping it never has to be used again
    /// </summary>
    class BezierCurve {
        #region fields and properties
        private BezierCurve _nextCurve;
        private List<Line> _nextLines;
        private  BezierCurve _prevCurve;
        private List<Line> _prevLines;
        private readonly CurveController _controller;
        private readonly int _linesPerSide;
        private readonly UIElementCollection _elementCollection;

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
                        _prevLines.Add(_elementCollection.Add<Line>(new Line(Vector2.Zero, Vector2.Zero, 1.0f)));
                    }
                }
            }
        }
        public BezierCurve NextCurveReference {
            set{
                _nextCurve = value;
                if (_nextLines == null){
                    _nextLines = new List<Line>(_linesPerSide);
                    for (int i = 0; i < _linesPerSide; i++){
                        _nextLines.Add(_elementCollection.Add<Line>(new Line(Vector2.Zero, Vector2.Zero, 1.0f)));
                    }
                }
            }
        }

        public void InsertBetweenCurves(BezierCurve prevCurve, BezierCurve nextCurve, float t){
            _prevCurve = prevCurve;
            _nextCurve = nextCurve;

            _prevCurve.NextCurveReference = this;
            _nextCurve.PrevCurveReference = this;


            Vector2 pt1;
            Vector2 pt2;
            Bezier.GetBezierValue(out pt1, _prevCurve.NextHandlePos, _prevCurve.NextHandlePos, PrevHandlePos, HandlePos, t);
            Bezier.GetBezierValue(out pt2, _prevCurve.NextHandlePos, _prevCurve.NextHandlePos, PrevHandlePos, HandlePos, t+0.001f);//limits are for fags
            /*
            pt1.X = (int)pt1.X;
            pt1.Y = (int)pt1.Y;
            pt2.X = (int)pt2.X;
            pt2.Y = (int)pt2.Y;
            */

            //get tangent and set to angle
            Vector2 pt3 = pt1 - pt2;
            float angle, magnitude;
                
            Common.GetAngleFromComponents(out angle, out magnitude, pt3.X, pt3.Y);

            _controller.Angle1 = angle;
            if (_prevLines == null) {
                _prevLines = new List<Line>(_linesPerSide);

                for (int i = 0; i < _linesPerSide; i++) {
                    _prevLines.Add(_elementCollection.Add<Line>(new Line(Vector2.Zero, Vector2.Zero, 1.0f)));
                }
            }
            else {
                for (int i = 0; i < _linesPerSide; i++) {
                    _prevLines[i].Dispose();
                    _prevLines[i] = _elementCollection.Add<Line>(new Line(Vector2.Zero, Vector2.Zero, 1.0f));
                }
            }

            if (_nextLines == null) {
                _nextLines = new List<Line>(_linesPerSide);
                for (int i = 0; i < _linesPerSide; i++) {
                    _nextLines.Add(_elementCollection.Add<Line>(new Line(Vector2.Zero, Vector2.Zero, 1.0f)));
                }
            }
            else {
                for (int i = 0; i < _linesPerSide; i++) {
                    _nextLines[i].Dispose();
                    _nextLines[i] = _elementCollection.Add<Line>(new Line(Vector2.Zero, Vector2.Zero, 1.0f));
                }
            }

            Update();
        }

        public void SetPrevCurve(BezierCurve val, float angle, float handleLength){
            _prevCurve = val;
            _prevCurve.NextCurveReference = this;
            if (_prevLines == null) {
                _prevLines = new List<Line>(_linesPerSide);

                for (int i = 0; i < _linesPerSide; i++) {
                    _prevLines.Add(_elementCollection.Add<Line>(new Line(Vector2.Zero, Vector2.Zero, 1.0f)));
                }
            }
            else {
                for (int i = 0; i < _linesPerSide; i++) {
                    _prevLines[i].Dispose();
                    _prevLines[i] = _elementCollection.Add<Line>(new Line(Vector2.Zero, Vector2.Zero, 1.0f));
                }
            }
            _controller.Angle1 = angle;
            _controller.PrevHandleLength = handleLength;
            Update();
        }

        public void SetNextCurve(BezierCurve val, float angle, float handleLength) {
            _nextCurve = val;
            _nextCurve.PrevCurveReference = this;
            if (_nextLines == null) {
                _nextLines = new List<Line>(_linesPerSide);
                for (int i = 0; i < _linesPerSide; i++) {
                    _nextLines.Add(_elementCollection.Add<Line>(new Line(Vector2.Zero, Vector2.Zero, 1.0f)));
                }
            }
            else {
                for (int i = 0; i < _linesPerSide; i++) {
                    _nextLines[i].Dispose();
                    _nextLines[i] = _elementCollection.Add<Line>(new Line(Vector2.Zero, Vector2.Zero, 1.0f));
                }
            }
            _controller.Angle1 = angle;
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
            _controller = new CurveController(x, y, 20, 20,0f);
            _elementCollection = new UIElementCollection();
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

        public Point PrevContains(MouseState state, out float t){
            var mousePoint = new Vector2(state.X,state.Y);
            const int width = 5;

            if (_prevLines != null){
                for (int i = 0; i < _prevLines.Count; i++){
                    if (Vector2.Distance(_prevLines[i].DestPoint, mousePoint) < width) {
                        t = ((float)i) / (_linesPerSide * 2) + 0.5f;
                        return new Point((int)_prevLines[i].DestPoint.X, (int)_prevLines[i].DestPoint.Y);
                    }

                }
                foreach (var line in _prevLines){

                }
            }
            //todo: fix these returns to not break on zero
            t = -1;
            return Point.Zero;
        }

        public Point NextContains(MouseState state, out float t){
            var mousePoint = new Vector2(state.X, state.Y);
            const int width = 5;
            if (_nextLines != null){
                for (int i = 0; i < _nextLines.Count; i++) {
                    if (Vector2.Distance(_nextLines[i].DestPoint, mousePoint) < width) {

                        t = ((float)i) / (_linesPerSide*2);
                        return new Point((int)_nextLines[i].DestPoint.X, (int)_nextLines[i].DestPoint.Y);
                    }

                }
            }
            t = -1;
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
