#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class WireframeBuffer : BaseBufferObject<VertexPositionColor>{
        public WireframeBuffer(int numIndicies, int numVerticies, int numPrimitives) : base(numIndicies, numVerticies, numPrimitives, PrimitiveType.LineList){
            BufferRasterizer = new RasterizerState();
            BufferRasterizer.CullMode = CullMode.None;
            BufferEffect = Gbl.LoadContent<Effect>("Shader_WireframeEffect").Clone();

            BufferEffect.Parameters["Projection"].SetValue(Gbl.ProjectionMatrix);
            BufferEffect.Parameters["World"].SetValue(Matrix.Identity);
        }

        public new IndexBuffer Indexbuffer{
            get { return base.Indexbuffer; }
        }

        public new VertexBuffer Vertexbuffer{
            get { return base.Vertexbuffer; }
        }

        public void Dispose(){
            base.Indexbuffer.Dispose();
            base.Vertexbuffer.Dispose();
            Enabled = false;
        }

        public void SetColor(Vector3 color){
            BufferEffect.Parameters["Color"].SetValue(color);
        }

        public void SetAlpha(float color) {
            BufferEffect.Parameters["Alpha"].SetValue(color);
        }
    }

    /*internal struct Vertex3 : IVertexType{
        public Vector3 Pos;

        public VertexDeclaration VertexDeclaration{
            get { 
                return new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0));
            }
        }

        public Vertex3(float x, float y, float z){
            Pos = new Vector3(x, y, z);
        }
    }*/
}