namespace Drydock.Utilities.ReferenceTypes{
    internal delegate void IntModCallback(IntRef caller, int oldValue, int newValue);

    internal class IntRef{
        int _value;

        public int Value{
            get { return _value; }
            set{
                int oldValue = _value;
                _value = value;
                RefModCallback.Invoke(this, oldValue, _value);
            }
        }

        public event IntModCallback RefModCallback;
    }
}