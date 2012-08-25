#region

using Drydock.UI;

#endregion

namespace Drydock.Logic{
    internal class EditorLogic{
        readonly HullEditor _e;

        public EditorLogic(){
            _e = new HullEditor();
        }

        public void Update(){
            //160,000 microsecond budget
            _e.Update();
            //4,000 microseconds

            UIContext.Update();
        }
    }
}