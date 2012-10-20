#region

using System;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.UI.Components{
    internal delegate void DraggableObjectClamp(IUIInteractiveElement owner, ref int x, ref int y, int oldX, int oldY);

    internal delegate void OnComponentDrag(object caller, int dx, int dy);


    /// <summary>
    ///   allows a UI element to be dragged. Required element to be IUIInteractiveComponent
    /// </summary>
    internal class DraggableComponent : IUIComponent, IAcceptLeftButtonPressEvent, IAcceptLeftButtonReleaseEvent, IAcceptMouseMovementEvent{
        bool _isEnabled;
        bool _isMoving;
        Vector2 _mouseOffset;
        IUIInteractiveElement _owner;

        #region properties


        #endregion

        #region ctor

        public DraggableComponent(){
            _mouseOffset = new Vector2();
        }

        public void ComponentCtor(IUIElement owner, ButtonEventDispatcher ownerEventDispatcher) {
            _owner = (IUIInteractiveElement)owner;
            _isEnabled = true;
            _isMoving = false;
            ownerEventDispatcher.OnLeftButtonPress.Add(this);
            ownerEventDispatcher.OnLeftButtonRelease.Add(this);
            ownerEventDispatcher.OnMouseMovement.Add(this);
        }

        #endregion

        #region IAcceptLeftButtonPressEvent Members

        public void OnLeftButtonPress(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
            if (!_isMoving && _isEnabled){
                if (_owner.BoundingBox.Contains(mousePos.X, mousePos.Y)){
                    _isMoving = true;
                    UIElementCollection.Collection.DisableEntryHandlers = true;
                    _mouseOffset.X = _owner.X - mousePos.X;
                    _mouseOffset.Y = _owner.Y - mousePos.Y;
                }
            }
        }

        #endregion

        #region IAcceptLeftButtonReleaseEvent Members

        public void OnLeftButtonRelease(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
            if (_isMoving){
                _isMoving = false;
                UIElementCollection.Collection.DisableEntryHandlers = false;
            }
        }

        #endregion

        #region IAcceptMouseMovementEvent Members

        public void OnMouseMovement(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
            if (_isMoving && _isEnabled){
                var oldX = (int) _owner.X;
                var oldY = (int) _owner.Y;
                var x = (int) (mousePos.X + _mouseOffset.X);
                var y = (int) (mousePos.Y + _mouseOffset.Y);

                //var x = (int)(state.X - nprevState.X +_owner.X);
                //var y = (int)(state.Y - nprevState.Y +_owner.Y);


                if (DragMovementClamp != null){
                    DragMovementClamp(_owner, ref x, ref y, oldX, oldY);
                }

                //this block checks if a drag clamp is preventing the owner from moving, if thats the case then kill the drag
                var tempRect = new Rectangle(x - (int) _owner.BoundingBox.Width*2, y - (int) _owner.BoundingBox.Height*2, (int) _owner.BoundingBox.Width*6, (int) _owner.BoundingBox.Height*6);
                if (!tempRect.Contains(mousePos.X, mousePos.Y)){
                    //_isMoving = false;
                    //_owner.Owner.DisableEntryHandlers = false;
                    //return InterruptState.AllowOtherEvents;
                    //int f = 5;
                }

                _owner.X = x;
                _owner.Y = y;

                if (DragMovementDispatcher != null){
                    DragMovementDispatcher(_owner, x - oldX, y - oldY);
                }
                allowInterpretation = false;
            }
        }

        #endregion

        #region IUIComponent Members


        public bool IsEnabled{
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public void Update(){
        }

        #endregion

        public event OnComponentDrag DragMovementDispatcher;
        public event DraggableObjectClamp DragMovementClamp;
    }
}