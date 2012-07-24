using Drydock.Render;

namespace Drydock.Logic{
    internal class Handle : IDraggable{
        private CDraggable _dragComponent;

        public Handle(){
            string textname = "box";
            _dragComponent = new CDraggable(this, 100, 100, 50, 50);
            ElementDongle = new Dongle2D(textname, 100, 100);
        }

        #region IDraggable Members

        public Dongle2D ElementDongle { get; set; }
        public void ClampDraggedPosition(ref int x, ref int y){ }

        #endregion
    }
}