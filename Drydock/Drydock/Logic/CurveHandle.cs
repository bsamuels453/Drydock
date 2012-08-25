#region

using System;
using Drydock.UI;
using Drydock.UI.Components;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Logic{
    internal class CurveHandle{
        #region HandleMovementRestriction enum

        public enum HandleMovementRestriction{
            NoRotationOnX,
            NoRotationOnY,
            Vertical,
            Horizontal
        }

        #endregion

        readonly Button _centerHandle;
        readonly Button _nextHandle;
        readonly Line _nextLine;
        readonly Button _prevHandle;
        readonly Line _prevLine;
        public CurveHandle SymmetricHandle;
        public TranslateDragToExtern TranslateToExtern;
        bool _dontTranslateHandles;
        bool _handleSymmetry;
        int _reflectionX;
        int _reflectionY;
        HandleMovementRestriction _rotRestriction;

        /// <summary>
        /// </summary>
        /// <param name="buttonTemplate"> </param>
        /// <param name="lineTemplate"> </param>
        /// <param name="uicollection"> </param>
        /// <param name="pos"> </param>
        /// <param name="prevComponent"> </param>
        /// <param name="nextComponent"> </param>
        public CurveHandle(ButtonGenerator buttonTemplate, LineGenerator lineTemplate, UIElementCollection uicollection, Vector2 pos, Vector2 prevComponent, Vector2 nextComponent){
            buttonTemplate.Identifier = (int) HandleType.Center;
            buttonTemplate.X = pos.X;
            buttonTemplate.Y = pos.Y;
            _centerHandle = uicollection.Add<Button>(buttonTemplate.GenerateButton());
            _centerHandle.GetComponent<DraggableComponent>().DragMovementDispatcher += TranslateToLinks;
            _centerHandle.GetComponent<DraggableComponent>().DragMovementClamp += ClampHandleMovement;

            buttonTemplate.Identifier = (int) HandleType.Prev;
            buttonTemplate.X = prevComponent.X + pos.X;
            buttonTemplate.Y = prevComponent.Y + pos.Y;
            _prevHandle = uicollection.Add<Button>(buttonTemplate.GenerateButton());
            _prevHandle.GetComponent<DraggableComponent>().DragMovementDispatcher += TranslateToLinks;
            _prevHandle.GetComponent<DraggableComponent>().DragMovementClamp += ClampHandleMovement;

            buttonTemplate.Identifier = (int) HandleType.Next;
            buttonTemplate.X = nextComponent.X + pos.X;
            buttonTemplate.Y = nextComponent.Y + pos.Y;
            _nextHandle = uicollection.Add<Button>(buttonTemplate.GenerateButton());
            _nextHandle.GetComponent<DraggableComponent>().DragMovementDispatcher += TranslateToLinks;
            _nextHandle.GetComponent<DraggableComponent>().DragMovementClamp += ClampHandleMovement;

            _prevLine = uicollection.Add<Line>(lineTemplate.GenerateLine());
            _nextLine = uicollection.Add<Line>(lineTemplate.GenerateLine());

            _prevLine.OriginPoint = _centerHandle.CentPosition;
            _prevLine.DestPoint = _prevHandle.CentPosition;

            _nextLine.OriginPoint = _centerHandle.CentPosition;
            _nextLine.DestPoint = _nextHandle.CentPosition;

            InterlinkButtonEvents();
        }

        public Vector2 CentPosition{
            get { return _centerHandle.CentPosition; }
        }

        public Vector2 NextPosition{
            get { return _nextHandle.CentPosition; }
        }

        public Vector2 PrevPosition{
            get { return _prevHandle.CentPosition; }
        }

        public float PrevLength{
            get { return _prevLine.Length; }
        }

        public float NextLength{
            get { return _nextLine.Length; }
        }

        public float Angle{
            set{
                _prevLine.Angle = value;
                _nextLine.Angle = (float) (value + Math.PI);
                _prevHandle.CentPosition = _prevLine.DestPoint;
                _nextHandle.CentPosition = _nextLine.DestPoint;
            }
            get { return _prevLine.Angle; }
        }

        public void SetReflectionType(PanelAlias panelType, HandleMovementRestriction restrictionType, bool hasInternalSymmetry = false){
            _handleSymmetry = hasInternalSymmetry;
            switch (panelType){
                case PanelAlias.Side:
                    _rotRestriction = restrictionType; //vert
                    _reflectionX = 0;
                    _reflectionY = 1;
                    _dontTranslateHandles = true;
                    break;
                case PanelAlias.Top:
                    _rotRestriction = restrictionType; //vert
                    _reflectionX = 1;
                    _reflectionY = -1;
                    _dontTranslateHandles = false;
                    break;
                case PanelAlias.Back:
                    _rotRestriction = restrictionType; //vert
                    _reflectionX = -1;
                    _reflectionY = 1;
                    _dontTranslateHandles = false;
                    break;
            }
        }

        public void TranslatePosition(int dx, int dy){
            RawCenterTranslate(dx, dy);
        }

        void ClampHandleMovement(IUIInteractiveElement owner, ref int x, ref int y, int oldX, int oldY){
            var button = (Button) owner;
            int dx = x - oldX;
            int dy = y - oldY;
            if (button == _prevHandle || button == _nextHandle){
                if (Common.GetDist(x, y, (int) _centerHandle.X, (int) _centerHandle.Y) < 20){
                    float scale = Common.GetDist(x, y, (int) _centerHandle.X, (int) _centerHandle.Y)/20;
                    dx = 0; //(int)(scale *dx);
                    dy = 0; //(int)(scale * dy);
                    x = oldX + dx;
                    y = oldY + dy;
                }
            }
            switch (_rotRestriction){
                case HandleMovementRestriction.Vertical:
                    if ((HandleType) button.Identifier != HandleType.Center){
                        Button handle = (HandleType) button.Identifier == HandleType.Prev ? _prevHandle : _nextHandle;
                        bool isHandleOnLeftSide = handle.CentPosition.X < _centerHandle.CentPosition.X;
                        if (isHandleOnLeftSide){
                            if (handle.CentPosition.X + dx >= _centerHandle.CentPosition.X){
                                x = (int) _centerHandle.X - 1;
                            }
                        }
                        else{
                            if (handle.CentPosition.X + dx <= _centerHandle.CentPosition.X){
                                x = (int) _centerHandle.X + 1;
                            }
                        }
                    }
                    break;
                case HandleMovementRestriction.Horizontal:
                    if ((HandleType) button.Identifier != HandleType.Center){
                        Button handle = (HandleType) button.Identifier == HandleType.Prev ? _prevHandle : _nextHandle;
                        bool isHandleOnLeftSide = handle.CentPosition.Y < _centerHandle.CentPosition.Y;
                        if (isHandleOnLeftSide){
                            if (handle.CentPosition.Y + dy >= _centerHandle.CentPosition.Y){
                                y = (int) _centerHandle.Y - 1;
                            }
                        }
                        else{
                            if (handle.CentPosition.Y + dy <= _centerHandle.CentPosition.Y){
                                y = (int) _centerHandle.Y + 1;
                            }
                        }
                    }
                    break;

                case HandleMovementRestriction.NoRotationOnX:
                    if ((HandleType) button.Identifier == HandleType.Center){
                        y = oldY;
                    }
                    else{
                        x = oldX;
                    }
                    break;
                case HandleMovementRestriction.NoRotationOnY:
                    if ((HandleType) button.Identifier == HandleType.Center){
                        x = oldX;
                    }
                    else{
                        y = oldY;
                    }
                    break;
            }
        }

        void RawCenterTranslate(int dx, int dy){
            _centerHandle.X += dx;
            _centerHandle.Y += dy;
            _prevHandle.X += dx;
            _prevHandle.Y += dy;

            _nextHandle.X += dx;
            _nextHandle.Y += dy;

            _prevLine.TranslateDestination(dx, dy);
            _prevLine.TranslateOrigin(dx, dy);
            _nextLine.TranslateDestination(dx, dy);
            _nextLine.TranslateOrigin(dx, dy);
        }

        void RawPrevTranslate(int dx, int dy){
            _prevHandle.X += dx;
            _prevHandle.Y += dy;

            _prevLine.TranslateDestination(dx, dy);
            _nextLine.Angle = (float) (_prevLine.Angle + Math.PI);

            _nextHandle.X = _nextLine.DestPoint.X - _nextHandle.BoundingBox.Width/2;
            _nextHandle.Y = _nextLine.DestPoint.Y - _nextHandle.BoundingBox.Height/2;
        }

        void RawNextTranslate(int dx, int dy){
            _nextHandle.X += dx;
            _nextHandle.Y += dy;
            _nextLine.TranslateDestination(dx, dy);
            _prevLine.Angle = (float) (_nextLine.Angle + Math.PI);

            _prevHandle.X = _prevLine.DestPoint.X - _prevHandle.BoundingBox.Width/2;
            _prevHandle.Y = _prevLine.DestPoint.Y - _prevHandle.BoundingBox.Height/2;
        }

        void TranslateToLinks(object caller, int dx, int dy){
            var obj = (Button) caller;
            switch ((HandleType) obj.Identifier){
                case HandleType.Center:
                    _prevHandle.X += dx;
                    _prevHandle.Y += dy;

                    _nextHandle.X += dx;
                    _nextHandle.Y += dy;

                    _prevLine.TranslateDestination(dx, dy);
                    _prevLine.TranslateOrigin(dx, dy);
                    _nextLine.TranslateDestination(dx, dy);
                    _nextLine.TranslateOrigin(dx, dy);
                    if (SymmetricHandle != null){
                        SymmetricHandle.RawCenterTranslate(_reflectionX*dx, _reflectionY*dy);
                    }
                    if (TranslateToExtern != null){
                        float dxf = dx;
                        float dyf = dy;

                        TranslateToExtern(this, ref dxf, ref dyf, true);
                    }
                    break;
                case HandleType.Prev:
                    _prevLine.TranslateDestination(dx, dy);
                    _nextLine.Angle = (float) (_prevLine.Angle + Math.PI);

                    _nextHandle.X = _nextLine.DestPoint.X - _nextHandle.BoundingBox.Width/2;
                    _nextHandle.Y = _nextLine.DestPoint.Y - _nextHandle.BoundingBox.Height/2;

                    if (SymmetricHandle != null && !_dontTranslateHandles){
                        SymmetricHandle.RawNextTranslate(_reflectionX*dx, _reflectionY*dy);
                    }
                    if (_handleSymmetry){
                        RawNextTranslate(dx*_reflectionX, dy*_reflectionY);
                    }
                    break;
                case HandleType.Next:
                    _nextLine.TranslateDestination(dx, dy);
                    _prevLine.Angle = (float) (_nextLine.Angle - Math.PI);

                    _prevHandle.X = _prevLine.DestPoint.X - _prevHandle.BoundingBox.Width/2;
                    _prevHandle.Y = _prevLine.DestPoint.Y - _prevHandle.BoundingBox.Height/2;
                    if (SymmetricHandle != null && !_dontTranslateHandles){
                        SymmetricHandle.RawPrevTranslate(_reflectionX*dx, _reflectionY*dy);
                    }
                    if (_handleSymmetry){
                        RawPrevTranslate(dx*_reflectionX, dy*_reflectionY);
                    }
                    break;
            }
        }

        void InterlinkButtonEvents(){
            FadeComponent.LinkFadeComponentTriggers(_prevHandle, _nextHandle, FadeComponent.FadeTrigger.EntryExit);
            FadeComponent.LinkFadeComponentTriggers(_prevHandle, _centerHandle, FadeComponent.FadeTrigger.EntryExit);
            FadeComponent.LinkFadeComponentTriggers(_nextHandle, _centerHandle, FadeComponent.FadeTrigger.EntryExit);


            FadeComponent.LinkOnewayFadeComponentTriggers(
                eventProcElements: new IUIElement[]{
                    _prevHandle,
                    _nextHandle,
                    _centerHandle
                },
                eventRecieveElements: new IUIElement[]{
                    _prevLine,
                    _nextLine
                },
                state: FadeComponent.FadeTrigger.EntryExit
                );
        }

        #region Nested type: HandleType

        enum HandleType{
            Center,
            Prev,
            Next
        }

        #endregion
    }
}