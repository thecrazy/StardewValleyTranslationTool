# StardewValleyTranslationTool
A tool to help validate translations of i18n files for Stardew Valley Mods.

NOTES:<br>

Developped in .Net for Windows, you will need the .Net framework installed to run this.<br>

Json validation is currently disabled as it needs more work. Use https://smapi.io/json with the i18n option instead.<br>

You need to have two files with the same line count loaded to be able to use the tool.<br>

THERE IS NO AUTO SAVE at the moment, backups originals, save often.<br>

Pay attention to highligted token and command characters, machine translation sometimes mess it up or add a space in the middle.<br>

The token \~FileChangeToken\~ indicates the position were two files were joinned. Remove the token and pay close attention to those areas.
