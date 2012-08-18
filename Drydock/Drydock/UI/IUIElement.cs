#region

using Drydock.Utilities;

#endregion

namespace Drydock.UI{
    internal interface IUIElement{
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        FloatingRectangle BoundingBox { get; } //move somewhere else?
        float Opacity { get; set; }
        float Depth { get; set; }
        string Texture { get; set; }
        int Identifier { get; }

        UIElementCollection Owner { get; set; }
        TComponent GetComponent<TComponent>();
        bool DoesComponentExist<TComponent>();
        void Update();
        void Dispose();
    }
}