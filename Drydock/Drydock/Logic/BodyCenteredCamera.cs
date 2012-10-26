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
            _cameraPhi = 0.32f;
            _cameraTheta = 0.63f;
            _cameraDistance = 100;
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
                    int dx = state.MousePos.X - state.PrevState.MousePos.X;
                    int dy = state.MousePos.Y - state.PrevState.MousePos.Y;

                    if (state.RightButtonState == ButtonState.Pressed){
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

                    state.AllowMouseMovementInterpretation = false;

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
                }
            }

            if (state.AllowMouseScrollInterpretation){
                if (_boundingBox.Contains(state.MousePos.X, state.MousePos.Y)){
                    _cameraDistance += -state.MouseScrollChange/20f;
                    if (_cameraDistance < 5){
                        _cameraDistance = 5;
                    }
                    Renderer.CameraPosition.X = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Sin(_cameraTheta)) + Renderer.CameraTarget.X;
                    Renderer.CameraPosition.Z = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Cos(_cameraTheta)) + Renderer.CameraTarget.Z;
                    Renderer.CameraPosition.Y = (float) (_cameraDistance*Math.Sin(_cameraPhi)) + Renderer.CameraTarget.Y;
                }
            }
        }

        #endregion

        public void SetCameraTarget(Vector3 target){
            Renderer.CameraTarget = target;
            Renderer.CameraPosition.X = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Sin(_cameraTheta)) + Renderer.CameraTarget.X;
            Renderer.CameraPosition.Z = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Cos(_cameraTheta)) + Renderer.CameraTarget.Z;
            Renderer.CameraPosition.Y = (float) (_cameraDistance*Math.Sin(_cameraPhi)) + Renderer.CameraTarget.Y;
        }
    }
}