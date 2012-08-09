using Drydock.Render;

namespace Drydock.Logic {
    class HullEditor {
        HullEditorPanel _sidepanel;


        public HullEditor(){

            _sidepanel = new HullEditorPanel(0, 0, ScreenData.GetScreenValueX(0.5f), ScreenData.GetScreenValueY(0.5f), "Config Files/SideCurveControllerDefaults.xml");
        }

        public void Update(){
            _sidepanel.Update();
        }
    }
}
