using System;
using System.Collections.Generic;
using System.Diagnostics;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI{
    internal class Button : IUIInteractiveElement{
        private const int _timeTillHoverProc = 1000;

        #region properties

        public Vector2 CentPosition{
            get { return _centPosition; }
        }

        public IAdvancedPrimitive Sprite{
            get { return _sprite; }
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

        public IUIElementComponent[] Components { get; set; }

        public float LayerDepth{
            get { return _layerDepth; }
        }

        public OnMouseAction MouseMovementHandler{
            get { return MouseMovementHandle; }
        }

        public OnMouseAction MouseClickHandler{
            get { return MouseClickHandle; }
        }

        public OnMouseAction MouseEntryHandler{
            get { return MouseEntryHandle; }
        }

        public OnMouseAction MouseExitHandler{
            get { return MouseExitHandle; }
        }

        public List<OnMouseAction> OnLeftButtonClick { get; set; }


        public List<OnMouseAction> OnLeftButtonDown { get; set; }


        public List<OnMouseAction> OnLeftButtonUp { get; set; }


        public List<OnMouseAction> OnMouseEntry { get; set; }


        public List<OnMouseAction> OnMouseExit { get; set; }


        public List<OnMouseAction> OnMouseHover { get; set; }


        public List<OnMouseAction> OnMouseMovement { get; set; }

        #endregion

        #region ctor

        public Button(int x, int y, int width, int height, float layerDepth, string textureName, IUIElementComponent[] components, int identifier = 0){
            _identifier = identifier;
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
            if (_hoverTimer.IsRunning){
                _hoverTimer.Restart();
            }
            return false;
        }

        private bool MouseClickHandle(MouseState state){
            //for this event, we can assume the mouse is within the button's boundingbox
            bool denyOtherElementsFromClick = false;

            if (state.LeftButton == ButtonState.Pressed){
                _clickTimer.Start();
                foreach (OnMouseAction t in OnLeftButtonDown){
                    if (t(state)){
                        denyOtherElementsFromClick = true;
                    }
                }
            }
            if (state.LeftButton == ButtonState.Released){
                foreach (OnMouseAction t in OnLeftButtonUp){
                    if (t(state)){
                        denyOtherElementsFromClick = true;
                    }
                }

                _clickTimer.Stop();
                if (_clickTimer.ElapsedMilliseconds < 200){
                    //okay, click registered. now dispatch events.
                    foreach (OnMouseAction t in OnLeftButtonClick){
                        if (t(state)){
                            denyOtherElementsFromClick = true;
                        }
                    }
                }
                _clickTimer.Reset();
            }
            return denyOtherElementsFromClick;
        }

        private bool MouseEntryHandle(MouseState state){
            foreach (OnMouseAction action in OnMouseEntry){
                action(state);
            }
            _hoverTimer.Start();

            return false;
        }

        private bool MouseExitHandle(MouseState state){
            foreach (OnMouseAction action in OnMouseExit){
                action(state);
            }
            _hoverTimer.Reset();
            return false;
        }

        #endregion

        #region button events

        //private List<OnMouseAction> OnLeftButtonClick; //in future change this to a dictionary enum in UI class
        //private List<OnMouseAction> OnLeftButtonDown;
        //private List<OnMouseAction> OnLeftButtonUp;
        //private List<OnMouseAction> OnMouseEntry;
        //private List<OnMouseAction> OnMouseExit;
        //private List<OnMouseAction> OnMouseHover;
        //private List<OnMouseAction> OnMouseMovement;

        #endregion

        #region other stuff

        public TComponent GetComponent<TComponent>(){
            foreach (IUIElementComponent component in Components){
                if (component.GetType() == typeof (TComponent)){
                    return (TComponent) component;
                }
            }
            throw new Exception("Request made to a Button object for a component that did not exist.");
        }

        public void Update(){
            foreach (IUIElementComponent component in Components){
                component.Update();
            }
            if (_hoverTimer.IsRunning){
                if (_hoverTimer.ElapsedMilliseconds > _timeTillHoverProc){
                    _hoverTimer.Reset();
                    foreach (OnMouseAction action in OnMouseHover){
                        action(Mouse.GetState());
                    }
                }
            }
        }

        public int Identifier{
            get { return _identifier; }
        }

        #endregion

        private readonly Stopwatch _clickTimer;
        private readonly Stopwatch _hoverTimer; //nonimp, put in superclass
        private readonly int _identifier;
        private readonly float _layerDepth; //the depth at which the layer is rendered at. Also used for MouseHandler click resolution.
        private readonly Sprite2D _sprite; //the button's sprite

        private Rectangle _boundingBox; //bounding box that represents the bounds of the button
        private Vector2 _centPosition; //represents the approximate center of the button
    }
}