﻿namespace Drydock.UI{
    internal interface IUIComponent{
        /// <summary>
        /// A reference to the owner of the element.
        /// </summary>
        IUIElement Owner { set; }

        /// <summary>
        /// Disabling a component will cause it to ignore all public method calls, and ignore all event dispatches.
        /// Enabling a component will undo these changes. Components start enabled by default.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// An update function that will be called by the component's owner element.
        /// </summary>
        void Update();
    }
}