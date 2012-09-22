#region

using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Utilities{
    internal static class Bezier{
        #region generation methods

        static void Lerp(ref Vector2 dest, Vector2 a, Vector2 b, float t){
            dest.X = a.X + (b.X - a.X)*t;
            dest.Y = a.Y + (b.Y - a.Y)*t;
        }

        static void DLerp(ref DVector2 dest, DVector2 a, DVector2 b, double t){
            dest.X = a.X + (b.X - a.X)*t;
            dest.Y = a.Y + (b.Y - a.Y)*t;
        }

        /// <summary>
        ///   b and c are controllers. a and d are statics. AB is origin and CD is dest
        /// </summary>
        /// <param name="dest"> </param>
        /// <param name="ptA"> </param>
        /// <param name="ptB"> dest </param>
        /// <param name="ptC"> </param>
        /// <param name="ptD"> </param>
        /// <param name="t"> </param>
        public static void DGetBezierValue(out DVector2 dest, DVector2 ptA, DVector2 ptB, DVector2 ptC, DVector2 ptD, double t){
            var ab = new DVector2();
            var bc = new DVector2();
            var cd = new DVector2();
            var abbc = new DVector2();
            var bccd = new DVector2();

            dest = new DVector2();

            DLerp(ref ab, ptA, ptB, t);
            DLerp(ref bc, ptB, ptC, t);
            DLerp(ref cd, ptC, ptD, t);
            DLerp(ref abbc, ab, bc, t);
            DLerp(ref bccd, bc, cd, t);
            DLerp(ref dest, abbc, bccd, t);
        }

        /// <summary>
        ///   partial double version for the inner pedantic
        /// </summary>
        /// <param name="dest"> </param>
        /// <param name="ptA"> </param>
        /// <param name="ptB"> dest </param>
        /// <param name="ptC"> </param>
        /// <param name="ptD"> </param>
        /// <param name="t"> </param>
        public static void GetBezierValue(out Vector2 dest, Vector2 ptA, Vector2 ptB, Vector2 ptC, Vector2 ptD, float t){
            var ab = new Vector2();
            var bc = new Vector2();
            var cd = new Vector2();
            var abbc = new Vector2();
            var bccd = new Vector2();

            var ddest = new Vector2();
            var dptA = new Vector2(ptA.X, ptA.Y);
            var dptB = new Vector2(ptB.X, ptB.Y);
            var dptC = new Vector2(ptC.X, ptC.Y);
            var dptD = new Vector2(ptD.X, ptD.Y);

            Lerp(ref ab, dptA, dptB, t);
            Lerp(ref bc, dptB, dptC, t);
            Lerp(ref cd, dptC, dptD, t);
            Lerp(ref abbc, ab, bc, t);
            Lerp(ref bccd, bc, cd, t);
            Lerp(ref ddest, abbc, bccd, t);

            dest = new Vector2(ddest.X, ddest.Y);
        }

        #endregion
    }
}