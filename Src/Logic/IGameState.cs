using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Control;

namespace Drydock.Logic {
    namespace Drydock.Logic {
        internal interface IGameState : IDisposable {
            void Update(InputState state, double timeDelta);
            void Draw();
        }
    }
}
