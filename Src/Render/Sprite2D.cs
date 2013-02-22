#region

using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class Sprite2D : IDrawableSprite{
        readonly RenderPanel _renderPanel;
        readonly FloatingRectangle _srcRect;
        public float Depth;
        public int Height;
        public bool Enabled;
        public float Opacity;
        public int Width;
        public int X;
        public int Y;

        Rectangle _destRect;
        bool _isDisposed;
        Texture2D _texture;

        /// <summary>
        ///   constructor for a normal sprite
        /// </summary>
        public Sprite2D(string textureName, int x, int y, int width, int height, float depth = 0.5f, float opacity = 1, float spriteRepeatX = 1, float spriteRepeatY = 1){
            _texture = Singleton.ContentManager.Load<Texture2D>(Singleton.ContentStrLookup[textureName]);
            _srcRect = new FloatingRectangle(0f, 0f, _texture.Height*spriteRepeatX, _texture.Width*spriteRepeatY);
            _destRect = new Rectangle();
            _isDisposed = false;
            _renderPanel = RenderPanel.Add(this);
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Depth = depth;
            Opacity = opacity;
            Enabled = true;
        }

        #region IDrawableSprite Members

        public Texture2D Texture{
            set { _texture = value; }
            get { return _texture; }
        }

        public void Dispose(){
            if (!_isDisposed){
                _renderPanel.Remove(this);
                _isDisposed = true;
            }
        }

        public void SetTextureFromString(string textureName){
            _texture = Singleton.ContentManager.Load<Texture2D>(textureName);
        }

        public void Draw(SpriteBatch batch, Vector2 renderTargOffset){
            if (Enabled){
                _destRect.X = X - (int) renderTargOffset.X;
                _destRect.Y = Y - (int) renderTargOffset.Y;
                _destRect.Width = Width;
                _destRect.Height = Height;
                batch.Draw(
                    _texture,
                    _destRect,
                    (Rectangle?) _srcRect,
                    Color.White*Opacity,
                    0,
                    Vector2.Zero,
                    SpriteEffects.None,
                    Depth
                    );
            }
        }

        #endregion

        ~Sprite2D(){
            if (!_isDisposed){
                _renderPanel.Remove(this);
                _isDisposed = true;
            }
        }
    }
}