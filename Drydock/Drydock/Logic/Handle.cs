using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Render;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Logic {
    class Handle : IDraggable{
        public Dongle2D ElementDongle { get; set; }
        private CDraggable _dragComponent;

        public Handle(){
            string textname = "box";
            _dragComponent = new CDraggable(this, 100, 100, 50, 50);
            ElementDongle = new Dongle2D(textname, 100, 100);
        }

        
    }
}
