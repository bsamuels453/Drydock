#region

using Drydock.Control;
using Drydock.Render;
using Drydock.UI.Widgets;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal class WallMenuTool : IToolbarTool{
        readonly Toolbar _toolbar;
        //todo: break this and put it in doodadui
        public WallMenuTool(HullDataManager hullData, RenderTarget target, GamestateManager manager){
            _toolbar = new Toolbar(target, "Templates/BuildToolbar.json");
            _toolbar.Enabled = false;

            _toolbar.BindButtonToTool(
                0,
                new WallBuildTool(hullData, manager)
                );

            _toolbar.BindButtonToTool(
                1,
                new WallDeleteTool(hullData, manager)
                );
        }

        #region IToolbarTool Members

        public void UpdateInput(ref InputState state){
            _toolbar.UpdateInput(ref state);
        }

        public void UpdateLogic(double timeDelta){
            _toolbar.UpdateLogic(timeDelta);
        }

        public bool Enabled{
            get { return _toolbar.Enabled; }
            set { _toolbar.Enabled = value; }
        }

        public void Draw(Matrix viewMatrix){
            _toolbar.Draw(viewMatrix);
        }

        #endregion
    }
}