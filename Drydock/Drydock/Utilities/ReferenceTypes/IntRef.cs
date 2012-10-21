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

    internal class IntRefLambda{
        #region Delegates

        public delegate int IntLambdaDelegate(int input);

        #endregion

        readonly IntRef _reference;
        readonly IntLambdaDelegate _function;

        public IntRefLambda(IntRef reference, IntLambdaDelegate functionToApply){
            _reference = reference;    
            _function = functionToApply;
        }

        public int Value{
            get { return _function.Invoke(_reference.Value); }
        }
    }
}