#region

using Drydock.Control;
using Drydock.Logic.DoodadEditorState.Tools;
using Drydock.UI;
using Drydock.UI.Widgets;

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

        public DoodadUI(HullDataManager hullData){
            _hullData = hullData;

            var buttonGen = new ButtonGenerator("ToolbarButton64.json");
            buttonGen.X = 50;
            buttonGen.Y = 50;
            buttonGen.TextureName = "DeckNavArrowUp";
            _deckUpButton = buttonGen.GenerateButton();
            buttonGen.Y = 50 + 64;
            buttonGen.TextureName = "DeckNavArrowDown";
            _deckDownButton = buttonGen.GenerateButton();
            _deckUpButton.OnLeftClickDispatcher += AddVisibleLevel;
            _deckDownButton.OnLeftClickDispatcher += RemoveVisibleLevel;


            _toolBar = new Toolbar("Templates/DoodadToolbar.json");

            _toolBar.BindButtonToTool(0, new WallMenuTool(
                                             hullData)
                );

            /*_toolBar.BindButtonToTool(1, new LadderBuildTool(
                                             geometryInfo,
                                             VisibleDecks
                                             ));*/
        }

        #region IInputUpdates Members

        public void UpdateInput(ref ControlState state){
            _toolBar.UpdateInput(ref state);
        }

        #endregion

        #region ILogicUpdates Members

        public void UpdateLogic(double timeDelta){
            _toolBar.UpdateLogic(timeDelta);
        }

        #endregion

        void AddVisibleLevel(int identifier){
            _hullData.MoveUpOneDeck();
        }

        void RemoveVisibleLevel(int identifier){
            _hullData.MoveDownOneDeck();
        }
    }
}