using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Render {
    static class AuxBufferManager {
        private static readonly IndexBuffer[] _indexbuffers;
        private static readonly VertexBuffer[] _vertexbuffers;
        private static readonly int[] _numTriangles;
        private static readonly Texture2D[] _textures;
        private static readonly DepthStencilState _depthStencil;
        private static readonly bool[] _isFrameOccupied;
        private static readonly Effect _effect;
        private static GraphicsDevice _device;
        private const int _maxVbos = 5;
        //private static RasterizerState _wireframeRasterizer;
        //private static BasicEffect _wireframeEffect;

        public static void Init(){
        }

        static AuxBufferManager(){
            _indexbuffers = new IndexBuffer[_maxVbos];
            _vertexbuffers = new VertexBuffer[_maxVbos];
            _textures = new Texture2D[_maxVbos];
            _isFrameOccupied = new bool[_maxVbos];
            _numTriangles = new int[_maxVbos];
            _depthStencil = new DepthStencilState();
            _depthStencil.DepthBufferEnable = true;
            _depthStencil.DepthBufferWriteEnable = true;
            _effect = Singleton.ContentManager.Load<Effect>("StandardEffect");

            for (int i = 0; i < _maxVbos; i++){
                _isFrameOccupied[i] = false;
            }

        }

        public static void Init(GraphicsDevice device, Matrix projectionMatrix){
            _device = device;
            _effect.Parameters["Projection"].SetValue(projectionMatrix);
            _effect.Parameters["World"].SetValue(Matrix.Identity);
            _effect.Parameters["AmbientColor"].SetValue(new Vector4(0f, 0f, 0f, 1f));
            _effect.Parameters["AmbientIntensity"].SetValue(0.1f);
            _effect.Parameters["AmbientColor"].SetValue(new Vector4(1, 1, 1, 1));
        }

        public static int AddVbo(int numVerts, int numIndicies, int numTriangles, string textureName){
            int index;
            for (index = 0; index < _maxVbos; index++) {
                if (!_isFrameOccupied[index]) {
                    break;
                }
            }
            if (index == _maxVbos){
                throw new Exception("Aux vbo storage is full");
            }

            _isFrameOccupied[index] = true;
            _vertexbuffers[index] = new VertexBuffer(
                _device,
                typeof(VertexPositionNormalTexture),
                numVerts,
                BufferUsage.None
                );

            _indexbuffers[index] = new IndexBuffer(
                _device,
                typeof(int),
                numIndicies,
                BufferUsage.None
                );
            _numTriangles[index] = numTriangles;
            _textures[index] = Singleton.ContentManager.Load<Texture2D>(textureName);

            return index;
        }

        public static void RemoveVbo(int id){
            _isFrameOccupied[id] = false;
        }

        public static void SetVerticies(int id, VertexPositionNormalTexture[] verts){
            _vertexbuffers[id].SetData(verts);
        }

        public static void SetIndicies(int id, int[] indicies){
            _indexbuffers[id].SetData(indicies);
        }

        public static void Draw(Matrix view) {
            _device.DepthStencilState = _depthStencil;
            _effect.Parameters["View"].SetValue(view);
            for (int i = 0; i < _maxVbos; i++){
                if (_isFrameOccupied[i]){
                    _effect.Parameters["Texture"].SetValue(_textures[i]);
                    foreach (EffectPass pass in _effect.CurrentTechnique.Passes) {
                        pass.Apply();
                        //go through each known vbo/ibo pair and draw them
                        _device.Indices = _indexbuffers[i];
                        _device.SetVertexBuffer(_vertexbuffers[i]);
                        _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _numTriangles[i] * 3, 0, _numTriangles[i]);
                    }
                }
            }
        }
    }
}
