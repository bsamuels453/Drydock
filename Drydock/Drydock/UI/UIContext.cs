#region

using System.Collections.Generic;

#endregion

namespace Drydock.UI{
    /// <summary>
    ///   this class serves as a super-container for all UIElementCollections. Its purpose is to dispatch updates to said collections, and to provide global UI modification methods.
    /// </summary>
    internal static class UIContext{
        static readonly List<UIElementCollection> _collections;

        static UIContext(){
            _collections = new List<UIElementCollection>();
        }

        public static void Add(UIElementCollection coll){
            _collections.Add(coll);
        }

        public static void Remove(UIElementCollection coll){
            _collections.Remove(coll);
        }

        public static void Clear(){
            _collections.Clear();
        }

        public static void Update(){
            for (int i = 0; i < _collections.Count; i++){
                if (_collections[i] != null){
                    _collections[i].Update();
                }
                else{
                    _collections.RemoveAt(i);
                }
            }
        }

        #region collection modification methods

        public static void EnableComponents<TComponent>(){
            //propogate changes to children
            foreach (var collection in _collections){
                collection.EnableComponents<TComponent>();
            }
        }

        public static void DisableComponents<TComponent>(){
            foreach (var collection in _collections){
                collection.DisableComponents<TComponent>();
            }
        }

        public static void SelectAllElements(){
            foreach (var collection in _collections){
                collection.SelectAllElements();
            }
        }

        public static void DeselectAllElements(){
            foreach (var collection in _collections){
                collection.DeselectAllElements();
            }
        }

        public static void FadeInAllElements(){
            foreach (var collection in _collections){
                collection.FadeInAllElements();
            }
        }

        public static void FadeOutAllElements(){
            foreach (var collection in _collections){
                collection.FadeOutAllElements();
            }
        }

        #endregion
    }
}