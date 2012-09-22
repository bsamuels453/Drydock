#region

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Drydock.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Drydock.Control{
    internal enum InterruptState{
        AllowOtherEvents,
        InterruptEventDispatch
    }

    internal delegate InterruptState OnMouseEvent(MouseState state, MouseState? prevState = null);

    internal delegate InterruptState OnKeyboardEvent(KeyboardState state);

    //these two delegates are to be used in literal(?) events
    internal delegate void EOnMouseEvent(MouseState state, MouseState? prevState = null);

    internal delegate void EOnKeyboardEvent(KeyboardState state);

    internal static class InputEventDispatcher{
        public static SpecialKeyboardRec SpecialKeyboardDispatcher;

        static readonly Stopwatch _clickTimer;
        static MouseState _prevMouseState;
        static KeyboardState _prevKeyboardState;
        public static ControlState CurrentControlState;

        static InputEventDispatcher(){

            _prevKeyboardState = Keyboard.GetState();
            _prevMouseState = Mouse.GetState();
            _clickTimer = new Stopwatch();
        }

        public static void Update(){
            //all this crap updates the CurrentControlState to whatever the hell is going on

            var curMouseState = Mouse.GetState();
            var curKeyboardState = Keyboard.GetState();

            var curControlState = new ControlState();

            if (_prevMouseState.X != curMouseState.X || _prevMouseState.Y != curMouseState.Y) {
                curControlState.AllowMouseMovementInterpretation = true;
                curControlState.MousePosition = new Vector2();
                curControlState.MousePosition.X = curMouseState.X;
                curControlState.MousePosition.Y = curMouseState.Y;
                curControlState.MouseChange = new Vector2();
                curControlState.MouseChange.X = curMouseState.X - _prevMouseState.X;
                curControlState.MouseChange.Y = curMouseState.Y - _prevMouseState.Y;
            }
            else{
                curControlState.AllowMouseMovementInterpretation = false;
            }

            if (_prevMouseState.LeftButton != curMouseState.LeftButton) {
                curControlState.LeftButtonChange = curMouseState.LeftButton;
                curControlState.AllowLeftButtonInterpretation = true;
                if (curMouseState.LeftButton == ButtonState.Released) {
                    //check if this qualifies as a click
                    if (_clickTimer.ElapsedMilliseconds < 200) {
                        curControlState.LeftButtonClick = true;
                        _clickTimer.Reset();
                    }
                    else {
                        curControlState.LeftButtonClick = false;
                    }
                }
                else{//button was pressed so start the click timer
                    _clickTimer.Start();
                }
            }
            else
                curControlState.AllowLeftButtonInterpretation = false;

            if (_prevMouseState.RightButton != curMouseState.RightButton) {
                curControlState.RightButtonChange = curMouseState.RightButton;
                curControlState.AllowRightButtonInterpretation = true;
            }
            else
                curControlState.AllowRightButtonInterpretation = false;

            if (_prevMouseState.ScrollWheelValue != curMouseState.ScrollWheelValue) {
                curControlState.MouseScrollChange = curMouseState.ScrollWheelValue - _prevMouseState.ScrollWheelValue;
                curControlState.AllowMouseScrollInterpretation = true;
            }
            else
                curControlState.AllowMouseScrollInterpretation = false;

            curControlState.KeyboardState = curKeyboardState;

            _prevKeyboardState = curKeyboardState;
            _prevMouseState = curMouseState;
        }
        /*
        static void UpdateMouse(){
            MouseState newState = Mouse.GetState();
            if (newState.LeftButton != _prevMouseState.LeftButton ||
                newState.RightButton != _prevMouseState.RightButton ||
                newState.MiddleButton != _prevMouseState.MiddleButton){
                if (newState.LeftButton == ButtonState.Released){
                    //dispatch onbuttonreleased
                    foreach (CanReceiveInputEvents subscriber in EventSubscribers){
                        if (subscriber.OnLeftButtonRelease(newState, _prevMouseState) == InterruptState.InterruptEventDispatch){
                            PrematureMouseExit(newState);
                            return;
                        }
                    }


                    if (_clickTimer.ElapsedMilliseconds < 200){
                        //dispatch onclick
                        foreach (CanReceiveInputEvents subscriber in EventSubscribers){
                            if (subscriber.OnLeftButtonClick(newState, _prevMouseState) == InterruptState.InterruptEventDispatch){
                                PrematureMouseExit(newState);
                                return;
                            }
                        }
                    }
                    _clickTimer.Reset();
                }

                if (newState.LeftButton == ButtonState.Pressed){
                    _clickTimer.Start();
                    //dispatch onbuttonpressed
                    foreach (CanReceiveInputEvents subscriber in EventSubscribers){
                        if (subscriber.OnLeftButtonPress(newState, _prevMouseState) == InterruptState.InterruptEventDispatch){
                            PrematureMouseExit(newState);
                            return;
                        }
                    }
                }
            }

            if (newState.X != _prevMouseState.X ||
                newState.Y != _prevMouseState.Y){
                //dispatch onmovement
                foreach (CanReceiveInputEvents subscriber in EventSubscribers){
                    if (subscriber.OnMouseMovement(newState, _prevMouseState) == InterruptState.InterruptEventDispatch){
                        PrematureMouseExit(newState);
                        return;
                    }
                }
            }
            if (newState.ScrollWheelValue != _prevMouseState.ScrollWheelValue){
                foreach (CanReceiveInputEvents subscriber in EventSubscribers){
                    if (subscriber.OnMouseScroll(newState, _prevMouseState) == InterruptState.InterruptEventDispatch){
                        PrematureMouseExit(newState);
                        return;
                    }
                }
            }

            _prevMouseState = newState;
        }

        static void UpdateKeyboard(){
            var state = Keyboard.GetState();

            if (state != _prevKeyboardState){
                foreach (CanReceiveInputEvents subscriber in EventSubscribers){
                    if (subscriber.OnKeyboardEvent(state) == InterruptState.InterruptEventDispatch){
                        break;
                    }
                }
                SpecialKeyboardDispatcher(state);
            }

            _prevKeyboardState = state;
        }
        */
    }

    internal class ControlState{
        public bool AllowMouseMovementInterpretation;
        public Vector2 MousePosition;
        public Vector2 MouseChange;

        public bool AllowLeftButtonInterpretation;
        public bool AllowRightButtonInterpretation;
        public bool LeftButtonClick;
        public ButtonState LeftButtonChange;
        public ButtonState RightButtonChange;

        public bool AllowMouseScrollInterpretation;
        public int MouseScrollChange;

        public bool AllowKeyboardInterpretation;
        public KeyboardState KeyboardState;
    }
}