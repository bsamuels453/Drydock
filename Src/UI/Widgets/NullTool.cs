#region

using Drydock.Control;

#endregion

namespace Drydock.UI.Widgets{
    internal class NullTool : IToolbarTool{
        #region IToolbarTool Members

        public void UpdateInput(ref ControlState state){
            //throw new NotImplementedException();
        }

        public void UpdateLogic(double timeDelta){
            //throw new NotImplementedException();
        }

        public void Enable(){
            //throw new NotImplementedException();
        }

        public void Disable(){
            //throw new NotImplementedException();
        }

        #endregion
    }
}