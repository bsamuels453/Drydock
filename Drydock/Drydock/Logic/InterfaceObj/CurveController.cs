using System;
using Drydock.Render;
using Microsoft.Xna.Framework;

namespace Drydock.Logic.InterfaceObj{
    internal class CurveController{
        private readonly CurveHandle _centerHandle;
        private readonly CurveHandle _handle1;
        private readonly CurveHandle _handle2;
        private readonly Line2D _line1;
        private readonly Line2D _line2;

        #region properties

        public Vector2 CenterHandlePos{
            get { return _centerHandle.CentPosition; }
        }

        public Vector2 PrevHandlePos{
            get { return _handle1.CentPosition; }
        }

        public Vector2 NextHandlePos{
            get { return _handle2.CentPosition; }
        }

        #endregion

        public CurveController(int initX, int initY, float length1, float length2, float angle1){
            Vector2 component1 = Common.GetComponentFromAngle(angle1, length1);
            Vector2 component2 = Common.GetComponentFromAngle((float) (angle1 - Math.PI), length2); // minus math.pi to reverse direction
            _handle1 = new CurveHandle(
                (int) component1.X + initX,
                (int) component1.Y + initY,
                1,
                this
                );
            _handle2 = new CurveHandle(
                (int) component2.X + initX,
                (int) component2.Y + initY,
                2,
                this
                );
            _centerHandle = new CurveHandle(
                initX,
                initY,
                0,
                this
                );

            _line1 = new Line2D(_centerHandle.CentX, _centerHandle.CentY, _handle1.CentX, _handle1.CentY);
            _line2 = new Line2D(_centerHandle.CentX, _centerHandle.CentY, _handle2.CentX, _handle2.CentY);
        }

        /// <summary>
        /// this function balances handle movement so that they stay in a straight line and their movements translate to other handles
        /// </summary>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="id"></param>
        public void BalanceHandleMovement(int newX, int newY, int dx, int dy, int id){ //nice naming
            switch (id){
                case 0:
                    _line1.TranslateOrigin(dx, dy);
                    _line1.TranslateDestination(dx, dy);
                    _line2.TranslateOrigin(dx, dy);
                    _line2.TranslateDestination(dx, dy);

                    _handle1.ManualTranslation(dx, dy);
                    _handle2.ManualTranslation(dx, dy);
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
    }
}