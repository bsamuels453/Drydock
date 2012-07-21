using System;
using Drydock.Render;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Control{
    internal class KeyboardHandler{
        private readonly ScreenText[] _positionDisplay;
        private KeyboardState _keyState;
        private KeyboardState _prevState;
        private readonly Renderer _renderer;



        public KeyboardHandler(Renderer renderer){
            _renderer = renderer;
            _positionDisplay = new ScreenText[3];
            _positionDisplay[0] = new ScreenText(0, 00, "X POS NOT SET");
            _positionDisplay[1] = new ScreenText(0, 10, "Y POS NOT SET");
            _positionDisplay[2] = new ScreenText(0, 20, "Z POS NOT SET");
            _prevState = Keyboard.GetState();
        }

        public void UpdateKeyboard(){
            _positionDisplay[0].EditText("X: " + _renderer.ViewportPosition.X.ToString());
            _positionDisplay[1].EditText("Y: " + _renderer.ViewportPosition.Y.ToString());
            _positionDisplay[2].EditText("Z: " + _renderer.ViewportPosition.Z.ToString());

            float movementspeed = 0.1f;
            if (_keyState.IsKeyDown(Keys.LeftShift)){
                movementspeed = 1.0f;
            }
            if (_keyState.IsKeyDown(Keys.LeftControl)){
                movementspeed = 0.02f;
            }

            _keyState = Keyboard.GetState();
            //and to think you thought calc 3 was useless!
            if (_keyState.IsKeyDown(Keys.W)){
                _renderer.ViewportPosition.X = _renderer.ViewportPosition.X + (float) Math.Sin(_renderer.ViewportYaw)*(float) Math.Cos(_renderer.ViewportPitch)*movementspeed;
                _renderer.ViewportPosition.Y = _renderer.ViewportPosition.Y + (float) Math.Sin(_renderer.ViewportPitch)*movementspeed;
                _renderer.ViewportPosition.Z = _renderer.ViewportPosition.Z + (float) Math.Cos(_renderer.ViewportYaw)*(float) Math.Cos(_renderer.ViewportPitch)*movementspeed;
            }
            if (_keyState.IsKeyDown(Keys.S)){
                _renderer.ViewportPosition.X = _renderer.ViewportPosition.X - (float) Math.Sin(_renderer.ViewportYaw)*(float) Math.Cos(_renderer.ViewportPitch)*movementspeed;
                _renderer.ViewportPosition.Y = _renderer.ViewportPosition.Y - (float) Math.Sin(_renderer.ViewportPitch)*movementspeed;
                _renderer.ViewportPosition.Z = _renderer.ViewportPosition.Z - (float) Math.Cos(_renderer.ViewportYaw)*(float) Math.Cos(_renderer.ViewportPitch)*movementspeed;
            }
            if (_keyState.IsKeyDown(Keys.A)){
                _renderer.ViewportPosition.X = _renderer.ViewportPosition.X + (float) Math.Sin(_renderer.ViewportYaw + 3.14159f/2)*movementspeed;
                _renderer.ViewportPosition.Z = _renderer.ViewportPosition.Z + (float) Math.Cos(_renderer.ViewportYaw + 3.14159f/2)*movementspeed;
            }

            if (_keyState.IsKeyDown(Keys.D)){
                _renderer.ViewportPosition.X = _renderer.ViewportPosition.X - (float) Math.Sin(_renderer.ViewportYaw + 3.14159f/2)*movementspeed;
                _renderer.ViewportPosition.Z = _renderer.ViewportPosition.Z - (float) Math.Cos(_renderer.ViewportYaw + 3.14159f/2)*movementspeed;
            }

            if (_keyState.IsKeyDown(Keys.Escape)){
            }

            _prevState = _keyState;
        }
    }
}