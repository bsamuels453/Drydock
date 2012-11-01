#region

using System.Diagnostics;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class ObjectBuffer : BaseBufferObject<VertexPositionNormalTexture>{
        //key=identifier
        readonly int[] _indicies;
        readonly int _indiciesPerObject;
        readonly int _maxObjects;
        readonly ObjectData[] _objectData;
        readonly VertexPositionNormalTexture[] _verticies;
        readonly int _verticiesPerObject;

        public ObjectBuffer(int maxObjects, int primitivesPerObject, int verticiesPerObject, int indiciesPerObject, string textureName) :
            base(indiciesPerObject * maxObjects, verticiesPerObject * maxObjects, primitivesPerObject * maxObjects, PrimitiveType.TriangleList) {
            BufferRasterizer = new RasterizerState{CullMode = CullMode.None};

            var texture = Singleton.ContentManager.Load<Texture2D>(textureName);
            BufferEffect = Singleton.ContentManager.Load<Effect>("StandardEffect").Clone();
            BufferEffect.Parameters["Projection"].SetValue(Singleton.ProjectionMatrix);
            BufferEffect.Parameters["World"].SetValue(Matrix.Identity);
            BufferEffect.Parameters["Texture"].SetValue(texture);

            _objectData = new ObjectData[maxObjects];
            _indicies = new int[maxObjects*indiciesPerObject];
            _verticies = new VertexPositionNormalTexture[maxObjects*verticiesPerObject];

            _indiciesPerObject = indiciesPerObject;
            _verticiesPerObject = verticiesPerObject;
            _maxObjects = maxObjects;
        }


        public Vector3 DiffuseDirection{
            set { BufferEffect.Parameters["DiffuseLightDirection"].SetValue(value); }
        }

        public float AmbientIntensity{
            set { BufferEffect.Parameters["AmbientIntensity"].SetValue(value); }
        }

        public void AddObject(object identifier, int[] indicies, VertexPositionNormalTexture[] verticies){
            Debug.Assert(indicies.Length == _indiciesPerObject);
            Debug.Assert(verticies.Length == _verticiesPerObject);

            int index = -1;
            for (int i = 0; i < _maxObjects; i++){
                if (_objectData[i] == null){
                    //add buffer offset to the indice list
                    for (int indice = 0; indice < indicies.Length; indice++){
                        indicies[indice] += i * _verticiesPerObject;
                    }

                    _objectData[i] = new ObjectData(identifier, i, indicies, verticies);
                    index = i;
                    break;
                }
            }
            Debug.Assert(index != -1, "not enough space in object buffer to add new object");

            indicies.CopyTo(_indicies, index*_indiciesPerObject);
            verticies.CopyTo(_verticies, index*_verticiesPerObject);

            base.Indexbuffer.SetData(_indicies);
            base.Vertexbuffer.SetData(_verticies);
        }

        public void RemoveObject(object identifier){
            for (int i = 0; i < _maxObjects; i++){
                if (_objectData[i].Identifier == identifier){
                    int index = _objectData[i].ObjectOffset;

                    _objectData[i] = null;
                    var emptyIndicies = new int[_indiciesPerObject];
                    emptyIndicies.CopyTo(_indicies, index*_indiciesPerObject);
                    base.Indexbuffer.SetData(_indicies);
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

        #region Nested type: ObjectData

        class ObjectData {
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