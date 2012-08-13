#region

using Drydock.Render;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.UI{

    internal abstract class ComponentThing{
        IUIComponent[] Components { get; set; }

    }

    internal interface IUIElement :  IDrawable{
        float Opacity { get; set; }
        float Depth { get; set; }
        string Texture { get; set; }
        int Identifier { get; }
        
        UIElementCollection Owner { get;  set; }
        TComponent GetComponent<TComponent>();
        bool DoesComponentExist<TComponent>();
        void Update();
        void Dispose();
    }
}