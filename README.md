# StardewValleyTranslationTool
A tool to help validate translations of i18n files for Stardew Valley Mods.

![Screenshot](https://user-images.githubusercontent.com/2411956/126434670-a6acffdd-37e7-4c5a-ab38-bdfb3f986b0c.png)

NOTES:<br>

Developped in .Net for Windows. You need the .Net framework installed to run this.<br>

Json validation is currently disabled as it needs more work. Use https://smapi.io/json with the i18n option instead.<br>

You need to have two files with the same line count loaded to be able to use the tool.<br>

THERE IS NO AUTO SAVE at the moment, backups originals, save often.<br>

Pay attention to highligted token and command characters, machine translation sometimes mess it up or add a space in the middle.<br>

The token \~FileChangeToken\~ indicates the position were two files were joinned. Remove the token and pay close attention to those areas.
