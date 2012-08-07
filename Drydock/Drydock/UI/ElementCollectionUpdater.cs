using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drydock.UI {
    static class ElementCollectionUpdater {
        private static readonly List<UIElementCollection> _collections;

        static ElementCollectionUpdater(){
            _collections = new List<UIElementCollection>();
        }

        public static void Add(UIElementCollection coll){
            _collections.Add(coll);
        }

        public static void Remove(UIElementCollection coll){
            _collections.Remove(coll);
        }

        public static void Update(){
            for(int i=0; i<_collections.Count; i++){
                if (_collections[i] != null) {
                    _collections[i].Update();
                }
                else{
                    _collections.RemoveAt(i);
                }
            }
        }

    }
}
