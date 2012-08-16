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
        private readonly List<BoundCache> _boundCache;

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

            _boundCache = new List<BoundCache>(curveinfo.Count-1);
            for (int i = 0; i < curveinfo.Count-1; i++){
                _boundCache.Add( new BoundCache(0, 1, curveinfo[i].Pos, curveinfo[i + 1].Pos, 0));
            }
        }

        public Vector2 GetIntersectionFromX(float x) {
            //we run on the assumption that t=0 is on the left, and t=1 is on the right
            //we also run on the assumption that the given controllers form a curve that passes the vertical line test

            //using the assumption of vertical line test and sorted controllers left->right, we can figure out which segment to check for intersection

            if (x <= 0){
                x = 0.000001f;
            }
            if (x >= _boundCache[_boundCache.Count - 1].RightVal.X){
                x = _boundCache[_boundCache.Count - 1].RightVal.X - 0.000001f;
            }


            int curvesToUse=-1;
            for (int i = 0; i < _boundCache.Count; i++) {
                if(_boundCache[i].Contains(x)){
                    curvesToUse = i;
                }
            }
            if (curvesToUse == -1){
                throw new Exception("Supplied X value is not contained within the bezier curve collection");
            }
            //now we traverse the cache
            BoundCache curCache = _boundCache[curvesToUse];
            while (true){
                if (curCache.LeftChild != null){
                    if (curCache.LeftChild.Contains(x)){
                        curCache = curCache.LeftChild;
                        continue;
                    }
                }
                if (curCache.RightChild != null){
                    if (curCache.RightChild.Contains(x)) {
                        curCache = curCache.RightChild;
                        continue;
                    }
                }
                break;
            }

            //now continue until we get to dest
            int numRuns = curCache.Depth;
            while (numRuns++ <= _resolution){

                Vector2 v = GenerateBoundValue((curCache.RightT + curCache.LeftT) / 2, curvesToUse);

                if (x > v.X) { //create a new rightchild
                    //leftbound.Val1 += (rightbound.Val1 - leftbound.Val1)/2;
                    float newLeftT = curCache.LeftT + (curCache.RightT - curCache.LeftT) / 2;

                    curCache.RightChild = new BoundCache(
                        newLeftT,
                        curCache.RightT,
                        GenerateBoundValue(newLeftT, curvesToUse),
                        curCache.RightVal,
                        curCache.Depth + 1
                        );
                    curCache = curCache.RightChild;
                }
                else{ //create a new leftchild
                    //rightbound.Val1 -= (rightbound.Val1 - leftbound.Val1)/2;
                    float newRightT = curCache.RightT - (curCache.RightT - curCache.LeftT) / 2;

                    curCache.LeftChild = new BoundCache(
                        curCache.LeftT,
                        newRightT,
                        curCache.LeftVal,
                        GenerateBoundValue(newRightT, curvesToUse),
                        curCache.Depth + 1
                        );
                    curCache = curCache.LeftChild;
                }

            }

            return (curCache.LeftVal + curCache.RightVal) / 2;
        }


        private float GenerateBoundValueX(float t, int curvesToUse){
            Vector2 v;
            Bezier.GetBezierValue(
                out v,
                _curveinfo[curvesToUse].Pos,
                _curveinfo[curvesToUse].NextControl,
                _curveinfo[curvesToUse+1].PrevControl,
                _curveinfo[curvesToUse+1].Pos,
                t
                );
            return v.X;
        }

        private Vector2 GenerateBoundValue(float t, int curvesToUse) {
            Vector2 v;
            Bezier.GetBezierValue(
                out v,
                _curveinfo[curvesToUse].Pos,
                _curveinfo[curvesToUse].NextControl,
                _curveinfo[curvesToUse + 1].PrevControl,
                _curveinfo[curvesToUse + 1].Pos,
                t
                );
            return v;
        }

        private class BoundCache{
            public readonly float LeftT;
            public readonly float RightT;

            public readonly Vector2 LeftVal;
            public readonly Vector2 RightVal;

            public BoundCache LeftChild;
            public BoundCache RightChild;
            public readonly int Depth;

            public BoundCache(float leftT, float rightT, Vector2 leftVal, Vector2 rightVal, int depth){
                Depth = depth;
                LeftT = leftT;
                RightT = rightT;
                LeftVal = leftVal;
                RightVal = rightVal;
            }

            public bool Contains(float x){
                if (x >= LeftVal.X && x <= RightVal.X){
                    return true;
                }
                return false;
            }
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
