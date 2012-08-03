using Drydock.Render;

namespace Drydock.UI{
    internal interface IUIElement : IDrawable{
        string TextureName { get; }
        float Opacity { get; set; }
        float Depth { get; set; }
        int Identifier { get; }
        IUIElementComponent[] Components { get; set; }
        IAdvancedPrimitive Sprite { get; }
        TComponent GetComponent<TComponent>();
        void Update();
    }
}