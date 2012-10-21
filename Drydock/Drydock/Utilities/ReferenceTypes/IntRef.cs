namespace Drydock.Utilities.ReferenceTypes{
    delegate void IntModCallback(IntRef caller, int oldValue, int newValue);
    internal class IntRef{
        int _value;

        public event IntModCallback RefModCallback;

        public int Value{
            get { return _value; }
            set {
                int oldValue = _value;
                _value = value;
                RefModCallback.Invoke(this, oldValue, _value);
            }
        }
    }
}