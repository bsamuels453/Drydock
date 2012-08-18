#region

using System.Collections.Generic;
using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class RenderPanel{
        private readonly List<IDrawableBuffer> _buffers;
        private readonly float _depth;
        private readonly SpriteBatch _panelSpriteBatch;
        private readonly Vector2 _position;
        private readonly RenderTarget2D _renderTarget;
        private readonly List<IDrawableSprite> _sprites;
        private Texture2D _renderedPanel;

        public RenderPanel(int x, int y, int width, int height, DepthLevel depth){
            _panelSpriteBatch = new SpriteBatch(Singleton.Device);
            _sprites = new List<IDrawableSprite>();
            _buffers = new List<IDrawableBuffer>();

            _depth = (float)depth / 10;

            _renderPanels.Add(this);

            _renderTarget = new RenderTarget2D(Singleton.Device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            _position = new Vector2(x, y);
        }

        public void Dispose(){
            _renderPanels.Remove(this);
        }

        ~RenderPanel(){
            _renderPanels.Remove(this);
        }

        private void DrawToTarget(Matrix viewMatrix){
            Singleton.Device.SetRenderTarget(_renderTarget);
            Singleton.Device.Clear(Color.CornflowerBlue);
            _panelSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
            foreach (var sprite in _sprites){
                sprite.Draw(_panelSpriteBatch, _position);
            }
            _panelSpriteBatch.End();
            Singleton.Device.DepthStencilState = _universalDepthStencil;
            foreach (var buffer in _buffers){
                buffer.Draw(viewMatrix);
            }

            _renderedPanel = _renderTarget;
            Singleton.Device.SetRenderTarget(null);
        }

        public void Remove(IDrawableSprite sprite){
            _sprites.Remove(sprite);
        }

        public void Remove(IDrawableBuffer buffer){
            _buffers.Remove(buffer);
        }

        #region static stuff

        private static List<RenderPanel> _renderPanels;
        private static RenderPanel _curRenderPanel;
        private static SpriteBatch _spriteBatch;
        private static DepthStencilState _universalDepthStencil;

        public static void SetRenderPanel(RenderPanel panel){
            _curRenderPanel = panel;
        }

        public static void Init(){
            _renderPanels = new List<RenderPanel>();
            _spriteBatch = new SpriteBatch(Singleton.Device);
            _universalDepthStencil = new DepthStencilState();
            _universalDepthStencil.DepthBufferEnable = true;
            _universalDepthStencil.DepthBufferWriteEnable = true;
        }

        public static void Draw(Matrix viewMatrix){
            foreach (var panel in _renderPanels){
                panel.DrawToTarget(viewMatrix);
            }
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
            foreach (var panel in _renderPanels){
                _spriteBatch.Draw(panel._renderedPanel, panel._position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, panel._depth);
            }
            _spriteBatch.End();
        }

        public static RenderPanel Add(IDrawableSprite sprite){
            _curRenderPanel._sprites.Add(sprite);
            return _curRenderPanel;
        }

        public static RenderPanel Add(IDrawableBuffer buffer){
            _curRenderPanel._buffers.Add(buffer);
            return _curRenderPanel;
        }

        #endregion
    }
}