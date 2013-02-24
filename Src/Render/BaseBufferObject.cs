#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal abstract class BaseBufferObject<T> : IDrawableBuffer{
        protected readonly IndexBuffer Indexbuffer;
        protected readonly VertexBuffer Vertexbuffer;
        readonly int _numIndicies;
        readonly int _numPrimitives;
        readonly PrimitiveType _primitiveType;

        protected Effect BufferEffect;
        protected RasterizerState BufferRasterizer;
        public bool Enabled; //this shouldnt be a field xx

        protected BaseBufferObject(int numIndicies, int numVerticies, int numPrimitives, PrimitiveType primitiveType){
            Enabled = true;
            _numPrimitives = numPrimitives;
            _numIndicies = numIndicies;
            _primitiveType = primitiveType;

            Indexbuffer = new IndexBuffer(
                Gbl.Device,
                typeof (int),
                numIndicies,
                BufferUsage.None
                );

            Vertexbuffer = new VertexBuffer(
                Gbl.Device,
                typeof (T),
                numVerticies,
                BufferUsage.None
                );
        }

        #region IDrawableBuffer Members

        public void Draw(Matrix viewMatrix){
            if (Enabled){
                UpdateViewMatrix(viewMatrix);
                Gbl.Device.RasterizerState = BufferRasterizer;

                foreach (EffectPass pass in BufferEffect.CurrentTechnique.Passes){
                    pass.Apply();
                    Gbl.Device.Indices = Indexbuffer;
                    Gbl.Device.SetVertexBuffer(Vertexbuffer);
                    Gbl.Device.DrawIndexedPrimitives(_primitiveType, 0, 0, _numIndicies, 0, _numPrimitives);
                }
                Gbl.Device.SetVertexBuffer(null);
            }
        }

        #endregion

        /*public void Dispose(){
            if (!_isDisposed){
                _bufferRenderTarget.Remove(this);
                _bufferRenderTarget = null;
                _isDisposed = true;
            }
        }*/

        public void UpdateViewMatrix(Matrix viewMatrix){
            BufferEffect.Parameters["View"].SetValue(viewMatrix);
        }


        ~BaseBufferObject(){
            //Dispose();
        }
    }
}