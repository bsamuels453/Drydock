using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using IDrawable = Drydock.Render.IDrawable;

namespace Drydock.UI {
    delegate void ClickDelegate(MouseState state);
    delegate void KeyDelegate(KeyboardState state);

    class Button : IDrawable {
        private Vector2 _centPosition;//represents the approximate center of the button
        private Rectangle _boundingBox;//bounding box that represents the bounds of the button
        private readonly Sprite2D _sprite;//the button's sprite
        private float _layerDepth;//the depth at which the layer is rendered at. Also used for MouseHandler click resolution.

        public IButtonComponent[] Components = new IButtonComponent[10];

        #region properties
        public Vector2 CentPosition{
            get { return _centPosition; }
        }

        public int X{
            get { return _boundingBox.X; }
            set { 
                _boundingBox.X = value;
                _centPosition.X = _boundingBox.X + _boundingBox.Width / 2;
            }
        }

        public int Y {
            get { return _boundingBox.Y; }
            set {
                _boundingBox.Y = value;
                _centPosition.Y = _boundingBox.Y + _boundingBox.Height / 2;
            }
        }

        public Rectangle BoundingBox{
            get { return _boundingBox; }
        }

        public Sprite2D Sprite{
            get { return _sprite; }
        }

        public float LayerDepth{
            get { return _layerDepth; }
        }

        #endregion

        public Button(int x, int y, int width, int height, float layerDepth, string textureName, IButtonComponent[] components){
            _centPosition = new Vector2();
            _boundingBox = new Rectangle(x, y, width, height);
            _centPosition.X = _boundingBox.X + _boundingBox.Width / 2;
            _centPosition.Y = _boundingBox.Y + _boundingBox.Height / 2;
            _sprite = new Sprite2D(textureName, this, 0);
            _layerDepth = layerDepth;
            Components = components;

            foreach (var component in Components){
                component.Owner = this;
            }
        }

    }
}
