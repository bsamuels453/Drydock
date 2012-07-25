using System;
using Drydock.Render;
using Microsoft.Xna.Framework;

namespace Drydock.Logic{
    internal class CurveController{
        private readonly CurveHandle _centerHandle;
        private readonly CurveHandle _handle1;
        private readonly CurveHandle _handle2;
        private readonly Line2D _line1;
        private readonly Line2D _line2;

        private float _angle;

        public CurveController(int initlX, int initlY, float initlLength1, float initlLength2, float initlAngle){
            _centerHandle = new CurveHandle(initlX, initlY, 0, this);
            Vector2 component1 = Common.GetComponent(initlAngle, initlLength1);
            Vector2 component2 = Common.GetComponent((float) (initlAngle - Math.PI), initlLength2); // minus math.pi to reverse direction
            _handle1 = new CurveHandle((int) component1.X + initlX, (int) component1.Y + initlY, 1, this);
            _handle2 = new CurveHandle((int) component2.X + initlX, (int) component2.Y + initlY, 2, this);

            _line1 = new Line2D(_centerHandle.CentX, _centerHandle.CentY, _handle1.CentX, _handle1.CentY);
            _line2 = new Line2D(_centerHandle.CentX, _centerHandle.CentY, _handle2.CentX, _handle2.CentY);
        }

        public void HandleHandleMovement(int newX, int newY, int id){ //nice naming
            switch (id){
                case 0:
                    _line1.OriginPoint = new Vector2(newX, newY);
                    _line2.OriginPoint = new Vector2(newX, newY);
                    break;
                case 1:
                    _line1.DestPoint = new Vector2(newX, newY);
                    break;
                case 2:
                    _line2.DestPoint = new Vector2(newX, newY);
                    break;
            }
        }
    }
}