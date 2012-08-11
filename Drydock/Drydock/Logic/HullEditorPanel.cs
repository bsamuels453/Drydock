#region

using System;
using System.IO;
using System.Xml;
using Drydock.UI;
using Drydock.Utilities;

#endregion


namespace Drydock.Logic{
    /// <summary>
    /// reflects what kind of curve this panel represents, which indicates
    /// how movement of certain handles should effect the movement of external handles
    /// </summary>
    enum CurveType {
        Side,
        Top
    }
    delegate void ModifyHandlePosition(HandleAlias handle, float dx, float dy);

    internal enum HandleAlias{
        First,
        Middle,
        Last
    }

    internal class HullEditorPanel{
        private readonly Button _background;
        private readonly FloatingRectangle _boundingBox;
        private readonly CurveControllerCollection _curves;
        private readonly UIElementCollection _elementCollection;
        private readonly CurveType _curveType;
        public ModifyHandlePosition ExternalHandleModifier;

        public HullEditorPanel(CurveType curveType, int x, int y, int width, int height, string defaultCurveConfiguration){
            _curveType = curveType;
            _boundingBox = new FloatingRectangle(x, y, width, height);
            _elementCollection = new UIElementCollection();
            _curves = new CurveControllerCollection(
                defaultConfig: defaultCurveConfiguration,
                areaToFill: new FloatingRectangle(
                    x + width*0.1f,
                    y + height*0.1f,
                    width - width*0.2f,
                    height - height*0.2f
                    ),
                parentCollection: _elementCollection
                );
            _curves.ElementCollection.AddDragConstraintCallback(ClampChildElements);
            _background = _elementCollection.Add<Button>(
                new Button(
                    x: x,
                    y: y,
                    width: width,
                    height: height,
                    depth: DepthLevel.Background,
                    owner: _elementCollection,
                    textureName: "panelBG",
                    spriteTexRepeatX: width/(_curves.PixelsPerMeter*10),
                    spriteTexRepeatY: height/(_curves.PixelsPerMeter*10)
                    )
                );


            switch(curveType){
                case CurveType.Side:
                    _curves.CurveList[0].Controller.ReactToControllerMovement = OnDragMovement;
                    _curves.CurveList[_curves.CurveList.Count - 1].Controller.ReactToControllerMovement += OnDragMovement;
                    break;

                case CurveType.Top:
                    _curves.CurveList[0].Controller.ReactToControllerMovement += OnDragMovement;
                    _curves.CurveList[_curves.CurveList.Count-1].Controller.ReactToControllerMovement += OnDragMovement;
                    _curves.CurveList[_curves.CurveList.Count/2].Controller.ReactToControllerMovement += OnDragMovement;
                    break;

            }
        }
        private void OnDragMovement(object caller, int dx, int dy) {
            float dxf = dx/_curves.PixelsPerMeter;
            float dyf = dy/ _curves.PixelsPerMeter;//unused?

            var controller = (CurveController)caller;
            switch (_curveType){
                case CurveType.Side:
                    if (controller == _curves.CurveList[0].Controller) {
                        _curves.CurveList[_curves.CurveList.Count - 1].Controller.TranslateControllerPos(0, dy);

                            if (ExternalHandleModifier != null) {
                                ExternalHandleModifier(HandleAlias.Middle, dxf, 0);
                            }
                            else {
                                throw new Exception("Hull editor panels were not linked correctly");
                            }
                        
                    }
                    else{//assume that it's the other handle
                        _curves.CurveList[0].Controller.TranslateControllerPos(0, dy);

                            if (ExternalHandleModifier != null) {
                                ExternalHandleModifier(HandleAlias.Last, dxf, 0);
                                ExternalHandleModifier(HandleAlias.First, dxf, 0);
                            }
                            else {
                                throw new Exception("Hull editor panels were not linked correctly");
                            }
                        
                    }

                    break;


                case CurveType.Top:
                    if (controller == _curves.CurveList[0].Controller) {
                        _curves.CurveList[_curves.CurveList.Count - 1].Controller.TranslateControllerPos(dx, -dy);

                            if (ExternalHandleModifier != null) {
                                ExternalHandleModifier(HandleAlias.Last, dxf, 0);
                            }
                            else {
                                throw new Exception("Hull editor panels were not linked correctly");
                            }
                        
                    }
                    if (controller == _curves.CurveList[_curves.CurveList.Count - 1].Controller) {
                        _curves.CurveList[0].Controller.TranslateControllerPos(dx, -dy);

                            if (ExternalHandleModifier != null) {
                                ExternalHandleModifier(HandleAlias.Last, dxf, 0);
                            }
                            else {
                                throw new Exception("Hull editor panels were not linked correctly");
                            }
                        
                    }
                    if (controller == _curves.CurveList[_curves.CurveList.Count / 2].Controller) {
                        //_curves.CurveList[0].Controller.TranslateControllerPos(dx, dy);
                       // _curves.CurveList[_curves.CurveList.Count - 1].Controller.TranslateControllerPos(dx, dy);

                            if (ExternalHandleModifier != null) {
                                ExternalHandleModifier(HandleAlias.First, dxf, 0);
                            }
                            else {
                                throw new Exception("Hull editor panels were not linked correctly");
                            }
                        
                    }

                    break;

            }

        }

