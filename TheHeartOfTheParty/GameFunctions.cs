using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace TheHeartOfTheParty;

internal unsafe class GameFunctions {
    [Signature("E8 ?? ?? ?? ?? 89 7B 44 EB 05")]
    private readonly delegate* unmanaged<AgentInterface*, uint*, byte> _setTitle;

    [Signature("40 53 48 83 EC 30 80 79 6A 00")]
    private readonly delegate* unmanaged<IntPtr, void> _requestTitles;

    [Signature("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 89 6E 58 B8", ScanType = ScanType.StaticAddress)]
    private readonly IntPtr _titleList;

    [Signature("BA ?? ?? ?? ?? E8 ?? ?? ?? ?? 41 8B 4D 08", Offset = 1)]
    private uint _agentId;

    internal GameFunctions() {
        Plugin.GameInteropProvider.InitializeFromAttributes(this);
    }

    internal void RequestTitles() {
        if (*(byte*) (this._titleList + 0x61) == 1) {
            return;
        }

        this._requestTitles(this._titleList);
    }

    internal bool IsTitleUnlocked(uint titleId) {
        if (titleId > ushort.MaxValue) {
            return false;
        }

        return ((TitleList*) this._titleList)->IsTitleUnlocked((ushort) titleId);
    }

    internal bool SetTitle(uint titleId) {
        var agent = Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId((AgentId) this._agentId);
        if (agent == null) {
            return false;
        }

        return this._setTitle(agent, &titleId) != 0;
    }
}
