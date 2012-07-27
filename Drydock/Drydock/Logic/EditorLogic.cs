using Drydock.Control;
using Drydock.Logic.InterfaceObj;
using Drydock.Render;
using Drydock.UI;

namespace Drydock.Logic{
    internal class EditorLogic{
        private readonly CurveControllerCollection _c;
        private readonly KeyboardHandler _keyboardHandler;

        public EditorLogic(Renderer renderer){
            MouseHandler.Init(renderer.Device);
            _keyboardHandler = new KeyboardHandler(renderer);

            //initalize component classes
            CDraggable.Init();
            _c = new CurveControllerCollection();
            //c = new CurveController(200, 200, 100, 100, 0.3f);
            //line = new Line2D(100, 100, 200, 200);
            var v = new Button(
                x: 50,
                y: 50,
                width: 50,
                height: 50,
                layerDepth: 0.5f,
                textureName: "box",
                components: new IButtonComponent[]{
                    new DraggableComponent()
                }
                );
        }

        public void Update(){
            MouseHandler.UpdateMouse();
            _keyboardHandler.UpdateKeyboard();
            _c.UpdateCurves();
        }
    }
}