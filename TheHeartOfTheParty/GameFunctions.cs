using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkEventDispatcher;

namespace TheHeartOfTheParty;

internal unsafe class GameFunctions {
    [Signature("E8 ?? ?? ?? ?? 89 7B 44 EB 05")]
    private readonly delegate* unmanaged<AgentInterface*, uint*, byte> _setTitle;

    internal GameFunctions() {
        Plugin.GameInteropProvider.InitializeFromAttributes(this);
    }

    internal void RequestTitles() {
        var titleList = UIState.Instance()->TitleList;
        if (titleList.DataPending) { 
            return;
        }

        titleList.RequestTitleList();
    }

    internal bool IsTitleUnlocked(uint titleId) {
        if (titleId > ushort.MaxValue) {
            return false;
        }


        return UIState.Instance()->TitleList.IsTitleUnlocked((ushort)titleId);
    }

    internal bool SetTitle(uint titleId)
    {
        var agent = Framework.Instance()->GetUIModule()->GetAgentModule()->GetAgentByInternalId(AgentId.CharacterTitle);
        if (agent == null) {
            return false;
        }

        return this._setTitle(agent, &titleId) != 0;
    }
}
