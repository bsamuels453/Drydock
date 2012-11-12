#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock{
    internal static class Singleton{
        public static ContentManager ContentManager;
        public static GraphicsDevice Device;
        public static Matrix ProjectionMatrix;
    }
}