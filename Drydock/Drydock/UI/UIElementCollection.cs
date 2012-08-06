using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Control;

namespace Drydock.UI {
    class UIElementCollection {
        public List<OnMouseAction> OnLeftButtonDownEvent;
        public List<OnMouseAction> OnLeftButtonUpEvent;
        public List<OnMouseAction> OnLeftButtonClickEvent;
        public List<OnKeyboardAction> OnKeyboardActionEvent;

        #region element addition methods

        public void Add(UIElementCollection childCollection){

        }

        public void Add(IUIElement element) {

        }

        #endregion

        #region event dispatchers



        #endregion

    }
}
