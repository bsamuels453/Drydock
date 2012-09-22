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
        public bool IsEnabled;
        RenderPanel _bufferRenderTarget;
        //bool _isDisposed;

        protected BufferObject(int numIndicies, int numVerticies, int numPrimitives){
            IsEnabled = true;
            _numPrimitives = numPrimitives;
            _numIndicies = numIndicies;

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
            //_isDisposed = false;
        }

        protected abstract Effect BufferEffect { get; }
        protected abstract RasterizerState BufferRasterizer { get; }

        #region IDrawableBuffer Members

        public void Draw(Matrix viewMatrix){
            if (IsEnabled){
                UpdateEffectParams(viewMatrix);
                Singleton.Device.RasterizerState = BufferRasterizer;

                foreach (EffectPass pass in BufferEffect.CurrentTechnique.Passes){
                    pass.Apply();
                    Singleton.Device.Indices = Indexbuffer;
                    Singleton.Device.SetVertexBuffer(Vertexbuffer);
                    Singleton.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _numIndicies, 0, _numPrimitives);
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

        protected abstract void UpdateEffectParams(Matrix viewMatrix);

        ~BufferObject(){
            //Dispose();
        }
    }
}