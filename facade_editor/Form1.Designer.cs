using System.Security.Principal;

namespace facade_editor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.globalCheckBox = new System.Windows.Forms.CheckBox();
            this.tripCheckBox = new System.Windows.Forms.CheckBox();
            this.graceCheckBox = new System.Windows.Forms.CheckBox();
            this.chanceTextBox = new System.Windows.Forms.TextBox();
            this.replaceLogTextBox = new System.Windows.Forms.TextBox();
            this.chanceCheckBox = new System.Windows.Forms.CheckBox();
            this.clearListButton = new System.Windows.Forms.Button();
            this.soundListLabel = new System.Windows.Forms.Label();
            this.replaceBrowseButton = new System.Windows.Forms.Button();
            this.replaceButton = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.animationsHardCorruptionCheckBox = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.restoreButton = new System.Windows.Forms.Button();
            this.useBackupFilesRadioButton = new System.Windows.Forms.RadioButton();
            this.dontUseFilesFromBackupRadioButton = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.lettersTextureIntactCheckBox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.warningLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pathTextbox = new System.Windows.Forms.TextBox();
            this.randomizeLogTextBox = new System.Windows.Forms.TextBox();
            this.animationsCheckBox = new System.Windows.Forms.CheckBox();
            this.randomizeButton = new System.Windows.Forms.Button();
            this.cursorsCheckBox = new System.Windows.Forms.CheckBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.soundsCheckBox = new System.Windows.Forms.CheckBox();
            this.texturesCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.javaDebugCheckBox = new System.Windows.Forms.CheckBox();
            this.launchButton = new System.Windows.Forms.Button();
            this.decompressedCheckBox = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.decompressButton = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.AICheckBox = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dramaManagerCheckBox = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.creditLabel = new System.Windows.Forms.Label();
            this.superSecretPictureBox = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.superSecretPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.globalCheckBox);
            this.tabPage2.Controls.Add(this.tripCheckBox);
            this.tabPage2.Controls.Add(this.graceCheckBox);
            this.tabPage2.Controls.Add(this.chanceTextBox);
            this.tabPage2.Controls.Add(this.replaceLogTextBox);
            this.tabPage2.Controls.Add(this.chanceCheckBox);
            this.tabPage2.Controls.Add(this.clearListButton);
            this.tabPage2.Controls.Add(this.soundListLabel);
            this.tabPage2.Controls.Add(this.replaceBrowseButton);
            this.tabPage2.Controls.Add(this.replaceButton);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(592, 419);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Replace";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(559, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Here you can add your own sound files, and the program will add them randomly to " +
    "the sound paths you select below.";
            // 
            // globalCheckBox
            // 
            this.globalCheckBox.AutoSize = true;
            this.globalCheckBox.Checked = true;
            this.globalCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.globalCheckBox.Location = new System.Drawing.Point(10, 103);
            this.globalCheckBox.Name = "globalCheckBox";
            this.globalCheckBox.Size = new System.Drawing.Size(219, 17);
            this.globalCheckBox.TabIndex = 18;
            this.globalCheckBox.Text = "Replace \"global\" sounds (Sound effects)";
            this.globalCheckBox.UseVisualStyleBackColor = true;
            // 
            // tripCheckBox
            // 
            this.tripCheckBox.AutoSize = true;
            this.tripCheckBox.Checked = true;
            this.tripCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tripCheckBox.Location = new System.Drawing.Point(10, 83);
            this.tripCheckBox.Name = "tripCheckBox";
            this.tripCheckBox.Size = new System.Drawing.Size(131, 17);
            this.tripCheckBox.TabIndex = 17;
            this.tripCheckBox.Text = "Replace Trip\'s sounds";
            this.tripCheckBox.UseVisualStyleBackColor = true;
            // 
            // graceCheckBox
            // 
            this.graceCheckBox.AutoSize = true;
            this.graceCheckBox.Checked = true;
            this.graceCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.graceCheckBox.Location = new System.Drawing.Point(10, 63);
            this.graceCheckBox.Name = "graceCheckBox";
            this.graceCheckBox.Size = new System.Drawing.Size(142, 17);
            this.graceCheckBox.TabIndex = 16;
            this.graceCheckBox.Text = "Replace Grace\'s sounds";
            this.graceCheckBox.UseVisualStyleBackColor = true;
            // 
            // chanceTextBox
            // 
            this.chanceTextBox.Enabled = false;
            this.chanceTextBox.Location = new System.Drawing.Point(121, 124);
            this.chanceTextBox.MaxLength = 3;
            this.chanceTextBox.Name = "chanceTextBox";
            this.chanceTextBox.Size = new System.Drawing.Size(25, 20);
            this.chanceTextBox.TabIndex = 15;
            this.chanceTextBox.Text = "100";
            this.chanceTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.chanceTextBox_KeyPress);
            // 
            // replaceLogTextBox
            // 
            this.replaceLogTextBox.Location = new System.Drawing.Point(6, 272);
            this.replaceLogTextBox.Multiline = true;
            this.replaceLogTextBox.Name = "replaceLogTextBox";
            this.replaceLogTextBox.ReadOnly = true;
            this.replaceLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.replaceLogTextBox.Size = new System.Drawing.Size(580, 141);
            this.replaceLogTextBox.TabIndex = 8;
            // 
            // chanceCheckBox
            // 
            this.chanceCheckBox.AutoSize = true;
            this.chanceCheckBox.Location = new System.Drawing.Point(10, 126);
            this.chanceCheckBox.Name = "chanceCheckBox";
            this.chanceCheckBox.Size = new System.Drawing.Size(192, 17);
            this.chanceCheckBox.TabIndex = 14;
            this.chanceCheckBox.Text = "Copy files only with          % chance";
            this.chanceCheckBox.UseVisualStyleBackColor = true;
            this.chanceCheckBox.CheckedChanged += new System.EventHandler(this.chanceCheckBox_CheckedChanged);
            // 
            // clearListButton
            // 
            this.clearListButton.Enabled = false;
            this.clearListButton.Location = new System.Drawing.Point(511, 28);
            this.clearListButton.Name = "clearListButton";
            this.clearListButton.Size = new System.Drawing.Size(75, 23);
            this.clearListButton.TabIndex = 11;
            this.clearListButton.Text = "Clear list";
            this.clearListButton.UseVisualStyleBackColor = true;
            this.clearListButton.Click += new System.EventHandler(this.clearListButton_Click);
            // 
            // soundListLabel
            // 
            this.soundListLabel.AutoSize = true;
            this.soundListLabel.Location = new System.Drawing.Point(6, 33);
            this.soundListLabel.Name = "soundListLabel";
            this.soundListLabel.Size = new System.Drawing.Size(386, 13);
            this.soundListLabel.TabIndex = 9;
            this.soundListLabel.Text = "Click browse to add the sound files. You can add more by clicking browse again.";
            // 
            // replaceBrowseButton
            // 
            this.replaceBrowseButton.Location = new System.Drawing.Point(430, 28);
            this.replaceBrowseButton.Name = "replaceBrowseButton";
            this.replaceBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.replaceBrowseButton.TabIndex = 2;
            this.replaceBrowseButton.Text = "Browse";
            this.replaceBrowseButton.UseVisualStyleBackColor = true;
            this.replaceBrowseButton.Click += new System.EventHandler(this.replaceBrowseButton_Click);
            // 
            // replaceButton
            // 
            this.replaceButton.Location = new System.Drawing.Point(511, 243);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(75, 23);
            this.replaceButton.TabIndex = 0;
            this.replaceButton.Text = "Replace";
            this.replaceButton.UseVisualStyleBackColor = true;
            this.replaceButton.Click += new System.EventHandler(this.replaceButton_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.animationsHardCorruptionCheckBox);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.restoreButton);
            this.tabPage1.Controls.Add(this.useBackupFilesRadioButton);
            this.tabPage1.Controls.Add(this.dontUseFilesFromBackupRadioButton);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.lettersTextureIntactCheckBox);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.warningLabel);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.pathTextbox);
            this.tabPage1.Controls.Add(this.randomizeLogTextBox);
            this.tabPage1.Controls.Add(this.animationsCheckBox);
            this.tabPage1.Controls.Add(this.randomizeButton);
            this.tabPage1.Controls.Add(this.cursorsCheckBox);
            this.tabPage1.Controls.Add(this.browseButton);
            this.tabPage1.Controls.Add(this.soundsCheckBox);
            this.tabPage1.Controls.Add(this.texturesCheckBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(592, 419);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Randomize";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // animationsHardCorruptionCheckBox
            // 
            this.animationsHardCorruptionCheckBox.AutoSize = true;
            this.animationsHardCorruptionCheckBox.Enabled = false;
            this.animationsHardCorruptionCheckBox.Location = new System.Drawing.Point(93, 201);
            this.animationsHardCorruptionCheckBox.Name = "animationsHardCorruptionCheckBox";
            this.animationsHardCorruptionCheckBox.Size = new System.Drawing.Size(99, 17);
            this.animationsHardCorruptionCheckBox.TabIndex = 19;
            this.animationsHardCorruptionCheckBox.Text = "Hard corruption";
            this.animationsHardCorruptionCheckBox.UseVisualStyleBackColor = true;
            this.animationsHardCorruptionCheckBox.Click += new System.EventHandler(this.animationsHardCorruptionCheckBox_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 110);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(105, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Randomize/Restore:";
            // 
            // restoreButton
            // 
            this.restoreButton.Enabled = false;
            this.restoreButton.Location = new System.Drawing.Point(513, 195);
            this.restoreButton.Name = "restoreButton";
            this.restoreButton.Size = new System.Drawing.Size(75, 23);
            this.restoreButton.TabIndex = 17;
            this.restoreButton.Text = "Restore";
            this.toolTip1.SetToolTip(this.restoreButton, "You can only use this if there is a backup generated already");
            this.restoreButton.UseVisualStyleBackColor = true;
            this.restoreButton.Click += new System.EventHandler(this.restoreButton_Click);
            // 
            // useBackupFilesRadioButton
            // 
            this.useBackupFilesRadioButton.AutoSize = true;
            this.useBackupFilesRadioButton.Enabled = false;
            this.useBackupFilesRadioButton.Location = new System.Drawing.Point(187, 243);
            this.useBackupFilesRadioButton.Name = "useBackupFilesRadioButton";
            this.useBackupFilesRadioButton.Size = new System.Drawing.Size(163, 17);
            this.useBackupFilesRadioButton.TabIndex = 16;
            this.useBackupFilesRadioButton.Text = "Use original files from backup";
            this.toolTip1.SetToolTip(this.useBackupFilesRadioButton, "You can only use this if there is a backup generated already");
            this.useBackupFilesRadioButton.UseVisualStyleBackColor = true;
            // 
            // dontUseFilesFromBackupRadioButton
            // 
            this.dontUseFilesFromBackupRadioButton.AutoSize = true;
            this.dontUseFilesFromBackupRadioButton.Checked = true;
            this.dontUseFilesFromBackupRadioButton.Location = new System.Drawing.Point(13, 243);
            this.dontUseFilesFromBackupRadioButton.Name = "dontUseFilesFromBackupRadioButton";
            this.dontUseFilesFromBackupRadioButton.Size = new System.Drawing.Size(168, 17);
            this.dontUseFilesFromBackupRadioButton.TabIndex = 15;
            this.dontUseFilesFromBackupRadioButton.TabStop = true;
            this.dontUseFilesFromBackupRadioButton.Text = "Use files that\'s already in there";
            this.toolTip1.SetToolTip(this.dontUseFilesFromBackupRadioButton, "This is useful if you already have custom files mixed in the game files, and it\'s" +
        " way faster");
            this.dontUseFilesFromBackupRadioButton.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 226);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Randomizing method: ";
            // 
            // lettersTextureIntactCheckBox
            // 
            this.lettersTextureIntactCheckBox.AutoSize = true;
            this.lettersTextureIntactCheckBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.lettersTextureIntactCheckBox.Enabled = false;
            this.lettersTextureIntactCheckBox.Location = new System.Drawing.Point(83, 153);
            this.lettersTextureIntactCheckBox.Name = "lettersTextureIntactCheckBox";
            this.lettersTextureIntactCheckBox.Size = new System.Drawing.Size(151, 17);
            this.lettersTextureIntactCheckBox.TabIndex = 13;
            this.lettersTextureIntactCheckBox.Text = "Leave letters texture intact";
            this.lettersTextureIntactCheckBox.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(175, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Here you can randomize stock files.";
            // 
            // warningLabel
            // 
            this.warningLabel.AutoSize = true;
            this.warningLabel.ForeColor = System.Drawing.Color.Red;
            this.warningLabel.Location = new System.Drawing.Point(7, 77);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(0, 13);
            this.warningLabel.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Path to the game folder";
            // 
            // pathTextbox
            // 
            this.pathTextbox.Location = new System.Drawing.Point(6, 54);
            this.pathTextbox.Name = "pathTextbox";
            this.pathTextbox.ReadOnly = true;
            this.pathTextbox.Size = new System.Drawing.Size(497, 20);
            this.pathTextbox.TabIndex = 3;
            this.pathTextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pathTextbox_KeyPress);
            // 
            // randomizeLogTextBox
            // 
            this.randomizeLogTextBox.Location = new System.Drawing.Point(6, 272);
            this.randomizeLogTextBox.Multiline = true;
            this.randomizeLogTextBox.Name = "randomizeLogTextBox";
            this.randomizeLogTextBox.ReadOnly = true;
            this.randomizeLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.randomizeLogTextBox.Size = new System.Drawing.Size(580, 141);
            this.randomizeLogTextBox.TabIndex = 7;
            // 
            // animationsCheckBox
            // 
            this.animationsCheckBox.AutoSize = true;
            this.animationsCheckBox.Location = new System.Drawing.Point(10, 201);
            this.animationsCheckBox.Name = "animationsCheckBox";
            this.animationsCheckBox.Size = new System.Drawing.Size(77, 17);
            this.animationsCheckBox.TabIndex = 9;
            this.animationsCheckBox.Text = "Animations";
            this.animationsCheckBox.UseVisualStyleBackColor = true;
            this.animationsCheckBox.Click += new System.EventHandler(this.animationsCheckBox_Click);
            // 
            // randomizeButton
            // 
            this.randomizeButton.Location = new System.Drawing.Point(513, 237);
            this.randomizeButton.Name = "randomizeButton";
            this.randomizeButton.Size = new System.Drawing.Size(75, 23);
            this.randomizeButton.TabIndex = 0;
            this.randomizeButton.Text = "Randomize";
            this.randomizeButton.UseVisualStyleBackColor = true;
            this.randomizeButton.Click += new System.EventHandler(this.randomizeButton_Click);
            // 
            // cursorsCheckBox
            // 
            this.cursorsCheckBox.AutoSize = true;
            this.cursorsCheckBox.Location = new System.Drawing.Point(10, 177);
            this.cursorsCheckBox.Name = "cursorsCheckBox";
            this.cursorsCheckBox.Size = new System.Drawing.Size(61, 17);
            this.cursorsCheckBox.TabIndex = 8;
            this.cursorsCheckBox.Text = "Cursors";
            this.cursorsCheckBox.UseVisualStyleBackColor = true;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(509, 54);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 4;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // soundsCheckBox
            // 
            this.soundsCheckBox.AutoSize = true;
            this.soundsCheckBox.Location = new System.Drawing.Point(10, 129);
            this.soundsCheckBox.Name = "soundsCheckBox";
            this.soundsCheckBox.Size = new System.Drawing.Size(62, 17);
            this.soundsCheckBox.TabIndex = 5;
            this.soundsCheckBox.Text = "Sounds";
            this.soundsCheckBox.UseVisualStyleBackColor = true;
            // 
            // texturesCheckBox
            // 
            this.texturesCheckBox.AutoSize = true;
            this.texturesCheckBox.Location = new System.Drawing.Point(10, 153);
            this.texturesCheckBox.Name = "texturesCheckBox";
            this.texturesCheckBox.Size = new System.Drawing.Size(67, 17);
            this.texturesCheckBox.TabIndex = 6;
            this.texturesCheckBox.Text = "Textures";
            this.texturesCheckBox.UseVisualStyleBackColor = true;
            this.texturesCheckBox.CheckedChanged += new System.EventHandler(this.texturesTextBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 458);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(11, 14);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(600, 445);
            this.tabControl1.TabIndex = 10;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.linkLabel1);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.javaDebugCheckBox);
            this.tabPage3.Controls.Add(this.launchButton);
            this.tabPage3.Controls.Add(this.decompressedCheckBox);
            this.tabPage3.Controls.Add(this.label13);
            this.tabPage3.Controls.Add(this.decompressButton);
            this.tabPage3.Controls.Add(this.label12);
            this.tabPage3.Controls.Add(this.AICheckBox);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.dramaManagerCheckBox);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(592, 419);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Advanced";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(6, 389);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(175, 13);
            this.linkLabel1.TabIndex = 12;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "How to decompile Java class files...";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(6, 278);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(413, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "When this setting is enabled, you can only launch the game with the \"Launch\" butt" +
    "on!";
            // 
            // javaDebugCheckBox
            // 
            this.javaDebugCheckBox.AutoSize = true;
            this.javaDebugCheckBox.Location = new System.Drawing.Point(6, 258);
            this.javaDebugCheckBox.Name = "javaDebugCheckBox";
            this.javaDebugCheckBox.Size = new System.Drawing.Size(345, 17);
            this.javaDebugCheckBox.TabIndex = 10;
            this.javaDebugCheckBox.Text = "Enable Java backend debug logging in a console window on the fly";
            this.javaDebugCheckBox.UseVisualStyleBackColor = true;
            this.javaDebugCheckBox.Click += new System.EventHandler(this.javaDebugCheckBox_Click);
            // 
            // launchButton
            // 
            this.launchButton.Location = new System.Drawing.Point(469, 271);
            this.launchButton.Name = "launchButton";
            this.launchButton.Size = new System.Drawing.Size(75, 23);
            this.launchButton.TabIndex = 8;
            this.launchButton.Text = "Launch";
            this.launchButton.UseVisualStyleBackColor = true;
            this.launchButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // decompressedCheckBox
            // 
            this.decompressedCheckBox.AutoSize = true;
            this.decompressedCheckBox.Location = new System.Drawing.Point(7, 195);
            this.decompressedCheckBox.Name = "decompressedCheckBox";
            this.decompressedCheckBox.Size = new System.Drawing.Size(450, 17);
            this.decompressedCheckBox.TabIndex = 7;
            this.decompressedCheckBox.Text = "let the game use the decompiled files instead, so you can still edit the code to " +
    "play around.";
            this.decompressedCheckBox.UseVisualStyleBackColor = true;
            this.decompressedCheckBox.Click += new System.EventHandler(this.decompressedCheckBox_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(4, 166);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(491, 26);
            this.label13.TabIndex = 6;
            this.label13.Text = resources.GetString("label13.Text");
            this.toolTip1.SetToolTip(this.label13, "More like decompress");
            // 
            // decompressButton
            // 
            this.decompressButton.Location = new System.Drawing.Point(3, 218);
            this.decompressButton.Name = "decompressButton";
            this.decompressButton.Size = new System.Drawing.Size(75, 23);
            this.decompressButton.TabIndex = 5;
            this.decompressButton.Text = "Disassemble";
            this.decompressButton.UseVisualStyleBackColor = true;
            this.decompressButton.Click += new System.EventHandler(this.decompressButton_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 119);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(342, 26);
            this.label12.TabIndex = 4;
            this.label12.Text = "Generates a detailed debug log text file at the end of a gaming session.\r\nThe log" +
    " file will be generated at Facade\\util\\sources\\facade\\eventlogs";
            // 
            // AICheckBox
            // 
            this.AICheckBox.AutoSize = true;
            this.AICheckBox.Location = new System.Drawing.Point(6, 99);
            this.AICheckBox.Name = "AICheckBox";
            this.AICheckBox.Size = new System.Drawing.Size(103, 17);
            this.AICheckBox.TabIndex = 3;
            this.AICheckBox.Text = "Enable \"AI Log\"";
            this.AICheckBox.UseVisualStyleBackColor = true;
            this.AICheckBox.Click += new System.EventHandler(this.AICheckBox_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 58);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(540, 26);
            this.label11.TabIndex = 2;
            this.label11.Text = resources.GetString("label11.Text");
            // 
            // dramaManagerCheckBox
            // 
            this.dramaManagerCheckBox.AutoSize = true;
            this.dramaManagerCheckBox.Location = new System.Drawing.Point(7, 38);
            this.dramaManagerCheckBox.Name = "dramaManagerCheckBox";
            this.dramaManagerCheckBox.Size = new System.Drawing.Size(186, 17);
            this.dramaManagerCheckBox.TabIndex = 1;
            this.dramaManagerCheckBox.Text = "Enable \"Drama Manager Monitor\"";
            this.dramaManagerCheckBox.UseVisualStyleBackColor = true;
            this.dramaManagerCheckBox.Click += new System.EventHandler(this.dramaManagerCheckBox_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 7);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(246, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Intended for those who know a bit of programming.";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.linkLabel2);
            this.tabPage4.Controls.Add(this.creditLabel);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(592, 419);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "About";
            this.tabPage4.UseVisualStyleBackColor = true;
            this.tabPage4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabPage4_Click);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(6, 12);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(84, 13);
            this.linkLabel2.TabIndex = 2;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "Made by G4B33";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // creditLabel
            // 
            this.creditLabel.AutoSize = true;
            this.creditLabel.Location = new System.Drawing.Point(6, 40);
            this.creditLabel.Name = "creditLabel";
            this.creditLabel.Size = new System.Drawing.Size(201, 13);
            this.creditLabel.TabIndex = 1;
            this.creditLabel.Text = "Special thanks to the Façade developers";
            // 
            // superSecretPictureBox
            // 
            this.superSecretPictureBox.Location = new System.Drawing.Point(0, 0);
            this.superSecretPictureBox.Name = "superSecretPictureBox";
            this.superSecretPictureBox.Size = new System.Drawing.Size(100, 50);
            this.superSecretPictureBox.TabIndex = 0;
            this.superSecretPictureBox.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(623, 478);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Façade corruptor";
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.superSecretPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox globalCheckBox;
        private System.Windows.Forms.CheckBox tripCheckBox;
        private System.Windows.Forms.CheckBox graceCheckBox;
        private System.Windows.Forms.TextBox chanceTextBox;
        private System.Windows.Forms.TextBox replaceLogTextBox;
        private System.Windows.Forms.CheckBox chanceCheckBox;
        private System.Windows.Forms.Button clearListButton;
        private System.Windows.Forms.Label soundListLabel;
        private System.Windows.Forms.Button replaceBrowseButton;
        private System.Windows.Forms.Button replaceButton;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RadioButton useBackupFilesRadioButton;
        private System.Windows.Forms.RadioButton dontUseFilesFromBackupRadioButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox lettersTextureIntactCheckBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pathTextbox;
        private System.Windows.Forms.TextBox randomizeLogTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox animationsCheckBox;
        private System.Windows.Forms.Button randomizeButton;
        private System.Windows.Forms.CheckBox cursorsCheckBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.CheckBox soundsCheckBox;
        private System.Windows.Forms.CheckBox texturesCheckBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button restoreButton;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox dramaManagerCheckBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox AICheckBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button decompressButton;
        private System.Windows.Forms.CheckBox decompressedCheckBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button launchButton;
        private System.Windows.Forms.CheckBox javaDebugCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.PictureBox superSecretPictureBox;
        private System.Windows.Forms.Label creditLabel;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.CheckBox animationsHardCorruptionCheckBox;
    }
}

