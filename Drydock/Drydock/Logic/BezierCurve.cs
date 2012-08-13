#region

using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int _linesPerSide = 20;
        public readonly CurveController Controller;
        private readonly UIElementCollection _elementCollection;
        private BezierCurve _nextCurve;
        private List<Line> _nextLines;
        private  BezierCurve _prevCurve;
        private List<Line> _prevLines;
        private readonly LineGenerator _lineTemplate; 

        public Vector2 PrevHandlePos {
            get { return Controller.PrevHandlePos; }
        }
        public Vector2 HandlePos{
            get { return Controller.CenterHandlePos; }
        }
        public Vector2 NextHandlePos {
            get { return Controller.NextHandlePos; }
        }
        public float Angle{
            get { return Controller.Angle1; }
        }

        public float PrevHandleLength { get { return Controller.PrevHandleLength; } }
        public float NextHandleLength { get { return Controller.NextHandleLength; } }

        public BezierCurve PrevCurveReference{
            set { 
                _prevCurve = value;
                if (_prevLines == null) {
                    _prevLines = new List<Line>(_linesPerSide);
                    for (int i = 0; i < _linesPerSide; i++) {
                        _prevLines.Add(_elementCollection.Add<Line>(_lineTemplate.GenerateLine()));
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
                        _nextLines.Add(_elementCollection.Add<Line>(_lineTemplate.GenerateLine()));
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

            Controller.Angle1 = angle;
            if (_prevLines == null) {
                _prevLines = new List<Line>(_linesPerSide);

                for (int i = 0; i < _linesPerSide; i++) {
                    _prevLines.Add(_elementCollection.Add<Line>(_lineTemplate.GenerateLine()));
                }
            }
            else {
                for (int i = 0; i < _linesPerSide; i++) {
                    _prevLines[i].Dispose();
                    _prevLines[i] = _elementCollection.Add<Line>(_lineTemplate.GenerateLine());
                }
            }

            if (_nextLines == null) {
                _nextLines = new List<Line>(_linesPerSide);
                for (int i = 0; i < _linesPerSide; i++) {
                    _nextLines.Add(_elementCollection.Add<Line>(_lineTemplate.GenerateLine()));
                }
            }
            else {
                for (int i = 0; i < _linesPerSide; i++) {
                    _nextLines[i].Dispose();
                    _nextLines[i] = _elementCollection.Add<Line>(_lineTemplate.GenerateLine());
                }
            }

            Update();
        }

        public void SetPrevCurve(BezierCurve val){
            _prevCurve = val;
            _prevCurve.NextCurveReference = this;
            if (_prevLines == null) {
                _prevLines = new List<Line>(_linesPerSide);

                for (int i = 0; i < _linesPerSide; i++) {
                    _prevLines.Add(_elementCollection.Add<Line>(_lineTemplate.GenerateLine()));
                }
            }
            else {
                for (int i = 0; i < _linesPerSide; i++) {
                    _prevLines[i].Dispose();
                    _prevLines[i] = _elementCollection.Add<Line>(_lineTemplate.GenerateLine());
                }
            }
            Update();
        }

        public void SetNextCurve(BezierCurve val) {
            _nextCurve = val;
            _nextCurve.PrevCurveReference = this;
            if (_nextLines == null) {
                _nextLines = new List<Line>(_linesPerSide);
                for (int i = 0; i < _linesPerSide; i++) {
                    _nextLines.Add(_elementCollection.Add<Line>(_lineTemplate.GenerateLine()));
                }
            }
            else {
                for (int i = 0; i < _linesPerSide; i++) {
                    _nextLines[i].Dispose();
                    _nextLines[i] = _elementCollection.Add<Line>(_lineTemplate.GenerateLine());
                }
            }
            Update();
        }


        #endregion

        /// <summary>
        /// usually used to add new curves between two points
        /// </summary>
        public BezierCurve( float offsetX, float offsetY, UIElementCollection parentCollection,CurveInitalizeData initData = null){
            if (initData != null) {
                Controller = new CurveController(
                    initData.HandlePosX + offsetX,
                    initData.HandlePosY + offsetY,
                    parentCollection,
                    initData.Length1,
                    initData.Length2,
                    initData.Angle
                    );
            }
            else{
                //lol
            }
            _elementCollection = parentCollection;
            _nextLines = null;
            _nextCurve = null;
            _prevCurve = null;
            _prevLines = null;
            
            _lineTemplate = new LineGenerator();
            _lineTemplate.V1 = Vector2.Zero;
            _lineTemplate.V2 = Vector2.Zero;
            _lineTemplate.Color = Color.White;
            _lineTemplate.Depth = DepthLevel.Low;
            _lineTemplate.Owner = _elementCollection;

        }
        public float GetNextArcLength() {
            float len = 0;
            if (_nextLines != null) {
                len += _nextLines.Sum(line => line.Length);
            }
            return len;
        }
        public float GetPrevArcLength(){
            float len = 0;
            if (_prevLines != null){
                len += _prevLines.Sum(line => line.Length);
            }
            return len;
        }

        public void Dispose(){
            throw new NotImplementedException();
        }

        public Vector2 PrevContains(MouseState state, out float t){
            var mousePoint = new Vector2(state.X,state.Y);
            const int width = 5;

            if (_prevLines != null){
                for (int i = 0; i < _prevLines.Count; i++){
                    if (Vector2.Distance(_prevLines[i].DestPoint, mousePoint) < width) {
                        t = ((float)i) / (_linesPerSide * 2) + 0.5f;
                        return new Vector2(_prevLines[i].DestPoint.X, _prevLines[i].DestPoint.Y);
                    }

                }
                foreach (var line in _prevLines){

                }
            }
            //todo: fix these returns to not break on zero
            t = -1;
            return Vector2.Zero;
        }

        public Vector2 NextContains(MouseState state, out float t){
            var mousePoint = new Vector2(state.X, state.Y);
            const int width = 5;
            if (_nextLines != null){
                for (int i = 0; i < _nextLines.Count; i++) {
                    if (Vector2.Distance(_nextLines[i].DestPoint, mousePoint) < width) {

                        t = ((float)i) / (_linesPerSide*2);
                        return new Vector2(_nextLines[i].DestPoint.X, _nextLines[i].DestPoint.Y);
                    }

                }
            }
            t = -1;
            return Vector2.Zero;

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
