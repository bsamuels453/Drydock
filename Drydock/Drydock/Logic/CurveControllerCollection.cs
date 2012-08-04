#region

using System.Collections.Generic;
using Drydock.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic{
    internal class CurveControllerCollection{
        private const int _segmentsBetweenControllers = 40;
        private const int _initlNumCurves = 4;

        private readonly List<BezierCurve> _curveList;

        public CurveControllerCollection(){
            _curveList = new List<BezierCurve>(_initlNumCurves);
            MouseHandler.ClickSubscriptions.Add(HandleMouseClick);
            int x=200;
            int y=200;
            for (int i = 0; i < _initlNumCurves; i++){
                _curveList.Add(new BezierCurve(x, y, _segmentsBetweenControllers));
                x += 200;
            }
            for (int i = 1; i < _initlNumCurves - 1; i++){
                _curveList[i].SetPrevCurve(_curveList[i - 1], 1.5f, 100);
                _curveList[i].SetNextCurve(_curveList[i + 1], 1.5f, 100);
            }
        }

        private bool HandleMouseClick(MouseState state){
            Point pos;
            if(state.LeftButton == ButtonState.Pressed){
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl)){
                    for (int i = 1; i < _curveList.Count; i++){
                        if ((pos = _curveList[i].PrevContains(state)) != Point.Zero){
                            //i -= 1;
                            _curveList.Insert(i, new BezierCurve(pos.X, pos.Y, _segmentsBetweenControllers));
                            _curveList[i].NextCurve = _curveList[i + 1];
                            _curveList[i].PrevCurve = _curveList[i - 1];
                            return true;
                        }
                    }
                    for (int i = 0; i < _curveList.Count-1; i++) {


                        if ((pos = _curveList[i].NextContains(state)) != Point.Zero) {
                            i += 1;
                           
                            _curveList.Insert(i, new BezierCurve(pos.X, pos.Y, _segmentsBetweenControllers));
                            _curveList[i].NextCurve = _curveList[i + 1];
                            _curveList[i].PrevCurve = _curveList[i - 1];
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void Update(){
            foreach (var curve in _curveList){
                curve.Update();
            }
        }
    }
}