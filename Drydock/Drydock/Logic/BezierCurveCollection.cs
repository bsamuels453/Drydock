#region

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Drydock.Control;
using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic{
    internal class BezierCurveCollection : ICanReceiveInputEvents, IEnumerable<BezierCurve>{
        #region fields

        public readonly UIElementCollection ElementCollection;
        public readonly float PixelsPerMeter;
        private readonly List<BezierCurve> _curveList;

        public double MaxX;
        public double MaxY;

        public BezierCurve MaxYCurve;
        public double MinX;
        public double MinY;
        private double[] _lenList;
        private double _totalArcLen;

        #endregion

        public BezierCurveCollection(string defaultConfig, FloatingRectangle areaToFill, UIElementCollection parentCollection = null){
            InputEventDispatcher.EventSubscribers.Add(this);
            if (parentCollection != null){
                ElementCollection = parentCollection.Add(new UIElementCollection());
            }
            else{
                ElementCollection = new UIElementCollection();
            }

            var reader = XmlReader.Create(defaultConfig);
            reader.ReadToFollowing("NumControllers");
            int numControllers = int.Parse(reader.ReadString());
            reader.Close();
            var curveInitData = new List<CurveInitalizeData>(numControllers);
            _curveList = new List<BezierCurve>(numControllers);

            for (int i = 0; i < numControllers; i++){
                curveInitData.Add(new CurveInitalizeData(defaultConfig, i));
            }

            //now get meters per pixel and scales
            float maxX = 0;
            float maxY = 0;
            foreach (var data in curveInitData){
                if (data.HandlePosX > maxX){
                    maxX = data.HandlePosX;
                }
                if (data.HandlePosY > maxY){
                    maxY = data.HandlePosY;
                }
            }
            float scaleX = areaToFill.Width/maxX;
            float scaleY = areaToFill.Height/maxY;
            float scale = scaleX > scaleY ? scaleY : scaleX; //scale can also be considered pixels per meter
            PixelsPerMeter = scale;

            float offsetX = (areaToFill.Width - maxX*scale)/2;
            float offsetY = (areaToFill.Height - maxY*scale)/2;

            foreach (var data in curveInitData){
                data.HandlePosX *= scale;
                data.HandlePosY *= scale;
                data.HandlePosX += offsetX + areaToFill.X;
                data.HandlePosY += offsetY + areaToFill.Y;
                data.Length1 *= scale;
                data.Length2 *= scale;
            }

            for (int i = 0; i < numControllers; i++){
                _curveList.Add(new BezierCurve(0, 0, ElementCollection, curveInitData[i]));
            }
            for (int i = 1; i < numControllers - 1; i++){
                _curveList[i].SetPrevCurve(_curveList[i - 1]);
                _curveList[i].SetNextCurve(_curveList[i + 1]);
            }
        }

        #region curve information retrieval methods

        /// <summary>
        ///   Returns the point on the curve associated with the parameter t
        /// </summary>
        /// <param name="t"> range from 0-1f </param>
        /// <param name="regenerateMethodCache"> </param>
        /// <returns> </returns>
        public Vector2 GetParameterizedPoint(double t, bool regenerateMethodCache = false){
            if (regenerateMethodCache){
                _lenList = new double[_curveList.Count - 1];
                _totalArcLen = 0;
                for (int i = 0; i < _lenList.Count(); i++){
                    _lenList[i] = _curveList[i].GetNextArcLength() + _curveList[i + 1].GetPrevArcLength();
                    _totalArcLen += _lenList[i];
                }
            }

            double pointArcLen = _totalArcLen*t;
            double tempLen = pointArcLen;

            //figure out which curve is going to contain point t
            int segmentIndex;
            for (segmentIndex = 0; segmentIndex < _lenList.Count(); segmentIndex++){
                tempLen -= _lenList[segmentIndex];
                if (tempLen < 0){
                    tempLen += _lenList[segmentIndex];
                    tempLen /= _lenList[segmentIndex]; //this turns tempLen into a t(0-1)
                    break;
                }
            }

            if (segmentIndex == _curveList.Count - 1){ //clamp it 
                segmentIndex--;
                tempLen = 1;
            }
            Vector2 point = GetBezierValue(_curveList[segmentIndex], _curveList[segmentIndex + 1], tempLen);

            //now we need to normalize the point to meters
            point.X = (float) (point.X - MinX)/PixelsPerMeter;
            point.Y = (float) (point.Y - MinY)/PixelsPerMeter;

            return point;
        }

        public Vector2 GetBezierValue(BezierCurve prevCurve, BezierCurve nextCurve, double t){
            Vector2 retVal;

            Bezier.GetBezierValue(
                out retVal,
                prevCurve.CenterHandlePos,
                prevCurve.NextHandlePos,
                nextCurve.PrevHandlePos,
                nextCurve.CenterHandlePos,
                t
                );

            return retVal;
        }

        /// <summary>
        ///   converts a point from screen pixels to meters.
        /// </summary>
        /// <param name="point"> </param>
        /// <returns> </returns>
        public Vector2 Normalize(Vector2 point){
            point.X = (float) (point.X - MinX)/PixelsPerMeter;
            point.Y = (float) (point.Y - MinY)/PixelsPerMeter;
            return point;
        }

        public double NormalizeY(double y){
            return (y - MinY)/PixelsPerMeter;
        }

        public double NormalizeX(double x){
            return (x - MinX)/PixelsPerMeter;
        }

        #endregion

        #region ICanReceiveInputEvents Members

        public InterruptState OnLeftButtonClick(MouseState state){
            //this is broken right now
            /*if (Keyboard.GetState().IsKeyDown(Keys.LeftControl)){
                Vector2 pos;
                float t;
                for (int i = 1; i < CurveList.Count; i++){
                    if ((pos = CurveList[i].PrevContains(state, out t)) != Vector2.Zero){
                        CurveList.Insert(i, new BezierCurve(pos.X, pos.Y, ElementCollection));
                        CurveList[i].InsertBetweenCurves(CurveList[i - 1], CurveList[i + 1], t);
                        return InterruptState.InterruptEventDispatch;
                    }
                }
                for (int i = 0; i < CurveList.Count - 1; i++){


                    if ((pos = CurveList[i].NextContains(state, out t)) != Vector2.Zero){
                        i += 1;
                        CurveList.Insert(i, new BezierCurve(pos.X, pos.Y, ElementCollection ));
                        CurveList[i].InsertBetweenCurves(CurveList[i - 1], CurveList[i + 1], t);
                        return InterruptState.InterruptEventDispatch;
                    }
                }
            }*/

            return InterruptState.AllowOtherEvents;
        }

        #endregion

        #region unused input events

        public InterruptState OnMouseMovement(MouseState state){
            return InterruptState.AllowOtherEvents;
        }

        public InterruptState OnLeftButtonPress(MouseState state){
            return InterruptState.AllowOtherEvents;
        }

        public InterruptState OnLeftButtonRelease(MouseState state){
            return InterruptState.AllowOtherEvents;
        }

        public InterruptState OnKeyboardEvent(KeyboardState state){
            return InterruptState.AllowOtherEvents;
        }

        #endregion

        public void Update(){
            foreach (var curve in _curveList){
                curve.Update();
            }
            MinX = _curveList[0].CenterHandlePos.X;
            MinY = _curveList[0].CenterHandlePos.Y;
            MaxX = 0;
            MaxY = 0;
            MaxYCurve = _curveList[0];
            foreach (var curve in _curveList){
                if (curve.CenterHandlePos.X < MinX){
                    MinX = curve.CenterHandlePos.X;
                }
                if (curve.CenterHandlePos.Y < MinY){
                    MinY = curve.CenterHandlePos.Y;
                }
                if (curve.CenterHandlePos.X > MaxX){
                    MaxX = curve.CenterHandlePos.X;
                }
                if (curve.CenterHandlePos.Y > MaxY){
                    MaxY = curve.CenterHandlePos.Y;
                    MaxYCurve = curve;
                }
            }
        }

        public List<BezierInfo> GetControllerInfo(float scaleX = 1, float scaleY = 1){
            var li = new List<BezierInfo>(_curveList.Count/2 + 1);
            for (int i = 0; i < _curveList.Count; i++){
                li.Add(
                    new BezierInfo(
                        pos: new Vector2((float) NormalizeX(_curveList[i].CenterHandlePos.X)*scaleX, (float) NormalizeY(_curveList[i].CenterHandlePos.Y)*scaleY),
                        prev: new Vector2((float) NormalizeX(_curveList[i].PrevHandlePos.X)*scaleX, (float) NormalizeY(_curveList[i].PrevHandlePos.Y)*scaleY),
                        next: new Vector2((float) NormalizeX(_curveList[i].NextHandlePos.X)*scaleX, (float) NormalizeY(_curveList[i].NextHandlePos.Y)*scaleY)
                        )
                    );
            }
            return li;
        }

        #region ienumerable members + accessors

        public BezierCurve this[int index]{
            get { return _curveList[index]; }
        }

        public int Count{
            get { return _curveList.Count; }
        }

        public IEnumerator<BezierCurve> GetEnumerator(){
            return ((IEnumerable<BezierCurve>) _curveList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator(){
            return _curveList.GetEnumerator();
        }

        #endregion
    }

    #region nested struct

    internal class CurveInitalizeData{
        public float Angle;
        public float HandlePosX;
        public float HandlePosY;
        public float Length1;
        public float Length2;

        public CurveInitalizeData(string xmlFile, int i){
            var reader = XmlReader.Create(xmlFile);
            reader.ReadToFollowing("Handle" + i);
            reader.ReadToFollowing("PosX");
            HandlePosX = float.Parse(reader.ReadString());
            reader.ReadToFollowing("PosY");
            HandlePosY = float.Parse(reader.ReadString());
            reader.ReadToFollowing("Angle");
            Angle = float.Parse(reader.ReadString());
            reader.ReadToFollowing("PrevLength");
            Length1 = float.Parse(reader.ReadString());
            reader.ReadToFollowing("NextLength");
            Length2 = float.Parse(reader.ReadString());
            reader.Close();
        }
    }

    #endregion
}