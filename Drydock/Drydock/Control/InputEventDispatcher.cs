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
        public static DepthSortedList EventSubscribers;
        public static SpecialKeyboardRec SpecialKeyboardDispatcher;

        static readonly Stopwatch _clickTimer;
        static ControlState _prevState;
        static MouseState _prevMouseState;
        static KeyboardState _prevKeyboardState;
        public static ControlState CurrentControlState;

        static InputEventDispatcher(){
            EventSubscribers = new DepthSortedList();

            _prevKeyboardState = Keyboard.GetState();
            _prevMouseState = Mouse.GetState();
            _clickTimer = new Stopwatch();
        }

        public static void Update(){
            //UpdateMouse();
            //UpdateKeyboard();

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
            }
            else
                curControlState.AllowLeftButtonInterpretation = false;

            if (_prevMouseState.RightButton != curMouseState.RightButton) {
                curControlState.RightButtonChange = curMouseState.RightButton;
                curControlState.AllowRightButtonInterpretation = true;
            }
            else
                curControlState.AllowRightButtonInterpretation = false;

            curControlState.KeyboardState = curKeyboardState;
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
        public ButtonState LeftButtonChange;
        public ButtonState RightButtonChange;

        public bool AllowMouseScrollInterpretation;
        public int MouseScrollChange;

        public bool AllowKeyboardInterpretation;
        public KeyboardState KeyboardState;

    }

    #region depth sorted list

    internal class DepthSortedList : IEnumerable{
        readonly List<float> _depthList;
        readonly List<CanReceiveInputEvents> _objList;

        public DepthSortedList(){
            _depthList = new List<float>();
            _objList = new List<CanReceiveInputEvents>();
        }

        public int Count{
            get { return _depthList.Count; }
        }

        public CanReceiveInputEvents this[int index]{
            get { return _objList[index]; }
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator(){
            return _objList.GetEnumerator();
        }

        #endregion

        public void Add(float depth, CanReceiveInputEvents element){
            _depthList.Add(depth);
            _objList.Add(element);

            for (int i = _depthList.Count - 1; i < 0; i--){
                if (_depthList[i] < _depthList[i - 1]){
                    _depthList.RemoveAt(i);
                    _objList.RemoveAt(i);
                    _depthList.Insert(i - 2, depth);
                    _objList.Insert(i - 2, element);
                }
                else{
                    break;
                }
            }
        }

        public void Clear(){
            _depthList.Clear();
            _objList.Clear();
        }

        public void RemoveAt(int index){
            _depthList.RemoveAt(index);
            _objList.RemoveAt(index);
        }

        public void Remove(CanReceiveInputEvents element){
            int i = 0;
            while (_objList[i] == element){
                i++;
                if (i == _objList.Count){
                    //return;
                }
            }
            RemoveAt(i);
        }
    }

    #endregion
}