#region

using System;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.UI.Components{
    delegate void FadeStateChange(FadeComponent.FadeState state);
    /// <summary>
    /// allows a UI element to be faded in and out. Required element to be IUIInteractiveComponent for certain settings.
    /// </summary>
    internal class FadeComponent : IUIComponent{
        #region FadeState enum

        public enum FadeState{
            Visible,
            Faded
        }

        #endregion

        #region FadeTrigger enum

        public enum FadeTrigger{
            EntryExit,
            None
        }

        #endregion

        private readonly FadeState _defaultState;
        private readonly float _fadeDuration;
        private readonly FadeTrigger _fadeTrigger;
        private readonly float _fadeoutOpacity;
        private bool _isEnabled;
        private bool _isFadingOut;
        private bool _isInTransition;
        private IUIElement _owner;
        private long _prevUpdateTimeIndex;

        #region properties

        public IUIElement Owner{
            set{
                _owner = value;
                ComponentCtor();
            }
        }

        public bool IsEnabled{
            get { return _isEnabled; }
            set{
                _isEnabled = value;
                var state = Mouse.GetState();
                if (_isEnabled){ //reset the timer
                    _prevUpdateTimeIndex = DateTime.Now.Ticks;
                    //because the mouse may have left the bounding box while this component was disabled
                    if(!_owner.BoundingBox.Contains(state.X, state.Y)){
                        ForceFadeout(state);
                    }
                }
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultState">default state of the parent object, faded or visible </param>
        /// <param name="trigger"> </param>
        /// <param name="fadeoutOpacity">opacity level to fade out to. range 0-1f</param>
        /// <param name="fadeDuration">the time it takes for sprite to fade out in milliseconds</param>
        public FadeComponent(FadeState defaultState, FadeTrigger trigger = FadeTrigger.None, float fadeoutOpacity = .05f, float fadeDuration = 250){
            _fadeoutOpacity = fadeoutOpacity;
            _fadeDuration = fadeDuration*10000; //10k ticks in a millisecond
            _isInTransition = false;
            _isFadingOut = false;
            _prevUpdateTimeIndex = DateTime.Now.Ticks;
            _defaultState = defaultState;
            _fadeTrigger = trigger;
            _isEnabled = true;
        }

        private void ComponentCtor(){
            if (_defaultState == FadeState.Faded){
                _owner.Opacity = _fadeoutOpacity;
            }
            switch (_fadeTrigger){
                case FadeTrigger.EntryExit:

                    
                    if (! (_owner is IUIInteractiveElement)){
                        throw new Exception("Invalid fade trigger: Unable to set an interactive trigger to a non-interactive element.");
                    }

                    ((IUIInteractiveElement) _owner).OnMouseEntry.Add(ForceFadein);
                    ((IUIInteractiveElement) _owner).OnMouseExit.Add(ForceFadeout);
                    break;

                case FadeTrigger.None:
                    break;
            }
        }

        #endregion

        #region IUIComponent Members

        public void Update(){
            if (IsEnabled){
                if (_isInTransition){
                    long timeSinceLastUpdate = DateTime.Now.Ticks - _prevUpdateTimeIndex;
                    float step = timeSinceLastUpdate/_fadeDuration;
                    if (_isFadingOut){
                        _owner.Opacity -= step;
                        if (_owner.Opacity < _fadeoutOpacity){
                            _owner.Opacity = _fadeoutOpacity;
                            _isInTransition = false;
                        }
                    }
                    else{
                        _owner.Opacity += step;
                        if (_owner.Opacity > 1){
                            _owner.Opacity = 1;
                            _isInTransition = false;
                        }
                    }
                }
                _prevUpdateTimeIndex = DateTime.Now.Ticks;
            }
        }

        #endregion

        #region modification methods

        public void ForceFadeout(MouseState state) {
            _owner.Owner.DisableEntryHandlers = false;
            if (IsEnabled){
                _isInTransition = true;
                _isFadingOut = true;
                if (FadeStateChangeDispatcher != null){
                    FadeStateChangeDispatcher(FadeState.Faded);
                }
            }
        }

        public void ForceFadein(MouseState state) {
            _owner.Owner.DisableEntryHandlers = true;
            //UIElementCollection.ForceExitHandlers(_owner);
            if (IsEnabled){
                _isInTransition = true;
                _isFadingOut = false;
                if (FadeStateChangeDispatcher != null) {
                    FadeStateChangeDispatcher(FadeState.Visible);
                }
            }
        }

        #endregion

        #region static methods

        /// <summary>
        /// This method links two elements together so that each element can proc the other's fade as defined by the FadeTrigger. 
        /// </summary>
        /// <param name="element1"></param>
        /// <param name="element2"></param>
        /// <param name="state"></param>
        public static void LinkFadeComponentTriggers(IUIElement element1, IUIElement element2, FadeTrigger state){
            switch (state){
                case FadeTrigger.EntryExit:
                    //first we check both elements to make sure they are both interactive. This check is specific for triggers that are interactive
                    if (!(element1 is IUIInteractiveElement) || !(element2 is IUIInteractiveElement)) {
                        throw new Exception("Unable to link interactive element fade triggers; one of the elements is not interactive");
                    }
                    //cast to interactive
                    var e1 = (IUIInteractiveElement)element1;
                    var e2 = (IUIInteractiveElement)element2;

                    e1.OnMouseEntry.Add(e2.GetComponent<FadeComponent>().ForceFadein);
                    e2.OnMouseEntry.Add(e1.GetComponent<FadeComponent>().ForceFadein);

                    e1.OnMouseExit.Add(e2.GetComponent<FadeComponent>().ForceFadeout);
                    e2.OnMouseExit.Add(e1.GetComponent<FadeComponent>().ForceFadeout);

                    break;
            }
        }

        /// <summary>
        /// This method allows an element's fade to trigger when another element undergoes a certain event as defined by the FadeTrigger.
        /// </summary>
        /// <param name="eventProcElement">The element whose events will proc the recieving element's fade.</param>
        /// <param name="eventRecieveElement">The recieving element.</param>
        /// <param name="state"></param>
        public static void LinkOnewayFadeComponentTriggers(IUIElement eventProcElement, IUIElement eventRecieveElement, FadeTrigger state){
            switch (state) {
                case FadeTrigger.EntryExit:
                    if (!(eventProcElement is IUIInteractiveElement)) {
                        throw new Exception("Unable to link interactive element fade triggers; the event proc element is not interactive.");
                    }
                    //cast to interactive
                    var e1 = (IUIInteractiveElement)eventProcElement;

                    e1.OnMouseEntry.Add(eventRecieveElement.GetComponent<FadeComponent>().ForceFadein);
                    e1.OnMouseExit.Add(eventRecieveElement.GetComponent<FadeComponent>().ForceFadeout);

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// This method allows an element's fade to trigger when another element undergoes a certain event as defined by the FadeTrigger.
        /// </summary>
        /// <param name="eventProcElements">The list of elements whose events will proc the recieving element's fade.</param>
        /// <param name="eventRecieveElements">The recieving elements.</param>
        /// <param name="state"></param>
        public static void LinkOnewayFadeComponentTriggers(IUIElement[] eventProcElements, IUIElement[] eventRecieveElements, FadeTrigger state) {
            switch (state) {
                case FadeTrigger.EntryExit:
                    foreach (var pElement in eventProcElements){
                        if (!(pElement is IUIInteractiveElement)) {
                            throw new Exception("Unable to link interactive element fade triggers; the event proc element is not interactive.");
                        }
                        foreach (var eElement in eventRecieveElements){
                            ((IUIInteractiveElement)pElement).OnMouseEntry.Add(eElement.GetComponent<FadeComponent>().ForceFadein);
                            ((IUIInteractiveElement)pElement).OnMouseExit.Add(eElement.GetComponent<FadeComponent>().ForceFadeout);
                        }
                    }

                    break;

                default:
                    break;
            }
        }

        #endregion

        public event FadeStateChange FadeStateChangeDispatcher;
    }
}