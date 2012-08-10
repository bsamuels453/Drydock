using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drydock.UI {
    /// <summary>
    /// This class manages depths for rendering order. It is to be only used in uielementcollection.
    /// </summary>
    class DepthManager {
        public int Depth;


        public DepthManager(){
            Depth = 1;
        }

        public float GetDepth(DepthLevel d){
            return (float) d/(float) Math.Pow(10, Depth);
        }
    }
    internal enum DepthLevel { //can have 10 levels max
        Highlight,
        High,
        Medium,
        Low,
        Border,
        Base,
        Background
    }
}
