namespace Drydock.UI{
    internal interface IUIElement{
        IUIElementComponent[] Components { get; set; }
        TComponent GetComponent<TComponent>();
        void Update();
    }
}