using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace TheHeartOfTheParty; 

internal class PluginUi : IDisposable {
    private Plugin Plugin { get; }
    
    private string _searchText = "";
    private bool _unlockedOnly = true;
    
    internal PluginUi(Plugin plugin) {
        this.Plugin = plugin;
        
        this.Plugin.Interface.UiBuilder.Draw += this.OnDraw;
    }
    
    public void Dispose() {
        this.Plugin.Interface.UiBuilder.Draw -= this.OnDraw;
    }

    private void OnDraw() {
        if (!ImGui.Begin(this.Plugin.Name)) {
            ImGui.End();
            return;
        }
        
        ImGui.Checkbox("Only show unlocked titles", ref this._unlockedOnly);
        
        ImGui.SetNextItemWidth(-1);
        ImGui.InputTextWithHint("##search", "Search...", ref this._searchText, 64);

        var fem = true;
        if (this.Plugin.ClientState.LocalPlayer is {} player) {
            fem = player.Customize[1] == 1;
        }

        var titles = this.Plugin.DataManager.GetExcelSheet<Title>()!
            .Where(row => row.Order != 0)
            .Select(row => (row, this.Plugin.GameFunctions.IsTitleUnlocked(row.RowId)));
        
        if (this._unlockedOnly) {
            titles = titles.Where(t => t.Item2);
        }
        
        if (this._searchText.Length > 0) {
            var search = this._searchText.ToLower();
            titles = titles.Where(t => t.Item1.Feminine.RawString.ToLower().Contains(search) || t.Item1.Masculine.RawString.ToLower().Contains(search));
        }
        
        titles = titles.OrderBy(t => t.Item1.Order);
        
        if (ImGui.BeginChild("##titles")) {
            foreach (var (title, unlocked) in titles) {
                var name = fem ? title.Feminine : title.Masculine;

                if (unlocked) {
                    if (ImGui.Selectable(name)) {
                        this.Plugin.GameFunctions.SetTitle(title.RowId);
                    }
                } else {
                    ImGui.TextDisabled(name);
                }
            }
            
            ImGui.EndChild();
        }

        ImGui.End();
    }
}
