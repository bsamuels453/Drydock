using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Logic {
    class EditorLogic {
        private readonly MouseHandler _mouseHandler;
        private readonly KeyboardHandler _keyboardHandler;
        private Handle h;

        public EditorLogic(Renderer renderer){
            _mouseHandler = new MouseHandler(renderer.Device);
            _keyboardHandler = new KeyboardHandler(renderer);

            //initalize component classes
            CDraggable.Init(_mouseHandler);
            h = new Handle();

        }

        public void Update(){
            _mouseHandler.UpdateMouse();
            _keyboardHandler.UpdateKeyboard();
        }
    }
}
