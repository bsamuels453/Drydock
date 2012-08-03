using System;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI.Components{
    internal delegate void ReactToSelection(SelectState state);

    internal enum SelectState{
        Selected,
        UnSelected
    };

    /// <summary>
    /// allows a UI element to be selected. Required element to be IUIInteractiveComponent
    /// </summary>
    internal class SelectableComponent : IUIElementComponent{
        private IUIInteractiveElement _owner;

        private readonly String _selectedTexture;
        private int _selectedWidth;
        private int _selectedHeight;

        public IUIElement Owner{
            set {
                if (!UIContext.IsElementInteractive(value)){
                    throw new Exception("Element is not interactive");
                }
                _owner = (IUIInteractiveElement)value;
                ComponentCtor();
            }
        }

        public bool IsEnabled { get; set; }


        public void Update(){

        }

        public SelectableComponent(string selectedTexture, int width, int height){
            _selectedTexture = selectedTexture;
            _selectedWidth = width;
            _selectedHeight = height;
        }

        private void ComponentCtor(){
            IsEnabled = true;

            _owner.OnLeftButtonClick.Add(OnMouseClick);

        }

        private bool OnMouseClick(MouseState state){

            return false;
        }

        private void SelectThis(){
            _owner.Sprite.SetTexture(_selectedTexture);

        }

        private void DeselectThis(){
            _owner.Sprite.SetTexture(_owner.TextureName);

        }
    }
}