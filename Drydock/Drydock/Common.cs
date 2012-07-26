using System;
using Microsoft.Xna.Framework;

namespace Drydock{
    internal static class Common{
        /// <summary>
        /// gets the components of a  vector
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Vector2 GetComponentFromAngle(float angle, float length){ //LINEAR ALGEBRA STRIKES BACK
            var up = new Vector2(1, 0);
            Matrix rotMatrix = Matrix.CreateRotationZ(angle);
            Vector2 direction = Vector2.Transform(up, rotMatrix);
            return new Vector2(direction.X*length, direction.Y*length);
        }

        /// <summary>
        /// gets angle of rotation around the origin, in radians
        /// </summary>
        /// <param name="x0"> </param>
        /// <param name="y0"> </param>
        /// <param name="dx">the change in the X coordinate from the translation</param>
        /// <param name="dy">the change in the Y coordinate from the translation</param>
        /// <returns></returns>
        public static float GetAngleOfRotation(int x0, int y0, int dx, int dy){
            var v0 = new Vector2(x0, y0);
            var v1 = new Vector2(v0.X + dx, v0.Y + dy);
            var angle = (float) Math.Acos(Vector2.Dot(v0, v1)/(v0.Length()*v1.Length()));
            return angle;
        }

        /*      public static float GetAngleOfRotation(int x0, int y0, int dx, int dy){
            var reference = new Vector2(1, 0);
            var v0 = new Vector2(x0, y0);
            var v1 = new Vector2(v0.X + dx, v0.Y + dy);

            var angle0 = (float)Math.Acos(Vector2.Dot(v0, reference) / (v0.Length() * reference.Length()));
            var angle1 = (float)Math.Acos(Vector2.Dot(v1, reference) / (v1.Length() * reference.Length()));


            return angle1 - angle0;*/
    }
}