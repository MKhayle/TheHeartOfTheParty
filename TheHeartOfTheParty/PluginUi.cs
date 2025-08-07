using System.Numerics;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace TheHeartOfTheParty;

internal class PluginUi : IDisposable {
    private Plugin Plugin { get; }

    private Dictionary<uint, Achievement> Achievements { get; } = new();

    private bool _visible;
    private string _searchText = string.Empty;

    internal PluginUi(Plugin plugin) {
        this.Plugin = plugin;

        foreach (var achievement in this.Plugin.DataManager.GetExcelSheet<Achievement>()!) {
            if (achievement.Title.RowId == 0) {
                continue;
            }

            this.Achievements[achievement.Title.RowId] = achievement;
        }

        this.Plugin.Interface.UiBuilder.Draw += this.OnDraw;
		this.Plugin.Interface.UiBuilder.OpenMainUi += this.Toggle;
	}

    public void Dispose() {
        this.Plugin.Interface.UiBuilder.Draw -= this.OnDraw;
		this.Plugin.Interface.UiBuilder.OpenMainUi -= this.Toggle;
	}

    internal void Toggle() {
        this._visible ^= true;
    }

    private void OnDraw() {
        if (!this._visible) {
            return;
        }

        ImGui.SetNextWindowSize(new Vector2(780, 450), ImGuiCond.FirstUseEver);

        if (!ImGui.Begin(Plugin.Name, ref this._visible)) {
            ImGui.End();
            return;
        }

        if (ImGui.IsWindowAppearing()) {
            this.Plugin.Functions.RequestTitles();
        }

        var anyChanged = false;

        anyChanged |= ImGui.Checkbox("Only show unlocked titles", ref this.Plugin.Config.OnlyUnlocked);

        if (ImGui.BeginCombo("Sort", this.Plugin.Config.SortOrder.ToString())) {
            foreach (var ordering in Enum.GetValues<SortOrder>()) {
                if (ImGui.Selectable(ordering.ToString(), this.Plugin.Config.SortOrder == ordering)) {
                    this.Plugin.Config.SortOrder = ordering;
                    anyChanged = true;
                }
            }

            ImGui.EndCombo();
        }

        if (anyChanged) {
            this.Plugin.SaveConfig();
        }

        var fem = true;
        if (this.Plugin.ClientState.LocalPlayer is { } player) {
            fem = player.Customize[1] == 1;
        }

        var titles = this.GetTitles(fem).ToList();

        var hint = titles.Count == 1 ? "Search 1 title..." : $"Search {titles.Count} titles...";

        ImGui.SetNextItemWidth(-1);
        ImGui.InputTextWithHint("##search", hint, ref this._searchText, 64);

        if (ImGui.BeginTable("##titles-table", 3, ImGuiTableFlags.ScrollY, ImGui.GetContentRegionAvail())) {
            ImGui.TableSetupColumn("Title", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Achievement", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Category", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupScrollFreeze(0, 1);

            ImGui.TableHeadersRow();

            foreach (var title in titles) {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();


                if (title.Unlocked) {
                    const ImGuiSelectableFlags flags = ImGuiSelectableFlags.SpanAllColumns
                                                       | ImGuiSelectableFlags.AllowItemOverlap;
                    // TODO: detect current title?
                    if (ImGui.Selectable(title.Text.ExtractText(), false, flags)) {
                        this.Plugin.Functions.SetTitle(title.Row.RowId, title.Text.ExtractText());
                    }
                } else {
                    ImGui.TextDisabled(title.Text.ExtractText());
                }

                ImGui.TableNextColumn();
                ImGui.TextUnformatted(title.Achievement?.Name.ExtractText() ?? "???");

                ImGui.TableNextColumn();
                ImGui.TextUnformatted(title.Achievement?.AchievementCategory.Value.AchievementKind.Value.Name.ExtractText() ?? "???");
            }

            ImGui.EndTable();
        }

        ImGui.End();
    }

    private IEnumerable<TitleInfo> GetTitles(bool fem) {
        var titles = this.Plugin.DataManager.GetExcelSheet<Title>()!
            .Where(row => row.Order != 0)
            .Select(row => new TitleInfo {
                Row = row,
                Unlocked = this.Plugin.Functions.IsTitleUnlocked(row.RowId),
                Text = fem ? row.Feminine : row.Masculine,
                Achievement = this.Achievements.TryGetValue(row.RowId, out var achievement) ? achievement : null,
            });

        if (this.Plugin.Config.OnlyUnlocked) {
            titles = titles.Where(t => t.Unlocked);
        }

        if (this._searchText.Length > 0) {
            var search = this._searchText.ToLowerInvariant();
            titles = titles.Where(t => t.Text.ExtractText().ToLowerInvariant().Contains(search));
        }

        titles = this.Plugin.Config.SortOrder switch {
            SortOrder.Default => titles.OrderBy(t => t.Row.Order),
            SortOrder.Alphabetical => titles.OrderBy(t => t.Text.ExtractText()),
            SortOrder.Achievement => titles.OrderBy(t => t.Achievement?.Name.ExtractText() ?? "???"),
            SortOrder.Category => titles.OrderBy(t => t.Achievement?.AchievementCategory.Value.AchievementKind.Value.Name.ExtractText() ?? "???"),
            _ => titles,
        };

        return titles;
    }
}

internal class TitleInfo {
    internal Title Row { get; init; }
    internal bool Unlocked { get; init; }
    internal ReadOnlySeString Text { get; init; }
    internal Achievement? Achievement { get; init; }
}
