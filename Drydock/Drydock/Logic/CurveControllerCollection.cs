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
                _curveList[i].PrevCurve = _curveList[i - 1];
                _curveList[i].NextCurve = _curveList[i + 1];

                _curveList[i + 1].PrevCurve = _curveList[i];
                _curveList[i - 1].NextCurve = _curveList[i];
            }
        }

        /// <summary>
        /// insert a new curve controller handle at the specified line segment 
        /// </summary>
        /// <param name="x"> </param>
        /// <param name="y"> </param>
        /// <param name="insertPos"> </param>
        private void AddController(int x, int y, int insertPos) {
            _curveList.Insert(insertPos, new BezierCurve(x, y, _segmentsBetweenControllers, _curveList[insertPos], _curveList[insertPos + 1]));
        }

        private bool HandleMouseClick(MouseState state){
            Point pos;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl)){
                for (int i = 0; i < _curveList.Count; i++){
                    if ((pos = _curveList[i].Contains(state)) == Point.Zero){
                        AddController(pos.X, pos.Y, i);
                        return true;
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