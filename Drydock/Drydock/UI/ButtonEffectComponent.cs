using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI {
    class ButtonEffectComponent : IButtonComponent{
        private Button _owner;
        private bool _isEnabled;

        #region properties
        public Button Owner{
            set { 
                _owner = value;
                _owner.OnLeftButtonClick.Add(OnMouseClick);
            }
        }

        public bool IsEnabled{
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }
        #endregion

        public ButtonEffectComponent(){


        }
        private bool OnMouseClick(MouseState state){
            _owner.Sprite.Dispose();
            return false;
        }

        public void Update(){
        }
    }
}
