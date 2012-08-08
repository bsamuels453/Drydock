#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class EnvironmentBatch{
        private const int _environmentScale = 16;
        private readonly Effect _environmentEffect;

        public EnvironmentBatch(GraphicsDevice device, ContentManager content, Matrix projection){
            _environmentEffect = content.Load<Effect>("AmbientEffect");
            _environmentEffect.Parameters["BaseTexture"].SetValue(content.Load<Texture2D>("Drydock Floor"));
            _environmentEffect.Parameters["Projection"].SetValue(projection);
            _environmentEffect.Parameters["World"].SetValue(Matrix.CreateScale(_environmentScale));
        }

        public void Draw(GraphicsDevice device, Matrix view){
            _environmentEffect.Parameters["View"].SetValue(view);
        }
    }
}