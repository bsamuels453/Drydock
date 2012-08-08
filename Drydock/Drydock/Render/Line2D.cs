#region

using Drydock.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    //todo: fix the fucking unimplemented functions you pleb
    internal class Line2D : IAdvancedPrimitive{
        #region properties and fields

        private readonly int _id;
        private bool _isDisposed;

        #endregion

        #region constructors

        public Line2D(Line owner, float opacity = 1){
            int i = 0;
            while (!_isFrameSlotAvail[i]){ //i cant wait for this to crash
                i++;
            }

            _isFrameSlotAvail[i] = false;
            _id = i;
            _isDisposed = false;

            _lineTextures[_id] = new Texture2D(_device, 1, 1, false, SurfaceFormat.Color);
            _lineTextures[_id].SetData(new[]{Color.Black});
            _lineOwners[_id] = owner;
        }

        #endregion

        #region destructors

        public Texture2D Texture{
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public void SetTextureFromString(string textureName){
            throw new System.NotImplementedException();
        }

        public void Dispose(){
            if (!_isDisposed){
                _isFrameSlotAvail[_id] = true;
                _isDisposed = true;
            }
        }

        ~Line2D(){
            Dispose();
        }

        #endregion

        #region static methods and fields

        private const int _maxLines = 10000;
        private static bool[] _isFrameSlotAvail;
        private static Texture2D[] _lineTextures;
        private static Line[] _lineOwners;
        private static SpriteBatch _spriteBatch;
        private static GraphicsDevice _device;

        public static void Init(GraphicsDevice device){
            _isFrameSlotAvail = new bool[_maxLines];
            _lineOwners = new Line[_maxLines];
            _spriteBatch = new SpriteBatch(device);
            _lineTextures = new Texture2D[_maxLines];

            for (int i = 0; i < _maxLines; i++){
                _isFrameSlotAvail[i] = true;
            }
            _device = device;
        }

        public static void Draw(){
            _spriteBatch.Begin();
            for (int i = 0; i < _maxLines; i++){
                if (_isFrameSlotAvail[i] == false){
                    _spriteBatch.Draw( //welp
                        _lineTextures[i],
                        _lineOwners[i].OriginPoint,
                        null,
                        new Color(1, 1, 1, _lineOwners[i].Opacity),
                        _lineOwners[i].Angle,
                        Vector2.Zero,
                        new Vector2(_lineOwners[i].Length, _lineOwners[i].LineWidth),
                        SpriteEffects.None,
                        _lineOwners[i].Depth
                        );
                }
            }
            _spriteBatch.End();
        }

        #endregion
    }
}