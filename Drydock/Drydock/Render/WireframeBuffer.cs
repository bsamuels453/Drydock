using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Render {
    class WireframeBuffer : BufferObject<VertexPositionColor> {
        public WireframeBuffer(int numIndicies, int numVerticies, int numPrimitives) : base(numIndicies, numVerticies, numPrimitives, PrimitiveType.LineList){
            BufferRasterizer = new RasterizerState();
            BufferRasterizer.CullMode = CullMode.None;
            BufferEffect = Singleton.ContentManager.Load<Effect>("WireframeEffect").Clone();

            BufferEffect.Parameters["Projection"].SetValue(Singleton.ProjectionMatrix);
            BufferEffect.Parameters["World"].SetValue(Matrix.Identity);
            
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
