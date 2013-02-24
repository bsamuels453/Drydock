#region

using System;
using Drydock.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class Line2D : IDrawableSprite{
        readonly Line _parent;
        readonly SpriteBatch _spriteBatch;
        //Color _color;
        bool _isDisposed;
        Texture2D _texture;


        public Line2D(RenderTarget target, Line parent, Color color){
            _isDisposed = false;
            _spriteBatch = target.SpriteBatch;
            _texture = new Texture2D(Gbl.Device, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[]{color});
            _parent = parent;
            //_color = color;
        }

        #region IDrawableSprite Members

        public void Draw(){
            _spriteBatch.Draw(
                _texture,
                _parent.OriginPoint,
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
                _isDisposed = true;
            }
        }

        #endregion

        ~Line2D(){
            if (!_isDisposed){
                _isDisposed = true;
            }
        }
    }
}