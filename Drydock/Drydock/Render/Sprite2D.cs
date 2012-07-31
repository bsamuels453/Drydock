using Drydock.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Render{
    internal class Sprite2D : IAdvancedPrimitive{
        #region class methods and fields

        private readonly int _id;
        private bool _isDisposed;

        public Sprite2D(string textureName, IUIElement parent){
            int i = 0;
            while (!_isFrameSlotAvail[i]){ //i cant wait for this to crash
                i++;
            }
            _isFrameSlotAvail[i] = false;
            _id = i;
            _frameTextures[i] = _contentManager.Load<Texture2D>(textureName);
            _frameParents[i] = parent;
            _isDisposed = false;
        }

        ~Sprite2D(){
            Dispose();
        }

        public void Dispose(){
            if (!_isDisposed){
                _isFrameSlotAvail[_id] = true;
                _isDisposed = true;
            }
        }

        public void ChangeTexture(string textureName){
            _frameTextures[_id] = _contentManager.Load<Texture2D>(textureName);
        }

        #endregion

        #region static methods and fields

        private const int _maxSprites = 100;
        private static bool[] _isFrameSlotAvail;
        private static Texture2D[] _frameTextures;
        private static IUIElement[] _frameParents;
        private static ContentManager _contentManager;
        private static SpriteBatch _spriteBatch;

        public static void Init(GraphicsDevice device, ContentManager content){
            _contentManager = content;
            _isFrameSlotAvail = new bool[_maxSprites];
            _frameTextures = new Texture2D[_maxSprites];
            _frameParents = new IUIElement[_maxSprites];

            for (int i = 0; i < _maxSprites; i++){
                _isFrameSlotAvail[i] = true;
            }

            _spriteBatch = new SpriteBatch(device);
        }

        public static void Draw(){
            _spriteBatch.Begin();
            for (int i = 0; i < _maxSprites; i++){
                if (_isFrameSlotAvail[i] == false){
                    _spriteBatch.Draw(
                        _frameTextures[i],
                        _frameParents[i].BoundingBox,
                        null,
                        new Color(1, 1, 1, _frameParents[i].Opacity),
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