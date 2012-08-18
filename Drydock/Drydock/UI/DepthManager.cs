#region

using System;

#endregion

namespace Drydock.UI{
    //return (float) d/(float) Math.Pow(10, Depth);
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