﻿namespace BuggyExchange
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxRemoveTrees = new System.Windows.Forms.CheckBox();
            this.checkBoxAddTrees = new System.Windows.Forms.CheckBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.labelTrackType = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.checkBoxRemoveTrack = new System.Windows.Forms.CheckBox();
            this.checkBoxAddTrack = new System.Windows.Forms.CheckBox();
            this.labelTrackGrade = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.labelSplineIndex = new System.Windows.Forms.Label();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkedListBox2 = new System.Windows.Forms.ListBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.panelTracks = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.panelTracks.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(10, 106);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(531, 760);
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(547, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(850, 850);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // label1
            // 
            this.label1.AllowDrop = true;
            this.label1.AutoSize = true;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(10, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Open Buggy File";
            this.toolTip1.SetToolTip(this.label1, "This file contains buggies you want to add into the map file.\r\nNote they will be " +
        "added keeping their position, cargo, etc.\r\nSo if you try to add duplicates they " +
        "are likely to yeet.");
            this.label1.Click += new System.EventHandler(this.label1_Click);
            this.label1.DragDrop += new System.Windows.Forms.DragEventHandler(this.label1_DragDrop);
            this.label1.DragEnter += new System.Windows.Forms.DragEventHandler(this.label1_DragEnter);
            // 
            // label2
            // 
            this.label2.AllowDrop = true;
            this.label2.AutoSize = true;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(10, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Choose Map File";
            this.toolTip1.SetToolTip(this.label2, "This file contains all of the tracks,\r\nand existing buggies, and trees.");
            this.label2.Click += new System.EventHandler(this.label2_Click);
            this.label2.DragDrop += new System.Windows.Forms.DragEventHandler(this.label2_DragDrop);
            this.label2.DragEnter += new System.Windows.Forms.DragEventHandler(this.label2_DragEnter);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(433, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Add Buggies";
            this.toolTip1.SetToolTip(this.button1, "Creates a new save file by adding all of the selected buggies into the Map file.");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(433, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Remove Buggies";
            this.toolTip1.SetToolTip(this.button2, "Creates a new save file based on the Buggy File but with all of the selected bugg" +
        "ies removed.");
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(363, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(49, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Help";
            this.toolTip1.SetToolTip(this.button3, "Shortcuts");
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label3
            // 
            this.label3.AllowDrop = true;
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(240, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Output to";
            this.toolTip1.SetToolTip(this.label3, "This file contains all of the tracks and existing cars.");
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(363, 45);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(49, 23);
            this.button4.TabIndex = 11;
            this.button4.Text = "Saves";
            this.toolTip1.SetToolTip(this.button4, "Attempts to open the savegame folder in explorer.");
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label4
            // 
            this.label4.AllowDrop = true;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 15);
            this.label4.TabIndex = 16;
            this.label4.Text = "0 found";
            this.toolTip1.SetToolTip(this.label4, "This file contains buggies you want to add into the map file. Note they will be a" +
        "dded keeping their position, cargo, etc. \\nSo if you try to add duplicates they " +
        "are likely to yeet.");
            // 
            // label5
            // 
            this.label5.AllowDrop = true;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 15);
            this.label5.TabIndex = 17;
            this.label5.Text = "0 removed";
            this.toolTip1.SetToolTip(this.label5, "This file contains buggies you want to add into the map file. Note they will be a" +
        "dded keeping their position, cargo, etc. \\nSo if you try to add duplicates they " +
        "are likely to yeet.");
            // 
            // label6
            // 
            this.label6.AllowDrop = true;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 15);
            this.label6.TabIndex = 18;
            this.label6.Text = "Zoom = 1.0";
            this.toolTip1.SetToolTip(this.label6, "This file contains buggies you want to add into the map file. Note they will be a" +
        "dded keeping their position, cargo, etc. \\nSo if you try to add duplicates they " +
        "are likely to yeet.");
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Segoe UI", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button7.Location = new System.Drawing.Point(3, 207);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(59, 23);
            this.button7.TabIndex = 24;
            this.button7.Text = "Plant All";
            this.toolTip1.SetToolTip(this.button7, "Replants all trees on the map, including around the industries.");
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(12, 144);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(87, 23);
            this.button8.TabIndex = 23;
            this.button8.Text = "Auto-Tree";
            this.toolTip1.SetToolTip(this.button8, "Trims trees away from tracks using the given radius. This does not currently clea" +
        "n up the industries.");
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // label7
            // 
            this.label7.AllowDrop = true;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 15);
            this.label7.TabIndex = 25;
            this.label7.Text = "Tree Stuff";
            this.toolTip1.SetToolTip(this.label7, "This file contains buggies you want to add into the map file. Note they will be a" +
        "dded keeping their position, cargo, etc. \\nSo if you try to add duplicates they " +
        "are likely to yeet.");
            // 
            // label8
            // 
            this.label8.AllowDrop = true;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 124);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 15);
            this.label8.TabIndex = 26;
            this.label8.Text = "Radius 5m";
            this.toolTip1.SetToolTip(this.label8, "This file contains buggies you want to add into the map file. Note they will be a" +
        "dded keeping their position, cargo, etc. \\nSo if you try to add duplicates they " +
        "are likely to yeet.");
            // 
            // checkBoxRemoveTrees
            // 
            this.checkBoxRemoveTrees.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxRemoveTrees.AutoSize = true;
            this.checkBoxRemoveTrees.Location = new System.Drawing.Point(12, 63);
            this.checkBoxRemoveTrees.Name = "checkBoxRemoveTrees";
            this.checkBoxRemoveTrees.Size = new System.Drawing.Size(89, 25);
            this.checkBoxRemoveTrees.TabIndex = 22;
            this.checkBoxRemoveTrees.Text = "Remove Trees";
            this.toolTip1.SetToolTip(this.checkBoxRemoveTrees, "Use the mouse to remove trees within the selected radius.");
            this.checkBoxRemoveTrees.UseVisualStyleBackColor = true;
            this.checkBoxRemoveTrees.CheckedChanged += new System.EventHandler(this.checkBoxRemoveTrees_CheckedChanged);
            // 
            // checkBoxAddTrees
            // 
            this.checkBoxAddTrees.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxAddTrees.AutoSize = true;
            this.checkBoxAddTrees.Location = new System.Drawing.Point(12, 32);
            this.checkBoxAddTrees.Name = "checkBoxAddTrees";
            this.checkBoxAddTrees.Size = new System.Drawing.Size(86, 25);
            this.checkBoxAddTrees.TabIndex = 21;
            this.checkBoxAddTrees.Text = "Add Trees      ";
            this.checkBoxAddTrees.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.checkBoxAddTrees, "Use the mouse to paint in trees within the selected radius.");
            this.checkBoxAddTrees.UseVisualStyleBackColor = true;
            this.checkBoxAddTrees.CheckedChanged += new System.EventHandler(this.checkBoxAddTrees_CheckedChanged);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1404, 563);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(129, 23);
            this.button5.TabIndex = 26;
            this.button5.Text = "Save Map File";
            this.toolTip1.SetToolTip(this.button5, "Writes the map, trees, and selected buggies to the selected save slot. (Same as \"" +
        "Add Buggies\" button.)");
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(228, 13);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(129, 23);
            this.button6.TabIndex = 27;
            this.button6.Text = "Edit Splines";
            this.toolTip1.SetToolTip(this.button6, "Writes the map, trees, and selected buggies to the selected save slot. (Same as \"" +
        "Add Buggies\" button.)");
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Visible = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // checkBox6
            // 
            this.checkBox6.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(1423, 532);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(92, 25);
            this.checkBox6.TabIndex = 28;
            this.checkBox6.Text = "Edit Splines     ";
            this.checkBox6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.checkBox6, "Use the mouse to paint in trees within the selected radius.");
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.checkBox6_CheckedChanged);
            // 
            // labelTrackType
            // 
            this.labelTrackType.AllowDrop = true;
            this.labelTrackType.AutoSize = true;
            this.labelTrackType.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelTrackType.Location = new System.Drawing.Point(3, 76);
            this.labelTrackType.Name = "labelTrackType";
            this.labelTrackType.Size = new System.Drawing.Size(68, 12);
            this.labelTrackType.TabIndex = 26;
            this.labelTrackType.Text = "None Selected";
            this.toolTip1.SetToolTip(this.labelTrackType, "Click to change the type of segment");
            // 
            // label10
            // 
            this.label10.AllowDrop = true;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 8);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 15);
            this.label10.TabIndex = 25;
            this.label10.Text = "Track Stuff";
            this.toolTip1.SetToolTip(this.label10, "This file contains buggies you want to add into the map file. Note they will be a" +
        "dded keeping their position, cargo, etc. \\nSo if you try to add duplicates they " +
        "are likely to yeet.");
            // 
            // checkBoxRemoveTrack
            // 
            this.checkBoxRemoveTrack.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxRemoveTrack.AutoSize = true;
            this.checkBoxRemoveTrack.Location = new System.Drawing.Point(57, 32);
            this.checkBoxRemoveTrack.Name = "checkBoxRemoveTrack";
            this.checkBoxRemoveTrack.Size = new System.Drawing.Size(60, 25);
            this.checkBoxRemoveTrack.TabIndex = 22;
            this.checkBoxRemoveTrack.Text = "Remove";
            this.toolTip1.SetToolTip(this.checkBoxRemoveTrack, "Use the mouse to remove the highlighted track.");
            this.checkBoxRemoveTrack.UseVisualStyleBackColor = true;
            // 
            // checkBoxAddTrack
            // 
            this.checkBoxAddTrack.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxAddTrack.AutoSize = true;
            this.checkBoxAddTrack.Location = new System.Drawing.Point(12, 32);
            this.checkBoxAddTrack.Name = "checkBoxAddTrack";
            this.checkBoxAddTrack.Size = new System.Drawing.Size(39, 25);
            this.checkBoxAddTrack.TabIndex = 21;
            this.checkBoxAddTrack.Text = "Add";
            this.checkBoxAddTrack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.checkBoxAddTrack, "Use the mouse to add a track point-by-point");
            this.checkBoxAddTrack.UseVisualStyleBackColor = true;
            // 
            // labelTrackGrade
            // 
            this.labelTrackGrade.AllowDrop = true;
            this.labelTrackGrade.AutoSize = true;
            this.labelTrackGrade.Location = new System.Drawing.Point(3, 91);
            this.labelTrackGrade.Name = "labelTrackGrade";
            this.labelTrackGrade.Size = new System.Drawing.Size(83, 15);
            this.labelTrackGrade.TabIndex = 27;
            this.labelTrackGrade.Text = "None Selected";
            this.toolTip1.SetToolTip(this.labelTrackGrade, "Highest and lowest grade on this segment");
            // 
            // label9
            // 
            this.label9.AllowDrop = true;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label9.Location = new System.Drawing.Point(3, 139);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 11);
            this.label9.TabIndex = 28;
            this.label9.Text = "None Selected";
            this.toolTip1.SetToolTip(this.label9, "Coordinates of starting point");
            // 
            // label11
            // 
            this.label11.AllowDrop = true;
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label11.Location = new System.Drawing.Point(3, 155);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 11);
            this.label11.TabIndex = 29;
            this.label11.Text = "None Selected";
            this.toolTip1.SetToolTip(this.label11, "Coordinates of first tangent");
            // 
            // label12
            // 
            this.label12.AllowDrop = true;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label12.Location = new System.Drawing.Point(3, 170);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 11);
            this.label12.TabIndex = 30;
            this.label12.Text = "None Selected";
            this.toolTip1.SetToolTip(this.label12, "Coordinates of end tangent");
            // 
            // label13
            // 
            this.label13.AllowDrop = true;
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label13.Location = new System.Drawing.Point(3, 185);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(57, 11);
            this.label13.TabIndex = 31;
            this.label13.Text = "None Selected";
            this.toolTip1.SetToolTip(this.label13, "Coordinates of end point");
            // 
            // labelSplineIndex
            // 
            this.labelSplineIndex.AllowDrop = true;
            this.labelSplineIndex.AutoSize = true;
            this.labelSplineIndex.Location = new System.Drawing.Point(3, 60);
            this.labelSplineIndex.Name = "labelSplineIndex";
            this.labelSplineIndex.Size = new System.Drawing.Size(83, 15);
            this.labelSplineIndex.TabIndex = 32;
            this.labelSplineIndex.Text = "None Selected";
            this.toolTip1.SetToolTip(this.labelSplineIndex, "Click to change the type of segment");
            // 
            // button9
            // 
            this.button9.Font = new System.Drawing.Font("Segoe UI", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button9.Location = new System.Drawing.Point(62, 207);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(64, 23);
            this.button9.TabIndex = 28;
            this.button9.Text = "Remove All";
            this.toolTip1.SetToolTip(this.button9, "Replants all trees on the map, including around the industries.");
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(12, 175);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(87, 23);
            this.button10.TabIndex = 29;
            this.button10.Text = "Thinning";
            this.toolTip1.SetToolTip(this.button10, "Trims trees away from tracks using the given radius. This does not currently clea" +
        "n up the industries.");
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "sav";
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(10, 74);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(531, 26);
            this.panel1.TabIndex = 8;
            // 
            // checkedListBox2
            // 
            this.checkedListBox2.FormattingEnabled = true;
            this.checkedListBox2.ItemHeight = 15;
            this.checkedListBox2.Location = new System.Drawing.Point(1547, 12);
            this.checkedListBox2.Name = "checkedListBox2";
            this.checkedListBox2.Size = new System.Drawing.Size(120, 574);
            this.checkedListBox2.TabIndex = 12;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(1403, 607);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(255, 255);
            this.pictureBox2.TabIndex = 15;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
            this.pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseMove);
            this.pictureBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseUp);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(3, 119);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(84, 19);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.Text = "Show Trees";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(3, 134);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(108, 19);
            this.checkBox2.TabIndex = 14;
            this.checkBox2.Text = "Show Removed";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Location = new System.Drawing.Point(0, 30);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(90, 19);
            this.checkBox3.TabIndex = 15;
            this.checkBox3.Text = "Show Tracks";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.Location = new System.Drawing.Point(3, 203);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(127, 19);
            this.checkBox4.TabIndex = 19;
            this.checkBox4.Text = "Show Map Buggies";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Checked = true;
            this.checkBox5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox5.Location = new System.Drawing.Point(3, 228);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(125, 19);
            this.checkBox5.TabIndex = 20;
            this.checkBox5.Text = "Show Add Buggies";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.checkBox5_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.checkBox8);
            this.panel2.Controls.Add(this.checkBox7);
            this.panel2.Controls.Add(this.checkBox5);
            this.panel2.Controls.Add(this.checkBox4);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.checkBox3);
            this.panel2.Controls.Add(this.checkBox2);
            this.panel2.Controls.Add(this.checkBox1);
            this.panel2.Location = new System.Drawing.Point(1403, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(131, 262);
            this.panel2.TabIndex = 14;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.Checked = true;
            this.checkBox8.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox8.Location = new System.Drawing.Point(0, 68);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(87, 19);
            this.checkBox8.TabIndex = 22;
            this.checkBox8.Text = "Show Types";
            this.checkBox8.UseVisualStyleBackColor = true;
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Checked = true;
            this.checkBox7.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox7.Location = new System.Drawing.Point(-1, 51);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(94, 19);
            this.checkBox7.TabIndex = 21;
            this.checkBox7.Text = "Show Grades";
            this.checkBox7.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.button10);
            this.panel3.Controls.Add(this.button9);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.trackBar1);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.button7);
            this.panel3.Controls.Add(this.button8);
            this.panel3.Controls.Add(this.checkBoxRemoveTrees);
            this.panel3.Controls.Add(this.checkBoxAddTrees);
            this.panel3.Location = new System.Drawing.Point(1403, 280);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(131, 235);
            this.panel3.TabIndex = 25;
            // 
            // trackBar1
            // 
            this.trackBar1.LargeChange = 10;
            this.trackBar1.Location = new System.Drawing.Point(12, 94);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Minimum = 5;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(104, 45);
            this.trackBar1.TabIndex = 27;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Value = 5;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // panelTracks
            // 
            this.panelTracks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTracks.Controls.Add(this.labelSplineIndex);
            this.panelTracks.Controls.Add(this.label13);
            this.panelTracks.Controls.Add(this.label12);
            this.panelTracks.Controls.Add(this.label11);
            this.panelTracks.Controls.Add(this.label9);
            this.panelTracks.Controls.Add(this.labelTrackGrade);
            this.panelTracks.Controls.Add(this.labelTrackType);
            this.panelTracks.Controls.Add(this.label10);
            this.panelTracks.Controls.Add(this.checkBoxRemoveTrack);
            this.panelTracks.Controls.Add(this.checkBoxAddTrack);
            this.panelTracks.Location = new System.Drawing.Point(1403, 280);
            this.panelTracks.Name = "panelTracks";
            this.panelTracks.Size = new System.Drawing.Size(131, 235);
            this.panelTracks.TabIndex = 29;
            this.panelTracks.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1679, 880);
            this.Controls.Add(this.panelTracks);
            this.Controls.Add(this.checkBox6);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.checkedListBox2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkedListBox1);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Buggy Exchanger";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.panelTracks.ResumeLayout(false);
            this.panelTracks.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckedListBox checkedListBox1;
        private PictureBox pictureBox1;
        private Label label1;
        private Label label2;
        private Button button1;
        private ToolTip toolTip1;
        private Button button2;
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;
        private Button button3;
        private Panel panel1;
        private Label label3;
        private Button button4;
        private ListBox checkedListBox2;
        private PictureBox pictureBox2;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private Label label4;
        private Label label5;
        private Label label6;
        private CheckBox checkBox4;
        private CheckBox checkBox5;
        private Panel panel2;
        private Panel panel3;
        private Label label7;
        private Button button7;
        private Button button8;
        private CheckBox checkBoxRemoveTrees;
        private CheckBox checkBoxAddTrees;
        private Label label8;
        private TrackBar trackBar1;
        private Button button5;
        private Button button6;
        private CheckBox checkBox6;
        private Panel panelTracks;
        private Label label13;
        private Label label12;
        private Label label11;
        private Label label9;
        private Label labelTrackGrade;
        private Label labelTrackType;
        private Label label10;
        private CheckBox checkBoxRemoveTrack;
        private CheckBox checkBoxAddTrack;
        private Label labelSplineIndex;
        private CheckBox checkBox7;
        private Button button9;
        private CheckBox checkBox8;
        private Button button10;
    }
}