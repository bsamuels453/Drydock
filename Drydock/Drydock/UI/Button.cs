using System.Collections.Generic;
using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using IDrawable = Drydock.Render.IDrawable;

namespace Drydock.UI{
    internal delegate bool OnMouseAction(MouseState state);

    internal class Button : IDrawable{
        private readonly float _layerDepth; //the depth at which the layer is rendered at. Also used for MouseHandler click resolution.
        private readonly Sprite2D _sprite; //the button's sprite

        public IButtonComponent[] Components;
        public List<OnMouseAction> OnLeftButtonClick;
        public List<OnMouseAction> OnLeftButtonDown;
        public List<OnMouseAction> OnLeftButtonUp;
        public List<OnMouseAction> OnMouseHover;
        public List<OnMouseAction> OnMouseMovement;
        private Rectangle _boundingBox; //bounding box that represents the bounds of the button
        private Vector2 _centPosition; //represents the approximate center of the button
        private MouseState _oldMouseClickState;
        private MouseState _oldMouseMovementState;

        #region properties

        public Vector2 CentPosition{
            get { return _centPosition; }
        }

        public Sprite2D Sprite{
            get { return _sprite; }
        }

        public float LayerDepth{
            get { return _layerDepth; }
        }

        public int X{
            get { return _boundingBox.X; }
            set{
                _boundingBox.X = value;
                _centPosition.X = _boundingBox.X + _boundingBox.Width/2;
            }
        }

        public int Y{
            get { return _boundingBox.Y; }
            set{
                _boundingBox.Y = value;
                _centPosition.Y = _boundingBox.Y + _boundingBox.Height/2;
            }
        }

        public Rectangle BoundingBox{
            get { return _boundingBox; }
        }

        #endregion

        #region ctor

        public Button(int x, int y, int width, int height, float layerDepth, string textureName, IButtonComponent[] components){
            _centPosition = new Vector2();
            _boundingBox = new Rectangle(x, y, width, height);
            _sprite = new Sprite2D(textureName, this, 0);
            OnLeftButtonDown = new List<OnMouseAction>();
            OnLeftButtonUp = new List<OnMouseAction>();
            OnLeftButtonClick = new List<OnMouseAction>();
            OnMouseMovement = new List<OnMouseAction>();
            OnMouseHover = new List<OnMouseAction>();

            _centPosition.X = _boundingBox.X + _boundingBox.Width/2;
            _centPosition.Y = _boundingBox.Y + _boundingBox.Height/2;

            _layerDepth = layerDepth;
            Components = components;

            foreach (IButtonComponent component in Components){
                component.Owner = this;
            }

            MouseHandler.ClickSubscriptions.Add(MouseClickHandler);
            MouseHandler.MovementSubscriptions.Add(MouseMovementHandler);
            _oldMouseMovementState = Mouse.GetState();
            _oldMouseClickState = Mouse.GetState();
        }

        #endregion

        private bool MouseMovementHandler(MouseState state){
            foreach (OnMouseAction t in OnMouseMovement){
                t(state);
            }
            _oldMouseMovementState = state;
            return false;
        }

        private bool MouseClickHandler(MouseState state){
            if (state.LeftButton == ButtonState.Pressed && _oldMouseMovementState.LeftButton == ButtonState.Released){
                foreach (OnMouseAction t in OnLeftButtonDown){
                    t(state);
                }
            }
            if (state.LeftButton == ButtonState.Released && _oldMouseMovementState.LeftButton == ButtonState.Pressed) {
                foreach (OnMouseAction t in OnLeftButtonUp) {
                    t(state);
                }
            }
            _oldMouseClickState = state;
            return false;
        }

        public void Update(){
        }
    }
}