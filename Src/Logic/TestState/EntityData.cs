#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Drydock.Logic.TestState{
    internal abstract class EntityDataContainer{
        readonly List<ValueReference> _valueReferences;

        protected EntityDataContainer(){
            _valueReferences = new List<ValueReference>();
        }

        public RefAccessor<T> GetReferenceAccessor<T>(string identifier){
            foreach (var reference in _valueReferences.Where(reference => reference.Identifier == identifier))
                return new RefAccessor<T>(reference);

            //need to create the accessor
            _valueReferences.Add(new ValueReference(identifier));
            return new RefAccessor<T>(_valueReferences.Last());
        }
    }

    internal class ValueReference{
        #region Delegates

        public delegate void ClampValChange(ref object obj);

        public delegate void RecieveValueChange(object obj);

        #endregion

        public string Identifier;

        object _value;

        public ValueReference(object initValue, string identifier){
            _value = initValue;
            Identifier = identifier;
        }

        public ValueReference(string identifier){
            _value = null;
            Identifier = identifier;
        }

        public object Value{
            get { return _value; }
            set{
                DispatchValueClamp.Invoke(ref value);
                _value = value;
                DispatchValueChange.Invoke(value);
            }
        }

        public event RecieveValueChange DispatchValueChange;
        public event ClampValChange DispatchValueClamp;
    }

    //refAccessor makes it so we can use the ambiguious "object" in the valueReference class, allowing all of the valueReferences to be stored in one list in the entity data container
    internal class RefAccessor<T>{
        public readonly ValueReference Reference;

        public RefAccessor(ValueReference reference){
            Reference = reference;
        }

        public T Value{
            get { return (T) Reference.Value; } //FIND OUT HOW BAD THIS CASTING OVERHEAD IS
            set { Reference.Value = value; }
        }
    }
}