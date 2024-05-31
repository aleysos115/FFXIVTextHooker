using System;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace FFXIVTextHooker.Windows;

public class ConfigWindow : Window, IDisposable
{
    private const float INDENT_SIZE = 10.0f;
    private Configuration Configuration;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("FFXIV Text Hooker Settings###With a constant ID")
    {
        //Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        //        ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(232, 75);
        SizeCondition = ImGuiCond.FirstUseEver;

        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        //if (Configuration.IsConfigWindowMovable)
        //{
        //    Flags &= ~ImGuiWindowFlags.NoMove;
        //}
        //else
        //{
        //    Flags |= ImGuiWindowFlags.NoMove;
        //}
    }

    public override void Draw()
    {
        var hookText = Configuration.HookText;
        if(ImGui.Checkbox("Hook text", ref hookText))
        {
            Configuration.HookText = hookText;
            Configuration.Save();
        }

        var hookBattleText = Configuration.HookBattleText;
        if (ImGui.Checkbox("Hook battle text", ref hookText))
        {
            Configuration.HookBattleText = hookBattleText;
            Configuration.Save();
        }

        var hookChat = Configuration.HookChat;
        if(ImGui.Checkbox("Hook chat", ref hookChat))
        {
            Configuration.HookChat = hookChat;
            Configuration.Save();
        }

        if (hookChat)
        {
            ImGuiHelpers.ScaledIndent(INDENT_SIZE);
            #region CHAT_OPTIONS
            if (ImGui.CollapsingHeader("Chat types"))
            {
                var none = Configuration.HookChatNone;
                if (ImGui.Checkbox("None", ref none))
                {
                    Configuration.HookChatNone = none;
                    Configuration.Save();
                }

                var debug = Configuration.HookChatDebug;
                if (ImGui.Checkbox("Debug", ref debug))
                {
                    Configuration.HookChatDebug = debug;
                    Configuration.Save();
                }

                var urgent = Configuration.HookChatUrgent;
                if (ImGui.Checkbox("Urgent", ref urgent))
                {
                    Configuration.HookChatUrgent = urgent;
                    Configuration.Save();
                }

                var notice = Configuration.HookChatNotice;
                if (ImGui.Checkbox("Notice", ref notice))
                {
                    Configuration.HookChatNotice = notice;
                    Configuration.Save();
                }

                var say = Configuration.HookChatSay;
                if (ImGui.Checkbox("Say", ref say))
                {
                    Configuration.HookChatSay = say;
                    Configuration.Save();
                }

                var shout = Configuration.HookChatShout;
                if (ImGui.Checkbox("Shout", ref shout))
                {
                    Configuration.HookChatShout = shout;
                    Configuration.Save();
                }

                var tellOutgoing = Configuration.HookChatTellOutgoing;
                if (ImGui.Checkbox("TellOutgoing", ref tellOutgoing))
                {
                    Configuration.HookChatTellOutgoing = tellOutgoing;
                    Configuration.Save();
                }

                var tellIncoming = Configuration.HookChatTellIncoming;
                if (ImGui.Checkbox("TellIncoming", ref tellIncoming))
                {
                    Configuration.HookChatTellIncoming = tellIncoming;
                    Configuration.Save();
                }

                var party = Configuration.HookChatParty;
                if (ImGui.Checkbox("Party", ref party))
                {
                    Configuration.HookChatParty = party;
                    Configuration.Save();
                }

                var alliance = Configuration.HookChatAlliance;
                if (ImGui.Checkbox("Alliance", ref alliance))
                {
                    Configuration.HookChatAlliance = alliance;
                    Configuration.Save();
                }

                var ls1 = Configuration.HookChatLs1;
                if (ImGui.Checkbox("Ls1", ref ls1))
                {
                    Configuration.HookChatLs1 = ls1;
                    Configuration.Save();
                }

                var ls2 = Configuration.HookChatLs2;
                if (ImGui.Checkbox("Ls2", ref ls2))
                {
                    Configuration.HookChatLs2 = ls2;
                    Configuration.Save();
                }

                var ls3 = Configuration.HookChatLs3;
                if (ImGui.Checkbox("Ls3", ref ls3))
                {
                    Configuration.HookChatLs3 = ls3;
                    Configuration.Save();
                }

                var ls4 = Configuration.HookChatLs4;
                if (ImGui.Checkbox("Ls4", ref ls4))
                {
                    Configuration.HookChatLs4 = ls4;
                    Configuration.Save();
                }

                var ls5 = Configuration.HookChatLs5;
                if (ImGui.Checkbox("Ls5", ref ls5))
                {
                    Configuration.HookChatLs5 = ls5;
                    Configuration.Save();
                }

                var ls6 = Configuration.HookChatLs6;
                if (ImGui.Checkbox("Ls6", ref ls6))
                {
                    Configuration.HookChatLs6 = ls6;
                    Configuration.Save();
                }

                var ls7 = Configuration.HookChatLs7;
                if (ImGui.Checkbox("Ls7", ref ls7))
                {
                    Configuration.HookChatLs7 = ls7;
                    Configuration.Save();
                }

                var ls8 = Configuration.HookChatLs8;
                if (ImGui.Checkbox("Ls8", ref ls8))
                {
                    Configuration.HookChatLs8 = ls8;
                    Configuration.Save();
                }

                var freeCompany = Configuration.HookChatFreeCompany;
                if (ImGui.Checkbox("FreeCompany", ref freeCompany))
                {
                    Configuration.HookChatFreeCompany = freeCompany;
                    Configuration.Save();
                }

                var noviceNetwork = Configuration.HookChatNoviceNetwork;
                if (ImGui.Checkbox("NoviceNetwork", ref noviceNetwork))
                {
                    Configuration.HookChatNoviceNetwork = noviceNetwork;
                    Configuration.Save();
                }

                var customEmote = Configuration.HookChatCustomEmote;
                if (ImGui.Checkbox("CustomEmote", ref customEmote))
                {
                    Configuration.HookChatCustomEmote = customEmote;
                    Configuration.Save();
                }

                var standardEmote = Configuration.HookChatStandardEmote;
                if (ImGui.Checkbox("StandardEmote", ref standardEmote))
                {
                    Configuration.HookChatStandardEmote = standardEmote;
                    Configuration.Save();
                }

                var yell = Configuration.HookChatYell;
                if (ImGui.Checkbox("Yell", ref yell))
                {
                    Configuration.HookChatYell = yell;
                    Configuration.Save();
                }

                var crossParty = Configuration.HookChatCrossParty;
                if (ImGui.Checkbox("CrossParty", ref crossParty))
                {
                    Configuration.HookChatCrossParty = crossParty;
                    Configuration.Save();
                }

                var pvPTeam = Configuration.HookChatPvPTeam;
                if (ImGui.Checkbox("PvPTeam", ref pvPTeam))
                {
                    Configuration.HookChatPvPTeam = pvPTeam;
                    Configuration.Save();
                }

                var crossLinkShell1 = Configuration.HookChatCrossLinkShell1;
                if (ImGui.Checkbox("CrossLinkShell1", ref crossLinkShell1))
                {
                    Configuration.HookChatCrossLinkShell1 = crossLinkShell1;
                    Configuration.Save();
                }

                var echo = Configuration.HookChatEcho;
                if (ImGui.Checkbox("Echo", ref echo))
                {
                    Configuration.HookChatEcho = echo;
                    Configuration.Save();
                }

                var systemError = Configuration.HookChatSystemError;
                if (ImGui.Checkbox("SystemError", ref systemError))
                {
                    Configuration.HookChatSystemError = systemError;
                    Configuration.Save();
                }

                var systemMessage = Configuration.HookChatSystemMessage;
                if (ImGui.Checkbox("SystemMessage", ref systemMessage))
                {
                    Configuration.HookChatSystemMessage = systemMessage;
                    Configuration.Save();
                }

                var gatheringSystemMessage = Configuration.HookChatGatheringSystemMessage;
                if (ImGui.Checkbox("GatheringSystemMessage", ref gatheringSystemMessage))
                {
                    Configuration.HookChatGatheringSystemMessage = gatheringSystemMessage;
                    Configuration.Save();
                }

                var errorMessage = Configuration.HookChatErrorMessage;
                if (ImGui.Checkbox("ErrorMessage", ref errorMessage))
                {
                    Configuration.HookChatErrorMessage = errorMessage;
                    Configuration.Save();
                }

                var nPCDialogue = Configuration.HookChatNPCDialogue;
                if (ImGui.Checkbox("NPCDialogue", ref nPCDialogue))
                {
                    Configuration.HookChatNPCDialogue = nPCDialogue;
                    Configuration.Save();
                }

                var nPCDialogueAnnouncements = Configuration.HookChatNPCDialogueAnnouncements;
                if (ImGui.Checkbox("NPCDialogueAnnouncements", ref nPCDialogueAnnouncements))
                {
                    Configuration.HookChatNPCDialogueAnnouncements = nPCDialogueAnnouncements;
                    Configuration.Save();
                }

                var retainerSale = Configuration.HookChatRetainerSale;
                if (ImGui.Checkbox("RetainerSale", ref retainerSale))
                {
                    Configuration.HookChatRetainerSale = retainerSale;
                    Configuration.Save();
                }

                var crossLinkShell2 = Configuration.HookChatCrossLinkShell2;
                if (ImGui.Checkbox("CrossLinkShell2", ref crossLinkShell2))
                {
                    Configuration.HookChatCrossLinkShell2 = crossLinkShell2;
                    Configuration.Save();
                }

                var crossLinkShell3 = Configuration.HookChatCrossLinkShell3;
                if (ImGui.Checkbox("CrossLinkShell3", ref crossLinkShell3))
                {
                    Configuration.HookChatCrossLinkShell3 = crossLinkShell3;
                    Configuration.Save();
                }

                var crossLinkShell4 = Configuration.HookChatCrossLinkShell4;
                if (ImGui.Checkbox("CrossLinkShell4", ref crossLinkShell4))
                {
                    Configuration.HookChatCrossLinkShell4 = crossLinkShell4;
                    Configuration.Save();
                }

                var crossLinkShell5 = Configuration.HookChatCrossLinkShell5;
                if (ImGui.Checkbox("CrossLinkShell5", ref crossLinkShell5))
                {
                    Configuration.HookChatCrossLinkShell5 = crossLinkShell5;
                    Configuration.Save();
                }

                var crossLinkShell6 = Configuration.HookChatCrossLinkShell6;
                if (ImGui.Checkbox("CrossLinkShell6", ref crossLinkShell6))
                {
                    Configuration.HookChatCrossLinkShell6 = crossLinkShell6;
                    Configuration.Save();
                }

                var crossLinkShell7 = Configuration.HookChatCrossLinkShell7;
                if (ImGui.Checkbox("CrossLinkShell7", ref crossLinkShell7))
                {
                    Configuration.HookChatCrossLinkShell7 = crossLinkShell7;
                    Configuration.Save();
                }

                var crossLinkShell8 = Configuration.HookChatCrossLinkShell8;
                if (ImGui.Checkbox("CrossLinkShell8", ref crossLinkShell8))
                {
                    Configuration.HookChatCrossLinkShell8 = crossLinkShell8;
                    Configuration.Save();
                }

                var fallback = Configuration.HookChatFallback;
                if (ImGui.Checkbox("Fallback", ref fallback))
                {
                    Configuration.HookChatFallback = fallback;
                    Configuration.Save();
                }

            }
            #endregion
            ImGuiHelpers.ScaledIndent(-INDENT_SIZE);
        }

        var hookTooltip = Configuration.HookTooltips;
        if (ImGui.Checkbox("Hook chat", ref hookTooltip))
        {
            Configuration.HookTooltips = hookTooltip;
            Configuration.Save();
        }

        if(hookTooltip)
        {
            ImGuiHelpers.ScaledIndent(INDENT_SIZE);

            ImGuiHelpers.ScaledIndent(-INDENT_SIZE);
        }
    }
}
