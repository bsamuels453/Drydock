using System;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI.Components{
    /// <summary>
    /// allows a UI element to be faded in and out. Required element to be IUIInteractiveComponent for certain settings.
    /// </summary>
    internal class FadeComponent : IUIElementComponent{
        //todo: some kind of synchronize function to get multiple ui elements' fade components to trigger
        //AddTriggerElement
        #region FadeState enum

        public enum FadeState{
            Visible,
            Faded
        }

        #endregion

        #region FadeTriggers enum

        public enum FadeTriggers{
            EntryExit,
            None
        }

        #endregion

        private readonly FadeState _defaultState;

        private readonly float _fadeDuration;
        private readonly FadeTriggers _fadeTrigger;
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
                if (_isEnabled){ //reset the timer
                    _prevUpdateTimeIndex = DateTime.Now.Ticks;
                }
                else{ //cancel currently running fade operation, if any
                    _isFadingOut = false;
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
        public FadeComponent(FadeState defaultState, FadeTriggers trigger = FadeTriggers.None, float fadeoutOpacity = .2f, float fadeDuration = 250){
            _fadeoutOpacity = fadeoutOpacity;
            _fadeDuration = fadeDuration*10000; //10k ticks in a millisecond
            _isInTransition = false;
            _isFadingOut = false;
            _prevUpdateTimeIndex = DateTime.Now.Ticks;
            _defaultState = defaultState;
            _fadeTrigger = trigger;
        }

        private void ComponentCtor(){
            if (_defaultState == FadeState.Faded){
                _owner.Sprite.Opacity = _fadeoutOpacity;
            }
            switch (_fadeTrigger){
                case FadeTriggers.EntryExit:

                    bool isParentInteractive = false;
                    foreach (Type type in _owner.GetType().Assembly.GetTypes()){
                        if (type == typeof (IUIInteractiveElement)){
                            isParentInteractive = true;
                        }
                    }
                    if (!isParentInteractive){
                        throw new Exception("Invalid fade trigger: Unable to set an interactive trigger to a non-interactive element.");
                    }

                    ((IUIInteractiveElement) _owner).OnMouseEntry.Add(ForceFadein);
                    ((IUIInteractiveElement) _owner).OnMouseExit.Add(ForceFadeout);
                    break;

                case FadeTriggers.None:
                    break;
            }
        }

        #endregion

        #region IUIElementComponent Members

        public void Update(){
            if (_isInTransition){
                long timeSinceLastUpdate = DateTime.Now.Ticks - _prevUpdateTimeIndex;
                float step = timeSinceLastUpdate/_fadeDuration;
                if (_isFadingOut){
                    _owner.Sprite.Opacity -= step;
                    if (_owner.Sprite.Opacity < _fadeoutOpacity){
                        _owner.Sprite.Opacity = _fadeoutOpacity;
                        _isInTransition = false;
                    }
                }
                else{
                    _owner.Sprite.Opacity += step;
                    if (_owner.Sprite.Opacity > 1){
                        _owner.Sprite.Opacity = 1;
                        _isInTransition = false;
                    }
                }
            }
            _prevUpdateTimeIndex = DateTime.Now.Ticks;
        }

        #endregion

        #region modification methods

        public bool ForceFadeout(MouseState state){
            _isInTransition = true;
            _isFadingOut = true;
            return false;
        }

        public bool ForceFadein(MouseState state){
            _isInTransition = true;
            _isFadingOut = false;
            return false;
        }

        #endregion
    }
}