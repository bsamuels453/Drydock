#region

using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class Sprite2D : IDrawableSprite{
        private readonly IUIElement _parent;
        private readonly RenderPanel _renderPanel;
        private readonly FloatingRectangle _srcRect;
        private bool _isDisposed;
        private Texture2D _texture;

        /// <summary>
        ///   constructor for a normal sprite
        /// </summary>
        /// <param name="textureName"> </param>
        /// <param name="parent"> </param>
        /// <param name="spriteRepeatX"> </param>
        /// <param name="spriteRepeatY"> </param>
        public Sprite2D(string textureName, IUIElement parent, float spriteRepeatX = 1, float spriteRepeatY = 1){
            _texture = Singleton.ContentManager.Load<Texture2D>(textureName);
            _parent = parent;
            _srcRect = new FloatingRectangle(0f, 0f, _texture.Height*spriteRepeatX, _texture.Width*spriteRepeatY);
            _isDisposed = false;
            _renderPanel = RenderPanel.Add(this);
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
            var rect = _parent.BoundingBox.Clone();
            rect.X -= (int) renderTargOffset.X;
            rect.Y -= (int) renderTargOffset.Y;
            batch.Draw(
                _texture,
                rect,
                (Rectangle?) _srcRect,
                Color.White*_parent.Opacity,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                _parent.Depth
                );
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