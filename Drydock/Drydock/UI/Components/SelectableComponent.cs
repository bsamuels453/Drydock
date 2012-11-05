/*
namespace Drydock.UI.Components{
    internal delegate void ReactToSelection(SelectState state);

    internal enum SelectState{
        Selected,
        UnSelected
    };

    /// <summary>
    ///  
    /// </summary>
    internal class SelectableComponent : IUIComponent, IAcceptLeftButtonClickEvent, IAcceptMouseMovementEvent{
        readonly int _selectedHeight;
        readonly String _highlightTexture;
        bool _isSelected;

        string _originalTexture;
        IUIInteractiveElement _owner;


        public SelectableComponent(string highlightTexture){
            _highlightTexture = highlightTexture;
            IsEnabled = true;
            _isSelected = false;
        }

        #region IAcceptLeftButtonClickEvent Members

        public void OnLeftButtonClick(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
            if (IsEnabled){
                if (_isSelected){
                    ProcDeselect();
                }
                else{
                    if (_owner.BoundingBox.Contains(mousePos.X, mousePos.Y)){
                        ProcSelect();
                    }
                }
            }
        }

        #endregion

        #region IAcceptMouseMovementEvent Members

        public void OnMouseMovement(ref bool allowInterpretation, Point mousePos, Point prevMousePos){
        }

        #endregion

        #region IUIComponent Members

        public void ComponentCtor(IUIElement owner, ButtonEventDispatcher ownerEventDispatcher){
            _owner = (IUIInteractiveElement) owner;
            IsEnabled = true;
            ownerEventDispatcher.OnLeftButtonClick.Add(this);
            ownerEventDispatcher.OnMouseMovement.Add(this);

            _originalTexture = _owner.Texture;
            /*_widthDx = (int) (_selectedWidth - _owner.BoundingBox.Width);
            _heightDx = (int) (_selectedHeight - _owner.BoundingBox.Height);
            _positionDx = _widthDx/2;
            _positionDy = _heightDx/2;*/
/*}

        public bool IsEnabled { get; set; }

        public void Update(){
            throw new NotImplementedException();
        }

        #endregion

        public void UpdateLogic(){
        }

        public void ProcSelect(){
            if (IsEnabled && !_isSelected){
                _owner.Texture = _highlightTexture;
                
                /*
                _owner.Width += _widthDx;
                _owner.Height += _heightDx;
                _owner.X -= _positionDx;
                _owner.Y -= _positionDy;
                 */
/*
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
/*
            }
        }

        public void ProcDeselect(){
            if (IsEnabled && _isSelected){
                _owner.Texture = _originalTexture;

                /*
                _owner.Width -= _widthDx;
                _owner.Height -= _heightDx;
                _owner.X += _positionDx;
                _owner.Y += _positionDy;
                 */
/*
                _isSelected = false;
                try{
                    _owner.GetComponent<FadeComponent>().IsEnabled = true;
                }
                    // ReSharper disable EmptyGeneralCatchClause
                catch (Exception){ /*there is no fade component*/
/*}
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
    }

}*/

