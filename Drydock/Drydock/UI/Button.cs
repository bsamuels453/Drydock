#region

using System;
using System.Collections.Generic;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.UI{
    internal class Button : IUIInteractiveElement{
        #region properties and fields

        public const int DefaultTexRepeat = 1;
        public const int DefaultIdentifier = 1;

        readonly FloatingRectangle _boundingBox; //bounding box that represents the bounds of the button
        readonly int _identifier; //non-function based identifier that can be used to differentiate buttons
        readonly Sprite2D _sprite; //the button's sprite
        Vector2 _centPosition; //represents the approximate center of the button
        string _texture;

        public Vector2 CentPosition{
            get { return _centPosition; }
            set{
                _centPosition = value;
                _boundingBox.X = _centPosition.X - _boundingBox.Width/2;
                _boundingBox.Y = _centPosition.Y - _boundingBox.Height/2;
            }
        }

        public IUIComponent[] Components { get; set; }

        public String Texture{
            get { return _texture; }
            set{
                _texture = value;
                _sprite.SetTextureFromString(value);
            }
        }

        public int Identifier{
            get { return _identifier; }
        }

        public float X{
            get { return _boundingBox.X; }
            set{
                _boundingBox.X = value;
                _centPosition.X = _boundingBox.X + _boundingBox.Width/2;
            }
        }

        public float Y{
            get { return _boundingBox.Y; }
            set{
                _boundingBox.Y = value;
                _centPosition.Y = _boundingBox.Y + _boundingBox.Height/2;
            }
        }

        public float Width{
            get { return _boundingBox.Width; }
            set { _boundingBox.Width = value; }
        }

        public float Height{
            get { return _boundingBox.Height; }
            set { _boundingBox.Height = value; }
        }

        public FloatingRectangle BoundingBox{
            get { return _boundingBox; }
        }

        public bool ContainsMouse { get; set; }
        public float Opacity { get; set; }
        public float Depth { get; set; }
        public List<OnMouseEvent> OnLeftButtonClick { get; set; }
        public List<OnMouseEvent> OnLeftButtonPress { get; set; }
        public List<OnMouseEvent> OnLeftButtonRelease { get; set; }
        public List<OnMouseEvent> OnMouseEntry { get; set; }
        public List<OnMouseEvent> OnMouseExit { get; set; }
        public List<OnMouseEvent> OnMouseMovement { get; set; }
        public List<EOnKeyboardEvent> OnKeyboardEvent { get; set; }
        public List<OnMouseEvent> OnMouseScroll { get; set; }

        #endregion

        #region ctor

        public Button(float x, float y, float width, float height, DepthLevel depth, string textureName, float spriteTexRepeatX = DefaultTexRepeat, float spriteTexRepeatY = DefaultTexRepeat, int identifier = DefaultIdentifier, IUIComponent[] components = null){
            _identifier = identifier;
            Depth = (float) depth/10;

            UIElementCollection.AddElement(this);

            _centPosition = new Vector2();
            _boundingBox = new FloatingRectangle(x, y, width, height);
            _sprite = new Sprite2D(textureName, this, spriteTexRepeatX, spriteTexRepeatY);

            OnLeftButtonClick = new List<OnMouseEvent>();
            OnLeftButtonPress = new List<OnMouseEvent>();
            OnLeftButtonRelease = new List<OnMouseEvent>();
            OnMouseEntry = new List<OnMouseEvent>();
            OnMouseExit = new List<OnMouseEvent>();
            OnMouseMovement = new List<OnMouseEvent>();
            OnKeyboardEvent = new List<EOnKeyboardEvent>();
            OnMouseScroll = new List<OnMouseEvent>();

            _centPosition.X = _boundingBox.X + _boundingBox.Width/2;
            _centPosition.Y = _boundingBox.Y + _boundingBox.Height/2;

            Components = components;
            Opacity = 1;
            if (Components != null){
                foreach (IUIComponent component in Components){
                    component.Owner = this;
                }
            }
        }

        #endregion

        #region other IUIElement derived methods

        public TComponent GetComponent<TComponent>(){
            if (Components != null){
                foreach (IUIComponent component in Components){
                    if (component is TComponent){
                        return (TComponent) component;
                    }
                }
            }
            throw new Exception("Request made to a Button object for a component that did not exist.");
        }

        public bool DoesComponentExist<TComponent>(){
            if (Components != null){
                return Components.OfType<TComponent>().Any();
            }
            return false;
        }

        public void Update(){
            if (Components != null){
                foreach (IUIComponent component in Components){
                    component.Update();
                }
            }
        }

        public void Dispose(){
            throw new NotImplementedException();
        }

        #endregion
    }

    internal class ButtonGenerator : ComponentGenerator{
        public Dictionary<string, object[]> Components;
        public DepthLevel? Depth;
        public float? Height;
        public int? Identifier;
        public UIElementCollection Owner;
        public float? SpriteTexRepeatX;
        public float? SpriteTexRepeatY;
        public string TextureName;
        public float? Width;
        public float? X;
        public float? Y;

        public ButtonGenerator(){
            Components = null;
            Depth = null;
            Height = null;
            Width = null;
            Identifier = null;
            Owner = null;
            SpriteTexRepeatX = null;
            SpriteTexRepeatY = null;
            TextureName = null;
            X = null;
            Y = null;
        }

        public Button GenerateButton(){
            //make sure we have all the data required
            if (X == null ||
                Y == null ||
                Width == null ||
                Height == null ||
                Depth == null ||
                TextureName == null){
                throw new Exception("Template did not contain all of the basic variables required to generate a button.");
            }
            //generate component list
            IUIComponent[] components = null;
            if (Components != null){
                components = GenerateComponents(Components);
            }

            //now we handle optional parameters
            float spriteTexRepeatX;
            float spriteTexRepeatY;
            int identifier;

            if (SpriteTexRepeatX != null)
                spriteTexRepeatX = (float) SpriteTexRepeatX;
            else
                spriteTexRepeatX = Button.DefaultTexRepeat;

            if (SpriteTexRepeatY != null)
                spriteTexRepeatY = (float) SpriteTexRepeatY;
            else
                spriteTexRepeatY = Button.DefaultTexRepeat;

            if (Identifier != null)
                identifier = (int) Identifier;
            else
                identifier = Button.DefaultIdentifier;

            return new Button(
                (float) X,
                (float) Y,
                (float) Width,
                (float) Height,
                (DepthLevel) Depth,
                TextureName,
                spriteTexRepeatX,
                spriteTexRepeatY,
                identifier,
                components
                );
        }
    }
}