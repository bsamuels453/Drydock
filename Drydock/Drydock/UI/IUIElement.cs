#region

using Drydock.Control;
using Drydock.Utilities;

#endregion

namespace Drydock.UI{
    internal interface IUIElement : ILogicUpdates{
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        FloatingRectangle BoundingBox { get; } //move somewhere else?
        float Opacity { get; set; }
        float Depth { get; set; }
        string Texture { get; set; }
        int Identifier { get; }

        TComponent GetComponent<TComponent>(string identifier=null);
        bool DoesComponentExist<TComponent>(string identifier=null);
    }
}