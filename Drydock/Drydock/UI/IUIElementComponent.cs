namespace Drydock.UI{
    internal interface IUIElementComponent{
        IUIElement Owner { set; }
        bool IsEnabled { get; set; }
        void Update();
    }
}