        /// <summary>
        /// this function accepts modifications in METERS
        /// </summary>
        /// <param name="handle"> </param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void ModifyHandlePosition(HandleAlias handle, float dx, float dy) {
            dx *= _curves.PixelsPerMeter;
            dy *= _curves.PixelsPerMeter;
            var dxi = (int)Math.Round(dx);
            var dyi = (int)Math.Round(dy);

            switch (handle){
                case HandleAlias.First:
                    _curves.CurveList[0].Controller.TranslateControllerPos(dxi, dyi);
                    break;
                case HandleAlias.Middle:
                    _curves.CurveList[_curves.CurveList.Count / 2].Controller.TranslateControllerPos(dxi, dyi);
                    break;
                case HandleAlias.Last:
                    _curves.CurveList[_curves.CurveList.Count - 1].Controller.TranslateControllerPos(dxi, dyi);
                    break;
            }
        }

        public void SaveCurves(string fileName){
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            Stream outputStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            var writer = XmlWriter.Create(outputStream, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Data");
            writer.WriteElementString("NumControllers", null, _curves.CurveList.Count.ToString());

            float minX=9999999, minY=9999999;
            foreach (var curve in _curves.CurveList){
                if (curve.HandlePos.X < minX){
                    minX = curve.HandlePos.X;
                }
                if (curve.HandlePos.Y < minY) {
                    minY = curve.HandlePos.Y;
                }
            }
            for (int i = 0; i < _curves.CurveList.Count; i++){
                writer.WriteStartElement("Handle" + i, null);
                writer.WriteElementString("PosX", null, ((_curves.CurveList[i].HandlePos.X - minX) / _curves.PixelsPerMeter).ToString());
                writer.WriteElementString("PosY", null, ((_curves.CurveList[i].HandlePos.Y - minY) / _curves.PixelsPerMeter).ToString());
                writer.WriteElementString("Angle", null, _curves.CurveList[i].Angle.ToString());
                writer.WriteElementString("PrevLength", null, (_curves.CurveList[i].PrevHandleLength / _curves.PixelsPerMeter).ToString());
                writer.WriteElementString("NextLength", null, (_curves.CurveList[i].NextHandleLength / _curves.PixelsPerMeter).ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Close();
            outputStream.Close();

        }

        private void ClampChildElements(IUIInteractiveElement owner, ref int x, ref int y, int oldX, int oldY){
            if (x > _boundingBox.X + _boundingBox.Width || x < _boundingBox.X){
                x = oldX;
            }
            if (y > _boundingBox.Y + _boundingBox.Height || y < _boundingBox.Y){
                y = oldY;
            }
        }



        public void Update(){
            _curves.Update();

        }

    }
}
