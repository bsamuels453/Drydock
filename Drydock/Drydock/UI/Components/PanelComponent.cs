#region

using Drydock.Control;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.UI.Components{
    /// <summary>
    ///   prevents mouse interactions from falling through the owner's bounding box
    /// </summary>
    internal class PanelComponent : IUIComponent{
        IUIInteractiveElement _owner;

        public PanelComponent(){
            IsEnabled = true;
        }

        #region IUIComponent Members

        public IUIElement Owner{
            set{
                _owner = (IUIInteractiveElement) value;
                ComponentCtor();
            }
        }

        public bool IsEnabled { get; set; }

        public void Update(){
        }

        #endregion

        void ComponentCtor(){
            _owner.OnLeftButtonPress.Add(OnMouseButtonAction);
            _owner.OnLeftButtonRelease.Add(OnMouseButtonAction);
            _owner.OnMouseScroll.Add(OnMouseButtonAction);
        }

        InterruptState OnMouseButtonAction(MouseState state, MouseState? prevState = null){
            if (_owner.BoundingBox.Contains(state.X, state.Y)){
                return InterruptState.InterruptEventDispatch;
            }
            return InterruptState.AllowOtherEvents;
        }
    }
}