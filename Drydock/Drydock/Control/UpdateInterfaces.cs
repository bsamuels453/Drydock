using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drydock.Control {
    interface IUpdatable {
        void Update(double timeDelta);
    }

    internal interface IInputUpdatable{
        void InputUpdate(ref ControlState state);
    }
}
