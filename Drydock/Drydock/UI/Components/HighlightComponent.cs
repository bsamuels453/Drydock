#region

using System;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

#endregion

namespace Drydock.UI.Components{
    internal class HighlightComponent : IUIComponent, IAcceptMouseMovementEvent, IAcceptLeftButtonPressEvent, IAcceptLeftButtonReleaseEvent{
        #region HighlightTrigger enum

        public enum HighlightTrigger{
            InvalidTrigger,
            MouseEntryExit,
            MousePressRelease,
            None
        }

        #endregion

        readonly float _highlightTexOpacity;
        readonly string _highlightTexture;
        readonly HighlightTrigger _highlightTrigger;

        Sprite2D _highlightSprite;
        IUIInteractiveElement _owner;

        public HighlightComponent(string highlightTexture, HighlightTrigger highlightTrigger, float highlightTexOpacity = 0.3f){
            _highlightTexture = highlightTexture;
            _highlightTrigger = highlightTrigger;
            _highlightTexOpacity = highlightTexOpacity;
            IsEnabled = true;
        }

        #region IAcceptLeftButtonPressEvent Members

        public void OnLeftButtonPress(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
            if (IsEnabled){
                if (_owner.ContainsMouse){
                    ProcHighlight();
                }
                else{
                    UnprocHighlight();
                }
            }
        }

        #endregion

        #region IAcceptLeftButtonReleaseEvent Members

        public void OnLeftButtonRelease(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
            if (IsEnabled){
                UnprocHighlight();
            }
        }

        #endregion

        #region IAcceptMouseMovementEvent Members

        public void OnMouseMovement(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
            if (IsEnabled){
                if (_owner.ContainsMouse){
                    ProcHighlight();
                }
                else{
                    UnprocHighlight();
                }
            }
        }

        #endregion

        #region IUIComponent Members

        public bool IsEnabled { get; set; }

        public void ComponentCtor(IUIElement owner, ButtonEventDispatcher ownerEventDispatcher){
            _owner = (IUIInteractiveElement) owner;

            //event stuff
            if (owner.DoesComponentExist<DraggableComponent>()){
                var dcomponent = _owner.GetComponent<DraggableComponent>();
                dcomponent.DragMovementDispatcher += OnOwnerDrag;
            }
            switch (_highlightTrigger){
                case HighlightTrigger.MouseEntryExit:
                    ownerEventDispatcher.OnMouseMovement.Add(this);
                    break;
                case HighlightTrigger.MousePressRelease:
                    ownerEventDispatcher.OnLeftButtonPress.Add(this);
                    ownerEventDispatcher.OnLeftButtonRelease.Add(this);
                    break;
                case HighlightTrigger.InvalidTrigger:
                    throw new Exception("invalid highlight trigger");
            }

            //create sprite
            _highlightSprite = new Sprite2D(_highlightTexture, (int) _owner.X, (int) _owner.Y, (int) _owner.Width, (int) _owner.Height, _owner.Depth - 0.01f, 0);
        }

        public void Update(){
        }

        #endregion

        public void ProcHighlight(){
            _highlightSprite.Opacity = _highlightTexOpacity;
        }

        public void UnprocHighlight(){
            _highlightSprite.Opacity = 0;
        }

        //xxx untested
        void OnOwnerDrag(object caller, int dx, int dy){
            _highlightSprite.X += dx;
            _highlightSprite.Y += dy;
        }

        public static HighlightComponent ConstructFromObject(JObject obj){
            var data = obj.ToObject<HighlightComponentCtorData>();

            if (data.HighlightTexture == null || data.HighlightTrigger == HighlightTrigger.InvalidTrigger)
                throw new Exception("not enough information to generate highlight component");

            return new HighlightComponent(data.HighlightTexture, data.HighlightTrigger, data.HighlightTexOpacity);
        }

        #region Nested type: HighlightComponentCtorData

        struct HighlightComponentCtorData{
            // ReSharper disable UnassignedField.Local
            public float HighlightTexOpacity;
            public string HighlightTexture;
            public HighlightTrigger HighlightTrigger;
            // ReSharper restore UnassignedField.Local
        }

        #endregion
    }
}