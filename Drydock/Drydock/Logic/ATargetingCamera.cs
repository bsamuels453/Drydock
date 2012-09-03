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
    internal abstract class ATargetingCamera : CanReceiveInputEvents{
        Rectangle _boundingBox;
        float _cameraDistance;
        float _cameraPhi;
        float _cameraTheta;

        /// <summary>
        ///   default constructor makes it recieve from entire screen
        /// </summary>
        /// <param name="boundingBox"> </param>
        protected ATargetingCamera(Rectangle? boundingBox = null){
            _cameraPhi = 0.32f;
            _cameraTheta = 0.63f;
            _cameraDistance = 100;
            if (boundingBox != null){
                _boundingBox = (Rectangle) boundingBox;
            }
            else{
                _boundingBox = new Rectangle(0, 0, ScreenData.ScreenWidth, ScreenData.ScreenHeight);
            }
            InputEventDispatcher.EventSubscribers.Add(1.0f, this);
        }

        protected void SetCameraTarget(Vector3 target){
            Renderer.CameraTarget = target;
            Renderer.CameraPosition.X = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Sin(_cameraTheta)) + Renderer.CameraTarget.X;
            Renderer.CameraPosition.Z = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Cos(_cameraTheta)) + Renderer.CameraTarget.Z;
            Renderer.CameraPosition.Y = (float) (_cameraDistance*Math.Sin(_cameraPhi)) + Renderer.CameraTarget.Y;
        }

        public override InterruptState OnMouseMovement(MouseState state, MouseState? prevState = null){
            if (prevState != null){
                if (_boundingBox.Contains(state.X, state.Y)){
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

                        //return InterruptState.InterruptEventDispatch;
                    }
                }
            }
            return InterruptState.AllowOtherEvents;
        }

        public override InterruptState OnMouseScroll(MouseState state, MouseState? prevState = null){
            if (prevState != null){
                if (_boundingBox.Contains(state.X, state.Y)){
                    _cameraDistance += (((MouseState) prevState).ScrollWheelValue - state.ScrollWheelValue)/20f;
                    if (_cameraDistance < 5){
                        _cameraDistance = 5;
                    }
                    Renderer.CameraPosition.X = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Sin(_cameraTheta)) + Renderer.CameraTarget.X;
                    Renderer.CameraPosition.Z = (float) (_cameraDistance*Math.Cos(_cameraPhi)*Math.Cos(_cameraTheta)) + Renderer.CameraTarget.Z;
                    Renderer.CameraPosition.Y = (float) (_cameraDistance*Math.Sin(_cameraPhi)) + Renderer.CameraTarget.Y;
                }
            }
            return InterruptState.AllowOtherEvents;
        }
    }
}