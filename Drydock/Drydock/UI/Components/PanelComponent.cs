﻿#region

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

        public void ComponentCtor(IUIElement owner, ButtonEventDispatcher ownerEventDispatcher){
            _owner = (IUIInteractiveElement) owner;
            ownerEventDispatcher.OnLeftButtonPress.Add(this);
            ownerEventDispatcher.OnLeftButtonRelease.Add(this);
            ownerEventDispatcher.OnMouseScroll.Add(this);
        }

        public bool IsEnabled { get; set; }

        public void Update(){
        }

        #endregion

        void PreventClickFallthrough(ref bool allowLeftButtonInterpretation, Point mousePos){
            if (allowLeftButtonInterpretation){
                if (_owner.BoundingBox.Contains(mousePos.X, mousePos.Y)){
                    allowLeftButtonInterpretation = false;
                }
            }
        }
    }
}