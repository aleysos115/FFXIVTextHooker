using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVTextHooker.Windows;
using XivCommon;
using XivCommon.Functions;
using System;
using XivCommon.Functions.Tooltips;
using Dalamud.Game.Gui;
using System.Runtime.Versioning;

namespace FFXIVTextHooker;

public sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/th";

    private IFramework Framework { get; init; }
    private IKeyState KeyState { get; init; }
    private DalamudPluginInterface PluginInterface { get; init; }
    private ICommandManager CommandManager { get; init; }
    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("FFXIVTextHooker");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    [PluginService] public static IChatGui Chat { get; private set; } = null!;

    private static XivCommonBase Common { get; set; }

    private ActionTooltip hoveredAction = null;
    private ItemTooltip hoveredItem = null;

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] ICommandManager commandManager,
        [RequiredVersion("1.0")] IKeyState keyState,
        [RequiredVersion("1.0")] IFramework framework)
    {
        PluginInterface = pluginInterface;
        CommandManager = commandManager;
        KeyState = keyState;
        Framework = framework;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        Common = new XivCommonBase(PluginInterface, Hooks.Talk | Hooks.BattleTalk | Hooks.Tooltips);
        
        // you might normally want to embed resources and load them from the manifest stream
        var file = new FileInfo(Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png"));

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens the text hooker menu"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        Common.Functions.Talk.OnTalk += this.GetTalk;
        Common.Functions.BattleTalk.OnBattleTalk += this.GetBattleTalk;
        Common.Functions.Tooltips.OnItemTooltip += this.GetItemTooltip;
        Common.Functions.Tooltips.OnActionTooltip += this.GetActionTooltip;

        Chat.ChatMessage += this.GetChat;

        Framework.Update += this.OnFrameworkTick;
    }

    private void OnFrameworkTick(IFramework framework)
    {
        if(IsKeyBindPressed())
        {
            if (Configuration.HookTooltips)
            {
                if (hoveredItem != null && Configuration.HookItemTooltips) PrintItemTooltip(hoveredItem);
                if (hoveredAction != null && Configuration.HookActionTooltips) PritnActionTooltip(hoveredAction);
            }
        }
    }

    private void CopyToClipboard(string origin, string text)
    {
        MainWindow.AddTextEntry(origin, DateTime.Now.ToString(), text);
        System.Windows.Forms.Clipboard.SetText(text);
    }

    private void GetTalk(ref SeString name, ref SeString text, ref TalkStyle style)
    {
        if (Configuration.HookText)
        {
            string combinedText = string.Format("{0}: {1}", name.TextValue, text.TextValue);
            CopyToClipboard("Talk", combinedText);
        }
    }

    private void GetBattleTalk(ref SeString sender, ref SeString message, ref BattleTalkOptions options, ref bool isHandled)
    {
        if (Configuration.HookBattleText)
        {
            string combinedText = string.Format("{0}: {1}", sender.TextValue, message.TextValue);
            CopyToClipboard("BattleTalk", combinedText);
        }
    }

    private void GetChat(XivChatType type, uint id, ref SeString sender, ref SeString message, ref bool handled)
    {
        if(Configuration.HookChat)
        {
            string text = string.Format("{0}: {1}", sender.TextValue, message.TextValue);
            #region CHAT_TYPES
            switch (type)
            {
                case XivChatType.None:
                    if (Configuration.HookChatNone) CopyToClipboard("Chat/None", text);
                    break;
                case XivChatType.Debug:
                    if (Configuration.HookChatDebug) CopyToClipboard("Chat/Debug", text);
                    break;
                case XivChatType.Urgent:
                    if (Configuration.HookChatUrgent) CopyToClipboard("Chat/Urgent", text);
                    break;
                case XivChatType.Notice:
                    if (Configuration.HookChatNotice) CopyToClipboard("Chat/Notice", text);
                    break;
                case XivChatType.Say:
                    if (Configuration.HookChatSay) CopyToClipboard("Chat/Say", text);
                    break;
                case XivChatType.Shout:
                    if (Configuration.HookChatShout) CopyToClipboard("Chat/Shout", text);
                    break;
                case XivChatType.TellOutgoing:
                    if (Configuration.HookChatTellOutgoing) CopyToClipboard("Chat/TellOutgoing", text);
                    break;
                case XivChatType.TellIncoming:
                    if (Configuration.HookChatTellIncoming) CopyToClipboard("Chat/TellIncoming", text);
                    break;
                case XivChatType.Party:
                    if (Configuration.HookChatParty) CopyToClipboard("Chat/Party", text);
                    break;
                case XivChatType.Alliance:
                    if (Configuration.HookChatAlliance) CopyToClipboard("Chat/Alliance", text);
                    break;
                case XivChatType.Ls1:
                    if (Configuration.HookChatLs1) CopyToClipboard("Chat/Ls1", text);
                    break;
                case XivChatType.Ls2:
                    if (Configuration.HookChatLs2) CopyToClipboard("Chat/Ls2", text);
                    break;
                case XivChatType.Ls3:
                    if (Configuration.HookChatLs3) CopyToClipboard("Chat/Ls3", text);
                    break;
                case XivChatType.Ls4:
                    if (Configuration.HookChatLs4) CopyToClipboard("Chat/Ls4", text);
                    break;
                case XivChatType.Ls5:
                    if (Configuration.HookChatLs5) CopyToClipboard("Chat/Ls5", text);
                    break;
                case XivChatType.Ls6:
                    if (Configuration.HookChatLs6) CopyToClipboard("Chat/Ls6", text);
                    break;
                case XivChatType.Ls7:
                    if (Configuration.HookChatLs7) CopyToClipboard("Chat/Ls7", text);
                    break;
                case XivChatType.Ls8:
                    if (Configuration.HookChatLs8) CopyToClipboard("Chat/Ls8", text);
                    break;
                case XivChatType.FreeCompany:
                    if (Configuration.HookChatFreeCompany) CopyToClipboard("Chat/FreeCompany", text);
                    break;
                case XivChatType.NoviceNetwork:
                    if (Configuration.HookChatNoviceNetwork) CopyToClipboard("Chat/NoviceNetwork", text);
                    break;
                case XivChatType.CustomEmote:
                    if (Configuration.HookChatCustomEmote) CopyToClipboard("Chat/CustomEmote", text);
                    break;
                case XivChatType.StandardEmote:
                    if (Configuration.HookChatStandardEmote) CopyToClipboard("Chat/StandardEmote", text);
                    break;
                case XivChatType.Yell:
                    if (Configuration.HookChatYell) CopyToClipboard("Chat/Yell", text);
                    break;
                case XivChatType.CrossParty:
                    if (Configuration.HookChatCrossParty) CopyToClipboard("Chat/CrossParty", text);
                    break;
                case XivChatType.PvPTeam:
                    if (Configuration.HookChatPvPTeam) CopyToClipboard("Chat/PvPTeam", text);
                    break;
                case XivChatType.CrossLinkShell1:
                    if (Configuration.HookChatCrossLinkShell1) CopyToClipboard("Chat/CrossLinkShell1", text);
                    break;
                case XivChatType.Echo:
                    if (Configuration.HookChatEcho) CopyToClipboard("Chat/Echo", text);
                    break;
                case XivChatType.SystemError:
                    if (Configuration.HookChatSystemError) CopyToClipboard("Chat/SystemError", text);
                    break;
                case XivChatType.SystemMessage:
                    if (Configuration.HookChatSystemMessage) CopyToClipboard("Chat/SystemMessage", text);
                    break;
                case XivChatType.GatheringSystemMessage:
                    if (Configuration.HookChatGatheringSystemMessage) CopyToClipboard("Chat/GatheringSystemMessage", text);
                    break;
                case XivChatType.ErrorMessage:
                    if (Configuration.HookChatErrorMessage) CopyToClipboard("Chat/ErrorMessage", text);
                    break;
                case XivChatType.NPCDialogue:
                    if (Configuration.HookChatNPCDialogue) CopyToClipboard("Chat/NPCDialogue", text);
                    break;
                case XivChatType.NPCDialogueAnnouncements:
                    if (Configuration.HookChatNPCDialogueAnnouncements) CopyToClipboard("Chat/NPCDialogueAnnouncements", text);
                    break;
                case XivChatType.RetainerSale:
                    if (Configuration.HookChatRetainerSale) CopyToClipboard("Chat/RetainerSale", text);
                    break;
                case XivChatType.CrossLinkShell2:
                    if (Configuration.HookChatCrossLinkShell2) CopyToClipboard("Chat/CrossLinkShell2", text);
                    break;
                case XivChatType.CrossLinkShell3:
                    if (Configuration.HookChatCrossLinkShell3) CopyToClipboard("Chat/CrossLinkShell3", text);
                    break;
                case XivChatType.CrossLinkShell4:
                    if (Configuration.HookChatCrossLinkShell4) CopyToClipboard("Chat/CrossLinkShell4", text);
                    break;
                case XivChatType.CrossLinkShell5:
                    if (Configuration.HookChatCrossLinkShell5) CopyToClipboard("Chat/CrossLinkShell5", text);
                    break;
                case XivChatType.CrossLinkShell6:
                    if (Configuration.HookChatCrossLinkShell6) CopyToClipboard("Chat/CrossLinkShell6", text);
                    break;
                case XivChatType.CrossLinkShell7:
                    if (Configuration.HookChatCrossLinkShell7) CopyToClipboard("Chat/CrossLinkShell7", text);
                    break;
                case XivChatType.CrossLinkShell8:
                    if (Configuration.HookChatCrossLinkShell8) CopyToClipboard("Chat/CrossLinkShell8", text);
                    break;
                default:
                    if (Configuration.HookChatFallback) CopyToClipboard("Chat/Fallback", text);
                    break;
            }
            #endregion
        }
    }

    //Note: GetRawValue ~ 3 = KeyPressed, 1 = KeyDown, 4 = KeyUp
    public bool IsKeyBindPressed()
    {
        if (this.Configuration.ModifierKey == Hotkey.Enum.VkNone && this.Configuration.PrimaryKey == Hotkey.Enum.VkNone) return false;
        if (this.Configuration.PrimaryKey == Hotkey.Enum.VkNone)
        {
            return KeyState.GetRawValue((byte)this.Configuration.ModifierKey) == 3;
        }

        if (this.Configuration.ModifierKey == Hotkey.Enum.VkNone)
        {
            return KeyState.GetRawValue((byte)this.Configuration.PrimaryKey) == 3;
        }

        return KeyState[(byte)this.Configuration.ModifierKey] && KeyState.GetRawValue((byte)this.Configuration.PrimaryKey) == 3;
    }

    private void GetItemTooltip(ItemTooltip itemTooltip, ulong itemId)
    {
        hoveredItem = itemTooltip;
        hoveredAction = null;
    }

    private void PrintItemTooltip(ItemTooltip itemTooltip)
    {
        string output;

        output = string.Format("{0}\n", itemTooltip[ItemTooltipString.Name].TextValue);
        if ((itemTooltip.Fields & ItemTooltipFields.GlamourIndicator) == ItemTooltipFields.GlamourIndicator)
            output += string.Format("({0})\n", itemTooltip[ItemTooltipString.GlamourName].TextValue);
        output += string.Format("{0}\n", itemTooltip[ItemTooltipString.Type].TextValue);
        if ((itemTooltip.Fields & ItemTooltipFields.Stat1) == ItemTooltipFields.Stat1)
            output += string.Format("{0}: {1} {2}\n", itemTooltip[ItemTooltipString.Stat1Label].TextValue,
                                                      itemTooltip[ItemTooltipString.Stat1].TextValue,
                                                      itemTooltip[ItemTooltipString.Stat1Delta]);
        if ((itemTooltip.Fields & ItemTooltipFields.Stat2) == ItemTooltipFields.Stat2)
            output += string.Format("{0}: {1} {2}\n", itemTooltip[ItemTooltipString.Stat2Label].TextValue,
                                                      itemTooltip[ItemTooltipString.Stat2].TextValue,
                                                      itemTooltip[ItemTooltipString.Stat2Delta]);
        if ((itemTooltip.Fields & ItemTooltipFields.Stat3) == ItemTooltipFields.Stat3)
            output += string.Format("{0}: {1} {2}\n", itemTooltip[ItemTooltipString.Stat3Label].TextValue,
                                                      itemTooltip[ItemTooltipString.Stat3].TextValue,
                                                      itemTooltip[ItemTooltipString.Stat3Delta]);
        if ((itemTooltip.Fields & ItemTooltipFields.Description) == ItemTooltipFields.Description)
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.Description].TextValue);
        output += string.Format("{0}\n", itemTooltip[ItemTooltipString.Quantity].TextValue);
        if ((itemTooltip.Fields & ItemTooltipFields.Effects) == ItemTooltipFields.Effects)
            output += string.Format("{0}: {1}\n", itemTooltip[ItemTooltipString.EffectsLabel].TextValue,
                                                  itemTooltip[ItemTooltipString.Effects].TextValue);
        if ((itemTooltip.Fields & ItemTooltipFields.Levels) == ItemTooltipFields.Levels)
        {
            if (itemTooltip[ItemTooltipString.EquipJobs].TextValue != string.Empty)
                output += string.Format("{0}\n", itemTooltip[ItemTooltipString.EquipJobs].TextValue);
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.EquipLevel].TextValue);

            //Only present when equip level is present
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.Level].TextValue);
        }
        if ((itemTooltip.Fields & ItemTooltipFields.VendorSellPrice) == ItemTooltipFields.VendorSellPrice)
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.VendorSellPrice].TextValue);
        if ((itemTooltip.Fields & ItemTooltipFields.Crafter) == ItemTooltipFields.Crafter)
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.Crafter].TextValue);
        //output += string.Format("{0}\n", itemTooltip[ItemTooltipString.Condition].TextValue);

        //Only present on equipment
        if ((itemTooltip.Fields & ItemTooltipFields.Levels) == ItemTooltipFields.Levels)
            output += string.Format("{0}: {1}\n", itemTooltip[ItemTooltipString.SpiritbondLabel].TextValue,
                                                  itemTooltip[ItemTooltipString.Spiritbond].TextValue);
        
        if ((itemTooltip.Fields & ItemTooltipFields.Bonuses) == ItemTooltipFields.Bonuses)
        {
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.BonusesLabel].TextValue);
            if (itemTooltip[ItemTooltipString.Bonus1].TextValue != string.Empty)
                output += string.Format("\t{0}\n", itemTooltip[ItemTooltipString.Bonus1].TextValue);
            if (itemTooltip[ItemTooltipString.Bonus2].TextValue != string.Empty)
                output += string.Format("\t{0}\n", itemTooltip[ItemTooltipString.Bonus2].TextValue);
            if (itemTooltip[ItemTooltipString.Bonus3].TextValue != string.Empty)
                output += string.Format("\t{0}\n", itemTooltip[ItemTooltipString.Bonus3].TextValue);
            if (itemTooltip[ItemTooltipString.Bonus4].TextValue != string.Empty)
                output += string.Format("\t{0}\n", itemTooltip[ItemTooltipString.Bonus4].TextValue);
        }
        if ((itemTooltip.Fields & ItemTooltipFields.Materia) == ItemTooltipFields.Materia)
        {
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.MateriaLabel].TextValue);
            if (itemTooltip[ItemTooltipString.Materia1].TextValue != string.Empty)
                output += string.Format("\t{0}\t{1}\n", itemTooltip[ItemTooltipString.Materia1].TextValue,
                                                      itemTooltip[ItemTooltipString.Materia1Effect].TextValue);
            if (itemTooltip[ItemTooltipString.Materia2].TextValue != string.Empty)
                output += string.Format("\t{0}\t{1}\n", itemTooltip[ItemTooltipString.Materia2].TextValue,
                                                      itemTooltip[ItemTooltipString.Materia2Effect].TextValue);
            if (itemTooltip[ItemTooltipString.Materia3].TextValue != string.Empty)
                output += string.Format("\t{0}\t{1}\n", itemTooltip[ItemTooltipString.Materia3].TextValue,
                                                      itemTooltip[ItemTooltipString.Materia3Effect].TextValue);
            if (itemTooltip[ItemTooltipString.Materia4].TextValue != string.Empty)
                output += string.Format("\t{0}\t{1}\n", itemTooltip[ItemTooltipString.Materia4].TextValue,
                                                      itemTooltip[ItemTooltipString.Materia4Effect].TextValue);
            if (itemTooltip[ItemTooltipString.Materia5].TextValue != string.Empty)
                output += string.Format("\t{0}\t{1}\n", itemTooltip[ItemTooltipString.Materia5].TextValue,
                                                      itemTooltip[ItemTooltipString.Materia5Effect].TextValue);
        }

        if ((itemTooltip.Fields & ItemTooltipFields.CraftingAndRepairs) == ItemTooltipFields.CraftingAndRepairs)
        {
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.RepairLevel].TextValue);
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.Materials].TextValue);
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.QuickRepairs].TextValue);
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.MateriaMelding].TextValue);

            //Putting capabilities in here for now until I find something that breaks this rule
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.Capabilities].TextValue);
        }
        if (itemTooltip[ItemTooltipString.ShopSellingPrice].TextValue != string.Empty)
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.ShopSellingPrice].TextValue);
        if (itemTooltip[ItemTooltipString.ControllerControls].TextValue != string.Empty)
            output += string.Format("{0}\n", itemTooltip[ItemTooltipString.ControllerControls].TextValue);
        CopyToClipboard("Item Tooltip", output);
    }

    private void GetActionTooltip(ActionTooltip actionTooltip, HoveredAction action)
    {
        hoveredAction = actionTooltip;
        hoveredItem = null;
    }

    private void PritnActionTooltip(ActionTooltip actionTooltip)
    {
        string output;
        output = string.Format("{0}\n", actionTooltip[ActionTooltipString.Name].TextValue);

        bool insertNewLine = false;
        if (actionTooltip[ActionTooltipString.Type].TextValue != string.Empty)
        {
            output += string.Format("{0}", actionTooltip[ActionTooltipString.Type].TextValue);
            insertNewLine = true;
        }
        if ((actionTooltip.Fields & ActionTooltipFields.Range) == ActionTooltipFields.Range)
        {
            output += string.Format("\t{0}{1}", actionTooltip[ActionTooltipString.RangeLabel].TextValue,
                                                actionTooltip[ActionTooltipString.Range].TextValue);
            insertNewLine = true;
        }
        if ((actionTooltip.Fields & ActionTooltipFields.Radius) == ActionTooltipFields.Radius)
        {
            output += string.Format("\t{0}{1}", actionTooltip[ActionTooltipString.RadiusLabel].TextValue,
                                                actionTooltip[ActionTooltipString.Radius].TextValue);
            insertNewLine = true;
        }
        if (insertNewLine) output += "\n";

        if ((actionTooltip.Fields & ActionTooltipFields.Cast) == ActionTooltipFields.Cast)
            output += string.Format("{0}: {1}\n", actionTooltip[ActionTooltipString.CastLabel].TextValue,
                                                actionTooltip[ActionTooltipString.Cast].TextValue);
        if ((actionTooltip.Fields & ActionTooltipFields.Recast) == ActionTooltipFields.Recast)
            output += string.Format("{0}: {1}\n", actionTooltip[ActionTooltipString.RecastLabel].TextValue,
                                                actionTooltip[ActionTooltipString.Recast].TextValue);
        if ((actionTooltip.Fields & ActionTooltipFields.Cost) == ActionTooltipFields.Cost)
            output += string.Format("{0}: {1}\n", actionTooltip[ActionTooltipString.CostLabel].TextValue,
                                                  actionTooltip[ActionTooltipString.Cost].TextValue);

        if ((actionTooltip.Fields & ActionTooltipFields.Description) == ActionTooltipFields.Description)
            output += string.Format("{0}\n", actionTooltip[ActionTooltipString.Description].TextValue);
        if ((actionTooltip.Fields & ActionTooltipFields.Acquired) == ActionTooltipFields.Acquired)
            output += string.Format("{0}", actionTooltip[ActionTooltipString.Acquired].TextValue);
        if ((actionTooltip.Fields & ActionTooltipFields.Affinity) == ActionTooltipFields.Affinity)
            output += string.Format("\t{0}", actionTooltip[ActionTooltipString.Affinity].TextValue);

        CopyToClipboard("Action Tooltip", output);
    }

    public void Dispose()
    {
        Framework.Update -= this.OnFrameworkTick;
        Chat.ChatMessage -= this.GetChat;
        Common.Functions.Talk.OnTalk -= this.GetTalk;
        Common.Functions.BattleTalk.OnBattleTalk -= this.GetBattleTalk;
        Common.Functions.Tooltips.OnItemTooltip -= this.GetItemTooltip;
        Common.Functions.Tooltips.OnActionTooltip -= this.GetActionTooltip;
        Common.Functions.Dispose();

        Common?.Dispose();

        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
