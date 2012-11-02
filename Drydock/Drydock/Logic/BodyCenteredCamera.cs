#region

using System;
using Drydock.Control;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Logic{
    /// <summary>
    ///   this abstract class creates a camera that rotates around a point
    /// </summary>
    internal class BodyCenteredCamera : IInputUpdates{
        Rectangle _boundingBox;
        float _cameraDistance;
        float _cameraPhi;
        float _cameraTheta;

        /// <summary>
        ///   default constructor makes it recieve from entire screen
        /// </summary>
        /// <param name="boundingBox"> </param>
        public BodyCenteredCamera(Rectangle? boundingBox = null){
            _cameraPhi = 1.2f;
            _cameraTheta = 1.93f;
            _cameraDistance = 60;
            if (boundingBox != null){
                _boundingBox = (Rectangle) boundingBox;
            }
            else{
                _boundingBox = new Rectangle(0, 0, ScreenData.ScreenWidth, ScreenData.ScreenHeight);
            }
        }

        #region IInputUpdates Members

        public void UpdateInput(ref ControlState state){
            if (_boundingBox.Contains(state.MousePos.X, state.MousePos.Y)){
                if (state.RightButtonState == ButtonState.Pressed){
                    if (!state.KeyboardState.IsKeyDown(Keys.LeftControl)){
                        int dx = state.MousePos.X - state.PrevState.MousePos.X;
                        int dy = state.MousePos.Y - state.PrevState.MousePos.Y;

                        if (state.RightButtonState == ButtonState.Pressed){
                            _cameraPhi -= dy*0.01f;
                            _cameraTheta += dx*0.01f;

                            if (_cameraPhi > (float)Math.PI - 0.01f) {
                                _cameraPhi = (float)Math.PI-0.01f;
                            }
                            if (_cameraPhi < 0.01f) {
                                _cameraPhi = 0.01f;
                            }

                            Renderer.CameraPosition.X = (float)(_cameraDistance * Math.Sin(_cameraPhi) * Math.Cos(_cameraTheta)) + Renderer.CameraTarget.X;
                            Renderer.CameraPosition.Z = (float)(_cameraDistance * Math.Sin(_cameraPhi) * Math.Sin(_cameraTheta)) + Renderer.CameraTarget.Z;
                            Renderer.CameraPosition.Y = (float)(_cameraDistance * Math.Cos(_cameraPhi)) + Renderer.CameraTarget.Y;

                        }

                        state.AllowMouseMovementInterpretation = false;
                    }
                    else{
                        int dx = state.MousePos.X - state.PrevState.MousePos.X;
                        int dy = state.MousePos.Y - state.PrevState.MousePos.Y;

                        _cameraPhi -= dy*0.005f;
                        _cameraTheta += dx*0.005f;

                        if (_cameraPhi > (float)Math.PI - 0.01f) {
                            _cameraPhi = (float)Math.PI - 0.01f;
                        }
                        if (_cameraPhi < 0.01f) {
                            _cameraPhi = 0.01f;
                        }

                        Renderer.CameraTarget.X = ((float)(_cameraDistance * Math.Sin(_cameraPhi + Math.PI) * Math.Cos(_cameraTheta + Math.PI)) - Renderer.CameraPosition.X)*-1;
                        Renderer.CameraTarget.Z = ((float)(_cameraDistance * Math.Sin(_cameraPhi + Math.PI) * Math.Sin(_cameraTheta + Math.PI)) - Renderer.CameraPosition.Z) * -1;
                        Renderer.CameraTarget.Y = ((float)(_cameraDistance * Math.Cos(_cameraPhi + Math.PI)) + Renderer.CameraPosition.Y) * 1;

                        int f = 4;

                    }
                }
            }
        

            if (state.AllowMouseScrollInterpretation){
                if (_boundingBox.Contains(state.MousePos.X, state.MousePos.Y)){
                    _cameraDistance += -state.MouseScrollChange/20f;
                    if (_cameraDistance < 5){
                        _cameraDistance = 5;
                    }

                    Renderer.CameraPosition.X = (float)(_cameraDistance * Math.Sin(_cameraPhi) * Math.Cos(_cameraTheta)) + Renderer.CameraTarget.X;
                    Renderer.CameraPosition.Z = (float)(_cameraDistance * Math.Sin(_cameraPhi) * Math.Sin(_cameraTheta)) + Renderer.CameraTarget.Z;
                    Renderer.CameraPosition.Y = (float)(_cameraDistance * Math.Cos(_cameraPhi)) + Renderer.CameraTarget.Y;
                }
            }
        }

        #endregion

        public void SetCameraTarget(Vector3 target){
            Renderer.CameraTarget = target;

            Renderer.CameraPosition.X = (float)(_cameraDistance * Math.Sin(_cameraPhi) * Math.Cos(_cameraTheta)) + Renderer.CameraTarget.X;
            Renderer.CameraPosition.Z = (float)(_cameraDistance * Math.Sin(_cameraPhi) * Math.Sin(_cameraTheta)) + Renderer.CameraTarget.Z;
            Renderer.CameraPosition.Y = (float)(_cameraDistance * Math.Cos(_cameraPhi)) + Renderer.CameraTarget.Y;
        }
    }
}