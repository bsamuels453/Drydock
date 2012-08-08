#region

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

//todo: integrate this into screentext class

namespace Drydock.Render{
    internal class TextBatch{
        private readonly SpriteBatch _spriteBatch;

        public TextBatch(GraphicsDevice device, ContentManager content){
            _spriteBatch = new SpriteBatch(device);
            ScreenText.Init(content);
        }

        public void Draw(){
            ScreenText.Draw(_spriteBatch);
        }
    }
}