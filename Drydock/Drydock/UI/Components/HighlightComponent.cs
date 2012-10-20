#region

using System;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

#endregion

namespace Drydock.UI.Components{
    internal class HighlightComponent : IUIComponent, IAcceptMouseMovementEvent{
        readonly string _highlightTexture;
        Sprite2D _highlightSprite;
        IUIInteractiveElement _owner;

        public HighlightComponent(string highlightTexture){
            _highlightTexture = highlightTexture;
        }

        #region IAcceptMouseMovementEvent Members

        public void OnMouseMovement(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
            _highlightSprite.Opacity = _owner.ContainsMouse ? 0.3f : 0;
        }

        #endregion

        #region IUIComponent Members

        public bool IsEnabled { get; set; }

        public void ComponentCtor(IUIElement owner, ButtonEventDispatcher ownerEventDispatcher){
            _owner = (IUIInteractiveElement) owner;

            //event stuff
            ownerEventDispatcher.OnMouseMovement.Add(this);
            if (owner.DoesComponentExist<DraggableComponent>()){
                var dcomponent = _owner.GetComponent<DraggableComponent>();
                dcomponent.DragMovementDispatcher += OnOwnerDrag;
            }

            //create sprite
            _highlightSprite = new Sprite2D(_highlightTexture, (int) _owner.X, (int) _owner.Y, (int) _owner.Width, (int) _owner.Height, owner.Depth - 0.01f, 0);
        }

        public void Update(){
        }

        #endregion

        //xxx untested
        void OnOwnerDrag(object caller, int dx, int dy){
            _highlightSprite.X += dx;
            _highlightSprite.Y += dy;
        }

        public static HighlightComponent ConstructFromObject(JObject obj){
            var data = obj.ToObject<HighlightComponentCtorData>();

            if (data.HighlightTexture == null)
                throw new Exception("not enough information to generate highlight component");

            return new HighlightComponent(data.HighlightTexture);
        }

        #region Nested type: HighlightComponentCtorData

        struct HighlightComponentCtorData{
// ReSharper disable UnassignedField.Local
            public string HighlightTexture;
// ReSharper restore UnassignedField.Local
        }

        #endregion
    }
}