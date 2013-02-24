#region

using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class ShipGeometryBuffer : StandardEffect{
        public ShipGeometryBuffer(int numIndicies, int numVerticies, int numPrimitives, string settingsFileName, CullMode cullMode = CullMode.None)
            : base(numIndicies, numVerticies, numPrimitives, settingsFileName){
            BufferRasterizer = new RasterizerState();
            BufferRasterizer.CullMode = cullMode;
        }

        public new IndexBuffer Indexbuffer{
            get { return base.Indexbuffer; }
        }

        public new VertexBuffer Vertexbuffer{
            get { return base.Vertexbuffer; }
        }
    }
}