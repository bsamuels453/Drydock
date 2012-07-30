using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Render{
    internal class Line2D{
        #region methods and fields

        private readonly int _id;
        private bool _isDisposed;
        private Vector2 _point1;
        private Vector2 _point2;

        #region constructors

        public Line2D(int x0, int y0, int x1, int y1, float depth, float opacity = 1){
            int i = 0;
            while (!_isFrameSlotAvail[i]){ //i cant wait for this to crash
                i++;
            }
            _point1 = new Vector2(x0, y0);
            _point2 = new Vector2(x1, y1);

            _isFrameSlotAvail[i] = false;
            _id = i;
            CalculateInfoFromPoints();
            _isDisposed = false;

            _lineTextures[_id] = new Texture2D(_device, 1, 1, false, SurfaceFormat.Color);
            _lineTextures[_id].SetData(new[] {Color.Black});
            _frameOpacity[_id] = opacity;
            _frameLayerLevels[_id] = depth;
        }

        public Line2D(Vector2 v1, Vector2 v2, float depth, float opacity = 1){
            int i = 0;
            while (!_isFrameSlotAvail[i]){ //i cant wait for this to crash
                i++;
            }
            _point1 = v1;
            _point2 = v2;

            _isFrameSlotAvail[i] = false;
            _id = i;
            CalculateInfoFromPoints();
            _isDisposed = false;
            _lineTextures[_id] = new Texture2D(_device, 1, 1, false, SurfaceFormat.Color);
            _lineTextures[_id].SetData(new[] { Color.Black });
            _frameOpacity[_id] = opacity;
            _frameLayerLevels[_id] = depth;
        }

        #endregion

        #region modification methods and properties

        public Vector2 OriginPoint{
            get { return _point1; }
            set{
                _point1 = value;
                CalculateInfoFromPoints();
            }
        }

        public Vector2 DestPoint{
            get { return _point2; }
            set{
                _point2 = value;
                CalculateInfoFromPoints();
            }
        }

        public float Angle{
            get { return _lineAngles[_id]; }
            set{
                _lineAngles[_id] = value;
                _lineUVectors[_id] = Common.GetComponentFromAngle(value, 1);
                CalculateDestFromUnitVector();
            }
        }

        public void TranslateOrigin(int dx, int dy){
            _point1.X += dx;
            _point1.Y += dy;
            CalculateInfoFromPoints();
        }

        public void TranslateDestination(int dx, int dy){
            _point2.X += dx;
            _point2.Y += dy;
            CalculateInfoFromPoints();
        }

        public float Opacity{
            get { return _frameOpacity[_id]; }
            set { _frameOpacity[_id] = value; }
        }

        #endregion

        #region private calculation functions

        /// <summary>
        /// calculates the line's destination point from the line's unit vector and length
        /// </summary>
        private void CalculateDestFromUnitVector(){
            _point2.X = _lineUVectors[_id].X*_lineLengths[_id] + _point1.X;
            _point2.Y = _lineUVectors[_id].Y*_lineLengths[_id] + _point1.Y;
        }

        /// <summary>
        /// calculates the line's blit location, angle, length, and unit vector based on the origin point and destination point
        /// </summary>
        private void CalculateInfoFromPoints(){
            _lineBlitLocations[_id] = new Vector2(_point1.X, _point1.Y);
            _lineAngles[_id] = (float) Math.Atan2(_point2.Y - _point1.Y, _point2.X - _point1.X);
            _lineUVectors[_id] = Common.GetComponentFromAngle(_lineAngles[_id], 1);
            _lineLengths[_id] = Vector2.Distance(_point1, _point2);
        }

        #endregion

        #region destructors

        ~Line2D(){
            Dispose();
        }

        public void Dispose(){
            if (!_isDisposed){
                _isFrameSlotAvail[_id] = true;
                _isDisposed = true;
            }
        }

        #endregion

        #endregion

        #region static methods and fields

        private const int _maxLines = 1000;
        private static bool[] _isFrameSlotAvail;
        private static Texture2D[] _lineTextures;
        private static Vector2[] _lineBlitLocations;
        private static float[] _lineAngles;
        private static float[] _lineLengths;
        private static Vector2[] _lineUVectors;
        private static float[] _frameLayerLevels;
        private static float[] _frameOpacity;
        private static SpriteBatch _spriteBatch;
        private static GraphicsDevice _device;

        public static void Init(GraphicsDevice device){
            _isFrameSlotAvail = new bool[_maxLines];
            _lineBlitLocations = new Vector2[_maxLines];
            _lineAngles = new float[_maxLines];
            _lineLengths = new float[_maxLines];
            _lineUVectors = new Vector2[_maxLines];
            _frameLayerLevels = new float[_maxLines];
            _frameOpacity = new float[_maxLines];

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
                        _lineBlitLocations[i],
                        null,
                        new Color(1,1,1,_frameOpacity[i]),
                        _lineAngles[i],
                        Vector2.Zero,
                        new Vector2(_lineLengths[i], 1),
                        SpriteEffects.None,
                        _frameLayerLevels[i]
                        );
                }
            }
            _spriteBatch.End();
        }

        #endregion
    }
}