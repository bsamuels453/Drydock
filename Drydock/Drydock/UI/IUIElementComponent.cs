namespace Drydock.UI{
    internal interface IUIElementComponent{
        Button.Button Owner { set; }
        bool IsEnabled { get; set; }
        void Update();
    }
}