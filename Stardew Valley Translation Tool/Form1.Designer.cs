namespace Stardew_Valley_Translation_Tool {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.L_Open = new System.Windows.Forms.Button();
            this.R_Save = new System.Windows.Forms.Button();
            this.L_FontDialog = new System.Windows.Forms.FontDialog();
            this.L_OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.R_SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.Font = new System.Windows.Forms.Button();
            this.HighlightContext = new System.Windows.Forms.Button();
            this.R_OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.R_Open = new System.Windows.Forms.Button();
            this.L_LineCount = new System.Windows.Forms.Label();
            this.R_LineCount = new System.Windows.Forms.Label();
            this.L_FileName = new System.Windows.Forms.Label();
            this.R_FileName = new System.Windows.Forms.Label();
            this.L_KeyCount = new System.Windows.Forms.Label();
            this.R_KeyCount = new System.Windows.Forms.Label();
            this.L_Json = new System.Windows.Forms.Label();
            this.R_Json = new System.Windows.Forms.Label();
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.LV_RichTextBox = new System.Windows.Forms.RichTextBoxSynchronizedScroll();
            this.RV_RichTextBox = new System.Windows.Forms.RichTextBoxSynchronizedScroll();
            this.NextLine = new System.Windows.Forms.Button();
            this.PrevLine = new System.Windows.Forms.Button();
            this.TotalLines = new System.Windows.Forms.Label();
            this.LinePosition = new System.Windows.Forms.NumericUpDown();
            this.LH_RichTextBox = new System.Windows.Forms.RichTextBoxSynchronizedScroll();
            this.RH_RichTextBox = new System.Windows.Forms.RichTextBoxSynchronizedScroll();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LinePosition)).BeginInit();
            this.SuspendLayout();
            // 
            // L_Open
            // 
            this.L_Open.Location = new System.Drawing.Point(3, 4);
            this.L_Open.Name = "L_Open";
            this.L_Open.Size = new System.Drawing.Size(100, 23);
            this.L_Open.TabIndex = 0;
            this.L_Open.Text = "Open Original";
            this.L_Open.UseVisualStyleBackColor = true;
            this.L_Open.Click += new System.EventHandler(this.L_Open_Click);
            // 
            // R_Save
            // 
            this.R_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.R_Save.Enabled = false;
            this.R_Save.Location = new System.Drawing.Point(680, 4);
            this.R_Save.Name = "R_Save";
            this.R_Save.Size = new System.Drawing.Size(100, 23);
            this.R_Save.TabIndex = 2;
            this.R_Save.Text = "Save Translation";
            this.R_Save.UseVisualStyleBackColor = true;
            this.R_Save.Click += new System.EventHandler(this.R_Save_Click);
            // 
            // L_OpenFileDialog
            // 
            this.L_OpenFileDialog.FileName = "openFileDialog1";
            // 
            // Font
            // 
            this.Font.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.Font.Location = new System.Drawing.Point(294, 188);
            this.Font.Name = "Font";
            this.Font.Size = new System.Drawing.Size(95, 23);
            this.Font.TabIndex = 6;
            this.Font.Text = "Font";
            this.Font.UseVisualStyleBackColor = true;
            this.Font.Click += new System.EventHandler(this.Font_Click);
            // 
            // HighlightContext
            // 
            this.HighlightContext.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.HighlightContext.Location = new System.Drawing.Point(196, 188);
            this.HighlightContext.Name = "HighlightContext";
            this.HighlightContext.Size = new System.Drawing.Size(95, 23);
            this.HighlightContext.TabIndex = 5;
            this.HighlightContext.Text = "Highlight Context";
            this.HighlightContext.UseVisualStyleBackColor = true;
            this.HighlightContext.Click += new System.EventHandler(this.HighlightContext_Click);
            // 
            // R_OpenFileDialog
            // 
            this.R_OpenFileDialog.FileName = "openFileDialog1";
            // 
            // R_Open
            // 
            this.R_Open.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.R_Open.Location = new System.Drawing.Point(392, 4);
            this.R_Open.Name = "R_Open";
            this.R_Open.Size = new System.Drawing.Size(100, 23);
            this.R_Open.TabIndex = 1;
            this.R_Open.Text = "Open Translation";
            this.R_Open.UseVisualStyleBackColor = true;
            this.R_Open.Click += new System.EventHandler(this.R_Open_Click);
            // 
            // L_LineCount
            // 
            this.L_LineCount.AutoSize = true;
            this.L_LineCount.Location = new System.Drawing.Point(4, 41);
            this.L_LineCount.Name = "L_LineCount";
            this.L_LineCount.Size = new System.Drawing.Size(63, 13);
            this.L_LineCount.TabIndex = 7;
            this.L_LineCount.Text = "Line count: ";
            // 
            // R_LineCount
            // 
            this.R_LineCount.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.R_LineCount.AutoSize = true;
            this.R_LineCount.Location = new System.Drawing.Point(393, 41);
            this.R_LineCount.Name = "R_LineCount";
            this.R_LineCount.Size = new System.Drawing.Size(63, 13);
            this.R_LineCount.TabIndex = 8;
            this.R_LineCount.Text = "Line count: ";
            // 
            // L_FileName
            // 
            this.L_FileName.AutoSize = true;
            this.L_FileName.Location = new System.Drawing.Point(4, 28);
            this.L_FileName.Name = "L_FileName";
            this.L_FileName.Size = new System.Drawing.Size(55, 13);
            this.L_FileName.TabIndex = 9;
            this.L_FileName.Text = "Filename: ";
            // 
            // R_FileName
            // 
            this.R_FileName.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.R_FileName.AutoSize = true;
            this.R_FileName.Location = new System.Drawing.Point(393, 28);
            this.R_FileName.Name = "R_FileName";
            this.R_FileName.Size = new System.Drawing.Size(55, 13);
            this.R_FileName.TabIndex = 10;
            this.R_FileName.Text = "Filename: ";
            // 
            // L_KeyCount
            // 
            this.L_KeyCount.AutoSize = true;
            this.L_KeyCount.Location = new System.Drawing.Point(4, 54);
            this.L_KeyCount.Name = "L_KeyCount";
            this.L_KeyCount.Size = new System.Drawing.Size(61, 13);
            this.L_KeyCount.TabIndex = 11;
            this.L_KeyCount.Text = "Key count: ";
            // 
            // R_KeyCount
            // 
            this.R_KeyCount.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.R_KeyCount.AutoSize = true;
            this.R_KeyCount.Location = new System.Drawing.Point(393, 54);
            this.R_KeyCount.Name = "R_KeyCount";
            this.R_KeyCount.Size = new System.Drawing.Size(61, 13);
            this.R_KeyCount.TabIndex = 12;
            this.R_KeyCount.Text = "Key count: ";
            // 
            // L_Json
            // 
            this.L_Json.AutoSize = true;
            this.L_Json.Location = new System.Drawing.Point(4, 67);
            this.L_Json.Name = "L_Json";
            this.L_Json.Size = new System.Drawing.Size(35, 13);
            this.L_Json.TabIndex = 13;
            this.L_Json.Text = "Json: ";
            // 
            // R_Json
            // 
            this.R_Json.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.R_Json.AutoSize = true;
            this.R_Json.Location = new System.Drawing.Point(393, 67);
            this.R_Json.Name = "R_Json";
            this.R_Json.Size = new System.Drawing.Size(35, 13);
            this.R_Json.TabIndex = 14;
            this.R_Json.Text = "Json: ";
            // 
            // SplitContainer1
            // 
            this.SplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitContainer1.Location = new System.Drawing.Point(0, 218);
            this.SplitContainer1.Name = "SplitContainer1";
            // 
            // SplitContainer1.Panel1
            // 
            this.SplitContainer1.Panel1.Controls.Add(this.LV_RichTextBox);
            // 
            // SplitContainer1.Panel2
            // 
            this.SplitContainer1.Panel2.Controls.Add(this.RV_RichTextBox);
            this.SplitContainer1.Size = new System.Drawing.Size(784, 340);
            this.SplitContainer1.SplitterDistance = 390;
            this.SplitContainer1.SplitterWidth = 2;
            this.SplitContainer1.TabIndex = 15;
            // 
            // LV_RichTextBox
            // 
            this.LV_RichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LV_RichTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LV_RichTextBox.Location = new System.Drawing.Point(3, 0);
            this.LV_RichTextBox.Name = "LV_RichTextBox";
            this.LV_RichTextBox.ReadOnly = true;
            this.LV_RichTextBox.Size = new System.Drawing.Size(386, 340);
            this.LV_RichTextBox.TabIndex = 3;
            this.LV_RichTextBox.Text = "";
            this.LV_RichTextBox.WordWrap = false;
            this.LV_RichTextBox.Click += new System.EventHandler(this.LV_RichTextBox_Click);
            // 
            // RV_RichTextBox
            // 
            this.RV_RichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RV_RichTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RV_RichTextBox.Location = new System.Drawing.Point(0, 0);
            this.RV_RichTextBox.Name = "RV_RichTextBox";
            this.RV_RichTextBox.ReadOnly = true;
            this.RV_RichTextBox.Size = new System.Drawing.Size(406, 340);
            this.RV_RichTextBox.TabIndex = 4;
            this.RV_RichTextBox.Text = "";
            this.RV_RichTextBox.WordWrap = false;
            this.RV_RichTextBox.Click += new System.EventHandler(this.RV_RichTextBox_Click);
            // 
            // NextLine
            // 
            this.NextLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NextLine.Location = new System.Drawing.Point(705, 188);
            this.NextLine.Name = "NextLine";
            this.NextLine.Size = new System.Drawing.Size(75, 23);
            this.NextLine.TabIndex = 18;
            this.NextLine.Text = "Next";
            this.NextLine.UseVisualStyleBackColor = true;
            this.NextLine.Click += new System.EventHandler(this.Next_Click);
            // 
            // PrevLine
            // 
            this.PrevLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PrevLine.Location = new System.Drawing.Point(628, 188);
            this.PrevLine.Name = "PrevLine";
            this.PrevLine.Size = new System.Drawing.Size(75, 23);
            this.PrevLine.TabIndex = 19;
            this.PrevLine.Text = "Prev";
            this.PrevLine.UseVisualStyleBackColor = true;
            this.PrevLine.Click += new System.EventHandler(this.Prev_Click);
            // 
            // TotalLines
            // 
            this.TotalLines.AutoSize = true;
            this.TotalLines.Location = new System.Drawing.Point(73, 193);
            this.TotalLines.Name = "TotalLines";
            this.TotalLines.Size = new System.Drawing.Size(25, 13);
            this.TotalLines.TabIndex = 20;
            this.TotalLines.Text = "of 0";
            // 
            // LinePosition
            // 
            this.LinePosition.Location = new System.Drawing.Point(4, 190);
            this.LinePosition.Name = "LinePosition";
            this.LinePosition.Size = new System.Drawing.Size(64, 20);
            this.LinePosition.TabIndex = 22;
            this.LinePosition.ValueChanged += new System.EventHandler(this.LinePosition_ValueChanged);
            // 
            // LH_RichTextBox
            // 
            this.LH_RichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LH_RichTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LH_RichTextBox.Location = new System.Drawing.Point(3, 92);
            this.LH_RichTextBox.Name = "LH_RichTextBox";
            this.LH_RichTextBox.ReadOnly = true;
            this.LH_RichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedHorizontal;
            this.LH_RichTextBox.Size = new System.Drawing.Size(777, 42);
            this.LH_RichTextBox.TabIndex = 16;
            this.LH_RichTextBox.Text = "";
            this.LH_RichTextBox.WordWrap = false;
            // 
            // RH_RichTextBox
            // 
            this.RH_RichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RH_RichTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RH_RichTextBox.Location = new System.Drawing.Point(3, 140);
            this.RH_RichTextBox.Name = "RH_RichTextBox";
            this.RH_RichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedHorizontal;
            this.RH_RichTextBox.Size = new System.Drawing.Size(777, 42);
            this.RH_RichTextBox.TabIndex = 17;
            this.RH_RichTextBox.Text = "";
            this.RH_RichTextBox.WordWrap = false;
            this.RH_RichTextBox.TextChanged += new System.EventHandler(this.RH_RichTextBox_TextChanged);
            this.RH_RichTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RH_RichTextBox_KeyDown);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button1.Location = new System.Drawing.Point(392, 188);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Split File";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button2.Location = new System.Drawing.Point(489, 188);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(95, 23);
            this.button2.TabIndex = 23;
            this.button2.Text = "Join Files";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LinePosition);
            this.Controls.Add(this.TotalLines);
            this.Controls.Add(this.PrevLine);
            this.Controls.Add(this.NextLine);
            this.Controls.Add(this.LH_RichTextBox);
            this.Controls.Add(this.RH_RichTextBox);
            this.Controls.Add(this.L_Open);
            this.Controls.Add(this.Font);
            this.Controls.Add(this.L_FileName);
            this.Controls.Add(this.L_LineCount);
            this.Controls.Add(this.L_KeyCount);
            this.Controls.Add(this.L_Json);
            this.Controls.Add(this.R_Open);
            this.Controls.Add(this.R_Save);
            this.Controls.Add(this.HighlightContext);
            this.Controls.Add(this.R_FileName);
            this.Controls.Add(this.R_LineCount);
            this.Controls.Add(this.R_KeyCount);
            this.Controls.Add(this.R_Json);
            this.Controls.Add(this.SplitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Title";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LinePosition)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button L_Open;
        private System.Windows.Forms.Button R_Save;
        private System.Windows.Forms.RichTextBoxSynchronizedScroll LV_RichTextBox;
        private System.Windows.Forms.RichTextBoxSynchronizedScroll RV_RichTextBox;
        private System.Windows.Forms.FontDialog L_FontDialog;
        private System.Windows.Forms.OpenFileDialog L_OpenFileDialog;
        private System.Windows.Forms.SaveFileDialog R_SaveFileDialog;
        private System.Windows.Forms.Button Font;
        private System.Windows.Forms.Button HighlightContext;
        private System.Windows.Forms.OpenFileDialog R_OpenFileDialog;
        private System.Windows.Forms.Button R_Open;
        private System.Windows.Forms.Label L_LineCount;
        private System.Windows.Forms.Label R_LineCount;
        private System.Windows.Forms.Label L_FileName;
        private System.Windows.Forms.Label R_FileName;
        private System.Windows.Forms.Label L_KeyCount;
        private System.Windows.Forms.Label R_KeyCount;
        private System.Windows.Forms.Label L_Json;
        private System.Windows.Forms.Label R_Json;
        private System.Windows.Forms.SplitContainer SplitContainer1;
        private System.Windows.Forms.RichTextBoxSynchronizedScroll LH_RichTextBox;
        private System.Windows.Forms.RichTextBoxSynchronizedScroll RH_RichTextBox;
        private System.Windows.Forms.Button NextLine;
        private System.Windows.Forms.Button PrevLine;
        private System.Windows.Forms.Label TotalLines;
        private System.Windows.Forms.NumericUpDown LinePosition;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

