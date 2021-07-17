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

namespace Stardew_Valley_Translation_Tool {
    
    internal enum LineChangeTypes { FWD, BWD, NUM }

    public partial class Form1 : Form {

        string title = "Stardew Valley Translation Tool";
        string version = "v0.1";
        string lineCountLabelPrefix = "Line count: ";
        string fileNameLabelPrefix = "Filename: ";
        string keyCountLabelPrefix = "Key count: ";
        string validLineCountLabelPrefix = "Valid Dialogs: ";
        string jsonLabelPrefix = "Json: ";
        string keyPattern = "^\".*\": \"";                // Matches: "keyString": "
        string validDialogPattern = "^\".*\": \".*\",$";  // Matches: "keyString": "contentString",
        string fileChangedToken = @"~FILE CHANGE~";

        private List<string> originalLines;
        private List<string> translatedLines;
        private int previousLinePosition = 0;
        private bool ignoreChanges;
        private Color defaultBackgroundColor;
        private Color defaultTextColor;
        private Color selectionTextColor = Color.Aquamarine;
        private Color selectionBackgroundColor = Color.SteelBlue;

        private bool IsOkToLooseChanges {
            get {
                DialogResult dialogResult = MessageBox.Show("You will lose unsaved changes, continue?", title, MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK) return true;
                return false;
            }
        }

