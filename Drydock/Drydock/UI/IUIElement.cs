using Drydock.Render;

namespace Drydock.UI{
    internal interface IUIElement : IDrawable{
        int Identifier { get;}
        IUIElementComponent[] Components { get; set; }
        TComponent GetComponent<TComponent>();
        IAdvancedPrimitive Sprite { get; }
        void Update();
    }
}