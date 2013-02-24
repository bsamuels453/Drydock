#region

using System;
using Drydock.Control;
using Drydock.Render;
using Drydock.Utilities;
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
        Vector3 _cameraPosition;
        Angle3 _cameraAngle;
        Vector3 _cameraTarget;
        readonly GamestateManager _manager;

        /// <summary>
        ///   default constructor makes it recieve from entire screen
        /// </summary>
        /// <param name="mgr"> </param>
        /// <param name="boundingBox"> </param>
        public BodyCenteredCamera(GamestateManager mgr, Rectangle? boundingBox = null){
            _manager = mgr;

            _cameraAngle = new Angle3();
            _cameraAngle.Pitch = 1.2f;
            _cameraAngle.Yaw = 1.93f;
            _cameraDistance = 60;

            _cameraTarget = new Vector3();

            _cameraPosition = new Vector3();
            _cameraPosition.X = (float)(_cameraDistance * Math.Sin(_cameraAngle.Pitch) * Math.Cos(_cameraAngle.Yaw)) + _cameraTarget.X;
            _cameraPosition.Z = (float)(_cameraDistance * Math.Sin(_cameraAngle.Pitch) * Math.Sin(_cameraAngle.Yaw)) + _cameraTarget.Z;
            _cameraPosition.Y = (float)(_cameraDistance * Math.Cos(_cameraAngle.Pitch)) + _cameraTarget.Y;


            _manager.AddSharedData(SharedStateData.PlayerPosition, _cameraPosition);
            _manager.AddSharedData(SharedStateData.PlayerLook, _cameraAngle);

            if (boundingBox != null){
                _boundingBox = (Rectangle) boundingBox;
            }
            else{
                _boundingBox = new Rectangle(0, 0, ScreenData.ScreenWidth, ScreenData.ScreenHeight);
            }
        }

        #region IInputUpdates Members

        public void UpdateInput(ref InputState state){
            if (_boundingBox.Contains(state.MousePos.X, state.MousePos.Y)){
                if (state.RightButtonState == ButtonState.Pressed){
                    if (!state.KeyboardState.IsKeyDown(Keys.LeftControl)){
                        int dx = state.MousePos.X - state.PrevState.MousePos.X;
                        int dy = state.MousePos.Y - state.PrevState.MousePos.Y;

                        if (state.RightButtonState == ButtonState.Pressed){
                            _cameraAngle.Pitch -= dy*0.01f;
                            _cameraAngle.Yaw += dx * 0.01f;

                            if (_cameraAngle.Pitch > (float)Math.PI - 0.01f) {
                                _cameraAngle.Pitch = (float)Math.PI - 0.01f;
                            }
                            if (_cameraAngle.Pitch < 0.01f) {
                                _cameraAngle.Pitch = 0.01f;
                            }

                            _cameraPosition.X = (float)(_cameraDistance * Math.Sin(_cameraAngle.Pitch) * Math.Cos(_cameraAngle.Yaw)) + _cameraTarget.X;
                            _cameraPosition.Z = (float)(_cameraDistance * Math.Sin(_cameraAngle.Pitch) * Math.Sin(_cameraAngle.Yaw)) + _cameraTarget.Z;
                            _cameraPosition.Y = (float)(_cameraDistance * Math.Cos(_cameraAngle.Pitch)) + _cameraTarget.Y;
                        }

                        state.AllowMouseMovementInterpretation = false;
                    }
                    else{
                        int dx = state.MousePos.X - state.PrevState.MousePos.X;
                        int dy = state.MousePos.Y - state.PrevState.MousePos.Y;

                        _cameraAngle.Pitch -= dy * 0.005f;
                        _cameraAngle.Yaw += dx * 0.005f;

                        if (_cameraAngle.Pitch > (float)Math.PI - 0.01f) {
                            _cameraAngle.Pitch = (float)Math.PI - 0.01f;
                        }
                        if (_cameraAngle.Pitch < 0.01f) {
                            _cameraAngle.Pitch = 0.01f;
                        }

                        _cameraPosition.X = ((float)(_cameraDistance * Math.Sin(_cameraAngle.Pitch + Math.PI) * Math.Cos(_cameraAngle.Yaw + Math.PI)) - _cameraPosition.X) * -1;
                        _cameraPosition.Z = ((float)(_cameraDistance * Math.Sin(_cameraAngle.Pitch + Math.PI) * Math.Sin(_cameraAngle.Yaw + Math.PI)) - _cameraPosition.Z) * -1;
                        _cameraPosition.Y = ((float)(_cameraDistance * Math.Cos(_cameraAngle.Pitch + Math.PI)) + _cameraPosition.Y) * 1;

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

                    _cameraPosition.X = (float)(_cameraDistance * Math.Sin(_cameraAngle.Pitch) * Math.Cos(_cameraAngle.Yaw)) + _cameraTarget.X;
                    _cameraPosition.Z = (float)(_cameraDistance * Math.Sin(_cameraAngle.Pitch) * Math.Sin(_cameraAngle.Yaw)) + _cameraTarget.Z;
                    _cameraPosition.Y = (float)(_cameraDistance * Math.Cos(_cameraAngle.Pitch)) + _cameraTarget.Y;
                }
            }
        }

        #endregion

        public void SetCameraTarget(Vector3 target){
            _cameraTarget = target;

            _cameraPosition.X = (float)(_cameraDistance * Math.Sin(_cameraAngle.Pitch) * Math.Cos(_cameraAngle.Yaw)) + _cameraTarget.X;
            _cameraPosition.Z = (float)(_cameraDistance * Math.Sin(_cameraAngle.Pitch) * Math.Sin(_cameraAngle.Yaw)) + _cameraTarget.Z;
            _cameraPosition.Y = (float)(_cameraDistance * Math.Cos(_cameraAngle.Pitch)) + _cameraTarget.Y;
        }
    }
}