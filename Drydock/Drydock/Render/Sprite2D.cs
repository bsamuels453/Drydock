#region

using System.Diagnostics;
using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class Sprite2D : IAdvancedPrimitive{
        #region class methods and fields

        private readonly int _id;
        private bool _isDisposed;

        /// <summary>
        /// constructor for a normal sprite
        /// </summary>
        /// <param name="textureName"></param>
        /// <param name="parent"></param>
        /// <param name="spriteRepeatX"> </param>
        /// <param name="spriteRepeatY"> </param>
        public Sprite2D(string textureName, IUIElement parent, float spriteRepeatX = 1, float spriteRepeatY = 1) {
            int i = 0;
            while (!_isFrameSlotAvail[i]){ //i cant wait for this to crash
                i++;
            }
            _isFrameSlotAvail[i] = false;
            _id = i;
            _frameTextures[i] = _contentManager.Load<Texture2D>(textureName);
            _frameParents[i] = parent;
            _srcRects[i] = new FloatingRectangle(0f, 0f, _frameTextures[i].Height * spriteRepeatX, _frameTextures[i].Width * spriteRepeatY);
            _isDisposed = false;
        }

        public void Dispose(){
            if (!_isDisposed){
                _isFrameSlotAvail[_id] = true;
                _isDisposed = true;
            }
        }

        public Texture2D Texture{
            set { _frameTextures[_id] = value; }
            get { return _frameTextures[_id]; }
        }

        public void SetTextureFromString(string textureName){
            _frameTextures[_id] = _contentManager.Load<Texture2D>(textureName);
        }

        ~Sprite2D(){
            Dispose();
        }

        #endregion

        #region static methods and fields

        private const int _maxSprites = 100;
        private static bool[] _isFrameSlotAvail;
        private static Texture2D[] _frameTextures;
        private static IUIElement[] _frameParents;
        private static ContentManager _contentManager;
        private static SpriteBatch _spriteBatch;
        private static FloatingRectangle[] _srcRects;

        public static void Init(GraphicsDevice device, ContentManager content){
            _contentManager = content;
            _isFrameSlotAvail = new bool[_maxSprites];
            _frameTextures = new Texture2D[_maxSprites];
            _frameParents = new IUIElement[_maxSprites];
            _srcRects = new FloatingRectangle[_maxSprites];

            for (int i = 0; i < _maxSprites; i++){
                _isFrameSlotAvail[i] = true;
                _srcRects[i] = null;
            }

            _spriteBatch = new SpriteBatch(device);
        }

        public static void Draw(){
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.LinearWrap,DepthStencilState.Default, RasterizerState.CullNone);
            for (int i = 0; i < _maxSprites; i++){
                if (_isFrameSlotAvail[i] == false){
                    _spriteBatch.Draw(
                        _frameTextures[i],
                        _frameParents[i].BoundingBox.ToRectangle,
                        (Rectangle?)_srcRects[i],
                        Color.White *  _frameParents[i].Opacity,
                        0,
                        Vector2.Zero,
                        SpriteEffects.None,
                        _frameParents[i].Depth
                        );
                }

            }
            _spriteBatch.End();
        }

       


    #endregion
    }
}