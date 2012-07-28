namespace Drydock.UI.Button {
    class FadeComponent : IButtonComponent{
        private Button _owner;
        private bool _isEnabled;
        private float _fadeoutOpacity;
        private float  _fadeoutDuration;
        private float _currentOpacity;
        private bool _isInTransition;
        private bool _isFadingOut;

        #region properties
        public Button Owner{
            set { 
                _owner = value;
            }
        }

        public bool IsEnabled{
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }
        #endregion

        #region ctor
        public FadeComponent(float fadeoutOpacity = 0, float fadeoutDuration = 1){
            _fadeoutOpacity = fadeoutOpacity;
            _fadeoutDuration = fadeoutDuration;
            _isInTransition = false;
            _isFadingOut = false;
        }

        #endregion

        public void Update(){
            //  
        }
    }
}
