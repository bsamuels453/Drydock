#region

using System;

#endregion

namespace Drydock.UI{
    /// <summary>
    ///   This class manages depths for rendering order. It is to be only used in uielementcollection.
    /// </summary>
    internal class DepthManager{
        public int Depth;


        public DepthManager(){
            Depth = 1;
        }

        public float GetDepth(DepthLevel d){
            return (float) d/(float) Math.Pow(10, Depth);
        }

        public static float GetDepthFloat(DepthLevel d){
            return (float) d/(float) Math.Pow(10, 1);
        }
    }

    internal enum DepthLevel{ //can have 10 levels max
        Highlight,
        High,
        Medium,
        Low,
        Border,
        Base,
        Background
    }
}