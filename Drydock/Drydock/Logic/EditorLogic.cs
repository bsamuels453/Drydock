using Drydock.Control;
using Drydock.Render;

namespace Drydock.Logic{
    internal class EditorLogic{
        private readonly KeyboardHandler _keyboardHandler;
        private readonly MouseHandler _mouseHandler;
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