        /// <returns>A string containing the paths of the selected file</returns>
        private string OpenSingleFileDialog(OpenFileDialog openDialog) {//, RichTextBoxSynchronizedScroll richTextBox, ref List<string> list, Label lineCountLabel, Label fileNameLabel, Label keyCountLabel, Label jsonLabel) {
            openDialog.Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*";

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
            openMultipleDialog.Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*";

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
        private string LoadTextFromMultipleFilePaths(string[] filePaths, RichTextBoxSynchronizedScroll richTextBox, ref List<string> list, Label lineCountLabel, Label fileNameLabel, Label keyCountLabel, Label jsonLabel) {
            if (filePaths == null) return null;     // If its null the user clicked cancel, return

            string loadedText = "";

            if (richTextBox == RV_RichTextBox && R_Save.Enabled && !IsOkToLooseChanges) return null; // Warn about unsaved changes

            ResetRichTextBoxes(richTextBox, true);

            for (int i = 0; i < filePaths.Length; i++) {
                if (i + 1 != filePaths.Length)
                Debug.WriteLine("i: " + i + "   filePaths.Length: " + filePaths.Length);
                loadedText += File.ReadAllText(filePaths[i]) + (i + 1 != filePaths.Length ? fileChangedToken:"");    // Load entire text from the file and insert a file change token
            }

            richTextBox.Text = loadedText;              // Write the loaded text in the richTextBox           
            
            list = richTextBox.Lines.ToList();          // Load all lines from the file in a string list         

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
            int validLineCount = 0;
            Match m;
            foreach (var line in list) {
                m = Regex.Match(line, keyPattern);
                if (m.Success) {
                    keyCount++;
                    m = Regex.Match(line, validDialogPattern);
                    if (m.Success) validLineCount++;
                } else {
                    noKeyCount++;
                }
            }

            keyCountLabel.Text = keyCountLabelPrefix + keyCount + "   " + validLineCountLabelPrefix + validLineCount;

            if (LV_RichTextBox.Text != string.Empty && RV_RichTextBox.Text != string.Empty) {
                SetSelectionLineColors(LV_RichTextBox, previousLinePosition, defaultTextColor, defaultBackgroundColor, true); // Deselect old lines
                SetSelectionLineColors(RV_RichTextBox, previousLinePosition, defaultTextColor, defaultBackgroundColor, true);
                SetSelectionLineColors(LV_RichTextBox, (int)LinePosition.Value, selectionTextColor, selectionBackgroundColor); // Select new lines
                SetSelectionLineColors(RV_RichTextBox, (int)LinePosition.Value, selectionTextColor, selectionBackgroundColor);
                NextLine.Enabled = LV_RichTextBox.Text != string.Empty;   // Make sure the buttons are only enabled if both text are loaded
                PrevLine.Enabled = RV_RichTextBox.Text != string.Empty;
            }
            HighlightKeywordsInAllBoxes(true);  // Skip vertical boxes
            //R_Save.Enabled = false;     // Loading a new document doesn't count as a change // In multiple, yes it does
            return loadedText;
        }

        /// <returns>A string containing the text loaded from the file</returns>    // TODO: Needs cleanup
        private string LoadTextFromFilePath(string filePath, RichTextBoxSynchronizedScroll richTextBox, ref List<string> list, Label lineCountLabel, Label fileNameLabel, Label keyCountLabel, Label jsonLabel) {
            if (string.IsNullOrWhiteSpace(filePath)) return null;   // If its null the user clicked cancel, return

            string loadedText;

            if (richTextBox == RV_RichTextBox && R_Save.Enabled && !IsOkToLooseChanges) return null; // Warn about unsaved changes

            ResetRichTextBoxes(richTextBox, true);

            loadedText = File.ReadAllText(filePath);                // Load entire text from the file

            richTextBox.Text = loadedText;                          // Write the loaded text in the richTextBox           

            //TODO: See if we can use richTextBox.Lines instead
            list = File.ReadAllLines(filePath).ToList();            // Load all lines from the file in a string list
           

            if (originalLines.Count > 0) LH_RichTextBox.Text = originalLines[0];     // Load first line in both horizontal boxes
            if (translatedLines.Count > 0) RH_RichTextBox.Text = translatedLines[0];
            LinePosition.Value = 0;
            TotalLines.Text = "of " + list.Count;

            lineCountLabel.Text = lineCountLabelPrefix + list.Count;

            fileNameLabel.Text = fileNameLabelPrefix + Path.GetFileName(filePath); //openDialog.SafeFileName;

            if (IsValidJson(loadedText)) {
                jsonLabel.Text = jsonLabelPrefix + "OK";
            } else {
                jsonLabel.Text = jsonLabelPrefix + "ERROR";
            }

            int keyCount = 0;
            int noKeyCount = 0;
            int validLineCount = 0;
            Match m;
            foreach (var line in list) {
                m = Regex.Match(line, keyPattern);
                if (m.Success) {
                    keyCount++;
                    m = Regex.Match(line, validDialogPattern);
                    if (m.Success) validLineCount++;
                } else {
                    noKeyCount++;
                }
            }

            keyCountLabel.Text = keyCountLabelPrefix + keyCount + "   " + validLineCountLabelPrefix + validLineCount;

            if (LV_RichTextBox.Text != string.Empty && RV_RichTextBox.Text != string.Empty) {
                SetSelectionLineColors(LV_RichTextBox, previousLinePosition, defaultTextColor, defaultBackgroundColor, true); // Deselect old lines
                SetSelectionLineColors(RV_RichTextBox, previousLinePosition, defaultTextColor, defaultBackgroundColor, true);
                SetSelectionLineColors(LV_RichTextBox, (int)LinePosition.Value, selectionTextColor, selectionBackgroundColor); // Select new lines
                SetSelectionLineColors(RV_RichTextBox, (int)LinePosition.Value, selectionTextColor, selectionBackgroundColor);
                if (IsOkToWorkOnFiles()) {  // Only enable if both box have the same number of lines
                    NextLine.Enabled = LV_RichTextBox.Text != string.Empty;   // Make sure the buttons are only enabled if both text are loaded
                    PrevLine.Enabled = RV_RichTextBox.Text != string.Empty;
                }
            }
            HighlightKeywordsInAllBoxes(true);  // Skip vertical boxes
            R_Save.Enabled = false;     // Loading a new document doesn't count as a change
            return loadedText;
        }

        private bool IsOkToWorkOnFiles() {
            if (originalLines.Count == translatedLines.Count && originalLines.Count > 0) {
                return true;
            }
            return false;
        }

        private void SaveFile(SaveFileDialog saveDialog, RichTextBoxSynchronizedScroll richTextBox) {
            saveDialog.Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*";
            saveDialog.DefaultExt = "txt";
            if (saveDialog.ShowDialog() == DialogResult.OK) //Check if it's all ok  
            {
                string filename = saveDialog.FileName;
                File.WriteAllText(filename, richTextBox.Text); //Writes to the file
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

        private void L_Open_Click(object sender, EventArgs e) => LoadTextFromFilePath(OpenSingleFileDialog(L_OpenFileDialog), LV_RichTextBox, ref originalLines, L_LineCount, L_FileName, L_KeyCount, L_Json);
        private void R_Open_Click(object sender, EventArgs e) => LoadTextFromFilePath(OpenSingleFileDialog(R_OpenFileDialog), RV_RichTextBox, ref translatedLines, R_LineCount, R_FileName, R_KeyCount, R_Json);
        private void R_Save_Click(object sender, EventArgs e) => SaveFile(R_SaveFileDialog, RV_RichTextBox);
        private void Next_Click(object sender, EventArgs e) => ChangeSelectedLine(LineChangeTypes.FWD);
        private void Prev_Click(object sender, EventArgs e) => ChangeSelectedLine(LineChangeTypes.BWD);
        private void LinePosition_ValueChanged(object sender, EventArgs e) { if (LinePosition.Focused) ChangeSelectedLine(LineChangeTypes.NUM, (int)LinePosition.Value); } // React to manual change only
        private void LV_RichTextBox_Click(object sender, EventArgs e) => ContextClicked(LV_RichTextBox, new Point(MousePosition.X, MousePosition.Y));
        private void RV_RichTextBox_Click(object sender, EventArgs e) => ContextClicked(RV_RichTextBox, new Point(MousePosition.X, MousePosition.Y));
        private void JoinFiles_Click(object sender, EventArgs e) => LoadTextFromMultipleFilePaths(OpenMultipleFilesDialog(J_OpenFileDialog), RV_RichTextBox, ref translatedLines, R_LineCount, R_FileName, R_KeyCount, R_Json);

        private void ClosingProgram(object sender, FormClosingEventArgs e) {
            if (R_Save.Enabled && !IsOkToLooseChanges) { // Warn about unsaved changes
                e.Cancel = true;
            }
        }
    }
}