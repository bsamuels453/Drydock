#region

using Drydock.Render;

#endregion

namespace Drydock.UI{
    internal interface IUIElement : IDrawable{
        float Opacity { get; set; }
        float Depth { get; set; }
        int Identifier { get; }
        IUIComponent[] Components { get; set; }
        IAdvancedPrimitive Sprite { get; }
        UIElementCollection Owner { get;  set; }
        TComponent GetComponent<TComponent>();
        bool DoesComponentExist<TComponent>();
        void Update();
        void Dispose();
    }
}