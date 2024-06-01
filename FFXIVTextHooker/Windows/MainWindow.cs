using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace FFXIVTextHooker.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin Plugin;

    List<TextEntry> textEntries;

    bool newItemAdded = false;

    public MainWindow(Plugin plugin)
        : base("FFXIV Text Hooker##Main Window", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        textEntries = new List<TextEntry>();
        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.BeginChild("##EntriesList", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y * 0.90f), true, ImGuiWindowFlags.AlwaysVerticalScrollbar);

        bool entryListHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows);

        for(int i = 0; i < textEntries.Count; i++)
        {
            ImGui.PushID(i);
            int lines = textEntries[i].Text.Split('\n').Length;
            lines = int.Max(2, lines);
            ImGui.BeginChild("EntryFrame", new Vector2(0, ImGui.GetFontSize() * lines + 10), false, ImGuiWindowFlags.NoScrollbar);
            bool isHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.AllowWhenBlockedByActiveItem);
            ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), string.Format("{0}\n{1} ", textEntries[i].Source, textEntries[i].Time));
            ImGui.SameLine();
            ImGui.Text(textEntries[i].Text);
            if (isHovered)
            {
                ImGui.SameLine(ImGui.GetContentRegionMax().X - 50);
                if (ImGui.Button("Copy"))
                    System.Windows.Forms.Clipboard.SetText(textEntries[i].Text);
            }
            ImGui.EndChild();
            ImGui.PopID();
        }

        if (newItemAdded && !entryListHovered)
        {
            ImGui.SetScrollHereY(1.0f);
            newItemAdded = false;
        }
        else if (entryListHovered)
            newItemAdded = false;

        ImGui.EndChild();

        ImGui.Spacing();
        if (ImGui.Button("Show Settings"))
        {
            Plugin.ToggleConfigUI();
        }

        ImGui.SameLine();
        if(ImGui.Button("Clear"))
        {
            textEntries.Clear();
        }
    }

    public void AddTextEntry(string _Source, string _Time, string _Text)
    {
        textEntries.Add(new TextEntry(_Source, _Time, _Text));
        newItemAdded = true;
    }
}
