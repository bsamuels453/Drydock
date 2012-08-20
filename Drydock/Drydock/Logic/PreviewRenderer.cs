#region

using System;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Drydock.UI;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic{
    internal class PreviewRenderer : CanReceiveInputEvents{
        private const int _meshVertexWidth = 64; //this is in primitives
        private readonly BezierCurveCollection _backCurves;
        private readonly ShipGeometryBuffer _geometryBuffer;
        private readonly int[] _indicies;
        private readonly Vector3[,] _mesh;
        private readonly RenderPanel _renderTarget;
        private readonly BezierCurveCollection _sideCurves;
        private readonly BezierCurveCollection _topCurves;
        private readonly VertexPositionNormalTexture[] _verticies;
        private float _cameraDistance;
        private float _cameraPhi;
        private float _cameraTheta;

        public PreviewRenderer(BezierCurveCollection sideCurves, BezierCurveCollection topCurves, BezierCurveCollection backCurves){
            _verticies = new VertexPositionNormalTexture[_meshVertexWidth*_meshVertexWidth*4];
            _indicies = new int[_meshVertexWidth*_meshVertexWidth*6]; // 6 indicies make up 2 triangles, can make this into triangle strip in future if have optimization boner
            _renderTarget = new RenderPanel(
                ScreenData.GetScreenValueX(0.5f),
                ScreenData.GetScreenValueY(0.5f),
                ScreenData.GetScreenValueX(0.5f),
                ScreenData.GetScreenValueY(0.5f),
                DepthLevel.Medium
                );
            RenderPanel.SetRenderPanel(_renderTarget);

            _cameraPhi = 0.32f;
            _cameraTheta = 0.63f;
            _cameraDistance = 300;
            InputEventDispatcher.EventSubscribers.Add((float) DepthLevel.Medium/10f, this);

            _geometryBuffer = new ShipGeometryBuffer(_indicies.Count(), _verticies.Count(), (_meshVertexWidth)*(_meshVertexWidth)*2, "whiteborder");

            _mesh = new Vector3[_meshVertexWidth,_meshVertexWidth];

            _sideCurves = sideCurves;
            _topCurves = topCurves;
            _backCurves = backCurves;

            //construct indice list
            //remember the clockwise-fu
            //+1-----+2
            //|     /
            //|   /    
            //| /     
            //+0
            //       +2
            //      / |
            //    /   | 
            //  /     |
            //+0-----+3
            int curVertex = 0;
            for (int i = 0; i < _indicies.Count(); i += 6){
                _indicies[i] = curVertex;
                _indicies[i + 1] = curVertex + 1;
                _indicies[i + 2] = curVertex + 2;

                _indicies[i + 3] = curVertex;
                _indicies[i + 4] = curVertex + 2;
                _indicies[i + 5] = curVertex + 3;

                curVertex += 4;
            }

            for (int i = 0; i < _verticies.Count(); i++){
                _verticies[i] = new VertexPositionNormalTexture();
            }
            int index = 0;
            for (int x = 0; x < _meshVertexWidth - 1; x++){
                for (int z = 0; z < _meshVertexWidth - 1; z++){
                    _verticies[index].TextureCoordinate = new Vector2(0, 0);
                    _verticies[index + 1].TextureCoordinate = new Vector2(0, 1);
                    _verticies[index + 2].TextureCoordinate = new Vector2(1, 1);
                    _verticies[index + 3].TextureCoordinate = new Vector2(1, 0);
                    index += 4;
                }
            }
            _geometryBuffer.Indexbuffer.SetData(_indicies);
            Update();
        }

        public void Update(){
            _topCurves.GetParameterizedPoint(0, true);

            var topPts = new Vector2[_meshVertexWidth];
            for (double i = 0; i < _meshVertexWidth; i++){
                double t = i/((_meshVertexWidth - 1)*2);
                topPts[(int) i] = _topCurves.GetParameterizedPoint(t);
            }


            topPts = topPts.Reverse().ToArray();
            float reflectionPoint = topPts[0].Y;
            var yDelta = new float[_meshVertexWidth];

            for (int i = 0; i < _meshVertexWidth; i++){
                yDelta[i] = Math.Abs(topPts[i].Y - reflectionPoint)*2/_meshVertexWidth;
            }


            _sideCurves.GetParameterizedPoint(0, true); //this refreshes internal fields
            //orient controllers correctly for the bezierintersect
            var li = _sideCurves.Select(bezierCurve => new BezierInfo(
                                                           _sideCurves.Normalize(bezierCurve.CenterHandlePos),
                                                           _sideCurves.Normalize(bezierCurve.PrevHandlePos),
                                                           _sideCurves.Normalize(bezierCurve.NextHandlePos))).ToList();


            var sideIntersectGenerator = new BezierIntersect(li);

            var sideIntersectionCache = new float[_meshVertexWidth];

            for (int x = 0; x < _meshVertexWidth; x++){
                sideIntersectionCache[x] = sideIntersectGenerator.GetIntersectionFromX(topPts[x].X).Y;
            }

            var maxY = (float) _backCurves.NormalizeY(_backCurves.MaxY);
            var backCurvesMaxWidth = (float) (_backCurves.NormalizeX(_backCurves[_backCurves.Count - 1].CenterHandlePos.X) - _backCurves.NormalizeX(_backCurves[0].CenterHandlePos.X));

            for (int x = 0; x < _meshVertexWidth; x++){
                float scaleX = Math.Abs((reflectionPoint - topPts[x].Y)*2)/backCurvesMaxWidth;
                float scaleY = sideIntersectionCache[x]/maxY;

                var bezierInfo = _backCurves.GetControllerInfo(scaleX, scaleY);
                var crossIntersectGenerator = new BezierIntersect(bezierInfo);

                for (int z = 0; z < _meshVertexWidth/2; z++){
                    Vector2 pos = crossIntersectGenerator.GetIntersectionFromX(yDelta[x]*(z));
                    _mesh[x, z] = new Vector3(topPts[x].X, pos.Y, topPts[x].Y + yDelta[x]*(z));
                    _mesh[x, _meshVertexWidth - 1 - z] = new Vector3(topPts[x].X, pos.Y, topPts[x].Y + yDelta[x]*(_meshVertexWidth - 1 - z));
                }
            }
            var normals = new Vector3[_meshVertexWidth,_meshVertexWidth];

            for (int vertX = 0; vertX < _meshVertexWidth - 1; vertX++){
                for (int vertZ = 0; vertZ < _meshVertexWidth - 1; vertZ++){
                    var crossSum = new Vector3();

                    var s1 = _mesh[vertX + 1, vertZ] - _mesh[vertX, vertZ];
                    var s2 = _mesh[vertX, vertZ + 1] - _mesh[vertX, vertZ];
                    var s3 = _mesh[vertX + 1, vertZ + 1] - _mesh[vertX, vertZ];

                    crossSum += Vector3.Cross(s1, s3);
                    crossSum += Vector3.Cross(s3, s2);

                    normals[vertX, vertZ] += crossSum;
                }
            }

            for (int vertX = 1; vertX < _meshVertexWidth; vertX++){
                for (int vertZ = 1; vertZ < _meshVertexWidth; vertZ++){
                    var crossSum = new Vector3();

                    var s1 = _mesh[vertX - 1, vertZ] - _mesh[vertX, vertZ];
                    var s2 = _mesh[vertX, vertZ - 1] - _mesh[vertX, vertZ];
                    var s3 = _mesh[vertX - 1, vertZ - 1] - _mesh[vertX, vertZ];

                    crossSum += Vector3.Cross(s1, s3);
                    crossSum += Vector3.Cross(s3, s2);

                    normals[vertX, vertZ] += crossSum;
                }
            }

            //convert from 2d array to 1d
            int index = 0;
            for (int x = 0; x < _meshVertexWidth - 1; x++){
                for (int z = 0; z < _meshVertexWidth - 1; z++){
                    _verticies[index].Position = -_mesh[x, z];
                    _verticies[index].Normal = normals[x, z];

                    _verticies[index + 1].Position = -_mesh[x, z + 1];
                    _verticies[index + 1].Normal = normals[x, z + 1];

                    _verticies[index + 2].Position = -_mesh[x + 1, z + 1];
                    _verticies[index + 2].Normal = normals[x + 1, z + 1];

                    _verticies[index + 3].Position = -_mesh[x + 1, z];
                    _verticies[index + 3].Normal = normals[x + 1, z];

                    index += 4;
                }
            }
            _geometryBuffer.Vertexbuffer.SetData(_verticies);

            var p = new Vector3();
            p += -_mesh[0, 0];
            p += -_mesh[_meshVertexWidth - 1, 0];
            p += -_mesh[0, _meshVertexWidth - 1];
            p += -_mesh[_meshVertexWidth - 1, _meshVertexWidth - 1];
            p /= 4;
            Renderer.CameraTarget = p;
            Renderer.CameraPosition.X = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Sin(_cameraTheta)) + Renderer.CameraTarget.X;
            Renderer.CameraPosition.Z = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Cos(_cameraTheta)) + Renderer.CameraTarget.Z;
            Renderer.CameraPosition.Y = (float) (_cameraDistance*Math.Sin(_cameraPhi)) + Renderer.CameraTarget.Y;
        }

        #region event handlers

        public override InterruptState OnMouseMovement(MouseState state, MouseState? prevState = null){
            if (prevState != null){
                if (_renderTarget.BoundingBox.Contains(state.X, state.Y)){
                    if (state.LeftButton == ButtonState.Pressed){
                        int dx = state.X - ((MouseState) prevState).X;
                        int dy = state.Y - ((MouseState) prevState).Y;

                        if (state.LeftButton == ButtonState.Pressed){
                            _cameraPhi += dy*0.01f;
                            _cameraTheta -= dx*0.01f;

                            if (_cameraPhi > 1.56f){
                                _cameraPhi = 1.56f;
                            }
                            if (_cameraPhi < -1.56f){
                                _cameraPhi = -1.56f;
                            }
                            Renderer.CameraPosition.X = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Sin(_cameraTheta)) + Renderer.CameraTarget.X;
                            Renderer.CameraPosition.Z = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Cos(_cameraTheta)) + Renderer.CameraTarget.Z;
                            Renderer.CameraPosition.Y = (float) (_cameraDistance*Math.Sin(_cameraPhi)) + Renderer.CameraTarget.Y;
                        }


                        return InterruptState.InterruptEventDispatch;
                    }
                    /*if (state.RightButton == ButtonState.Pressed) {
                        int dx = state.X - ((MouseState)prevState).X;
                        int dy = state.Y - ((MouseState)prevState).Y;

                        _cameraPhi += dy * 0.01f;
                        _cameraTheta += dx * 0.01f;

                        if (_cameraPhi > 1.56f) {
                            _cameraPhi = 1.56f;
                        }
                        if (_cameraPhi < -1.56f) {
                            _cameraPhi = -1.56f;
                        }

                        Renderer.CameraTarget.X = (float)(_cameraDistance * Math.Cos(_cameraPhi + Math.PI) * Math.Sin(_cameraTheta + Math.PI)) - Renderer.CameraPosition.X;
                        Renderer.CameraTarget.Z = (float)(_cameraDistance * Math.Cos(_cameraPhi + Math.PI) * Math.Cos(_cameraTheta + Math.PI)) - Renderer.CameraPosition.Z;
                        Renderer.CameraTarget.Y = (float)(_cameraDistance * Math.Sin(_cameraPhi + Math.PI)) + Renderer.CameraPosition.Y;
                        return InterruptState.InterruptEventDispatch;
                    }*/
                    return InterruptState.InterruptEventDispatch;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public override InterruptState OnMouseScroll(MouseState state, MouseState? prevState = null){
            if (prevState != null){
                _cameraDistance += (((MouseState) prevState).ScrollWheelValue - state.ScrollWheelValue)/5f;
                if (_cameraDistance < 50){
                    _cameraDistance = 50;
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        #endregion
    }
}