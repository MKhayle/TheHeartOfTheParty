﻿using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace TheHeartOfTheParty;

internal unsafe class GameFunctions {

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

    internal void SetTitle(uint titleId, string title)
    {
        if (titleId != 0)
        {
            using var titleStr = new Utf8String(title);
              RaptureLogModule.Instance()->ShowLogMessageString(3846u, &titleStr);
        }

        UIState.Instance()->TitleController.SendTitleIdUpdate((ushort)titleId);
    }
}
