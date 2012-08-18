#region

using System;
using System.Collections.Generic;
using System.Linq;
using Drydock.UI;
using Drydock.UI.Components;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic{
    /// <summary>
    ///   this class is a fucking mess, here's to hoping it never has to be used again
    /// </summary>
    internal class BezierCurve{
        #region private fields

        private const int _linesPerSide = 50;
        private readonly Button _centerHandle;
        private readonly UIElementCollection _elementCollection;
        private readonly Button _handle1;
        private readonly Button _handle2;
        private readonly Line _line1;
        private readonly Line _line2;
        private readonly LineGenerator _lineTemplate;
        private BezierCurve _nextCurve;
        private List<Line> _nextLines;
        private BezierCurve _prevCurve;
        private List<Line> _prevLines;

        #endregion

        #region neighbor info and setters

        public BezierCurve PrevCurveReference{
            set{
                _prevCurve = value;
                if (_prevLines == null){
                    _prevLines = new List<Line>(_linesPerSide);
                    for (int i = 0; i < _linesPerSide; i++){
                        _prevLines.Add(_elementCollection.Add<Line>(_lineTemplate.GenerateLine()));
                    }
                }
            }
        }

        public BezierCurve NextCurveReference{
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
            Bezier.GetBezierValue(out pt1, _prevCurve.NextHandlePos, _prevCurve.NextHandlePos, PrevHandlePos, CenterHandlePos, t);
            Bezier.GetBezierValue(out pt2, _prevCurve.NextHandlePos, _prevCurve.NextHandlePos, PrevHandlePos, CenterHandlePos, t + 0.001f); //limits are for fags

            //get tangent and set to angle
            Vector2 pt3 = pt1 - pt2;
            float angle, magnitude;

            Common.GetAngleFromComponents(out angle, out magnitude, pt3.X, pt3.Y);

            Angle = angle;
            if (_prevLines == null){
                _prevLines = new List<Line>(_linesPerSide);

                for (int i = 0; i < _linesPerSide; i++){
                    _prevLines.Add(_elementCollection.Add<Line>(_lineTemplate.GenerateLine()));
                }
            }
            else{
                for (int i = 0; i < _linesPerSide; i++){
                    _prevLines[i].Dispose();
                    _prevLines[i] = _elementCollection.Add<Line>(_lineTemplate.GenerateLine());
                }
            }

            if (_nextLines == null){
                _nextLines = new List<Line>(_linesPerSide);
                for (int i = 0; i < _linesPerSide; i++){
                    _nextLines.Add(_elementCollection.Add<Line>(_lineTemplate.GenerateLine()));
                }
            }
            else{
                for (int i = 0; i < _linesPerSide; i++){
                    _nextLines[i].Dispose();
                    _nextLines[i] = _elementCollection.Add<Line>(_lineTemplate.GenerateLine());
                }
            }

            Update();
        }

        public void SetPrevCurve(BezierCurve val){
            _prevCurve = val;
            _prevCurve.NextCurveReference = this;
            if (_prevLines == null){
                _prevLines = new List<Line>(_linesPerSide);

                for (int i = 0; i < _linesPerSide; i++){
                    _prevLines.Add(_elementCollection.Add<Line>(_lineTemplate.GenerateLine()));
                }
            }
            else{
                for (int i = 0; i < _linesPerSide; i++){
                    _prevLines[i].Dispose();
                    _prevLines[i] = _elementCollection.Add<Line>(_lineTemplate.GenerateLine());
                }
            }
            Update();
        }

        public void SetNextCurve(BezierCurve val){
            _nextCurve = val;
            _nextCurve.PrevCurveReference = this;
            if (_nextLines == null){
                _nextLines = new List<Line>(_linesPerSide);
                for (int i = 0; i < _linesPerSide; i++){
                    _nextLines.Add(_elementCollection.Add<Line>(_lineTemplate.GenerateLine()));
                }
            }
            else{
                for (int i = 0; i < _linesPerSide; i++){
                    _nextLines[i].Dispose();
                    _nextLines[i] = _elementCollection.Add<Line>(_lineTemplate.GenerateLine());
                }
            }
            Update();
        }

        #endregion

        #region curve information

        public Vector2 CenterHandlePos{
            get { return _centerHandle.CentPosition; }
        }

        public Vector2 PrevHandlePos{
            get { return _handle1.CentPosition; }
        }

        public Vector2 NextHandlePos{
            get { return _handle2.CentPosition; }
        }

        public float PrevHandleLength{
            get { return _line1.Length; }
        }

        public float NextHandleLength{
            get { return _line2.Length; }
        }

        public float Angle{
            set{
                _line1.Angle = value;
                _line2.Angle = (float) (value + Math.PI);
                _handle1.CentPosition = _line1.DestPoint;
                _handle2.CentPosition = _line2.DestPoint;
            }
            get { return _line1.Angle; }
        }

        public float GetNextArcLength(){
            float len = 0;
            if (_nextLines != null){
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

        public Vector2 PrevContains(MouseState state, out float t){
            var mousePoint = new Vector2(state.X, state.Y);
            const int width = 5;

            if (_prevLines != null){
                for (int i = 0; i < _prevLines.Count; i++){
                    if (Vector2.Distance(_prevLines[i].DestPoint, mousePoint) < width){
                        t = ((float) i)/(_linesPerSide*2) + 0.5f;
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
                for (int i = 0; i < _nextLines.Count; i++){
                    if (Vector2.Distance(_nextLines[i].DestPoint, mousePoint) < width){
                        t = ((float) i)/(_linesPerSide*2);
                        return new Vector2(_nextLines[i].DestPoint.X, _nextLines[i].DestPoint.Y);
                    }
                }
            }
            t = -1;
            return Vector2.Zero;
        }

        #endregion

        #region curve modification

        /// <summary>
        ///   this method will move the entire controller by the defined dx and dy
        /// </summary>
        /// <param name="dx"> </param>
        /// <param name="dy"> </param>
        public void TranslateControllerPos(int dx, int dy){
            _line1.TranslateOrigin(dx, dy);
            _line1.TranslateDestination(dx, dy);
            _line2.TranslateOrigin(dx, dy);
            _line2.TranslateDestination(dx, dy);
            _handle1.X += dx;
            _handle1.Y += dy;

            _handle2.X += dx;
            _handle2.Y += dy;

            _centerHandle.X += dx;
            _centerHandle.Y += dy;
        }

        #endregion

        #region ctor and disposal

        /// <summary>
        ///   usually used to add new curves between two points
        /// </summary>
        public BezierCurve(float offsetX, float offsetY, UIElementCollection parentCollection, CurveInitalizeData initData){
            float initX = initData.HandlePosX + offsetX;
            float initY = initData.HandlePosY + offsetY;


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


            Vector2 component1 = Common.GetComponentFromAngle(initData.Angle, initData.Length1);
            Vector2 component2 = Common.GetComponentFromAngle((float) (initData.Angle - Math.PI), initData.Length2); // minus math.pi to reverse direction

            #region stuff for generating ui elements

            var buttonTemplate = new ButtonGenerator();
            buttonTemplate.Width = 9;
            buttonTemplate.Height = 9;
            buttonTemplate.Depth = DepthLevel.Medium;
            buttonTemplate.Owner = _elementCollection;
            buttonTemplate.TextureName = "whitebox";
            buttonTemplate.Components = new Dictionary<string, object[]>{
                {"DraggableComponent", null},
                {"FadeComponent", new object[]{FadeComponent.FadeState.Faded, FadeComponent.FadeTrigger.EntryExit}},
                //{"SelectableComponent", new object[]{"bigbox", 15, 15}}
            };

            var lineTemplate = new LineGenerator();
            lineTemplate.Depth = DepthLevel.Medium;
            lineTemplate.Owner = _elementCollection;
            lineTemplate.Color = Color.Black;
            lineTemplate.Components = new Dictionary<string, object[]>{
                {"FadeComponent", new object[]{FadeComponent.FadeState.Faded}}
            };

            buttonTemplate.Identifier = 1;
            buttonTemplate.X = component1.X + initX;
            buttonTemplate.Y = component1.Y + initY;
            _handle1 = _elementCollection.Add<Button>(buttonTemplate.GenerateButton());
            _handle1.GetComponent<DraggableComponent>().DragMovementDispatcher += ReactToDragMovement;

            buttonTemplate.Identifier = 2;
            buttonTemplate.X = component2.X + initX;
            buttonTemplate.Y = component2.Y + initY;
            _handle2 = _elementCollection.Add<Button>(buttonTemplate.GenerateButton());
            _handle2.GetComponent<DraggableComponent>().DragMovementDispatcher += ReactToDragMovement;

            buttonTemplate.Identifier = 0;
            buttonTemplate.X = initX;
            buttonTemplate.Y = initY;
            _centerHandle = _elementCollection.Add<Button>(buttonTemplate.GenerateButton());
            _centerHandle.GetComponent<DraggableComponent>().DragMovementDispatcher += ReactToDragMovement;

            lineTemplate.V1 = _centerHandle.CentPosition;
            lineTemplate.V2 = _handle1.CentPosition;
            _line1 = _elementCollection.Add<Line>(lineTemplate.GenerateLine());

            lineTemplate.V2 = _handle2.CentPosition;
            _line2 = _elementCollection.Add<Line>(lineTemplate.GenerateLine());

            #endregion

            InterlinkButtonEvents();
        }

        public void Dispose(){
            throw new NotImplementedException();
        }

        #endregion

        #region event stuff

        public OnDragMovement ReactToControllerMovement;

        /// <summary>
        ///   this function balances handle movement so that they stay in a straight line and their movements translate to other handles
        /// </summary>
        public void ReactToDragMovement(object caller, int dx, int dy){
            var calle = (Button) caller;
            switch (calle.Identifier){
                case 0:
                    _line1.TranslateOrigin(dx, dy);
                    _line1.TranslateDestination(dx, dy);
                    _line2.TranslateOrigin(dx, dy);
                    _line2.TranslateDestination(dx, dy);
                    _handle1.X += dx;
                    _handle1.Y += dy;

                    _handle2.X += dx;
                    _handle2.Y += dy;

                    if (ReactToControllerMovement != null){
                        ReactToControllerMovement(this, dx, dy);
                    }

                    break;
                case 1:
                    _line1.TranslateDestination(dx, dy);
                    _line2.Angle = (float) (_line1.Angle + Math.PI);

                    _handle2.X = (int) _line2.DestPoint.X - _handle2.BoundingBox.Width/2;
                    _handle2.Y = (int) _line2.DestPoint.Y - _handle2.BoundingBox.Height/2;

                    break;
                case 2:
                    _line2.TranslateDestination(dx, dy);
                    _line1.Angle = (float) (_line2.Angle + Math.PI);

                    _handle1.X = (int) _line1.DestPoint.X - _handle1.BoundingBox.Width/2;
                    _handle1.Y = (int) _line1.DestPoint.Y - _handle1.BoundingBox.Height/2;
                    break;
            }
        }

        private void ClampHandleMovement(IUIInteractiveElement owner, ref int x, ref int y, int oldX, int oldY){
        }

        #endregion

        public void Update(){
            float t, dt;
            Vector2 firstPos, secondPos;
            if (_prevCurve != null){
                t = 0.5f;
                dt = 1/(float) (_linesPerSide*2);
                foreach (var line in _prevLines){
                    Bezier.GetBezierValue(
                        out firstPos,
                        _prevCurve.CenterHandlePos,
                        _prevCurve.NextHandlePos,
                        PrevHandlePos,
                        CenterHandlePos,
                        t
                        );

                    Bezier.GetBezierValue(
                        out secondPos,
                        _prevCurve.CenterHandlePos,
                        _prevCurve.NextHandlePos,
                        PrevHandlePos,
                        CenterHandlePos,
                        t + dt
                        );
                    line.OriginPoint = firstPos;
                    line.DestPoint = secondPos;
                    t += dt;
                }
            }
            if (_nextCurve != null){
                t = 0;
                dt = 1/(float) (_linesPerSide*2);
                foreach (var line in _nextLines){
                    Bezier.GetBezierValue(
                        out firstPos,
                        CenterHandlePos,
                        NextHandlePos,
                        _nextCurve.PrevHandlePos,
                        _nextCurve.CenterHandlePos,
                        t
                        );

                    Bezier.GetBezierValue(
                        out secondPos,
                        CenterHandlePos,
                        NextHandlePos,
                        _nextCurve.PrevHandlePos,
                        _nextCurve.CenterHandlePos,
                        t + dt
                        );
                    line.OriginPoint = firstPos;
                    line.DestPoint = secondPos;
                    t += dt;
                }
            }
        }

        private void InterlinkButtonEvents(){
            FadeComponent.LinkFadeComponentTriggers(_handle1, _handle2, FadeComponent.FadeTrigger.EntryExit);
            FadeComponent.LinkFadeComponentTriggers(_handle1, _centerHandle, FadeComponent.FadeTrigger.EntryExit);
            FadeComponent.LinkFadeComponentTriggers(_handle2, _centerHandle, FadeComponent.FadeTrigger.EntryExit);


            FadeComponent.LinkOnewayFadeComponentTriggers(
                eventProcElements: new IUIElement[]{
                    _handle1,
                    _handle2,
                    _centerHandle,
                },
                eventRecieveElements: new IUIElement[]{
                    _line1,
                    _line2,
                },
                state: FadeComponent.FadeTrigger.EntryExit
                );
        }

        /* //this might be useful someday
        private Vector2 GetPerpendicularBisector(Vector2 originPoint, Vector2 destPoint, Vector2 perpendicularPoint) {

            destPoint *= 10;

            var list = Common.Bresenham(originPoint, destPoint);
            var distances = new List<float>(list.Count);

            for (int i = 0; i < list.Count; i++) {
                distances.Add(Vector2.Distance(list[i], perpendicularPoint));
            }
            float lowestValue = 9999999999; //my condolences to players with screens larger than 9999999999x9999999999
            int lowestIndex = -1;

            for (int i = 0; i < distances.Count; i++) {
                if (distances[i] < lowestValue) {
                    lowestIndex = i;
                    lowestValue = distances[i];
                }
            }

            return list[lowestIndex];
        }
        */
    }
}