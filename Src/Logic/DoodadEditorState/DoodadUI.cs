#region

using Drydock.Control;
using Drydock.Logic.DoodadEditorState.Tools;
using Drydock.Render;
using Drydock.UI;
using Drydock.UI.Widgets;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Logic.DoodadEditorState{
    /// <summary>
    ///   this class handles the display of the prototype airship and all of its components
    /// </summary>
    internal class DoodadUI : IInputUpdates, ILogicUpdates{
        readonly Button _deckDownButton;
        readonly Button _deckUpButton;
        readonly HullDataManager _hullData;

        readonly Toolbar _toolBar;

        public DoodadUI(HullDataManager hullData, RenderTarget target, GamestateManager manager){
            _hullData = hullData;

            var buttonGen = new ButtonGenerator("ToolbarButton64.json");
            buttonGen.Target = target;
            buttonGen.X = 50;
            buttonGen.Y = 50;
            buttonGen.TextureName = "UI_DeckNavArrowUp";
            _deckUpButton = buttonGen.GenerateButton();
            buttonGen.Y = 50 + 64;
            buttonGen.TextureName = "UI_DeckNavArrowDown";
            _deckDownButton = buttonGen.GenerateButton();
            _deckUpButton.OnLeftClickDispatcher += AddVisibleLevel;
            _deckDownButton.OnLeftClickDispatcher += RemoveVisibleLevel;


            _toolBar = new Toolbar(target, "Templates/DoodadToolbar.json");

            _toolBar.BindButtonToTool(0, new WallMenuTool(hullData, target, manager));

            _toolBar.BindButtonToTool(1, new LadderBuildTool(hullData, manager));
        }

        #region IInputUpdates Members

        public void UpdateInput(ref InputState state){
            _toolBar.UpdateInput(ref state);
        }

        #endregion

        #region ILogicUpdates Members

        public void UpdateLogic(double timeDelta){
            _toolBar.UpdateLogic(timeDelta);
        }

        #endregion

        public void Draw(Matrix viewMatrix){
            _deckDownButton.Draw();
            _deckUpButton.Draw();
            _toolBar.Draw(viewMatrix);
        }

        void AddVisibleLevel(int identifier){
            _hullData.MoveUpOneDeck();
        }

        void RemoveVisibleLevel(int identifier){
            _hullData.MoveDownOneDeck();
        }
    }
}