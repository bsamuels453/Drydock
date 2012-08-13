#region

using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Utilities{
    internal static class Bezier{
        private static void Lerp(ref Vector2 dest, Vector2 a, Vector2 b, float t){
            dest.X = a.X + (b.X - a.X)*t;
            dest.Y = a.Y + (b.Y - a.Y)*t;
        }

        private static void DLerp(ref DVector2 dest, DVector2 a, DVector2 b, double t) {
            dest.X = a.X + (b.X - a.X) * t;
            dest.Y = a.Y + (b.Y - a.Y) * t;
        }

        /// <summary>
        /// b and c are controllers. a and d are statics. AB is origin and CD is dest
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="ptA"></param>
        /// <param name="ptB">dest</param>
        /// <param name="ptC"></param>
        /// <param name="ptD"></param>
        /// <param name="t"></param>
        public static void GetBezierValue(out Vector2 dest, Vector2 ptA, Vector2 ptB, Vector2 ptC, Vector2 ptD, float t){
            var ab = new Vector2();
            var bc = new Vector2();
            var cd = new Vector2();
            var abbc = new Vector2();
            var bccd = new Vector2();

            dest = new Vector2();

            Lerp(ref ab, ptA, ptB, t);
            Lerp(ref bc, ptB, ptC, t);
            Lerp(ref cd, ptC, ptD, t);
            Lerp(ref abbc, ab, bc, t);
            Lerp(ref bccd, bc, cd, t);
            Lerp(ref dest, abbc, bccd, t);
        }

        /// <summary>
        /// partial double version for the inner pedantic
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="ptA"></param>
        /// <param name="ptB">dest</param>
        /// <param name="ptC"></param>
        /// <param name="ptD"></param>
        /// <param name="t"></param>
        public static void GetBezierValue(out Vector2 dest, Vector2 ptA, Vector2 ptB, Vector2 ptC, Vector2 ptD, double t) {
            var ab = new DVector2();
            var bc = new DVector2();
            var cd = new DVector2();
            var abbc = new DVector2();
            var bccd = new DVector2();

            var ddest = new DVector2();
            var dptA = new DVector2(ptA.X, ptA.Y);
            var dptB = new DVector2(ptB.X, ptB.Y);
            var dptC = new DVector2(ptC.X, ptC.Y);
            var dptD = new DVector2(ptD.X, ptD.Y);

            DLerp(ref ab, dptA, dptB, t);
            DLerp(ref bc, dptB, dptC, t);
            DLerp(ref cd, dptC, dptD, t);
            DLerp(ref abbc, ab, bc, t);
            DLerp(ref bccd, bc, cd, t);
            DLerp(ref ddest, abbc, bccd, t);

            dest = new Vector2((float)ddest.X, (float)ddest.Y);
        }

        private class DVector2{
            public double X;
            public double Y;

            public DVector2(float x, float y){
                X = x;
                Y = y;
            }

            public DVector2(){
                X = 0;
                Y = 0;
            }
        }
    }
}