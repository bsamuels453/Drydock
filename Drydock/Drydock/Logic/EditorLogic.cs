using Drydock.Control;
using Drydock.Logic.InterfaceObj;
using Drydock.Render;
using Drydock.UI;
using Drydock.UI.Button;

namespace Drydock.Logic{
    internal class EditorLogic{
        private readonly Button _b;
        private readonly CurveControllerCollection _c;
        private readonly UIContext _ctxt;
        private readonly KeyboardHandler _keyboardHandler;

        public EditorLogic(Renderer renderer){
            MouseHandler.Init(renderer.Device);
            _keyboardHandler = new KeyboardHandler(renderer);

            //initalize component classes
            CDraggable.Init();
            _c = new CurveControllerCollection();
            //c = new CurveController(200, 200, 100, 100, 0.3f);
            //line = new Line2D(100, 100, 200, 200);
            var g = new UIContext();
            g.Update();
            _ctxt = new UIContext();
            _b = _ctxt.Add<Button>(
                new Button(
                    x: 50,
                    y: 50,
                    width: 50,
                    height: 50,
                    layerDepth: 0.5f,
                    textureName: "box",
                    components: new IUIElementComponent[]{
                        new DraggableComponent(),
                        new FadeComponent(FadeComponent.FadeState.Faded)
                    }
                    )
                );
            _b.OnMouseExit.Add(_b.GetComponent<FadeComponent>().ForceFadein);
            _b.OnMouseHover.Add(_b.GetComponent<FadeComponent>().ForceFadeout);

            //v.GetComponent<DraggableComponent>().Owner
        }

        public void Update(){
            MouseHandler.UpdateMouse();
            _keyboardHandler.UpdateKeyboard();
            _c.UpdateCurves();
            //v.Update();
            _ctxt.Update();
        }
    }
}