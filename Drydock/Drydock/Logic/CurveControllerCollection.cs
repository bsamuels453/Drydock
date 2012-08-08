﻿#region

using System;
using System.Collections.Generic;
using Drydock.Control;
using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic{
    internal class CurveControllerCollection : ICanReceiveInputEvents{
        private const int _segmentsBetweenControllers = 40;
        public readonly UIElementCollection ElementCollection;

        public readonly List<BezierCurve> CurveList;

        public CurveControllerCollection(string defaultConfig){
            InputEventDispatcher.EventSubscribers.Add(this);
            ElementCollection = new UIElementCollection();

            var config = new ConfigRetriever(defaultConfig);
            int numControllers = int.Parse(config.GetValue("NumControllers"));
            var curveInitData = new List<CurveInitalizeData>(numControllers);
            CurveList = new List<BezierCurve>(numControllers);

            for (int i = 0; i < numControllers; i++){
                curveInitData.Add(new CurveInitalizeData(defaultConfig, i));

            }

            for (int i = 0; i < numControllers; i++){
                CurveList.Add(new BezierCurve(0, 0, ElementCollection,curveInitData[i]));
            }
            for (int i = 1; i < numControllers - 1; i++){
                CurveList[i].SetPrevCurve(CurveList[i - 1], curveInitData[i].Angle, 20);
                CurveList[i].SetNextCurve(CurveList[i + 1], curveInitData[i].Angle - (float)Math.PI, 20);
            }
        }

        public void Update(){
            foreach (var curve in CurveList){
                curve.Update();
            }
        }

        public InterruptState OnLeftButtonClick(MouseState state){
            Vector2 pos;
            float t;

            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl)){
                for (int i = 1; i < CurveList.Count; i++){
                    if ((pos = CurveList[i].PrevContains(state, out t)) != Vector2.Zero){
                        //i -= 1;
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
            public float HandlePosX;
            public float HandlePosY;
            public float Angle;
            public float Length1;
            public float Length2;

            public void RetrieveDataFromXML(string xmlFile, int i){
                var config = new ConfigRetriever(xmlFile);
                HandlePosX = float.Parse(config.GetValue("Handle" + i + "X"));
                HandlePosY = float.Parse(config.GetValue("Handle" + i + "Y"));
                Angle = float.Parse(config.GetValue("Handle" + i + "Angle"));
                Length1 = float.Parse(config.GetValue("Handle" + i + "Length1"));
                Length2 = float.Parse(config.GetValue("Handle" + i + "Length2"));
            }

            public CurveInitalizeData(string xmlFile, int i){
                var config = new ConfigRetriever(xmlFile);
                HandlePosX = float.Parse(config.GetValue("Handle" + i + "X"));
                HandlePosY = float.Parse(config.GetValue("Handle" + i + "Y"));
                Angle = float.Parse(config.GetValue("Handle" + i + "Angle"));
                Length1 = float.Parse(config.GetValue("Handle" + i + "Length1"));
                Length2 = float.Parse(config.GetValue("Handle" + i + "Length2"));
            }

        }

        #endregion
}