using Microsoft.Xna.Framework;

namespace Drydock{
    internal static class Common{
        public static Vector2 GetComponent(float angle, float length){ //LINEAR ALGEBRA STRIKES BACK
            var up = new Vector2(1, 0);
            Matrix rotMatrix = Matrix.CreateRotationZ(angle);
            Vector2 direction = Vector2.Transform(up, rotMatrix);
            return new Vector2(direction.X*length, direction.Y*length);
        }
    }
}