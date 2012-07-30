using System;
using Microsoft.Xna.Framework.Input;

namespace Drydock.UI.Button{
    internal class FadeComponent : IUIElementComponent{
        private readonly float _fadeDuration;
        private readonly float _fadeoutOpacity;
        private bool _isEnabled;
        private bool _isFadingOut;
        private bool _isInTransition;
        private Button _owner;
        private long _prevUpdateTimeIndex;

        #region properties

        public Button Owner{
            set { _owner = value; }
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
        /// <param name="fadeoutOpacity">opacity level to fade out to. range 0-1f</param>
        /// <param name="fadeDuration">the time it takes for sprite to fade out in milliseconds</param>
        public FadeComponent(float fadeoutOpacity = .2f, float fadeDuration = 250){
            _fadeoutOpacity = fadeoutOpacity;
            _fadeDuration = fadeDuration*10000; //10k ticks in a millisecond
            _isInTransition = false;
            _isFadingOut = false;
            _prevUpdateTimeIndex = DateTime.Now.Ticks;
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

        public void ForceFadein(MouseState state){
            _isInTransition = true;
            _isFadingOut = false;
        }
        #endregion
    }
}