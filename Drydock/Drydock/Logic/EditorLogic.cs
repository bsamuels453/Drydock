using Drydock.Control;
using Drydock.Render;

namespace Drydock.Logic{
    internal class EditorLogic{
        private readonly KeyboardHandler _keyboardHandler;
        private readonly MouseHandler _mouseHandler;
        private CurveController c;
        // private Line2D line;

        public EditorLogic(Renderer renderer){
            _mouseHandler = new MouseHandler(renderer.Device);
            _keyboardHandler = new KeyboardHandler(renderer);

            //initalize component classes
            CDraggable.Init(_mouseHandler);
            c = new CurveController(200, 200, 10, 10, 0.3f);
            //line = new Line2D(100, 100, 200, 200);
        }

        public void Update(){
            _mouseHandler.UpdateMouse();
            _keyboardHandler.UpdateKeyboard();
        }
    }
}