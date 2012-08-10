#region

using System;
using System.Collections.Generic;
using System.Linq;
using Drydock.UI.Components;

#endregion

namespace Drydock.UI{
    internal class ComponentGenerator{
        protected IUIComponent[] GenerateComponents(Dictionary<string, object[]> componentDict){
            var components = new List<IUIComponent>(componentDict.Count);
            foreach (var component in componentDict){
                switch (component.Key){
                        #region dragcomponent

                    case "DraggableComponent":
                        //there are no constructor parameters for DraggableComponent
                        components.Add(new DraggableComponent());
                        break;

                        #endregion

                        #region fadecomponent

                    case "FadeComponent":
                        if (component.Value.Count() < 2)
                            throw new Exception("not enough data to create a FadeComponent from template");

                        var defaultState = (FadeComponent.FadeState) component.Value[0];
                        var fadeTrigger = (FadeComponent.FadeTrigger) component.Value[1];
                        float fadeOpacity;
                        float fadeDuration;

                        //this is tricky because FadeComponent has a few parameters with default values
                        if (component.Value.Count() > 2)
                            fadeOpacity = (float) component.Value[2];
                        else{
                            fadeOpacity = FadeComponent.DefaultFadeoutOpacity;
                        }

                        if (component.Value.Count() > 3)
                            fadeDuration = (float) component.Value[3];
                        else
                            fadeDuration = FadeComponent.DefaultFadeDuration;

                        components.Add(new FadeComponent(defaultState, fadeTrigger, fadeOpacity, fadeDuration));
                        break;

                        #endregion

                        #region selectablecomponent

                    case "SelectableComponent":
                        if (component.Value.Count() < 3)
                            throw new Exception("not enough data to create a SelectableComponent from template");
                        components.Add(new SelectableComponent((string) component.Value[0], (int) component.Value[1], (int) component.Value[2]));
                        break;

                        #endregion

                    default:
                        throw new Exception("Template was given the name of a component that does not exist");
                }
            }
            return components.ToArray();
        }
    }
}
