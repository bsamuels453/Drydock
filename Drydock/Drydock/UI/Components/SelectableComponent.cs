#region

using System;
using Drydock.Control;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.UI.Components{
    internal delegate void ReactToSelection(SelectState state);

    internal enum SelectState{
        Selected,
        UnSelected
    };

    /// <summary>
    ///   allows a UI element to be selected. Required element to be IUIInteractiveComponent
    /// </summary>
    internal class SelectableComponent : IUIComponent{
        private readonly int _selectedHeight;
        private readonly String _selectedTexture;
        private readonly int _selectedWidth;
        private int _heightDx;
        private bool _isSelected;

        //these fields contain the "differences" in bounding boxes between the owner's selected texture/bbox and normal texture/bbox
        private string _originalTexture;
        private IUIInteractiveElement _owner;
        private int _positionDx;
        private int _positionDy;
        private int _widthDx;

        public SelectableComponent(string selectedTexture, int width, int height){
            _selectedTexture = selectedTexture;
            _selectedWidth = width;
            _selectedHeight = height;
            IsEnabled = true;
            _isSelected = false;
        }

        #region IUIComponent Members

        public IUIElement Owner{
            set{
                if (value is IUIInteractiveElement){
                    _owner = (IUIInteractiveElement) value;
                    ComponentCtor();
                }
                else{
                    throw new Exception("Element is not interactive");
                }
            }
        }

        public bool IsEnabled { get; set; }

        public void Update(){
        }

        #endregion

        public event ReactToSelection ReactToSelectionDispatcher;

        private void ComponentCtor(){
            IsEnabled = true;
            _owner.OnLeftButtonClick.Add(OnMouseClick);

            _originalTexture = _owner.Texture;
            _widthDx = (int) (_selectedWidth - _owner.BoundingBox.Width);
            _heightDx = (int) (_selectedHeight - _owner.BoundingBox.Height);
            _positionDx = _widthDx/2;
            _positionDy = _heightDx/2;
        }

        private InterruptState OnMouseClick(MouseState state){
            if (IsEnabled){
                if (_isSelected){
                    DeselectThis();
                }
                else{
                    if (_owner.BoundingBox.Contains(state.X, state.Y)){
                        SelectThis();
                    }
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public void SelectThis(){
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
                catch (Exception){ /*there is no fade component*/
                }
                // ReSharper restore EmptyGeneralCatchClause
                if (ReactToSelectionDispatcher != null){
                    ReactToSelectionDispatcher(SelectState.Selected);
                }
            }
        }

        public void DeselectThis(){
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
                if (ReactToSelectionDispatcher != null){
                    ReactToSelectionDispatcher(SelectState.UnSelected);
                }
            }
        }
    }
}