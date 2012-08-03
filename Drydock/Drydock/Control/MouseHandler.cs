using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Drydock.Control{
    internal delegate bool OnMouseAction(MouseState state);

    internal static class MouseHandler{
        //private  int _curMousePosX;
        //private  int _curMousePosY;
        //private readonly bool _isMouseClamped;
        private static Vector2 _viewportSize;
        private static MouseState _previousMouseState;

        public static List<OnMouseAction> ClickSubscriptions { get; set; }
        public static List<OnMouseAction> MovementSubscriptions { get; set; }

        public static void Init(GraphicsDevice device){
            _viewportSize = new Vector2(device.Viewport.Bounds.Width, device.Viewport.Bounds.Height);
            Mouse.SetPosition((int) _viewportSize.X/2, (int) _viewportSize.Y/2);
            //_curMousePosX = (int)_viewportSize.X / 2;
            //_curMousePosY = (int) _viewportSize.Y/ 2;
            //_isMouseClamped = false;
            ClickSubscriptions = new List<OnMouseAction>();
            MovementSubscriptions = new List<OnMouseAction>();
            _previousMouseState = Mouse.GetState();
        }

        public static void UpdateMouse(){
            MouseState newState = Mouse.GetState();
            if (newState.LeftButton != _previousMouseState.LeftButton ||
                newState.RightButton != _previousMouseState.RightButton ||
                newState.MiddleButton != _previousMouseState.MiddleButton){
                    foreach (OnMouseAction t in ClickSubscriptions) {
                    if (t(newState)){
                        //break;
                    }
                }
            }

            if (newState.X != _previousMouseState.X ||
                newState.Y != _previousMouseState.Y){
                foreach (OnMouseAction t in MovementSubscriptions){
                    if (t(newState)){
                        break;
                    }
                }
            }
            _previousMouseState = newState;

            //old mouse update code from forge
            /*_textDisplay[0].EditText("Pitch: " + _renderer.ViewportPitch);
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
                    _curMousePosX > (int)_viewportSize.X*(1 - tolerance) ||
                    _curMousePosX < (int)_viewportSize.X*tolerance ||
                    _curMousePosY > (int)_viewportSize.Y*(1 - tolerance) ||
                    _curMousePosY < (int)_viewportSize.X*tolerance
                    ){
                    //move mouse to center of screen
                    Mouse.SetPosition((int)_viewportSize.X/2, (int)_viewportSize.Y/2);
                    _curMousePosX = (int)_viewportSize.X/2;
                    _curMousePosY = (int)_viewportSize.Y/2;
                }
            }


            //now apply viewport changes
            _renderer.ViewportYaw -= dx*0.005f;
            if (
                (_renderer.ViewportPitch - dy*0.005f) < 1.55 &&
                (_renderer.ViewportPitch - dy*0.005f) > -1.55){
                _renderer.ViewportPitch -= dy*0.005f;
            }*/
        }

        public static void ToggleMouseClamp(){
        }
    }
}