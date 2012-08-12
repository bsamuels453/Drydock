#region

using System;
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

    internal class CurveControllerCollection : ICanReceiveInputEvents{
        public readonly List<BezierCurve> CurveList;
        public readonly UIElementCollection ElementCollection;
        public readonly float PixelsPerMeter;

        public CurveControllerCollection(string defaultConfig,FloatingRectangle areaToFill, UIElementCollection parentCollection = null){
            InputEventDispatcher.EventSubscribers.Add(this);
            if (parentCollection != null) {
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
            CurveList = new List<BezierCurve>(numControllers);

            for (int i = 0; i < numControllers; i++){
                curveInitData.Add(new CurveInitalizeData(defaultConfig, i));
            }

            //now get meters per pixel and scales
            float maxX=0;
            float maxY=0;
            foreach (var data in curveInitData){
                if (data.HandlePosX > maxX){
                    maxX = data.HandlePosX;
                }
                if (data.HandlePosY > maxY) {
                    maxY = data.HandlePosY;
                }
            }
            float scaleX = areaToFill.Width / maxX;
            float scaleY = areaToFill.Height / maxY;
            float scale = scaleX > scaleY ? scaleY : scaleX; //scale can also be considered pixels per meter
            PixelsPerMeter = scale;

            float offsetX = (areaToFill.Width - maxX*scale)/2;
            float offsetY = (areaToFill.Height - maxY*scale) / 2;

            foreach (var data in curveInitData){
                data.HandlePosX *= scale;
                data.HandlePosY *= scale;
                data.HandlePosX += offsetX + areaToFill.X;
                data.HandlePosY += offsetY + areaToFill.Y;
                data.Length1 *= scale;
                data.Length2 *= scale;
            }

            for (int i = 0; i < numControllers; i++){
                CurveList.Add(new BezierCurve(0, 0, ElementCollection,curveInitData[i]));
            }
            for (int i = 1; i < numControllers - 1; i++){
                CurveList[i].SetPrevCurve(CurveList[i - 1]);
                CurveList[i].SetNextCurve(CurveList[i + 1]);
            }
        }

        /// <summary>
        /// Returns the point on the curve associated with the parameter t
        /// </summary>
        /// <param name="t">range from 0-1f</param>
        /// <returns></returns>
        public Vector2 GetParameterizedPoint(float t){
            var lenList = new float[CurveList.Count];
            float totalArcLen = 0;
            for (int i = 0; i < lenList.Count(); i++){
                lenList[i] = CurveList[i].GetArcLength();
                totalArcLen += lenList[i];
            }

            float pointArcLen = totalArcLen * t;
            float tempLen = pointArcLen;

            //figure out which curve is going to contain point t
            int curveIndex = 0;
            for (curveIndex = 0; curveIndex < lenList.Count(); curveIndex++) {
                tempLen -= lenList[curveIndex];
                if ((int)tempLen <= 0) {
                    tempLen += lenList[curveIndex];
                    tempLen /= lenList[curveIndex];//this turns tempLen into a t(0-1)
                    break;
                }
            }
            if (curveIndex == 0) {
                tempLen += 1f;
                tempLen /= 2;
            }
            if (curveIndex == lenList.Count() - 1) {
                tempLen /= 2;
            }

            Vector2 unNormalizedPoint = CurveList[curveIndex].GetBezierValue(tempLen); 
            //var unNormalizedPoint = new Vector2();
            //now we need to normalize the point to meters
            float  minX = 9999999, minY = 9999999;
            foreach (var curve in CurveList) {
                if (curve.HandlePos.X < minX) {
                    minX = curve.HandlePos.X;
                }
                if (curve.HandlePos.Y < minY) {
                    minY = curve.HandlePos.Y;
                }
            }

            var normalizedPoint = new Vector2();
            normalizedPoint.X = (unNormalizedPoint.X - minX) / PixelsPerMeter;
            normalizedPoint.Y = (unNormalizedPoint.Y - minY) / PixelsPerMeter;

            return normalizedPoint;
        }

        public void Update() {
            foreach (var curve in CurveList) {
                curve.Update();
            }
        }

        #region ICanReceiveInputEvents Members

        public InterruptState OnLeftButtonClick(MouseState state){
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl)){
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
            }

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

    }

    #region nested struct

        class CurveInitalizeData{
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