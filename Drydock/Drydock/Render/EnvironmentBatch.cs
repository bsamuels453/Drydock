using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Render {
    internal class EnvironmentBatch{
        private readonly Effect _environmentEffect;
        private const int _environmentScale = 16;

        public EnvironmentBatch(GraphicsDevice device, ContentManager content, Matrix projection){
            _environmentEffect = content.Load<Effect>("AmbientEffect");
            _environmentEffect.Parameters["BaseTexture"].SetValue(content.Load<Texture2D>("Drydock Floor"));
            _environmentEffect.Parameters["Projection"].SetValue(projection);
            _environmentEffect.Parameters["World"].SetValue(Matrix.CreateScale(_environmentScale));
        }

        public void Draw(GraphicsDevice device, Matrix view) {

            _environmentEffect.Parameters["View"].SetValue(view);

        }
    }
}
