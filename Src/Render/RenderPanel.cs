﻿#region

using System;
using System.Collections.Generic;
using Drydock.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class RenderPanel{
        public readonly Rectangle BoundingBox;
        readonly List<IDrawableBuffer> _buffers;
        readonly float _depth;
        readonly SpriteBatch _panelSpriteBatch;
        readonly Vector2 _position;
        readonly RenderTarget2D _renderTarget;
        readonly List<IDrawableSprite> _sprites;
        Texture2D _renderedPanel;

        public RenderPanel(int x, int y, int width, int height, DepthLevel depth = DepthLevel.Medium){
            _panelSpriteBatch = new SpriteBatch(Gbl.Device);
            _sprites = new List<IDrawableSprite>();
            _buffers = new List<IDrawableBuffer>();

            _depth = (float) depth/10;

            _renderPanels.Add(this);

            _renderTarget = new RenderTarget2D(Gbl.Device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            _position = new Vector2(x, y);
            BoundingBox = new Rectangle(x, y, width, height);
        }

        ~RenderPanel(){
            if (_renderPanels.Contains(this)){
                throw new Exception("render panel that is in use is being garbage collected");
            }
        }

        public void Dispose(){
            _renderPanels.Remove(this);
            _sprites.Clear();
            _buffers.Clear();
        }

        void DrawToTarget(Matrix viewMatrix){
            Gbl.Device.SetRenderTarget(_renderTarget);
            Gbl.Device.Clear(Color.CornflowerBlue);
            //_panelSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
            _panelSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
            foreach (var sprite in _sprites){
                sprite.Draw(_panelSpriteBatch, _position);
            }
            _panelSpriteBatch.End();
            Gbl.Device.DepthStencilState = _universalDepthStencil;

            foreach (var buffer in _buffers){
                buffer.Draw(viewMatrix);
            }

            _renderedPanel = _renderTarget;
            Gbl.Device.SetRenderTarget(null);
        }

        public void Remove(IDrawableSprite sprite){
            _sprites.Remove(sprite);
        }

        public void Remove(IDrawableBuffer buffer){
            _buffers.Remove(buffer);
        }

        #region static stuff

        static List<RenderPanel> _renderPanels;
        static RenderPanel _curRenderPanel;
        static SpriteBatch _spriteBatch;
        static DepthStencilState _universalDepthStencil;

        public static void BindRenderTarget(RenderPanel panel){
            _curRenderPanel = panel;
        }

        public static void UnbindRenderTarget(){
            //_curRenderPanel = null;
        }

        public static void Init(){
            _renderPanels = new List<RenderPanel>();
            _spriteBatch = new SpriteBatch(Gbl.Device);
            _universalDepthStencil = new DepthStencilState();
            _universalDepthStencil.DepthBufferEnable = true;
            _universalDepthStencil.DepthBufferWriteEnable = true;
        }

        public static void Clear(){
            _renderPanels.Clear();
            _curRenderPanel = null;
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
            if (_curRenderPanel != null){
                _curRenderPanel._sprites.Add(sprite);
            }
            else{
                throw new Exception("no render panel bound");
            }
            return _curRenderPanel;
        }

        public static RenderPanel Add(IDrawableBuffer buffer){
            if (_curRenderPanel != null){
                _curRenderPanel._buffers.Add(buffer);
            }
            else{
                throw new Exception("no render panel bound");
            }
            return _curRenderPanel;
        }

        #endregion
    }
}