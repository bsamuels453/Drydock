#region

using System.Collections;
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

    internal delegate InterruptState OnMouseEvent(MouseState state, MouseState? prevState = null);

    internal delegate InterruptState OnKeyboardEvent(KeyboardState state);

    //these two delegates are to be used in literal(?) events
    internal delegate void EOnMouseEvent(MouseState state, MouseState? prevState = null);

    internal delegate void EOnKeyboardEvent(KeyboardState state);

    internal static class InputEventDispatcher{
        public static DepthSortedList EventSubscribers;
        static KeyboardState _prevKeyboardState;
        static MouseState _prevMouseState;
        static readonly Stopwatch _clickTimer;

        static InputEventDispatcher(){
            EventSubscribers = new DepthSortedList();

            _prevKeyboardState = Keyboard.GetState();
            _prevMouseState = Mouse.GetState();
            _clickTimer = new Stopwatch();
        }

        public static void Update(){
            UpdateMouse();
            UpdateKeyboard();
        }

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

        static void PrematureMouseExit(MouseState state, MouseState? prevState = null){
            _prevMouseState = state;
        }

        static void UpdateKeyboard(){
            var state = Keyboard.GetState();

            if (state != _prevKeyboardState){
                foreach (CanReceiveInputEvents subscriber in EventSubscribers){
                    if (subscriber.OnKeyboardEvent(state) == InterruptState.InterruptEventDispatch){
                        break;
                    }
                }
            }

            _prevKeyboardState = state;
        }
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