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
            Horizontal,
            Quadrant
        }

        #endregion
        private delegate void ClampByNeighbors(ref float dx, ref float dy, Button button);
        const int _handleMinDist = 20;
        readonly Button _centerHandle;
        readonly Button _nextHandle;
        readonly Line _nextLine;
        readonly Button _prevHandle;
        readonly Line _prevLine;
        public CurveHandle SymmetricHandle;
        public CurveHandle NextHandle;
        public CurveHandle PrevHandle;
        public TranslateDragToExtern TranslateToExtern;
        bool _dontTranslateHandles;
        bool _internalSymmetry;
        int _reflectionX;
        int _reflectionY;
        HandleMovementRestriction _rotRestriction;
        ClampByNeighbors _clampByNeighbors;

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
            
            _nextHandle.GetComponent<FadeComponent>().ForceFadeout();
            _prevHandle.GetComponent<FadeComponent>().ForceFadeout();
            _centerHandle.GetComponent<FadeComponent>().ForceFadeout();

            _prevLine.GetComponent<FadeComponent>().ForceFadeout();
            _nextLine.GetComponent<FadeComponent>().ForceFadeout();

            _nextHandle.GetComponent<FadeComponent>().ForceFadeout();
            _prevHandle.GetComponent<FadeComponent>().ForceFadeout();
            _centerHandle.GetComponent<FadeComponent>().ForceFadeout();

            _prevLine.GetComponent<FadeComponent>().ForceFadeout();
            _nextLine.GetComponent<FadeComponent>().ForceFadeout();


            InterlinkButtonEvents();
        }

        #region properties

        public Vector2 CentButtonCenter{
            get { return _centerHandle.CentPosition; }
        }

        public Vector2 PrevButtonCenter{
            get { return _prevHandle.CentPosition; } 
        }

        public Vector2 NextButtonCenter {
            get { return _nextHandle.CentPosition; }
        }

        public Vector2 CentButtonPos{
            get { return _centerHandle.CentPosition; }
        }

        public Vector2 NextButtonPos{
            get { return _nextHandle.CentPosition; }
        }

        public Vector2 PrevButtonPos{
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
        #endregion

        public void SetReflectionType(PanelAlias panelType, HandleMovementRestriction restrictionType, bool hasInternalSymmetry = false){
            _internalSymmetry = hasInternalSymmetry;
            switch (panelType){
                case PanelAlias.Side:
                    _rotRestriction = restrictionType; //vert
                    _reflectionX = 0;
                    _reflectionY = 1;
                    _dontTranslateHandles = true;
                    _clampByNeighbors = SideNeighborClamp;
                    break;
                case PanelAlias.Top:
                    _rotRestriction = restrictionType; //vert
                    _reflectionX = 1;
                    _reflectionY = -1;
                    _dontTranslateHandles = false;
                    _clampByNeighbors = TopNeighborClamp;
                    if (hasInternalSymmetry) {
                        _reflectionX = 0;
                    }
                    break;
                case PanelAlias.Back:
                    _rotRestriction = restrictionType; //vert
                    _reflectionX = -1;
                    _reflectionY = 1;
                    _dontTranslateHandles = false;
                    _clampByNeighbors = BackNeighborClamp;
                    if (hasInternalSymmetry) {
                        _reflectionY = 0;
                    }
                    break;
            }
        }

        public void TranslatePosition(float dx, float dy){
            BalancedCenterTranslate(dx, dy);
        }

        public void ClampPositionFromExternal(float dx, float dy){

        }

        void ClampHandleMovement(IUIInteractiveElement owner, ref int x, ref int y, int oldX, int oldY){
            var button = (Button)owner;
            float dx = x - oldX;
            float dy = y - oldY;
            InternalMovementClamp(ref dx, ref dy, button);
            x = (int)dx + oldX;
            y = (int)dy + oldY;

        }

        void InternalMovementClamp(ref float dx, ref float dy, Button button) {
            
            #region region clamp
            if (_rotRestriction == HandleMovementRestriction.Vertical || _rotRestriction == HandleMovementRestriction.Quadrant) {
                if ((HandleType)button.Identifier != HandleType.Center) {
                    Button handle = (HandleType)button.Identifier == HandleType.Prev ? _prevHandle : _nextHandle;
                    bool isHandleOnLeftSide = handle.CentPosition.X < _centerHandle.CentPosition.X;
                    if (isHandleOnLeftSide) {
                        if (handle.CentPosition.X + dx >= _centerHandle.CentPosition.X) {
                            dx = _centerHandle.X - handle.X - 1;
                        }
                    }
                    else {
                        if (handle.CentPosition.X + dx <= _centerHandle.CentPosition.X) {
                            dx = _centerHandle.X - handle.X + 1;
                        }
                    }
                }
            }
            if (_rotRestriction == HandleMovementRestriction.Horizontal || _rotRestriction == HandleMovementRestriction.Quadrant) {
                if ((HandleType)button.Identifier != HandleType.Center) {
                    Button handle = (HandleType)button.Identifier == HandleType.Prev ? _prevHandle : _nextHandle;
                    bool isHandleOnLeftSide = handle.CentPosition.Y < _centerHandle.CentPosition.Y;
                    if (isHandleOnLeftSide) {
                        if (handle.CentPosition.Y + dy >= _centerHandle.CentPosition.Y) {
                            dy = _centerHandle.Y - handle.Y - 1;
                        }
                    }
                    else {
                        if (handle.CentPosition.Y + dy <= _centerHandle.CentPosition.Y) {
                            dy = _centerHandle.Y - handle.Y + 1;
                        }
                    }
                }
            }
            if (_rotRestriction == HandleMovementRestriction.NoRotationOnX) {
                if (button == _centerHandle) {
                    dy = 0;
                }
                else {
                    dx = 0;
                    //this next part prevents handles from "crossing over" the center to the other side
                    if (button == _prevHandle) {
                        if (button.Y + dy >= _centerHandle.Y-9){
                            dy = _centerHandle.X - button.Y-10;
                        }
                    }
                    else{
                        if (button.Y + dy <= _centerHandle.Y+9) {
                            dy = _centerHandle.Y - button.Y +10;
                        }
                    }
                }
            }
            if (_rotRestriction == HandleMovementRestriction.NoRotationOnY) {
                if (button == _centerHandle) {
                    dx = 0;
                }
                else {
                    dy = 0;
                    //this next part prevents handles from "crossing over" the center to the other side
                    if (button == _prevHandle) {
                        if (button.X + dx >= _centerHandle.X-9){
                            dx = _centerHandle.X - button.X-10;
                        }
                    }
                    else{
                        if (button.X + dx <= _centerHandle.X+9) {
                            dx = _centerHandle.X - button.X + 10;
                        }
                    }
                }
            }
            #endregion

            #region distance clamp

            if (button == _prevHandle) {
                if (Common.GetDist((button.CentPosition.X + dx), (button.CentPosition.Y + dy), _centerHandle.CentPosition.X, _centerHandle.CentPosition.Y) < _handleMinDist) {
                    Vector2 tempDest = _prevLine.DestPoint - _prevLine.OriginPoint;
                    tempDest.X += dx;
                    tempDest.Y += dy;
                    tempDest.Normalize();
                    tempDest *= _handleMinDist;
                    tempDest += _prevLine.OriginPoint;

                    dx = tempDest.X - _prevLine.DestPoint.X;
                    dy = tempDest.Y - _prevLine.DestPoint.Y;
                }
            }
            if(button == _nextHandle){
                if (Common.GetDist((button.CentPosition.X + dx), (button.CentPosition.Y + dy), _centerHandle.CentPosition.X, _centerHandle.CentPosition.Y) < _handleMinDist) {
                    Vector2 tempDest = _nextLine.DestPoint - _nextLine.OriginPoint;
                    tempDest.X += dx;
                    tempDest.Y += dy;
                    tempDest.Normalize();
                    tempDest *= _handleMinDist;
                    tempDest += _nextLine.OriginPoint;

                    dx = tempDest.X - _nextLine.DestPoint.X;
                    dy = tempDest.Y - _nextLine.DestPoint.Y;
                }
            }
            
            #endregion
            _clampByNeighbors(ref dx, ref dy, button);
            
        }

        void BackNeighborClamp(ref float dx, ref float dy, Button button){
            //prevent symmetric buttons from crossing each other
            if (PrevHandle != null && NextHandle == null){
                if (button.CentPosition.X + dx < PrevHandle.CentButtonCenter.X){
                    dx = PrevHandle.CentButtonCenter.X - button.CentPosition.X;
                }
            }
            if (PrevHandle == null && NextHandle != null) {
                if (button.CentPosition.X + dx > NextHandle.CentButtonCenter.X){
                    dx = NextHandle.CentButtonCenter.X - button.CentPosition.X;
                }
            }

            //prevent buttons from crossing the middle handle's center position
            //also prevents center handle from crossing the two bounding handle's satellite buttons
            CurveHandle handleToUse = NextHandle ?? PrevHandle;
            switch ((HandleType) button.Identifier){
                case HandleType.Prev:
                    if (NextHandle == null || PrevHandle == null){ //assume  this is a bounding handle
                        if (button.CentPosition.Y + dy > handleToUse.PrevButtonCenter.Y){
                            dy = handleToUse.PrevButtonCenter.Y - _prevHandle.CentPosition.Y;
                        }
                    }

                    break;
                case HandleType.Center:
                    if (NextHandle != null && PrevHandle != null){ //assume this is the center curve handle
                        if (button.CentPosition.Y + dy < NextHandle.PrevButtonCenter.Y){
                            dy = NextHandle.PrevButtonCenter.Y - _centerHandle.CentPosition.Y;
                        }
                    }
                    break;
                case HandleType.Next:
                    if (NextHandle == null || PrevHandle == null){ //assume  this is a bounding handle
                        if (button.CentPosition.Y + dy > handleToUse.PrevButtonCenter.Y){
                            dy = handleToUse.PrevButtonCenter.Y - _nextHandle.CentPosition.Y;
                        }
                    }
                    break;
            }

            //prevents bounding handles from crossing center handle's satellite buttons (x+y direction)
            switch ((HandleType) button.Identifier){
                case HandleType.Center:
                    if (NextHandle == null && PrevHandle != null){//assume this is a next bounding handle
                        if (button.CentPosition.X + dx < PrevHandle.NextButtonCenter.X){
                            dx = PrevHandle.NextButtonCenter.X - button.CentPosition.X;
                        }
                        if (_prevHandle.CentPosition.Y + dy > PrevHandle.NextButtonCenter.Y) {
                            dy = PrevHandle.NextButtonCenter.Y - _prevHandle.CentPosition.Y;
                        }
                    }
                    if (NextHandle != null && PrevHandle == null) {//assume this is a prev bounding handle
                        if (button.CentPosition.X + dx > NextHandle.PrevButtonCenter.X) {
                            dx = NextHandle.PrevButtonCenter.X - button.CentPosition.X;
                        }
                        if (_nextHandle.CentPosition.Y + dy > NextHandle.NextButtonCenter.Y) {
                            dy = NextHandle.NextButtonCenter.Y - _nextHandle.CentPosition.Y;
                        }
                    }
                    break;
            }

            //prevents the center button's satellite buttons from crossing the bounding handle centers
            if (NextHandle != null && PrevHandle != null){
                if ((HandleType) button.Identifier == HandleType.Prev){
                    if (button.CentPosition.X + dx < PrevHandle.CentButtonCenter.X){
                        dx = PrevHandle.CentButtonCenter.X - button.CentPosition.X;
                    }
                }
                if ((HandleType)button.Identifier == HandleType.Next) {
                    if (button.CentPosition.X + dx > NextHandle.CentButtonCenter.X) {
                        dx = NextHandle.CentButtonCenter.X - button.CentPosition.X;
                    }
                }
            }
        }

        void TopNeighborClamp(ref float dx, ref float dy, Button button) {
            /*if (NextHandle != null) {
                if (button.CentPosition.Y + dy >= NextHandle.CentButtonCenter.Y) {
                    dy = _centerHandle.Y - NextHandle.CentButtonPos.Y + 1;
                }
            }
            if (PrevHandle != null) {
                if (button.CentPosition.Y + dy <= PrevHandle.CentButtonCenter.Y) {
                    dy = _centerHandle.Y - PrevHandle.CentButtonPos.Y - 1;
                }
            }*/
        }

        void SideNeighborClamp(ref float dx, ref float dy, Button button) {
            BackNeighborClamp(ref dx, ref dy, button);
            TopNeighborClamp(ref dx, ref dy, button);
        }

        void BalancedCenterTranslate(float dx, float dy) {
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
        }

        void BalancedPrevTranslate(int dx, int dy){
            RawPrevTranslate(dx, dy);
            _nextLine.Angle = (float) (_prevLine.Angle + Math.PI);

            _nextHandle.X = _nextLine.DestPoint.X - _nextHandle.BoundingBox.Width/2;
            _nextHandle.Y = _nextLine.DestPoint.Y - _nextHandle.BoundingBox.Height/2;
        }

        void RawNextTranslate(int dx, int dy){
            _nextHandle.X += dx;
            _nextHandle.Y += dy;
            _nextLine.TranslateDestination(dx, dy);
        }

        void BalancedNextTranslate(int dx, int dy){
            RawNextTranslate(dx, dy);
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
                        SymmetricHandle.BalancedCenterTranslate(_reflectionX*dx, _reflectionY*dy);
                    }
                    if (TranslateToExtern != null){
                        float dxf = dx;
                        float dyf = dy;

                        TranslateToExtern(this, ref dxf, ref dyf, false);
                    }
                    break;
                case HandleType.Prev:
                    _prevLine.TranslateDestination(dx, dy);
                    if (_internalSymmetry) {
                        RawNextTranslate(dx * _reflectionX, dy * _reflectionY);
                    }
                    else {
                        _nextLine.Angle = (float)(_prevLine.Angle + Math.PI);

                        _nextHandle.X = _nextLine.DestPoint.X - _nextHandle.BoundingBox.Width / 2;
                        _nextHandle.Y = _nextLine.DestPoint.Y - _nextHandle.BoundingBox.Height / 2;
                    }

                    if (SymmetricHandle != null && !_dontTranslateHandles){
                        SymmetricHandle.BalancedNextTranslate(_reflectionX*dx, _reflectionY*dy);
                    }

                    break;
                case HandleType.Next:
                    _nextLine.TranslateDestination(dx, dy);
                    if (_internalSymmetry) {
                        RawPrevTranslate(dx * _reflectionX, dy * _reflectionY);
                    }
                    else {
                        _prevLine.Angle = (float)(_nextLine.Angle - Math.PI);

                        _prevHandle.X = _prevLine.DestPoint.X - _prevHandle.BoundingBox.Width / 2;
                        _prevHandle.Y = _prevLine.DestPoint.Y - _prevHandle.BoundingBox.Height / 2;
                    }

                    if (SymmetricHandle != null && !_dontTranslateHandles){
                        SymmetricHandle.BalancedPrevTranslate(_reflectionX*dx, _reflectionY*dy);
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