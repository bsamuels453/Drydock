using System.Diagnostics;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI;

namespace Drydock.Logic{
    internal class EditorLogic{
        private readonly CurveControllerCollection _c;

        public EditorLogic(Renderer renderer){


            UIContext.Init();

            //initalize component classes
            _c = new CurveControllerCollection();

        }

        public void Update(){
            //160,000 microsecond budget

            //4,000 microseconds
            InputEventDispatcher.Update();
            _c.Update();
            UIContext.Update();

        }
    }
}