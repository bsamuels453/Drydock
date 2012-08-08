﻿#region

using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Utilities {
    /// <summary>
    /// floating point-based rectangle. This exists to fix many of the quantization problems 
    /// experienced in the ui namespace caused by screen coordinates being expressed as integers.
    /// This class also provides a bit of utility in the .Position and ToRectangle methods, reducing some heap overhead.
    /// </summary>
    class FloatingRectangle {
        private float _height;
        private Rectangle _intRect;//integer based rectangle
        private Vector2 _position;
        private float _width;
        private float _x;
        private float _y;

        public FloatingRectangle(){
            _intRect = new Rectangle();
            _position = new Vector2();
        }

        public FloatingRectangle(int x, int y, int width, int height) {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _intRect = new Rectangle(x,y,width,height);
            _position = new Vector2(x,y);
        }

        public FloatingRectangle(float x, float y, float width, float height) {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _intRect = new Rectangle((int)x, (int)y, (int)width, (int)height);
            _position = new Vector2(x, y);
        }

        public float X{
            get { return _x; }
            set {
                _x = value;
                _position.X = _x;
                _intRect.X = (int)_x;
            }
        }
        public float Y{
            get { return _y; }
            set {
                _y = value;
                _position.Y = _y;
                _intRect.Y = (int)_y;
            }
        }
        public float Width{
            get { return _width; }
            set {
                _width = value;
                _intRect.Width = (int)_width;
            }
        }
        public float Height{
            get { return _height; }
            set {
                _height = value;
                _intRect.Height = (int)_height;
            }
        }

        public Rectangle ToRectangle { get { return _intRect; } }
        public Vector2 Position { get { return _position; } }

        public bool Contains(int x, int y){
            if (_x < x && x < _x + _width && _y < y && y < _y + _height){
                return true;
            }
            return false;
        }

        public bool Contains(Vector2 point){
            if (_x < point.X && point.X < _x + _width && _y < point.Y && point.Y < _y + _height) {
                return true;
            }
            return false;
        }

        public bool Contains(float x, float y){
            if (_x < x && x < _x + _width && _y < y && y < _y + _height) {
                return true;
            }
            return false;
        }
    }
}
