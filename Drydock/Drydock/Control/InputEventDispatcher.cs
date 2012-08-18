#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Drydock.Render;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Control{
    internal enum InterruptState{
        AllowOtherEvents,
        InterruptEventDispatch
    }

    internal delegate InterruptState OnMouseEvent(MouseState state);

    internal delegate InterruptState OnKeyboardEvent(KeyboardState state);

    //these two delegates are to be used in literal events
    internal delegate void EOnMouseEvent(MouseState state);

    internal delegate void EOnKeyboardEvent(KeyboardState state);

    internal static class InputEventDispatcher{
        public static List<ICanReceiveInputEvents> EventSubscribers;

        private static KeyboardState _prevKeyboardState;
        private static MouseState _prevMouseState;
        private static readonly Stopwatch _clickTimer;

        private static readonly ScreenText _mousePos;

        static InputEventDispatcher(){
            EventSubscribers = new List<ICanReceiveInputEvents>();

            _prevKeyboardState = Keyboard.GetState();
            _prevMouseState = Mouse.GetState();
            _clickTimer = new Stopwatch();
            _mousePos = new ScreenText(0, 0, "not init");
        }

        public static void Update(Renderer renderer){
            UpdateMouse(renderer);
            UpdateKeyboard(renderer);
        }

        private static void UpdateMouse(Renderer renderer){
            MouseState newState = Mouse.GetState();
            if (newState.LeftButton != _prevMouseState.LeftButton ||
                newState.RightButton != _prevMouseState.RightButton ||
                newState.MiddleButton != _prevMouseState.MiddleButton){
                if (newState.LeftButton == ButtonState.Released){
                    //dispatch onbuttonreleased
                    foreach (var subscriber in EventSubscribers){
                        if (subscriber.OnLeftButtonRelease(newState) == InterruptState.InterruptEventDispatch){
                            break;
                        }
                    }


                    if (_clickTimer.ElapsedMilliseconds < 200){
                        //dispatch onclick
                        foreach (var subscriber in EventSubscribers){
                            if (subscriber.OnLeftButtonClick(newState) == InterruptState.InterruptEventDispatch){
                                break;
                            }
                        }
                    }
                    _clickTimer.Reset();
                }

                if (newState.LeftButton == ButtonState.Pressed){
                    _clickTimer.Start();
                    //dispatch onbuttonpressed
                    foreach (var subscriber in EventSubscribers){
                        if (subscriber.OnLeftButtonPress(newState) == InterruptState.InterruptEventDispatch){
                            break;
                        }
                    }
                }
            }

            if (newState.X != _prevMouseState.X ||
                newState.Y != _prevMouseState.Y){
                bool interrupt = false;
                //dispatch onmovement
                foreach (var subscriber in EventSubscribers){
                    if (subscriber.OnMouseMovement(newState) == InterruptState.InterruptEventDispatch){
                        interrupt = true;
                        break;
                    }
                }
                int dx = newState.X - _prevMouseState.X;
                int dy = newState.Y - _prevMouseState.Y;

                //now apply viewport changes
                /*renderer.ViewportYaw -= dx*0.005f;
                if (
                    (renderer.ViewportPitch - dy*0.005f) < 1.55 &&
                    (renderer.ViewportPitch - dy*0.005f) > -1.55){
                    renderer.ViewportPitch -= dy*0.005f;
                }*/
                if (!interrupt){
                    if (newState.LeftButton == ButtonState.Pressed){
                        renderer.CameraPhi += dy*0.01f;
                        renderer.CameraTheta -= dx*0.01f;
                    }
                }
                _mousePos.EditText("phi:" + renderer.CameraPhi + "  theta:" + renderer.CameraTheta);
            }
            _prevMouseState = newState;
        }

        private static void UpdateKeyboard(Renderer renderer){
            var state = Keyboard.GetState();

            if (state != _prevKeyboardState){
                foreach (var subscriber in EventSubscribers){
                    if (subscriber.OnKeyboardEvent(state) == InterruptState.InterruptEventDispatch){
                        break;
                    }
                }
            }
            float movementspeed = 2f;
            if (state.IsKeyDown(Keys.W)){
                renderer.ViewportPosition.X = renderer.ViewportPosition.X + (float) Math.Sin(renderer.ViewportYaw)*(float) Math.Cos(renderer.ViewportPitch)*movementspeed;
                renderer.ViewportPosition.Y = renderer.ViewportPosition.Y + (float) Math.Sin(renderer.ViewportPitch)*movementspeed;
                renderer.ViewportPosition.Z = renderer.ViewportPosition.Z + (float) Math.Cos(renderer.ViewportYaw)*(float) Math.Cos(renderer.ViewportPitch)*movementspeed;
            }
            if (state.IsKeyDown(Keys.S)){
                renderer.ViewportPosition.X = renderer.ViewportPosition.X - (float) Math.Sin(renderer.ViewportYaw)*(float) Math.Cos(renderer.ViewportPitch)*movementspeed;
                renderer.ViewportPosition.Y = renderer.ViewportPosition.Y - (float) Math.Sin(renderer.ViewportPitch)*movementspeed;
                renderer.ViewportPosition.Z = renderer.ViewportPosition.Z - (float) Math.Cos(renderer.ViewportYaw)*(float) Math.Cos(renderer.ViewportPitch)*movementspeed;
            }
            if (state.IsKeyDown(Keys.A)){
                renderer.ViewportPosition.X = renderer.ViewportPosition.X + (float) Math.Sin(renderer.ViewportYaw + 3.14159f/2)*movementspeed;
                renderer.ViewportPosition.Z = renderer.ViewportPosition.Z + (float) Math.Cos(renderer.ViewportYaw + 3.14159f/2)*movementspeed;
            }

            if (state.IsKeyDown(Keys.D)){
                renderer.ViewportPosition.X = renderer.ViewportPosition.X - (float) Math.Sin(renderer.ViewportYaw + 3.14159f/2)*movementspeed;
                renderer.ViewportPosition.Z = renderer.ViewportPosition.Z - (float) Math.Cos(renderer.ViewportYaw + 3.14159f/2)*movementspeed;
            }

            _prevKeyboardState = state;
        }
    }
}