using Drydock.Render;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Control{
    internal class MouseHandler{
        private  int _curMousePosX;
        private  int _curMousePosY;
        private readonly bool _isMouseClamped;
        private readonly ScreenText[] _textDisplay;
        private readonly Renderer _renderer;

        public MouseHandler(Renderer renderer){
            _renderer = renderer;
            
            Mouse.SetPosition(_renderer.Device.Viewport.Bounds.Width/2, _renderer.Device.Viewport.Bounds.Height/2);
            _curMousePosX = _renderer.Device.Viewport.Bounds.Width/2;
            _curMousePosY = _renderer.Device.Viewport.Bounds.Height/2;
            _isMouseClamped = false;

            _textDisplay = new ScreenText[2];
            _textDisplay[0] = new ScreenText(0, 35, "PITCH NOT SET");
            _textDisplay[1] = new ScreenText(0, 45, "YAW NOT SET");
        }

        public  void UpdateMouse(){
            _textDisplay[0].EditText("Pitch: " + _renderer.ViewportPitch);
            _textDisplay[1].EditText("Yaw: " + _renderer.ViewportYaw);


            //first handle changes to viewport from mouse movement
            MouseState ms = Mouse.GetState();
            int newMousePosX = ms.X;
            int newMousePosY = ms.Y;

            int dx = newMousePosX - _curMousePosX;
            int dy = newMousePosY - _curMousePosY;

            _curMousePosX = newMousePosX;
            _curMousePosY = newMousePosY;


            //now make sure mouse isnt on its way out of the window if it is clamped
            if ((dx != 0 || dy != 0) && _isMouseClamped){ //if movement
                const double tolerance = 0.2f;

                //check to see if mouse is outside of permitted area
                if (
                    _curMousePosX > _renderer.Device.Viewport.Bounds.Width*(1 - tolerance) ||
                    _curMousePosX < _renderer.Device.Viewport.Bounds.Width*tolerance ||
                    _curMousePosY > _renderer.Device.Viewport.Bounds.Height*(1 - tolerance) ||
                    _curMousePosY < _renderer.Device.Viewport.Bounds.Width*tolerance
                    ){
                    //move mouse to center of screen
                    Mouse.SetPosition(_renderer.Device.Viewport.Bounds.Width/2, _renderer.Device.Viewport.Bounds.Height/2);
                    _curMousePosX = _renderer.Device.Viewport.Bounds.Width/2;
                    _curMousePosY = _renderer.Device.Viewport.Bounds.Height/2;
                }
            }


            //now apply viewport changes
            _renderer.ViewportYaw -= dx*0.005f;
            if (
                (_renderer.ViewportPitch - dy*0.005f) < 1.55 &&
                (_renderer.ViewportPitch - dy*0.005f) > -1.55){
                _renderer.ViewportPitch -= dy*0.005f;
            }
        }

        public  void ToggleMouseClamp(){
        }
    }


}