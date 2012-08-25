#region

using System;
using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class Line2D : IDrawableSprite{
        readonly Line _parent;
        readonly RenderPanel _renderPanel;
        Color _color;
        bool _isDisposed;
        Texture2D _texture;


        public Line2D(Line parent, Color color){
            _isDisposed = false;

            _texture = new Texture2D(Singleton.Device, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[]{color});
            _parent = parent;
            _renderPanel = RenderPanel.Add(this);
            //_color = color;
        }

        #region IDrawableSprite Members

        public void Draw(SpriteBatch batch, Vector2 renderTargOffset){
            batch.Draw(
                _texture,
                _parent.OriginPoint - renderTargOffset,
                null,
                Color.White*_parent.Opacity,
                _parent.Angle,
                Vector2.Zero,
                new Vector2(_parent.Length, _parent.LineWidth),
                SpriteEffects.None,
                _parent.Depth
                );
        }

        public Texture2D Texture{
            get { return _texture; }
            set { _texture = value; }
        }

        public void SetTextureFromString(string textureName){
            throw new NotImplementedException();
        }

        public void Dispose(){
            if (!_isDisposed){
                _renderPanel.Remove(this);
                _isDisposed = true;
            }
        }

        #endregion

        ~Line2D(){
            if (!_isDisposed){
                _renderPanel.Remove(this);
                _isDisposed = true;
            }
        }
    }
}