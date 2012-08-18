using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Control;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI.Components {
    /// <summary>
    /// prevents mouse interactions from falling through the owner's bounding box
    /// </summary>
    class PanelComponent : IUIComponent {
        IUIInteractiveElement _owner;
        bool _isEnabled;

        public IUIElement Owner{
            set {
                _owner = (IUIInteractiveElement)value;
                ComponentCtor();
            }
        }

        public bool IsEnabled{
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public void Update(){}

        public PanelComponent(){
            _isEnabled = true;
        }

        private void ComponentCtor(){
            _owner.OnLeftButtonPress.Add(OnMouseButtonAction);
            _owner.OnLeftButtonRelease.Add(OnMouseButtonAction);
            _owner.OnMouseScroll.Add(OnMouseButtonAction);
        }

        private InterruptState OnMouseButtonAction(MouseState state){
            if (_owner.BoundingBox.Contains(state.X, state.Y)){
                return InterruptState.InterruptEventDispatch;
            }
            return InterruptState.AllowOtherEvents;
        }

    }
}
