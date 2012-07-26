using Microsoft.Xna.Framework;

namespace Drydock.Utilities {
    static class Bezier {
        private static void Lerp(ref Vector2 dest, Vector2 a, Vector2 b, float t){
            dest.X = a.X + (b.X - a.X) * t;
            dest.Y = a.Y + (b.Y - a.Y) * t;
        }
        /// <summary>
        /// b and c are controllers. a and d are statics
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="ptA"></param>
        /// <param name="ptB"></param>
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
    }
}
