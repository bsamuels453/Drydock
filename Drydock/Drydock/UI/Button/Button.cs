using System;
using System.Collections.Generic;
using System.Diagnostics;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using IDrawable = Drydock.Render.IDrawable;

namespace Drydock.UI.Button{
    internal class Button : IDrawable, IUIInteractiveElement{
        private readonly Stopwatch _clickTimer;
        private readonly float _layerDepth; //the depth at which the layer is rendered at. Also used for MouseHandler click resolution.
        private readonly Sprite2D _sprite; //the button's sprite

        private Rectangle _boundingBox; //bounding box that represents the bounds of the button
        private Vector2 _centPosition; //represents the approximate center of the button
        private Stopwatch _hoverTimer; //nonimp, put in superclass

        #region properties

        public Vector2 CentPosition{
            get { return _centPosition; }
        }

        public Sprite2D Sprite{
            get { return _sprite; }
        }

        public UIContext UIContxt { get; set; }

        #region IDrawable Members

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

        #region IUIInteractiveElement Members

        public IUIElementComponent[] Components { get; set; }

        public float LayerDepth{
            get { return _layerDepth; }
        }

        public OnMouseAction MouseMovementHandler {
            get { return MouseMovementHandle; }
        }

        public OnMouseAction MouseClickHandler {
            get { return MouseClickHandle; }
        }

        public OnMouseAction MouseEntryHandler {
            get { return MouseEntryHandle; }
        }

        public OnMouseAction MouseExitHandler {
            get { return MouseExitHandle; }
        }

        #endregion

        #endregion

        #region ctor

        public Button(int x, int y, int width, int height, float layerDepth, string textureName, IUIElementComponent[] components){
            _centPosition = new Vector2();
            _boundingBox = new Rectangle(x, y, width, height);
            _sprite = new Sprite2D(textureName, this, 0, 1);
            _clickTimer = new Stopwatch();
            _hoverTimer = new Stopwatch();
            OnLeftButtonDown = new List<OnMouseAction>();
            OnLeftButtonUp = new List<OnMouseAction>();
            OnLeftButtonClick = new List<OnMouseAction>();
            OnMouseMovement = new List<OnMouseAction>();
            OnMouseHover = new List<OnMouseAction>();
            OnMouseEntry = new List<OnMouseAction>();
            OnMouseExit = new List<OnMouseAction>();

            _centPosition.X = _boundingBox.X + _boundingBox.Width/2;
            _centPosition.Y = _boundingBox.Y + _boundingBox.Height/2;

            _layerDepth = layerDepth;
            Components = components;

            foreach (IUIElementComponent component in Components){
                component.Owner = this;
            }
        }

        #endregion

        #region event handlers

        private bool MouseMovementHandle(MouseState state){
            foreach (OnMouseAction t in OnMouseMovement){
                t(state);
            }
            return false;
        }

        private bool MouseClickHandle(MouseState state){
            //for this event, we can assume the mouse is within the button's boundingbox

            if (state.LeftButton == ButtonState.Pressed){
                _clickTimer.Start();
                foreach (OnMouseAction t in OnLeftButtonDown){
                    t(state);
                }
            }
            if (state.LeftButton == ButtonState.Released){
                foreach (OnMouseAction t in OnLeftButtonUp){
                    t(state);
                }

                _clickTimer.Stop();
                if (_clickTimer.ElapsedMilliseconds < 200){
                    //okay, click registered. now dispatch events.
                    foreach (OnMouseAction t in OnLeftButtonClick){
                        t(state);
                    }
                }
                _clickTimer.Reset();
            }
            return false;
        }

        private bool MouseEntryHandle(MouseState state){
            foreach (var action in OnMouseEntry){
                action(state);
            }
            return false;
        }

        private bool MouseExitHandle(MouseState state){
            foreach (var action in OnMouseExit){
                action(state);
            }
            return false;
        }

        #endregion

        #region button events

        public List<OnMouseAction> OnLeftButtonClick; //in future change this to a dictionary enum in UI class
        public List<OnMouseAction> OnLeftButtonDown;
        public List<OnMouseAction> OnLeftButtonUp;
        public List<OnMouseAction> OnMouseEntry;
        public List<OnMouseAction> OnMouseExit;
        public List<OnMouseAction> OnMouseHover;
        public List<OnMouseAction> OnMouseMovement;

        #endregion

        #region other stuff

        public TComponent GetComponent<TComponent>(){
            foreach (IUIElementComponent component in Components){
                if (component.GetType() == typeof (TComponent)){
                    return (TComponent) component;
                }
            }
            throw new Exception();
        }

        public void Update(){
            foreach (IUIElementComponent component in Components){
                component.Update();
            }
        }

        #endregion
    }
}