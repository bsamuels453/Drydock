#region



#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI.Components{
    internal delegate void ReactToSelection(SelectState state);

    internal enum SelectState{
        Selected,
        UnSelected
    };

    /// <summary>
    ///   allows a UI element to be selected. Required element to be IUIInteractiveComponent
    /// </summary>
    internal class SelectableComponent : IUIComponent, IAcceptLeftButtonClickEvent, IAcceptMouseMovementEvent {
        readonly int _selectedHeight;
        readonly String _selectedTexture;
        readonly int _selectedWidth;
        int _heightDx;
        bool _isSelected;

        //these fields contain the "differences" in bounding boxes between the owner's selected texture/bbox and normal texture/bbox
        string _originalTexture;
        IUIInteractiveElement _owner;
        int _positionDx;
        int _positionDy;
        int _widthDx;

        public SelectableComponent(string selectedTexture, int width, int height){
            _selectedTexture = selectedTexture;
            _selectedWidth = width;
            _selectedHeight = height;
            IsEnabled = true;
            _isSelected = false;
        }

        #region IUIComponent Members

        public void ComponentCtor(IUIElement owner, ButtonEventDispatcher ownerEventDispatcher){
            _owner = (IUIInteractiveElement)owner;
            IsEnabled = true;
            ownerEventDispatcher.OnLeftButtonClick.Add(this);
            ownerEventDispatcher.OnMouseMovement.Add(this);

            _originalTexture = _owner.Texture;
            _widthDx = (int)(_selectedWidth - _owner.BoundingBox.Width);
            _heightDx = (int)(_selectedHeight - _owner.BoundingBox.Height);
            _positionDx = _widthDx / 2;
            _positionDy = _heightDx / 2;
        }

        public bool IsEnabled { get; set; }

        public void Update(){
            throw new NotImplementedException();
        }

        public void UpdateLogic(){
        }

        #endregion

        public void ProcSelect(){
            if (IsEnabled && !_isSelected){
                _owner.Texture = _selectedTexture;
                _owner.Width += _widthDx;
                _owner.Height += _heightDx;
                _owner.X -= _positionDx;
                _owner.Y -= _positionDy;
                _isSelected = true;

                try{
                    _owner.GetComponent<FadeComponent>().IsEnabled = false;
                }
                    // ReSharper disable EmptyGeneralCatchClause
                catch (Exception){
                } /*there is no fade component*/

                // ReSharper restore EmptyGeneralCatchClause
                //if (ReactToSelectionDispatcher != null){
                //    ReactToSelectionDispatcher(SelectState.Selected);
                //}
            }
        }

        public void ProcDeselect(){
            if (IsEnabled && _isSelected){
                _owner.Texture = _originalTexture;
                _owner.Width -= _widthDx;
                _owner.Height -= _heightDx;
                _owner.X += _positionDx;
                _owner.Y += _positionDy;
                _isSelected = false;
                try{
                    _owner.GetComponent<FadeComponent>().IsEnabled = true;
                }
                    // ReSharper disable EmptyGeneralCatchClause
                catch (Exception){ /*there is no fade component*/
 }
                // ReSharper restore EmptyGeneralCatchClause
                //if (ReactToSelectionDispatcher != null){
                //    ReactToSelectionDispatcher(SelectState.UnSelected);
                //}
            }
        }

        public void ProcHover(){

        }

        public void ProcUnhover(){

        }

        public void OnLeftButtonClick(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
            if (IsEnabled) {
                if (_isSelected) {
                    ProcDeselect();
                }
                else {
                    if (_owner.BoundingBox.Contains(mousePos.X, mousePos.Y)) {
                        ProcSelect();
                    }
                }
            }
        }

        public void OnMouseMovement(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
            
        }
    }
}
