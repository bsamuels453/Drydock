#region

using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal abstract class BufferObject<T> : IDrawableBuffer{
        public readonly IndexBuffer Indexbuffer;
        public readonly VertexBuffer Vertexbuffer;
        readonly int _numIndicies;
        readonly int _numPrimitives;
        readonly PrimitiveType _primitiveType;

        protected Effect BufferEffect;
        protected RasterizerState BufferRasterizer;
        public bool IsEnabled; //this shouldnt be a field xx
        RenderPanel _bufferRenderTarget;

        protected BufferObject(int numIndicies, int numVerticies, int numPrimitives, PrimitiveType primitiveType){
            IsEnabled = true;
            _numPrimitives = numPrimitives;
            _numIndicies = numIndicies;
            _primitiveType = primitiveType;

            Indexbuffer = new IndexBuffer(
                Singleton.Device,
                typeof (int),
                numIndicies,
                BufferUsage.None
                );

            Vertexbuffer = new VertexBuffer(
                Singleton.Device,
                typeof (T),
                numVerticies,
                BufferUsage.None
                );

            _bufferRenderTarget = RenderPanel.Add(this);
        }

        #region IDrawableBuffer Members

        public void Draw(Matrix viewMatrix){
            if (IsEnabled){
                UpdateViewMatrix(viewMatrix);
                Singleton.Device.RasterizerState = BufferRasterizer;

                foreach (EffectPass pass in BufferEffect.CurrentTechnique.Passes){
                    pass.Apply();
                    Singleton.Device.Indices = Indexbuffer;
                    Singleton.Device.SetVertexBuffer(Vertexbuffer);
                    Singleton.Device.DrawIndexedPrimitives(_primitiveType, 0, 0, _numIndicies, 0, _numPrimitives);
                }
                Singleton.Device.SetVertexBuffer(null);
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


        ~BufferObject(){
            //Dispose();
        }
    }
}