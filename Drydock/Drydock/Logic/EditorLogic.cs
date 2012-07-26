using Drydock.Control;
using Drydock.Logic.InterfaceObj;
using Drydock.Render;

namespace Drydock.Logic{
    internal class EditorLogic{
        private readonly CurveControllerCollection _c;
        private readonly KeyboardHandler _keyboardHandler;
        private readonly MouseHandler _mouseHandler;

        public EditorLogic(Renderer renderer){
            _mouseHandler = new MouseHandler(renderer.Device);
            _keyboardHandler = new KeyboardHandler(renderer);

            //initalize component classes
            CDraggable.Init(_mouseHandler);
            _c = new CurveControllerCollection();
            //c = new CurveController(200, 200, 100, 100, 0.3f);
            //line = new Line2D(100, 100, 200, 200);
        }

        public void Update(){
            _mouseHandler.UpdateMouse();
            _keyboardHandler.UpdateKeyboard();
            _c.UpdateCurves();
        }
    }
}