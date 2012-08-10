#region

using System.IO;
using System.Xml;
using Drydock.UI;
using Drydock.Utilities;

#endregion

namespace Drydock.Logic{
    internal class HullEditorPanel{
        private readonly Button _background;
        private readonly FloatingRectangle _boundingBox;
        private readonly CurveControllerCollection _curves;
        private readonly UIElementCollection _elementCollection;

        public HullEditorPanel(int x, int y, int width, int height, string defaultCurveConfiguration){
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

        }

        public void SaveCurves(string fileName){
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            Stream outputStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            var writer = XmlWriter.Create(outputStream, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Data");
            writer.WriteElementString("NumControllers", null, _curves.CurveList.Count.ToString());
            float offsetX = _curves.CurveList[0].HandlePos.X;
            float offsetY = _curves.CurveList[0].HandlePos.Y;
            for (int i = 0; i < _curves.CurveList.Count; i++){
                writer.WriteStartElement("Handle" + i, null);
                writer.WriteElementString("PosX", null, ((_curves.CurveList[i].HandlePos.X - offsetX) / _curves.PixelsPerMeter).ToString());
                writer.WriteElementString("PosY", null, ((_curves.CurveList[i].HandlePos.Y - offsetY) / _curves.PixelsPerMeter).ToString());
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
