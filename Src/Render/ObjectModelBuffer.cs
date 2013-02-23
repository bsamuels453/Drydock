using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Render{
    /// <summary>
    /// this is nearly identical to ObjectBuffer with the exception that it's for handling non-dynamic content (models)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ObjectModelBuffer<T> : IDrawableBuffer where T : IEquatable<T>{
        //key=identifier
        readonly bool[] _isSlotOccupied;
        readonly int _maxObjects;
        readonly List<ObjectData> _objectData; 

        public ObjectModelBuffer(int maxObjects){
            _objectData = new List<ObjectData>();
            _maxObjects = maxObjects;
            _isSlotOccupied = new bool[maxObjects];
        }

        public void AddObject(IEquatable<T> identifier, Model model){
            int index = -1;
            for (int i = 0; i < _maxObjects; i++){
                if (_isSlotOccupied[i] == false){
                    _objectData.Add(new ObjectData(identifier, i, model));
                    _isSlotOccupied[i] = true;
                    index = i;
                    break;
                }
            }
            Debug.Assert(index != -1, "not enough space in object buffer to add new object");
        }

        public void RemoveObject(IEquatable<T> identifier){
            ObjectData objectToRemove = (
                                            from obj in _objectData
                                            where obj.Identifier.Equals(identifier)
                                            select obj
                                        ).FirstOrDefault();

            if (objectToRemove == null)
                return;

            _isSlotOccupied[objectToRemove.ObjectOffset] = false;
            _objectData.Remove(objectToRemove);
        }

        public void ClearObjects(){
            _objectData.Clear();
            for (int i = 0; i < _maxObjects; i++){
                _isSlotOccupied[i] = false;
            }
        }

        public bool EnableObject(IEquatable<T> identifier){
            ObjectData objToEnable = null;
            foreach (var obj in _objectData){
                if (obj.Identifier.Equals(identifier)){
                    objToEnable = obj;
                }
            }
            if (objToEnable == null)
                return false;

            objToEnable.Enabled = true;
            return true;
        }

        public bool DisableObject(IEquatable<T> identifier){
            ObjectData objToDisable = null;
            foreach (var obj in _objectData){
                if (obj.Identifier.Equals(identifier)){
                    objToDisable = obj;
                }
            }
            if (objToDisable == null)
                return false;

            objToDisable.Enabled = false;
            return true;
        }

        /// <summary>
        ///   really cool method that will take another objectbuffer and absorb its objects into this objectbuffer. also clears the other buffer afterwards.
        /// </summary>
        public void AbsorbBuffer(ObjectModelBuffer<T> buffer){
            foreach (var objectData in buffer._objectData){
                bool isDuplicate = false;
                foreach (var data in _objectData){
                    if (data.Identifier.Equals(objectData.Identifier))
                        isDuplicate = true;
                }
                if (isDuplicate)
                    continue;
                AddObject(objectData.Identifier, objectData.Model);
            }
            buffer.ClearObjects();
        }

        #region Nested type: ObjectData

        class ObjectData{
            // ReSharper disable MemberCanBePrivate.Local
            public readonly IEquatable<T> Identifier;
            public readonly Model Model;
            public readonly int ObjectOffset;
            public bool Enabled;
            // ReSharper restore MemberCanBePrivate.Local

            public ObjectData(IEquatable<T> identifier, int objectOffset, Model model){
                Enabled = true;
                Identifier = identifier;
                ObjectOffset = objectOffset;
                Model = model;
            }
        }

        #endregion

        public void Draw(Matrix viewMatrix){
            foreach (var obj in _objectData){
                foreach (var mesh in obj.Model.Meshes){
                    foreach (var effect in mesh.Effects){
                        effect.Parameters["View"].SetValue(viewMatrix);
                    }
                    mesh.Draw();
                }
            }
        }
    }
}


