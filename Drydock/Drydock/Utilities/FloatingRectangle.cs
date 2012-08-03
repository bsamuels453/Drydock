using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Drydock.Utilities {
    /// <summary>
    /// floating point-based rectangle. This exists to fix many of the quantization problems 
    /// experienced in the ui namespace caused by screen coordinates being expressed as integers.
    /// This class also provides a bit of utility in the .Position and ToRectangle methods, reducing some heap overhead.
    /// </summary>
    class FloatingRectangle {
        private Rectangle _intRect;//integer based rectangle
        private Vector2 _position;
        private float _x;
        private float _y;
        private float _width;
        private float _height;

        public FloatingRectangle(){
            _intRect = new Rectangle();
            _position = new Vector2();
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
    }
}
