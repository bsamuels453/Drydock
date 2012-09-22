#region

using Microsoft.Xna.Framework;

#endregion

namespace Drydock.UI.Components{
    /// <summary>
    ///   prevents mouse interactions from falling through the owner's bounding box
    /// </summary>
    internal class PanelComponent : IUIComponent, IAcceptLeftButtonPressEvent, IAcceptLeftButtonReleaseEvent, IAcceptMouseScrollEvent{
        IUIInteractiveElement _owner;

        public PanelComponent(){
            IsEnabled = true;
        }

        #region IAcceptLeftButtonPressEvent Members

        public void OnLeftButtonPress(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
            PreventClickFallthrough(ref allowInterpretation, mousePos);
        }

        #endregion

        #region IAcceptLeftButtonReleaseEvent Members

        public void OnLeftButtonRelease(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
            PreventClickFallthrough(ref allowInterpretation, mousePos);
        }

        #endregion

        #region IAcceptMouseScrollEvent Members

        public void OnMouseScrollwheel(ref bool allowInterpretation, float wheelChange, Point mousePos){
            PreventClickFallthrough(ref allowInterpretation, mousePos);
        }

        #endregion

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
            _owner.OnLeftButtonPress.Add(this);
            _owner.OnLeftButtonRelease.Add(this);
            _owner.OnMouseScroll.Add(this);
        }

        void PreventClickFallthrough(ref bool allowLeftButtonInterpretation, Point mousePos){
            if (allowLeftButtonInterpretation){
                if (_owner.BoundingBox.Contains(mousePos.X, mousePos.Y)){
                    allowLeftButtonInterpretation = false;
                }
            }
        }
    }
}