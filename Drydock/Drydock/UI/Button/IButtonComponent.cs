namespace Drydock.UI.Button {
    interface IButtonComponent {
        Button Owner { set; }
        bool IsEnabled { get; set; }
        void Update();
    }
}
