# FFXIV Text Hooker

### A FFXIV Text Hooker / Text Extractor for Dalamud.

FFXIV Text Hooker will automatically copy the following text to the system clipboard:
* Cutscene / NPC Text
* Battle Text
* Player Chat (All chat sources)
* Item and Action tooltips (Configurable hotkey press required)

The primary purpose of the plugin is to support those wishing to learn languages through playing FFXIV.

## Installation

## Chat Commands

***/th*** : Opens the Text Hooker logger interface

## How To Use

Texthooker will automatically copy all the game text it is able to to the system clipboard (except item and action tooltips which require a hotkey to extract). Item and action tooltips can be extracted when mouse hovering over a item or action and pressing the configured hotkey. The default hotkey is ***Shift+t***. What the text hooker extracts can be configured in the configuration menu accessable via either the Dalamud plugin page or through the interface accessable via the above chat command. The logger will list all the hooked text as configured in the configuration window, this text can be recopied to clipboard by pressing the copy button that appears next to the text when hovered over by mouse. 

FFXIV Text Hooker was designed to be used in conjuction with language learning pipelines recommended by communities such as <a href="https://refold.la/"> Refold </a> or <a href="https://learnjapanese.moe/"> TheMoeWay </a>. A basic setup would look as follows: 
* FFXIV Text Hooker extracts the game text to system clipboard.
* A text hooker <a href="https://learnjapanese.moe/texthooker.html">webpage</a> in combination with a clipboard paster <a href="https://github.com/laplus-sadness/lap-clipboard-inserter">web-plugin</a> dumps all the extracted text on the webpage.
* A pop-up word lookup web-plugin such as <a href="https://github.com/themoeway/yomitan">yomitan</a> is used to lookup unfamiliar words. 

Happy language learning!

## To do
* Extract Quest Log text
* Extract Party Finder text
* Extract speech bubbles

## Known issues
* (Japanese) Text with furigana on top will not be copied.

## Thanks
- goaats for FFXIVQuickLauncher
- annaclemens for XivCommon
- Price Check plugin for teaching me how to capture key input

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/vending_machine) 

###### Final Fantasy XIV © 2010-2024 SQUARE ENIX CO., LTD. All Rights Reserved. I am not affiliated with SQUARE ENIX CO., LTD. in any way.
