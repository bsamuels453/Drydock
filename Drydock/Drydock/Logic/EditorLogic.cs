#region

using Drydock.Control;
using Drydock.Render;
using Drydock.UI;

#endregion

namespace Drydock.Logic{
    internal class EditorLogic{
        private HullEditor _e;

        public EditorLogic(Renderer renderer){

            _e = new HullEditor();


        }

        public void Update(){
            //160,000 microsecond budget
            _e.Update();
            //4,000 microseconds
            InputEventDispatcher.Update();
            UIContext.Update();

        }
    }
}