using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Security;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stardew_Valley_Translation_Tool {

    internal enum LineChangeTypes { FWD, BWD, NUM }

    public partial class Form1 : Form {

        string title = "Stardew Valley Translation Tool";
        string version = "v0.1.1";
        string lineCountLabelPrefix = "Line count: ";
        string fileNameLabelPrefix = "Filename: ";
        string keyCountLabelPrefix = "Key count: ";
        //string validLineCountLabelPrefix = "Valid Dialogs: ";
        string jsonLabelPrefix = "Json: ";
        string keyPattern = "^\".*\": \"";                // Matches: "keyString": "
        //string validDialogPattern = "^\".*\": \".*\",$";  // Matches: "keyString": "contentString",
        string fileChangedToken = @"~FILE CHANGE~";

        private List<string> originalLines;
        private List<string> translatedLines;
        private List<string> originalKeys;
        private List<string> translatedKeys;
        private int previousLinePosition = 0;
        private bool ignoreChanges;
        private Color defaultBackgroundColor;
        private Color defaultTextColor;
        private Color selectionTextColor = Color.Aquamarine;
        private Color selectionBackgroundColor = Color.SteelBlue;
        private Color errorBackgroundColor = Color.Tomato;
        private int originalKeyCount;       // TODO: Make a class to store original and translation values
        private int translationKeyCount;
        private int lineNumOnFocus;


        private bool IsOkToLooseChanges {
            get {
                DialogResult dialogResult = MessageBox.Show("You will lose unsaved changes, continue?", title, MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK) return true;
                return false;
            }
        }

        /// <returns>A string containing the paths of the selected file</returns>
        private string OpenSingleFileDialog(OpenFileDialog openDialog) {//, RichTextBoxSynchronizedScroll richTextBox, ref List<string> list, Label lineCountLabel, Label fileNameLabel, Label keyCountLabel, Label jsonLabel) {
            openDialog.Filter = "Text Documents (*.json, *.txt)|*.json;*.txt|All Files (*.*)|*.*";

            if (openDialog.ShowDialog() == DialogResult.OK) {
                try {
                    return openDialog.FileName;
                }
                catch (SecurityException ex) {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
            return null;
        }

        /// <returns>A string array containing the paths of the selected files</returns>
        private string[] OpenMultipleFilesDialog(OpenFileDialog openMultipleDialog) {       // Allow user to select and open multiple files
            openMultipleDialog.Multiselect = true;
            openMultipleDialog.Filter = "Text Documents (*.json, *.txt)|*.json;*.txt|All Files (*.*)|*.*";

            if (J_OpenFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    return openMultipleDialog.FileNames;
                }
                catch (SecurityException ex) {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
            return null;
        }

        /// <returns>A string containing the text loaded from the files</returns>    // TODO: Clean up and refactor LoadTextFromMultipleFilePaths & LoadTextFromFilePath
        private string LoadTextFromMultipleFilePaths(string[] filePaths, RichTextBoxSynchronizedScroll rtb, ref List<string> list, Label lineCountLabel, Label fileNameLabel, Label keyCountLabel, Label jsonLabel) {
            if (filePaths == null) return null;     // If its null the user clicked cancel, return

            string loadedText = "";

            if (rtb == RV_RichTextBox && R_Save.Enabled && !IsOkToLooseChanges) return null; // Warn about unsaved changes

            ResetRichTextBoxes(rtb, true);

            for (int i = 0; i < filePaths.Length; i++) {
                if (i + 1 != filePaths.Length)
                    Debug.WriteLine("i: " + i + "   filePaths.Length: " + filePaths.Length);
                loadedText += File.ReadAllText(filePaths[i]) + (i + 1 != filePaths.Length ? fileChangedToken : "");    // Load entire text from the file and insert a file change token
            }

            rtb.Text = loadedText;              // Write the loaded text in the richTextBox           

            list = rtb.Lines.ToList();          // Load all lines from the file in a string list         

            if (originalLines.Count > 0) LH_RichTextBox.Text = originalLines[0];     // Load first line in both horizontal boxes
            if (translatedLines.Count > 0) RH_RichTextBox.Text = translatedLines[0];
            LinePosition.Value = 0;
            TotalLines.Text = "of " + list.Count;

            lineCountLabel.Text = lineCountLabelPrefix + list.Count;

            fileNameLabel.Text = fileNameLabelPrefix + "New file";

            if (IsValidJson(loadedText)) {
                jsonLabel.Text = jsonLabelPrefix + "OK";
            } else {
                jsonLabel.Text = jsonLabelPrefix + "ERROR";
            }

            int keyCount = 0;
            int noKeyCount = 0;
            //int validLineCount = 0;
            Match m;
            foreach (var line in list) {
                m = Regex.Match(line, keyPattern);
                if (m.Success) {
                    keyCount++;
                    //m = Regex.Match(line, validDialogPattern);
                    //if (m.Success) validLineCount++;
                } else {
                    noKeyCount++;
                }
            }

            //keyCountLabel.Text = keyCountLabelPrefix + keyCount + "   " + validLineCountLabelPrefix + validLineCount;
            keyCountLabel.Text = keyCountLabelPrefix + keyCount;

            if (LV_RichTextBox.Text != string.Empty && RV_RichTextBox.Text != string.Empty) {
                SetSelectionLineColors(LV_RichTextBox, previousLinePosition, defaultTextColor, defaultBackgroundColor, true); // Deselect old lines
                SetSelectionLineColors(RV_RichTextBox, previousLinePosition, defaultTextColor, defaultBackgroundColor, true);
                SetSelectionLineColors(LV_RichTextBox, (int)LinePosition.Value, selectionTextColor, selectionBackgroundColor); // Select new lines
                SetSelectionLineColors(RV_RichTextBox, (int)LinePosition.Value, selectionTextColor, selectionBackgroundColor);
                NextLine.Enabled = LV_RichTextBox.Text != string.Empty;   // Make sure the buttons are only enabled if both text are loaded
                PrevLine.Enabled = RV_RichTextBox.Text != string.Empty;
                CheckForErrors();
            }
            HighlightKeywordsInAllBoxes(true);  // Skip vertical boxes
            //R_Save.Enabled = false;     // Loading a new document doesn't count as a change // In multiple, yes it does
            return loadedText;
        }

        /// <returns>A string containing the text loaded from the file</returns>    // TODO: Needs cleanup
        private string LoadTextFromFilePath(string filePath, RichTextBoxSynchronizedScroll rtb, ref List<string> list, Label lineCountLabel, Label fileNameLabel, Label keyCountLabel, Label jsonLabel, bool IsOriginal) {
            if (string.IsNullOrWhiteSpace(filePath)) return null;   // If its null the user clicked cancel, return

            if (IsOriginal) {
                originalKeys = new List<string>();
            } else {
                translatedKeys = new List<string>();
            }

            string loadedText;

            if (rtb == RV_RichTextBox && R_Save.Enabled && !IsOkToLooseChanges) return null; // Warn about unsaved changes

            ResetRichTextBoxes(rtb, true);

            loadedText = File.ReadAllText(filePath);                // Load entire text from the file

            // BEGIN JSON TEST ####################################################################
#if DEBUG
            // DESERIALIZE

            Dictionary<string, string> dialogs = JsonConvert.DeserializeObject<Dictionary<string, string>>(loadedText);

            Console.WriteLine("Dialog count: " + dialogs.Count);    // + "\n" + string.Join(", ", dialogs.Keys.ToArray()));
            foreach (var item in dialogs) {
                Console.WriteLine("Key: " + item.Key + "   Dialog: " + item.Value);
            }

            // SERIALIZE

            string serialized = JsonConvert.SerializeObject(dialogs, Formatting.Indented);
            // {
            //   "Email": "james@example.com",
            //   "Active": true,
            //   "CreatedDate": "2013-01-20T00:00:00Z",
            //   "Roles": [
            //     "User",
            //     "Admin"
            //   ]
            // }

            FlexibleMessageBox.Show(serialized);

#endif
            // END JSON TEST ######################################################################


            rtb.Text = loadedText;                          // Write the loaded text in the richTextBox           

            //TODO: See if we can use richTextBox.Lines instead
            list = File.ReadAllLines(filePath).ToList();            // Load all lines from the file in a string list


            if (originalLines.Count > 0) LH_RichTextBox.Text = originalLines[0];     // Load first line in both horizontal boxes
            if (translatedLines.Count > 0) RH_RichTextBox.Text = translatedLines[0];
            LinePosition.Value = 0;                 // Set the starting line position to the begining
            TotalLines.Text = "of " + list.Count;   // Set line count beside the current line input box

            lineCountLabel.Text = lineCountLabelPrefix + list.Count;

            fileNameLabel.Text = fileNameLabelPrefix + Path.GetFileName(filePath); //openDialog.SafeFileName;

            if (IsValidJson(loadedText)) {
                jsonLabel.Text = jsonLabelPrefix + "OK";
            } else {
                jsonLabel.Text = jsonLabelPrefix + "ERROR";
            }

            // Count Keys
            int keyCount = 0;
            int noKeyCount = 0;
            //int validLineCount = 0;
            Match m;
            for (int i = 0; i < list.Count; i++) {
                string line = list[i];
                m = Regex.Match(line, keyPattern);
                if (m.Success) {
                    keyCount++;
                    if (IsOriginal) {
                        originalKeys.Add(m.Value);
                    } else {
                        translatedKeys.Add(m.Value);
                    }

                    //m = Regex.Match(line, validDialogPattern);
                    //if (m.Success) validLineCount++;
                } else {
                    noKeyCount++;
                }
            }
            if (IsOriginal) {
                originalKeyCount = keyCount;
            } else {
                translationKeyCount = keyCount;
            }

            //keyCountLabel.Text = keyCountLabelPrefix + keyCount + "   " + validLineCountLabelPrefix + validLineCount;
            keyCountLabel.Text = keyCountLabelPrefix + keyCount;

            if (LV_RichTextBox.Text != string.Empty && RV_RichTextBox.Text != string.Empty) {
                SetSelectionLineColors(LV_RichTextBox, previousLinePosition, defaultTextColor, defaultBackgroundColor, true); // Reset the previous selected line's colors
                SetSelectionLineColors(RV_RichTextBox, previousLinePosition, defaultTextColor, defaultBackgroundColor, true);
                SetSelectionLineColors(LV_RichTextBox, (int)LinePosition.Value, selectionTextColor, selectionBackgroundColor); // Set the new selected line's colors
                SetSelectionLineColors(RV_RichTextBox, (int)LinePosition.Value, selectionTextColor, selectionBackgroundColor);
                if (IsOkToWorkOnFiles()) {  // Only enable if both box have the same number of lines
                    NextLine.Enabled = LV_RichTextBox.Text != string.Empty;   // Make sure the buttons are only enabled if both text are loaded
                    PrevLine.Enabled = RV_RichTextBox.Text != string.Empty;
                }
                CheckForErrors();
            }
            HighlightKeywordsInAllBoxes(true);  // Skip vertical boxes
            R_Save.Enabled = false;     // Loading a new document doesn't count as a change
            return loadedText;
        }

        private void CheckForErrors() {
#if DEBUG
            if (originalLines.Count == translatedLines.Count) {
                L_LineCount.BackColor = R_LineCount.BackColor = defaultBackgroundColor;
                L_LineCount.ForeColor = R_LineCount.ForeColor = defaultTextColor;
            } else {
                L_LineCount.BackColor = R_LineCount.BackColor = errorBackgroundColor;
                L_LineCount.ForeColor = R_LineCount.ForeColor = Color.White;
            }

            if (originalKeyCount == translationKeyCount) {
                L_KeyCount.BackColor = R_KeyCount.BackColor = defaultBackgroundColor;
                L_KeyCount.ForeColor = R_KeyCount.ForeColor = defaultTextColor;
            } else {
                L_KeyCount.BackColor = R_KeyCount.BackColor = errorBackgroundColor;
                L_KeyCount.ForeColor = R_KeyCount.ForeColor = Color.White;
            }
            //L_LineCount.BackColor = R_LineCount.BackColor = originalLines.Count == translatedLines.Count ? defaultBackgroundColor : errorBackgroundColor;
            //L_KeyCount.BackColor = R_KeyCount.BackColor = originalKeyCount == translationKeyCount ? defaultBackgroundColor : errorBackgroundColor;

            // In Original Only
            string missingKeys = "";
            IEnumerable<string> differenceQuery = originalKeys.Except(translatedKeys);      // Create the query. Note that method syntax must be used here.  
            foreach (string s in differenceQuery) {  // Execute the query.
                missingKeys += s + originalKeys.FindIndex(x => x == s) + "\n";
            }
            FlexibleMessageBox.Show("The following keys are in the ORIGINAL but not the translation: \n\n" + missingKeys, title + " - Original key mismatch!", MessageBoxButtons.OKCancel);  // Output result

            // In Translation Only
            missingKeys = "";
            differenceQuery = translatedKeys.Except(originalKeys);      // Create the query. Note that method syntax must be used here.  
            foreach (string s in differenceQuery) {  // Execute the query.
                missingKeys += s + translatedKeys.FindIndex(x => x == s) + "\n";
            }
            FlexibleMessageBox.Show("The following keys are in the TRANSLATION but not the original: \n\n" + missingKeys, title + " - Translation key mismatch!", MessageBoxButtons.OKCancel);  // Output result
#endif
        }

        private bool IsOkToWorkOnFiles() {
            if (originalLines.Count == translatedLines.Count && originalLines.Count > 0) {
                return true;
            }
            return false;
        }

        private void SaveFile(SaveFileDialog saveDialog, RichTextBoxSynchronizedScroll rtb) {
            saveDialog.Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*";
            saveDialog.DefaultExt = "txt";
            if (saveDialog.ShowDialog() == DialogResult.OK) //Check if it's all ok  
            {
                string filename = saveDialog.FileName;
                File.WriteAllText(filename, rtb.Text); //Writes to the file
                //this.Text = title;
                R_FileName.Text = fileNameLabelPrefix + filename;
                R_Save.Enabled = false;
            }
        }

        void OverwriteLine(RichTextBoxSynchronizedScroll rtb, int lineIndex, string text) {
            int s1 = rtb.GetFirstCharIndexFromLine(lineIndex);
            int s2 = lineIndex < rtb.Lines.Count() - 1 ? rtb.GetFirstCharIndexFromLine(lineIndex + 1) - 1 : rtb.Text.Length;
            rtb.Select(s1, s2 - s1);
            rtb.SelectedText = text;
            translatedLines[lineIndex] = text;
        }

        private void ResetRichTextBoxes(RichTextBoxSynchronizedScroll richTextBox, bool resetSaveButton = false) {
            if (richTextBox == LV_RichTextBox || richTextBox == RH_RichTextBox) {           //TODO: Verify this code, looks weird but it works. Broke everything when I tried to change it, why?
                ResetRichTextBox(LV_RichTextBox);
                ResetRichTextBox(RH_RichTextBox);
            } else if (richTextBox == RV_RichTextBox || richTextBox == LH_RichTextBox) {
                ResetRichTextBox(RV_RichTextBox);
                ResetRichTextBox(LH_RichTextBox);
            }
            if (resetSaveButton) R_Save.Enabled = false;
        }

        private void ResetRichTextBox(RichTextBoxSynchronizedScroll richTextBox, Color? backgroundColor = null) {
            richTextBox.Text = "";  // Clear text
            Debug.WriteLine("ResetRichTextBox");
            SearchKeywordAndSetColors(richTextBox, @"(.*?)", backgroundColor ?? defaultBackgroundColor, defaultTextColor);  // Clear highlights
        }

        private void ChangeSelectedLine(LineChangeTypes changeType, int lineIndex = 0, bool skipAutoScroll = false) {
            ignoreChanges = true;
            previousLinePosition = (int)LinePosition.Value;
            switch (changeType) {
                case LineChangeTypes.FWD:
                    if (LinePosition.Value + 1 < originalLines.Count) LinePosition.Value++;
                    break;
                case LineChangeTypes.BWD:
                    if (LinePosition.Value - 1 >= 0) LinePosition.Value--;
                    break;
                default: // LineChangeTypes.NUM
                    if (lineIndex >= 0 && lineIndex < originalLines.Count) LinePosition.Value = lineIndex;
                    break;
            }

            ResetRichTextBox(LH_RichTextBox);
            LH_RichTextBox.Text = originalLines[(int)LinePosition.Value];   // Load text in horizontal boxes
            SetSelectionLineColors(LV_RichTextBox, previousLinePosition, defaultTextColor, defaultBackgroundColor, true);   // Deselect old lines
            SetSelectionLineColors(LV_RichTextBox, (int)LinePosition.Value, selectionTextColor, selectionBackgroundColor, skipAutoScroll);  // Select new lines

            ResetRichTextBox(RH_RichTextBox);
            RH_RichTextBox.Text = translatedLines[(int)LinePosition.Value]; // Load text in horizontal boxes
            SetSelectionLineColors(RV_RichTextBox, previousLinePosition, defaultTextColor, defaultBackgroundColor, true);   // Deselect old lines
            SetSelectionLineColors(RV_RichTextBox, (int)LinePosition.Value, selectionTextColor, selectionBackgroundColor, skipAutoScroll);  // Select new lines

            HighlightKeywordsInAllBoxes(true);  // Highlight tokens in horizontal boxes only
        }

        private void SetSelectionLineColors(RichTextBoxSynchronizedScroll richTextBox, int lineIndex, Color textColor, Color backgroundColor, bool skipScrollbarHandling = false) {
            int indexStart = richTextBox.GetFirstCharIndexFromLine(lineIndex);
            int indexEnd = richTextBox.GetFirstCharIndexFromLine(lineIndex + 1);
            if (!skipScrollbarHandling) {
                if (lineIndex > 0) {                                                                // If not the first line
                    richTextBox.Select(richTextBox.GetFirstCharIndexFromLine(lineIndex - 1), 0);    // Select the scroll position one line above
                } else {
                    richTextBox.Select(indexStart, 0);                                              // Select the scroll position
                }
                richTextBox.ScrollToCaret();                                                        // Scroll to position

                if (richTextBox == RV_RichTextBox && !string.IsNullOrWhiteSpace(richTextBox.Lines[lineIndex])) { // Workaround to disable horizontal autoscrolling
                    if (!RV_RichTextBox.Focused) RV_RichTextBox.Focus();   // Focus box
                    SendKeys.Send("{END}");     // Scroll all the way to the right
                    SendKeys.Send("{HOME}");    // Scroll all the way to the left (didn't work to send just HOME)
                }
            }

            richTextBox.Select(indexStart, 0);      // Select the proper line to highlight
            if (indexEnd < 0) indexEnd = richTextBox.Text.Length;
            richTextBox.SelectionStart = indexStart;
            richTextBox.SelectionLength = indexEnd - indexStart;
            richTextBox.SelectionColor = textColor;
            richTextBox.SelectionBackColor = backgroundColor;
        }

        private void HighlightKeywordsInAllBoxes(bool skipContextBoxes = false) {
            bool saveStatus = R_Save.Enabled;   // Backup save button status
            if (!skipContextBoxes) {
                if (LV_RichTextBox.Text != String.Empty) HighlightKeywords(LV_RichTextBox);
                if (RV_RichTextBox.Text != String.Empty) HighlightKeywords(RV_RichTextBox);
            }
            if (LH_RichTextBox.Text != String.Empty) HighlightKeywords(LH_RichTextBox);
            if (RH_RichTextBox.Text != String.Empty) HighlightKeywords(RH_RichTextBox);
            R_Save.Enabled = saveStatus;        // Restore save status so highlights dont count as a change
        }

        private void HighlightKeywords(RichTextBoxSynchronizedScroll richTextBox) { //, int? lineIndex = null) {
            // REGEX NOTES
            // Dialog format example    "keyString": "contentString",
            // Regex generator          https://regex-generator.olafneumann.org/
            // Look ahead abc(?!$)      Matches "abc" as long as it's not at the end of a line
            // Look behind (?<!^)abc    Matches "abc" as long as it's not at the begining of a line
            // Many occurences [a]{2,}  Matches 2 or more consecutive "a"

            // Errors
            SearchKeywordAndSetColors(richTextBox, @"\$", Color.Red, Color.White);                      //Incomplete portrait command (Highlight all $ in red. The correct ones get turned blue below)
            SearchKeywordAndSetColors(richTextBox, @"(?<!^)[\s]{2,}(?!$)", Color.Red, Color.White);     //Consecutive white space not at the begining or end of a line
            SearchKeywordAndSetColors(richTextBox, @"^\s*", Color.Red, Color.White);                    //Leading white spaces  // TODO: Find why we can't match the start of line char ^
            SearchKeywordAndSetColors(richTextBox, @"\s*$", Color.Red, Color.White);                    //Trailling white spaces

            // Custom Token
            SearchKeywordAndSetColors(richTextBox, fileChangedToken, Color.Purple, Color.White);    //Chunk change token

            // Key     // Sample: "event-75160123.23":     // ^\".*\": \"      // ^".*": "
            SearchKeywordAndSetColors(richTextBox, @""".*"": ", Color.Yellow);  // TODO: Find why we can't match the start of line char ^

            // Tokens
            SearchKeywordAndSetColors(richTextBox, @"\#", Color.Aquamarine, Color.Black);
            SearchKeywordAndSetColors(richTextBox, @"\{", Color.Aquamarine, Color.Black);
            SearchKeywordAndSetColors(richTextBox, @"\^", Color.Aquamarine, Color.Black);
            SearchKeywordAndSetColors(richTextBox, @"\%", Color.Aquamarine, Color.Black);
            SearchKeywordAndSetColors(richTextBox, @"\*", Color.Aquamarine, Color.Black);

            //Portrait commands
            SearchKeywordAndSetColors(richTextBox, @"\$[a-zA-Z0-9]", Color.Blue, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%fork", Color.Blue, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"\[[a-zA-Z0-9 {}:/.]+\]", Color.Blue, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%revealtaste[a-zA-Z]+[0-9]+", Color.Blue, Color.White);

            //Replacer commands
            SearchKeywordAndSetColors(richTextBox, @"@", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%adj", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%noun", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%place", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%spouse", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%name", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%firstnameletter", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%time", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%band", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%book", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%rival", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%pet", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%farm", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%favorite", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%kid1", Color.Orange, Color.White);
            SearchKeywordAndSetColors(richTextBox, @"%kid2", Color.Orange, Color.White);
        }

        private void SearchKeywordAndSetColors(RichTextBoxSynchronizedScroll rtb, string keyWord, Color backColor, Color? textColor = null) {
            foreach (Match match in Regex.Matches(rtb.Text, keyWord)) {
                SetRangeColors(rtb, match.Index, match.Length, backColor, textColor);
            }
        }

        private void SetRangeColors(RichTextBoxSynchronizedScroll rtb, int startIndex, int length, Color backgroundColor, Color? textColor = null) {
            rtb.SelectionStart = startIndex;
            rtb.SelectionLength = length;
            rtb.SelectionBackColor = backgroundColor;
            if (textColor.HasValue) rtb.SelectionColor = textColor.Value;
        }

        private void ContextClicked(RichTextBoxSynchronizedScroll rtb, Point clickPos) {    // TODO: Not ideal to check this EVERY time
            if (IsOkToWorkOnFiles()) {  // Only enable if both box have the same number of lines
                int clickedIndex = rtb.GetLineFromCharIndex(rtb.GetCharIndexFromPosition(rtb.PointToClient(clickPos)));
                ChangeSelectedLine(LineChangeTypes.NUM, clickedIndex, true);
            }
        }

        //private static bool IsValidJson(string jsonString) {  // Uses System.Json
        // Taken from: https://stackoverflow.com/questions/14977848/how-to-make-sure-that-string-is-valid-json-using-json-net
        //    try {
        //        var tmpObj = JsonValue.Parse(jsonString);
        //    }
        //    catch (FormatException fex) {
        //        //Invalid json format
        //        Console.WriteLine(fex);
        //    }
        //    catch (Exception ex) //some other exception
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //}

        private static bool IsValidJson(string strInput) {    // Uses Newtonsoft.Json
            // Taken from: https://stackoverflow.com/questions/14977848/how-to-make-sure-that-string-is-valid-json-using-json-net
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{ ") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex) {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            } else {
                return false;
            }
        }

        // ########################################################################################################
        // ################################################ EVENTS ################################################
        // ########################################################################################################

        // TODO: Verify if we want to use the richTextBox.lines array instead of storing copies in the originalLines and translatedLines lists
        // TODO: Add button to delete active line on one or both sides
        // TODO: Add button to add a line on one or both sides (make both original writtable when adding a new line)
        // TODO: Take into account command parameters when highlighting
        // TODO: Clean up and refactor code
        // TODO: Find better names for buttons, controls, events, methods and variables
        // TODO: Reorder code properly

        public Form1() {
            InitializeComponent();
            LinePosition.Controls[0].Hide();
            //LinePosition.Controls[1].Width -= 25; // DOESNT WORK?
            LinePosition.Maximum = 9999999;
            LV_RichTextBox.BindScroll(RV_RichTextBox);
            RH_RichTextBox.BindScroll(LH_RichTextBox);
            NextLine.Enabled = false;
            PrevLine.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.Text = title + " " + version;      // Set window title
            originalLines = new List<string>();     // init list
            translatedLines = new List<string>();   // init list
            defaultTextColor = LV_RichTextBox.ForeColor;        // Save default colors
            defaultBackgroundColor = LV_RichTextBox.BackColor;
        }

        private void Font_Click(object sender, EventArgs e) {   // Open font dialog and set font in all boxes
            L_FontDialog.Font = LV_RichTextBox.Font;
            if (L_FontDialog.ShowDialog() == DialogResult.OK) {
                LV_RichTextBox.Font = L_FontDialog.Font;
                RV_RichTextBox.Font = L_FontDialog.Font;
                RH_RichTextBox.Font = L_FontDialog.Font;
                LH_RichTextBox.Font = L_FontDialog.Font;
            }
        }

        private void HighlightContext_Click(object sender, EventArgs e) {
            if ((originalLines.Count + translatedLines.Count) > 5000) {
                DialogResult dialogResult = MessageBox.Show("Processing " + (originalLines.Count + translatedLines.Count) + " lines will take a while. Are you sure?", title, MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.Cancel) return;
            }
            HighlightKeywordsInAllBoxes();
        }

        private void RH_RichTextBox_KeyDown(object sender, KeyEventArgs e) {     // Filter input
            if (e.KeyCode == Keys.Enter ||
                e.KeyCode == Keys.Return ||
                e.KeyCode == Keys.Up ||
                e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.PageUp ||
                e.KeyCode == Keys.PageDown) {

                e.Handled = true;   // Mark event as complete to discard the input
            }
        }

        private void RH_RichTextBox_TextChanged(object sender, EventArgs e) {
            R_Save.Enabled = true;  // Enable save button
            if (!ignoreChanges && originalLines.Count > 0 && translatedLines.Count > 0 && RV_RichTextBox.Lines[(int)LinePosition.Value] != RH_RichTextBox.Text) {
                OverwriteLine(RV_RichTextBox, (int)LinePosition.Value, RH_RichTextBox.Text);
            }
            ignoreChanges = false;
        }

        private void L_Open_Click(object sender, EventArgs e) => LoadTextFromFilePath(OpenSingleFileDialog(L_OpenFileDialog), LV_RichTextBox, ref originalLines, L_LineCount, L_FileName, L_KeyCount, L_Json, true);
        private void R_Open_Click(object sender, EventArgs e) => LoadTextFromFilePath(OpenSingleFileDialog(R_OpenFileDialog), RV_RichTextBox, ref translatedLines, R_LineCount, R_FileName, R_KeyCount, R_Json, false);
        private void R_Save_Click(object sender, EventArgs e) => SaveFile(R_SaveFileDialog, RV_RichTextBox);
        private void Next_Click(object sender, EventArgs e) => ChangeSelectedLine(LineChangeTypes.FWD);
        private void Prev_Click(object sender, EventArgs e) => ChangeSelectedLine(LineChangeTypes.BWD);
        private void LV_RichTextBox_Click(object sender, EventArgs e) => ContextClicked(LV_RichTextBox, new Point(MousePosition.X, MousePosition.Y));
        private void RV_RichTextBox_Click(object sender, EventArgs e) => ContextClicked(RV_RichTextBox, new Point(MousePosition.X, MousePosition.Y));
        private void JoinFiles_Click(object sender, EventArgs e) => LoadTextFromMultipleFilePaths(OpenMultipleFilesDialog(J_OpenFileDialog), RV_RichTextBox, ref translatedLines, R_LineCount, R_FileName, R_KeyCount, R_Json);

        private void ClosingProgram(object sender, FormClosingEventArgs e) {
            if (R_Save.Enabled && !IsOkToLooseChanges) { // Warn about unsaved changes
                e.Cancel = true;
            }
        }

        private void TestButton_Click(object sender, EventArgs e) {
            //FlexibleMessageBox.Show(TranslateText("auto", "en", "Saucisse"));
            TranslateText2("auto", "en", "Saucisse");
        }
        public string TranslateText(string sourceLanguage, string targetLanguage, string textToTranslate) {
            //string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
            string url = String.Format("https://translate.google.ca/?sl={0}&tl={1}&text={2}", textToTranslate, targetLanguage, textToTranslate);
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.UTF8;
            string result = webClient.DownloadString(url);
            result = result.Substring(result.IndexOf("<span title=\"") + "<span title=\"".Length);
            result = result.Substring(result.IndexOf(">") + 1);
            result = result.Substring(0, result.IndexOf("</span>"));
            return result.Trim();
            //return result;
        }

        //public string TranslateText2(string sourceLanguage, string targetLanguage, string textToTranslate) {
        //    HttpClient httpClient = new HttpClient();

        //    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        //    string url = String.Format("https://translate.google.ca/?sl={0}&tl={1}&text={2}", textToTranslate, targetLanguage, textToTranslate);
        //    httpClient.BaseAddress = new Uri(url);
        //    httpClient.DefaultRequestHeaders.Accept.Clear();
        //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        //    var task = httpClient.PostAsXmlAsync<DeviceRequest>("api/SaveData", request);

        //}

        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        static readonly HttpClient client = new HttpClient();

        static async Task TranslateText2(string sourceLanguage, string targetLanguage, string textToTranslate) {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try {
                string url = String.Format("https://translate.google.ca/?sl={0}&tl={1}&text={2}", textToTranslate, targetLanguage, textToTranslate);
                //HttpResponseMessage response = await client.GetAsync(url);
                //response.EnsureSuccessStatusCode();
                //string responseBody = await response.Content.ReadAsStringAsync();
                //// Above three lines can be replaced with new helper method below
                string responseBody = await client.GetStringAsync(url);

                //Console.WriteLine(responseBody);
                FlexibleMessageBox.Show(responseBody);
            }
            catch (HttpRequestException e) {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }


        private void LinePosition_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && LinePosition.Focused) {
                e.Handled = e.SuppressKeyPress = true;      // Suppress enter key so we dont hear a warning sound
                lineNumOnFocus = (int)LinePosition.Value;
                ChangeSelectedLine(LineChangeTypes.NUM, (int)LinePosition.Value);
            }
        }

        private void LinePosition_LeaveFocus(object sender, EventArgs e) {
            LinePosition.Value = lineNumOnFocus;    // Restore the proper line num
        }

        private void LinePosition_EnterFocus(object sender, EventArgs e) {
            lineNumOnFocus = (int)LinePosition.Value;
        }
    }
}

// TODO: When clicking next we go to the next key index in the original document and we "search" for the match in the translation
// TODO: If the key is not found in the translation we offer to create it (by copying over)
// TODO: We need to show a screen with the extra keys in both the original and translation (a label/button called "Unique Keys" with a count and when you click it it loads the unique keys of each in the context
// TODO: When viewing the unique keys you click "back" to return to the main document (Maybe UNIQUE KEYS is a tab instead of a button)
// TODO: When viewing unique keys, clicking a unique key returns to the main document with the clicked key selected
// TODO: When a selected key is unique, offer actions to copy over the key, delete it or ignore it (enable grayed out buttons?)
// TODO: For any other selected line allow CREATING a new key or DELETING one (how do we deal with unique keys then? add new key to both sides? Add new key to ORIGINAL and offer to SAVE in original, translation, or both)

// TODO: OOPS! Comment on deal avec les comments et le custom formating du coter "ORIGNAL" si on peut creer des key? Peut etre quon petu pas faire ca. A la place on offre un devlopper mode ou tu edit un seul doc.
