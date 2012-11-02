#region

using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class ObjectBuffer : StandardEffect{
        //key=identifier
        readonly int[] _indicies;
        readonly int _indiciesPerObject;
        readonly int _maxObjects;
        readonly ObjectData[] _objectData;
        readonly VertexPositionNormalTexture[] _verticies;
        readonly int _verticiesPerObject;

        public bool UpdateBufferManually;

        public ObjectBuffer(int maxObjects, int primitivesPerObject, int verticiesPerObject, int indiciesPerObject, string textureName) :
            base(indiciesPerObject*maxObjects, verticiesPerObject*maxObjects, primitivesPerObject*maxObjects, textureName){
            BufferRasterizer = new RasterizerState{CullMode = CullMode.None};

            _objectData = new ObjectData[maxObjects];
            _indicies = new int[maxObjects*indiciesPerObject];
            _verticies = new VertexPositionNormalTexture[maxObjects*verticiesPerObject];

            _indiciesPerObject = indiciesPerObject;
            _verticiesPerObject = verticiesPerObject;
            _maxObjects = maxObjects;
            UpdateBufferManually = false;
        }

        public void UpdateBuffers(){
            Debug.Assert(UpdateBufferManually, "cannot update a buffer that's set to automatic updating");
            base.Indexbuffer.SetData(_indicies);
            base.Vertexbuffer.SetData(_verticies);
        }

        public void AddObject(object identifier, int[] indicies, VertexPositionNormalTexture[] verticies){
            Debug.Assert(indicies.Length == _indiciesPerObject);
            Debug.Assert(verticies.Length == _verticiesPerObject);

            int index = -1;
            for (int i = 0; i < _maxObjects; i++){
                if (_objectData[i] == null){
                    //add buffer offset to the indice list
                    for (int indice = 0; indice < indicies.Length; indice++){
                        indicies[indice] += i*_verticiesPerObject;
                    }

                    _objectData[i] = new ObjectData(identifier, i, indicies, verticies);
                    index = i;
                    break;
                }
            }
            Debug.Assert(index != -1, "not enough space in object buffer to add new object");

            indicies.CopyTo(_indicies, index*_indiciesPerObject);
            verticies.CopyTo(_verticies, index*_verticiesPerObject);
            if (!UpdateBufferManually){
                base.Indexbuffer.SetData(_indicies);
                base.Vertexbuffer.SetData(_verticies);
            }
        }

        public void RemoveObject(object identifier){
            for (int i = 0; i < _maxObjects; i++){
                if (_objectData[i].Identifier == identifier){
                    int index = _objectData[i].ObjectOffset;

                    _objectData[i] = null;
                    var emptyIndicies = new int[_indiciesPerObject];
                    emptyIndicies.CopyTo(_indicies, index*_indiciesPerObject);
                    if (!UpdateBufferManually){
                        base.Indexbuffer.SetData(_indicies);
                    }
                }
            }
        }

        public void ClearObjects(){
            for (int i = 0; i < _maxObjects; i++){
                _objectData[i] = null;
                _indicies[i] = 0;
            }
            base.Indexbuffer.SetData(_indicies);
        }

        /// <summary>
        ///   really cool method that will take another objectbuffer and absorb its objects into this objectbuffer. also clears the other buffer afterwards.
        /// </summary>
        public void AbsorbBuffer(ObjectBuffer buffer){
            bool buffUpdateState = UpdateBufferManually;
            UpdateBufferManually = true; //temporary for this heavy copy algo

            foreach (var objectData in buffer._objectData){
                if (objectData != null){
                    int offset = objectData.ObjectOffset*_verticiesPerObject;
                    var indicies = from index in objectData.Indicies
                                   select index - offset;

                    AddObject(objectData.Identifier, indicies.ToArray(), objectData.Verticies);
                }
            }
            UpdateBuffers();
            UpdateBufferManually = buffUpdateState;
            buffer.ClearObjects();
        }

        #region Nested type: ObjectData

        class ObjectData{
            // ReSharper disable MemberCanBePrivate.Local
            public readonly object Identifier;
            public readonly int[] Indicies;
            public readonly int ObjectOffset;
            public readonly VertexPositionNormalTexture[] Verticies;
            // ReSharper restore MemberCanBePrivate.Local

            public ObjectData(object identifier, int objectOffset, int[] indicies, VertexPositionNormalTexture[] verticies){
                Identifier = identifier;
                ObjectOffset = objectOffset;
                Indicies = indicies;
                Verticies = verticies;
            }
        }

        #endregion
    }
}