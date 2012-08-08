namespace Drydock.Logic {
    class HullEditor {
        HullEditorPanel _sidepanel;


        public HullEditor(){
            _sidepanel = new HullEditorPanel(0, 0, 500, 500, "Config Files/SideCurveControllerDefaults.xml");
        }

        public void Update(){
            _sidepanel.Update();
        }
    }
}
