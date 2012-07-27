using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drydock.UI {
    interface IButtonComponent {
        Button Owner { set; }
        bool IsEnabled { get; set; }
        void Update();
    }
}
