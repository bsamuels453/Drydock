using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Logic;
using Drydock.UI;
using Microsoft.Xna.Framework;

namespace Drydock.Utilities{
    /// <summary>
    /// REMEMEBER TO THROW AWAY THIS CLASS'S INSTANCES EVERY TIME THE RELEVANT CURVE CHANGES!
    /// </summary>
    internal class BezierIntersect{
        private readonly List<BezierInfo> _curveinfo;
        private readonly int _resolution; //represents the number of halfing operations required to get a result with an accuracy of less than one pixel in the worst case


        public BezierIntersect(List<BezierInfo> curveinfo) {
            _curveinfo = curveinfo;
            float dist = 0;
            for (int i = 0; i < _curveinfo.Count - 1; i++) {
                dist += Vector2.Distance(_curveinfo[i].Pos, _curveinfo[i + 1].Pos);
            }

            float estimatedArcLen = dist*2;

            int powResult = 1;
            int pow = 1;
            while (powResult < estimatedArcLen){
                pow++;
                powResult = (int) Math.Pow(2, pow);
            }
            _resolution = pow;
        }

        public Vector2 GetIntersectionFromX(float x){
            //we run on the assumption that t=0 is on the left, and t=1 is on the right
            //we also run on the assumption that the given controllers form a curve that passes the vertical line test

            //using the assumption of vertical line test and sorted controllers left->right, we can figure out which segment to check for intersection
            int curvesToUse=-1;
            for (int i = 0; i < _curveinfo.Count - 1; i++){
                if (x > _curveinfo[i].Pos.X && x < _curveinfo[i + 1].Pos.X){
                    curvesToUse = i;
                }
            }
            if (curvesToUse == -1){
                throw new Exception("Supplied X value is not contained within the bezier curve collection");
            }

            var leftbound = new Pair<float, Vector2>();
            var rightbound = new Pair<float, Vector2>();
            leftbound.Val1 = 0;
            rightbound.Val1 = 1;
            GenerateBoundValues(leftbound, curvesToUse);
            GenerateBoundValues(rightbound, curvesToUse);

            int numRuns = 0;
            while (numRuns++ <= _resolution){
                float leftDist = Math.Abs(leftbound.Val2.X - x);
                float rightDist = Math.Abs(rightbound.Val2.X - x);

                if (leftDist > rightDist){ //adjust the leftbound
                    leftbound.Val1 += (rightbound.Val1 - leftbound.Val1)/2;
                }
                else{ //adjust the rightbound
                    rightbound.Val1 -= (rightbound.Val1 - leftbound.Val1)/2;
                }
                GenerateBoundValues(leftbound, curvesToUse);
                GenerateBoundValues(rightbound, curvesToUse);
            }
            return (leftbound.Val2 + rightbound.Val2)/2;
        }


        private void GenerateBoundValues(Pair<float, Vector2> bound, int curvesToUse){
            Bezier.GetBezierValue(
                out bound.Val2,
                _curveinfo[curvesToUse].Pos,
                _curveinfo[curvesToUse].NextControl,
                _curveinfo[curvesToUse+1].PrevControl,
                _curveinfo[curvesToUse+1].Pos,
                bound.Val1
                );

        }




    }

    public struct BezierInfo{
        public Vector2 Pos;
        public Vector2 PrevControl;
        public Vector2 NextControl;

        public BezierInfo(Vector2 pos, Vector2 prev, Vector2 next){
            Pos = pos;
            PrevControl = prev;
            NextControl = next;
        }

    }
}
