﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Render{
    internal class Sprite2D{
        #region class methods and fields

        private readonly int _id;
        private bool _isDisposed;

        public Sprite2D(string textureName, int x, int y){
            int i = 0;
            while (!_isFrameSlotAvail[i]){ //i cant wait for this to crash
                i++;
            }
            _isFrameSlotAvail[i] = false;
            _id = i;
            _frameTextures[i] = _contentManager.Load<Texture2D>(textureName);
            _frameBlitLocations[i] = new Vector2(x, y);
            _isDisposed = false;
        }



        ~Sprite2D(){
            Dispose();
        }

        public int X{
            get { return (int) _frameBlitLocations[_id].X; }
            set { _frameBlitLocations[_id].X = value; }
        }

        public int Y{
            get { return (int) _frameBlitLocations[_id].Y; }
            set { _frameBlitLocations[_id].Y = value; }
        }

        public void Dispose(){
            if (!_isDisposed){
                _isFrameSlotAvail[_id] = true;
                _isDisposed = true;
            }
        }

        public void EditDongleTexture(string textureName){
            _frameTextures[_id] = _contentManager.Load<Texture2D>(textureName);
        }

        #endregion

        #region static methods and fields

        private const int _maxDonglesDisplayable = 100;
        private static bool[] _isFrameSlotAvail;
        private static Texture2D[] _frameTextures;
        private static Vector2[] _frameBlitLocations;
        // private static float[] _frameLayerLevels;
        private static ContentManager _contentManager;
        private static SpriteBatch _spriteBatch;

        public static void Init(GraphicsDevice device, ContentManager content){
            _contentManager = content;
            _isFrameSlotAvail = new bool[_maxDonglesDisplayable];
            _frameTextures = new Texture2D[_maxDonglesDisplayable];
            _frameBlitLocations = new Vector2[_maxDonglesDisplayable];
            // _frameLayerLevels = new float[_maxDonglesDisplayable];

            for (int i = 0; i < _maxDonglesDisplayable; i++){
                _isFrameSlotAvail[i] = true;
            }

            _spriteBatch = new SpriteBatch(device);
        }

        public static void Draw(){
            _spriteBatch.Begin();
            for (int i = 0; i < _maxDonglesDisplayable; i++){
                if (_isFrameSlotAvail[i] == false){
                    _spriteBatch.Draw(_frameTextures[i], _frameBlitLocations[i], Color.White);
                }
            }
            _spriteBatch.End();
        }

        #endregion
    }
}