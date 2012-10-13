#region

using System;
using System.Collections.Generic;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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

        public event OnBasicMouseEvent OnLeftClickDispatcher;
        public event OnBasicMouseEvent OnLeftPressDispatcher;
        public event OnBasicMouseEvent OnLeftReleaseDispatcher;
        public bool ContainsMouse { get; set; }
        public List<IAcceptLeftButtonClickEvent> OnLeftButtonClick { get; set; }
        public List<IAcceptLeftButtonPressEvent> OnLeftButtonPress { get; set; }
        public List<IAcceptLeftButtonReleaseEvent> OnLeftButtonRelease { get; set; }
        public List<IAcceptMouseEntryEvent> OnMouseEntry { get; set; }
        public List<IAcceptMouseExitEvent> OnMouseExit { get; set; }
        public List<IAcceptMouseMovementEvent> OnMouseMovement { get; set; }
        public List<IAcceptMouseScrollEvent> OnMouseScroll { get; set; }
        public List<IAcceptKeyboardEvent> OnKeyboardEvent { get; set; }

        public float Opacity { get; set; }
        public float Depth { get; set; }

        #endregion

        #region ctor

        public Button(float x, float y, float width, float height, DepthLevel depth, string textureName, float spriteTexRepeatX = DefaultTexRepeat, float spriteTexRepeatY = DefaultTexRepeat, int identifier = DefaultIdentifier, IUIComponent[] components = null){
            _identifier = identifier;
            Depth = (float) depth/10;

            UIElementCollection.AddElement(this);
            OnLeftButtonClick = new List<IAcceptLeftButtonClickEvent>();
            OnLeftButtonPress = new List<IAcceptLeftButtonPressEvent>();
            OnLeftButtonRelease = new List<IAcceptLeftButtonReleaseEvent>();
            OnMouseEntry = new List<IAcceptMouseEntryEvent>();
            OnMouseExit = new List<IAcceptMouseExitEvent>();
            OnMouseMovement = new List<IAcceptMouseMovementEvent>();
            OnMouseScroll = new List<IAcceptMouseScrollEvent>();
            OnKeyboardEvent = new List<IAcceptKeyboardEvent>();

            _centPosition = new Vector2();
            _boundingBox = new FloatingRectangle(x, y, width, height);
            _sprite = new Sprite2D(textureName, this, spriteTexRepeatX, spriteTexRepeatY);

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

        public void UpdateLogic(double timeDelta){
            if (Components != null){
                foreach (IUIComponent component in Components){
                    component.Update();
                }
            }
        }

        public void UpdateInput(ref ControlState state){
            if (state.AllowLeftButtonInterpretation){
                if (state.LeftButtonClick){
                    foreach (var @event in OnLeftButtonClick){
                        @event.OnLeftButtonClick(ref state.AllowLeftButtonInterpretation, state.MousePos, state.PrevMousePos);
                        if (!state.AllowLeftButtonInterpretation)
                            break;
                    }
                }
            }
            if (state.AllowLeftButtonInterpretation){
                if (state.LeftButtonState == ButtonState.Pressed){
                    foreach (var @event in OnLeftButtonPress){
                        @event.OnLeftButtonPress(ref state.AllowLeftButtonInterpretation, state.MousePos, state.PrevMousePos);
                        if (!state.AllowLeftButtonInterpretation)
                            break;
                    }
                }
            }
            if (state.AllowLeftButtonInterpretation){
                if (state.LeftButtonState == ButtonState.Released){
                    foreach (var @event in OnLeftButtonRelease){
                        @event.OnLeftButtonRelease(ref state.AllowLeftButtonInterpretation, state.MousePos, state.PrevMousePos);
                        if (!state.AllowLeftButtonInterpretation)
                            break;
                    }
                }
            }
            if (state.AllowMouseMovementInterpretation){
                foreach (var @event in OnMouseMovement){
                    @event.OnMouseMovement(ref state.AllowMouseMovementInterpretation, state.MousePos, state.PrevMousePos);
                    if (!state.AllowMouseMovementInterpretation)
                        break;
                }
            }
            if (state.AllowMouseMovementInterpretation){
                if (BoundingBox.Contains(state.MousePos.X, state.MousePos.Y) && !ContainsMouse){
                    ContainsMouse = true;
                    foreach (var @event in OnMouseEntry){
                        @event.OnMouseEntry(ref state.AllowMouseMovementInterpretation, state.MousePos, state.PrevMousePos);
                        if (!state.AllowMouseMovementInterpretation)
                            break;
                    }
                }
            }
            if (state.AllowMouseMovementInterpretation){
                if (!BoundingBox.Contains(state.MousePos.X, state.MousePos.Y) && ContainsMouse){
                    ContainsMouse = false;
                    foreach (var @event in OnMouseExit){
                        @event.OnMouseExit(ref state.AllowMouseMovementInterpretation, state.MousePos, state.PrevMousePos);
                        if (!state.AllowMouseMovementInterpretation)
                            break;
                    }
                }
            }
            if (state.AllowKeyboardInterpretation){
                foreach (var @event in OnKeyboardEvent){
                    @event.OnKeyboardEvent(ref state.AllowKeyboardInterpretation, state.KeyboardState);
                    if (!state.AllowKeyboardInterpretation)
                        break;
                }
            }

            //now dispatch the external delegates
            if (state.AllowLeftButtonInterpretation){
                if (BoundingBox.Contains(state.MousePos.X, state.MousePos.Y)){
                    if (state.LeftButtonClick){
                        if (OnLeftClickDispatcher != null){
                            OnLeftClickDispatcher.Invoke();
                        }
                    }
                    if (state.LeftButtonState == ButtonState.Released){
                        if (OnLeftReleaseDispatcher != null){
                            OnLeftReleaseDispatcher.Invoke();
                        }
                    }
                    if (state.LeftButtonState == ButtonState.Pressed){
                        if (OnLeftPressDispatcher != null){
                            OnLeftPressDispatcher.Invoke();
                        }
                    }
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