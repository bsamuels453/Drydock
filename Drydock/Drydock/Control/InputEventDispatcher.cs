#region

using System;
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

    internal delegate InterruptState OnMouseEvent(MouseState state);

    internal delegate InterruptState OnKeyboardEvent(KeyboardState state);

    //these two delegates are to be used in literal(?) events
    internal delegate void EOnMouseEvent(MouseState state);

    internal delegate void EOnKeyboardEvent(KeyboardState state);

    internal static class InputEventDispatcher{
        public static DepthSortedList EventSubscribers;
        private static KeyboardState _prevKeyboardState;
        private static MouseState _prevMouseState;
        private static readonly Stopwatch _clickTimer;
        private static readonly ScreenText _mousePos;

        static InputEventDispatcher(){
            EventSubscribers = new DepthSortedList();

            _prevKeyboardState = Keyboard.GetState();
            _prevMouseState = Mouse.GetState();
            _clickTimer = new Stopwatch();
            _mousePos = new ScreenText(0, 0, "not init");
        }

        public static void Update(){
            UpdateMouse();
            UpdateKeyboard();
        }

        private static void UpdateMouse(){
            MouseState newState = Mouse.GetState();
            if (newState.LeftButton != _prevMouseState.LeftButton ||
                newState.RightButton != _prevMouseState.RightButton ||
                newState.MiddleButton != _prevMouseState.MiddleButton){
                if (newState.LeftButton == ButtonState.Released){
                    //dispatch onbuttonreleased
                    foreach (ICanReceiveInputEvents subscriber in EventSubscribers) {
                        if (subscriber.OnLeftButtonRelease(newState) == InterruptState.InterruptEventDispatch){
                            PrematureMouseExit(newState);
                            return;
                        }
                    }


                    if (_clickTimer.ElapsedMilliseconds < 200){
                        //dispatch onclick
                        foreach (ICanReceiveInputEvents subscriber in EventSubscribers) {
                            if (subscriber.OnLeftButtonClick(newState) == InterruptState.InterruptEventDispatch){
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
                    foreach (ICanReceiveInputEvents subscriber in EventSubscribers) {
                        if (subscriber.OnLeftButtonPress(newState) == InterruptState.InterruptEventDispatch){
                            PrematureMouseExit(newState);
                            return;
                        }
                    }
                }
            }

            if (newState.X != _prevMouseState.X ||
                newState.Y != _prevMouseState.Y){
                bool interrupt = false;
                //dispatch onmovement
                foreach (ICanReceiveInputEvents subscriber in EventSubscribers) {
                    if (subscriber.OnMouseMovement(newState) == InterruptState.InterruptEventDispatch){
                        interrupt = true;
                        //PrematureMouseExit(newState);
                        //return;
                    }
                }
                int dx = newState.X - _prevMouseState.X;
                int dy = newState.Y - _prevMouseState.Y;

                if (!interrupt){
                    if (newState.LeftButton == ButtonState.Pressed){
                        Renderer.CameraPhi += dy*0.01f;
                        Renderer.CameraTheta -= dx*0.01f;

                        if (Renderer.CameraPhi > 1.56f){
                            Renderer.CameraPhi = 1.56f;
                        }
                        if (Renderer.CameraPhi < -1.56f) {
                            Renderer.CameraPhi = -1.56f;
                        }
                    }
                }
                _mousePos.EditText("phi:" + Renderer.CameraPhi + "  theta:" + Renderer.CameraTheta);
            }
            if (newState.ScrollWheelValue != _prevMouseState.ScrollWheelValue) {
                Renderer.CameraDistance += (_prevMouseState.ScrollWheelValue-newState.ScrollWheelValue)/5f;
                if (Renderer.CameraDistance < 50){
                    Renderer.CameraDistance = 50;
                }
                foreach (ICanReceiveInputEvents subscriber in EventSubscribers) {
                    if (subscriber.OnMouseScroll(newState) == InterruptState.InterruptEventDispatch) {
                        PrematureMouseExit(newState);
                        return;
                    }
                }
            }

            _prevMouseState = newState;
        }

        private static void PrematureMouseExit(MouseState state){
            _prevMouseState = state;
        }

        private static void UpdateKeyboard(){
            var state = Keyboard.GetState();

            if (state != _prevKeyboardState){
                foreach (ICanReceiveInputEvents subscriber in EventSubscribers){
                    if (subscriber.OnKeyboardEvent(state) == InterruptState.InterruptEventDispatch) {
                        break;
                    }
                }
            }

            if (state.IsKeyDown(Keys.W)){
                Renderer.CameraTarget.X += 5f;
            }
            if (state.IsKeyDown(Keys.A)) {
                Renderer.CameraTarget.Y += 5f;
            }
            if (state.IsKeyDown(Keys.S)) {
                Renderer.CameraTarget.X -= 5f;
            }
            if (state.IsKeyDown(Keys.D)) {
                Renderer.CameraTarget.Y -= 5f;
            }

            _prevKeyboardState = state;
        }
    }
    #region depth sorted list

    internal class DepthSortedList : IEnumerable {
        private readonly List<float> _depthList;
        private readonly List<ICanReceiveInputEvents> _objList;

        public DepthSortedList() {
            _depthList = new List<float>();
            _objList = new List<ICanReceiveInputEvents>();
        }

        public int Count {
            get { return _depthList.Count; }
        }

        public ICanReceiveInputEvents this[int index] {
            get { return _objList[index]; }
        }

        public void Add(float depth, ICanReceiveInputEvents element) {
            _depthList.Add(depth);
            _objList.Add(element);

            for (int i = _depthList.Count - 1; i < 0; i--) {
                if (_depthList[i] < _depthList[i - 1]) {
                    _depthList.RemoveAt(i);
                    _objList.RemoveAt(i);
                    _depthList.Insert(i - 2, depth);
                    _objList.Insert(i - 2, element);
                }
                else {
                    break;
                }
            }
        }

        public void Clear() {
            _depthList.Clear();
            _objList.Clear();
        }

        public void RemoveAt(int index) {
            _depthList.RemoveAt(index);
            _objList.RemoveAt(index);
        }

        public void Remove(ICanReceiveInputEvents element) {
            int i = 0;
            while (_objList[i] == element) {
                i++;
                if (i == _objList.Count) {
                    //return;
                }
            }
            RemoveAt(i);
        }

        public IEnumerator GetEnumerator(){
            return _objList.GetEnumerator();
        }
    }

    #endregion
}