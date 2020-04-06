namespace Colocalization
{
    partial class MainForm
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
            this.panel_Settings = new System.Windows.Forms.Panel();
            this.groupBox_Results = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox_Settings = new System.Windows.Forms.GroupBox();
            this.textBox_ThreshCh2 = new System.Windows.Forms.TextBox();
            this.textBox_ThreshCh1 = new System.Windows.Forms.TextBox();
            this.textBox_TCh2 = new System.Windows.Forms.TextBox();
            this.textBox_TCh1 = new System.Windows.Forms.TextBox();
            this.trackBar_ThreshCh2 = new System.Windows.Forms.TrackBar();
            this.trackBar_ThreshCh1 = new System.Windows.Forms.TrackBar();
            this.trackBar_TimeCh2 = new System.Windows.Forms.TrackBar();
            this.trackBar_TimeCh1 = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox_Ch2 = new System.Windows.Forms.ComboBox();
            this.comboBox_Ch1 = new System.Windows.Forms.ComboBox();
            this.comboBox_Zoom = new System.Windows.Forms.ComboBox();
            this.checkBox_UseROIs = new System.Windows.Forms.CheckBox();
            this.checkBox_UseBinary = new System.Windows.Forms.CheckBox();
            this.checkBox_LockFrames = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel_Exporters = new System.Windows.Forms.Panel();
            this.button_SaveResults = new System.Windows.Forms.Button();
            this.panel_GLControl = new System.Windows.Forms.FlowLayoutPanel();
            this.panel_Settings.SuspendLayout();
            this.groupBox_Results.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox_Settings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ThreshCh2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ThreshCh1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_TimeCh2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_TimeCh1)).BeginInit();
            this.panel_Exporters.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_Settings
            // 
            this.panel_Settings.Controls.Add(this.groupBox_Results);
            this.panel_Settings.Controls.Add(this.groupBox_Settings);
            this.panel_Settings.Controls.Add(this.panel_Exporters);
            this.panel_Settings.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel_Settings.Location = new System.Drawing.Point(402, 0);
            this.panel_Settings.Name = "panel_Settings";
            this.panel_Settings.Size = new System.Drawing.Size(398, 543);
            this.panel_Settings.TabIndex = 0;
            // 
            // groupBox_Results
            // 
            this.groupBox_Results.Controls.Add(this.dataGridView1);
            this.groupBox_Results.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_Results.Location = new System.Drawing.Point(0, 347);
            this.groupBox_Results.Name = "groupBox_Results";
            this.groupBox_Results.Size = new System.Drawing.Size(398, 196);
            this.groupBox_Results.TabIndex = 2;
            this.groupBox_Results.TabStop = false;
            this.groupBox_Results.Text = "Results";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 16);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(392, 177);
            this.dataGridView1.TabIndex = 0;
            // 
            // groupBox_Settings
            // 
            this.groupBox_Settings.Controls.Add(this.textBox_ThreshCh2);
            this.groupBox_Settings.Controls.Add(this.textBox_ThreshCh1);
            this.groupBox_Settings.Controls.Add(this.textBox_TCh2);
            this.groupBox_Settings.Controls.Add(this.textBox_TCh1);
            this.groupBox_Settings.Controls.Add(this.trackBar_ThreshCh2);
            this.groupBox_Settings.Controls.Add(this.trackBar_ThreshCh1);
            this.groupBox_Settings.Controls.Add(this.trackBar_TimeCh2);
            this.groupBox_Settings.Controls.Add(this.trackBar_TimeCh1);
            this.groupBox_Settings.Controls.Add(this.label6);
            this.groupBox_Settings.Controls.Add(this.label7);
            this.groupBox_Settings.Controls.Add(this.comboBox_Ch2);
            this.groupBox_Settings.Controls.Add(this.comboBox_Ch1);
            this.groupBox_Settings.Controls.Add(this.comboBox_Zoom);
            this.groupBox_Settings.Controls.Add(this.checkBox_UseROIs);
            this.groupBox_Settings.Controls.Add(this.checkBox_UseBinary);
            this.groupBox_Settings.Controls.Add(this.checkBox_LockFrames);
            this.groupBox_Settings.Controls.Add(this.label5);
            this.groupBox_Settings.Controls.Add(this.label4);
            this.groupBox_Settings.Controls.Add(this.label3);
            this.groupBox_Settings.Controls.Add(this.label2);
            this.groupBox_Settings.Controls.Add(this.label1);
            this.groupBox_Settings.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox_Settings.Location = new System.Drawing.Point(0, 44);
            this.groupBox_Settings.Name = "groupBox_Settings";
            this.groupBox_Settings.Size = new System.Drawing.Size(398, 303);
            this.groupBox_Settings.TabIndex = 1;
            this.groupBox_Settings.TabStop = false;
            this.groupBox_Settings.Text = "Settings";
            // 
            // textBox_ThreshCh2
            // 
            this.textBox_ThreshCh2.Enabled = false;
            this.textBox_ThreshCh2.Location = new System.Drawing.Point(334, 230);
            this.textBox_ThreshCh2.Name = "textBox_ThreshCh2";
            this.textBox_ThreshCh2.Size = new System.Drawing.Size(51, 20);
            this.textBox_ThreshCh2.TabIndex = 20;
            // 
            // textBox_ThreshCh1
            // 
            this.textBox_ThreshCh1.Enabled = false;
            this.textBox_ThreshCh1.Location = new System.Drawing.Point(334, 209);
            this.textBox_ThreshCh1.Name = "textBox_ThreshCh1";
            this.textBox_ThreshCh1.Size = new System.Drawing.Size(51, 20);
            this.textBox_ThreshCh1.TabIndex = 19;
            // 
            // textBox_TCh2
            // 
            this.textBox_TCh2.Location = new System.Drawing.Point(334, 151);
            this.textBox_TCh2.Name = "textBox_TCh2";
            this.textBox_TCh2.Size = new System.Drawing.Size(51, 20);
            this.textBox_TCh2.TabIndex = 18;
            // 
            // textBox_TCh1
            // 
            this.textBox_TCh1.Location = new System.Drawing.Point(334, 130);
            this.textBox_TCh1.Name = "textBox_TCh1";
            this.textBox_TCh1.Size = new System.Drawing.Size(51, 20);
            this.textBox_TCh1.TabIndex = 17;
            // 
            // trackBar_ThreshCh2
            // 
            this.trackBar_ThreshCh2.AutoSize = false;
            this.trackBar_ThreshCh2.Enabled = false;
            this.trackBar_ThreshCh2.Location = new System.Drawing.Point(107, 234);
            this.trackBar_ThreshCh2.Name = "trackBar_ThreshCh2";
            this.trackBar_ThreshCh2.Size = new System.Drawing.Size(231, 22);
            this.trackBar_ThreshCh2.TabIndex = 16;
            this.trackBar_ThreshCh2.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // trackBar_ThreshCh1
            // 
            this.trackBar_ThreshCh1.AutoSize = false;
            this.trackBar_ThreshCh1.Enabled = false;
            this.trackBar_ThreshCh1.Location = new System.Drawing.Point(107, 209);
            this.trackBar_ThreshCh1.Name = "trackBar_ThreshCh1";
            this.trackBar_ThreshCh1.Size = new System.Drawing.Size(231, 22);
            this.trackBar_ThreshCh1.TabIndex = 15;
            this.trackBar_ThreshCh1.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // trackBar_TimeCh2
            // 
            this.trackBar_TimeCh2.AutoSize = false;
            this.trackBar_TimeCh2.Location = new System.Drawing.Point(107, 151);
            this.trackBar_TimeCh2.Name = "trackBar_TimeCh2";
            this.trackBar_TimeCh2.Size = new System.Drawing.Size(231, 20);
            this.trackBar_TimeCh2.TabIndex = 14;
            this.trackBar_TimeCh2.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // trackBar_TimeCh1
            // 
            this.trackBar_TimeCh1.AutoSize = false;
            this.trackBar_TimeCh1.Location = new System.Drawing.Point(107, 129);
            this.trackBar_TimeCh1.Name = "trackBar_TimeCh1";
            this.trackBar_TimeCh1.Size = new System.Drawing.Size(231, 20);
            this.trackBar_TimeCh1.TabIndex = 13;
            this.trackBar_TimeCh1.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Enabled = false;
            this.label6.Location = new System.Drawing.Point(6, 236);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Threshold Ch2:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Enabled = false;
            this.label7.Location = new System.Drawing.Point(6, 213);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Threshold Ch1:";
            // 
            // comboBox_Ch2
            // 
            this.comboBox_Ch2.FormattingEnabled = true;
            this.comboBox_Ch2.Location = new System.Drawing.Point(114, 72);
            this.comboBox_Ch2.Name = "comboBox_Ch2";
            this.comboBox_Ch2.Size = new System.Drawing.Size(121, 21);
            this.comboBox_Ch2.TabIndex = 10;
            this.comboBox_Ch2.SelectedIndexChanged += new System.EventHandler(this.comboBox_Ch2_SelectedIndexChanged);
            // 
            // comboBox_Ch1
            // 
            this.comboBox_Ch1.FormattingEnabled = true;
            this.comboBox_Ch1.Location = new System.Drawing.Point(114, 48);
            this.comboBox_Ch1.Name = "comboBox_Ch1";
            this.comboBox_Ch1.Size = new System.Drawing.Size(121, 21);
            this.comboBox_Ch1.TabIndex = 9;
            this.comboBox_Ch1.SelectedIndexChanged += new System.EventHandler(this.comboBox_Ch1_SelectedIndexChanged);
            // 
            // comboBox_Zoom
            // 
            this.comboBox_Zoom.FormattingEnabled = true;
            this.comboBox_Zoom.Items.AddRange(new object[] {
            "6.25 %",
            "12.5 %",
            "25 %",
            "50 %",
            "100 %",
            "200 %",
            "300 %",
            "400 %",
            "500 %",
            "600 %",
            "700 %",
            "800 %",
            "900 %",
            "1000 %"});
            this.comboBox_Zoom.Location = new System.Drawing.Point(114, 23);
            this.comboBox_Zoom.Name = "comboBox_Zoom";
            this.comboBox_Zoom.Size = new System.Drawing.Size(121, 21);
            this.comboBox_Zoom.TabIndex = 8;
            this.comboBox_Zoom.Text = "100 %";
            this.comboBox_Zoom.SelectedIndexChanged += new System.EventHandler(this.comboBox_Zoom_SelectedIndexChanged);
            // 
            // checkBox_UseROIs
            // 
            this.checkBox_UseROIs.AutoSize = true;
            this.checkBox_UseROIs.Checked = true;
            this.checkBox_UseROIs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_UseROIs.Location = new System.Drawing.Point(9, 270);
            this.checkBox_UseROIs.Name = "checkBox_UseROIs";
            this.checkBox_UseROIs.Size = new System.Drawing.Size(72, 17);
            this.checkBox_UseROIs.TabIndex = 7;
            this.checkBox_UseROIs.Text = "Use ROIs";
            this.checkBox_UseROIs.UseVisualStyleBackColor = true;
            this.checkBox_UseROIs.CheckedChanged += new System.EventHandler(this.checkBox_UseROIs_CheckedChanged);
            // 
            // checkBox_UseBinary
            // 
            this.checkBox_UseBinary.AutoSize = true;
            this.checkBox_UseBinary.Location = new System.Drawing.Point(9, 182);
            this.checkBox_UseBinary.Name = "checkBox_UseBinary";
            this.checkBox_UseBinary.Size = new System.Drawing.Size(144, 17);
            this.checkBox_UseBinary.TabIndex = 6;
            this.checkBox_UseBinary.Text = "Apply intensity thresholds";
            this.checkBox_UseBinary.UseVisualStyleBackColor = true;
            this.checkBox_UseBinary.CheckedChanged += new System.EventHandler(this.checkBox_UseBinary_CheckedChanged);
            // 
            // checkBox_LockFrames
            // 
            this.checkBox_LockFrames.AutoSize = true;
            this.checkBox_LockFrames.Checked = true;
            this.checkBox_LockFrames.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_LockFrames.Location = new System.Drawing.Point(9, 107);
            this.checkBox_LockFrames.Name = "checkBox_LockFrames";
            this.checkBox_LockFrames.Size = new System.Drawing.Size(99, 17);
            this.checkBox_LockFrames.TabIndex = 5;
            this.checkBox_LockFrames.Text = "Lock T position";
            this.checkBox_LockFrames.UseVisualStyleBackColor = true;
            this.checkBox_LockFrames.CheckedChanged += new System.EventHandler(this.checkBox_LockFrames_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Time frame Ch2:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Time frame Ch1:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Channel 2:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Channel 1:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Zoom:";
            // 
            // panel_Exporters
            // 
            this.panel_Exporters.Controls.Add(this.button_SaveResults);
            this.panel_Exporters.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Exporters.Location = new System.Drawing.Point(0, 0);
            this.panel_Exporters.Name = "panel_Exporters";
            this.panel_Exporters.Size = new System.Drawing.Size(398, 44);
            this.panel_Exporters.TabIndex = 0;
            // 
            // button_SaveResults
            // 
            this.button_SaveResults.Location = new System.Drawing.Point(6, 10);
            this.button_SaveResults.Name = "button_SaveResults";
            this.button_SaveResults.Size = new System.Drawing.Size(75, 23);
            this.button_SaveResults.TabIndex = 0;
            this.button_SaveResults.Text = "Save results";
            this.button_SaveResults.UseVisualStyleBackColor = true;
            this.button_SaveResults.Click += new System.EventHandler(this.button_SaveResults_Click);
            // 
            // panel_GLControl
            // 
            this.panel_GLControl.BackColor = System.Drawing.Color.DimGray;
            this.panel_GLControl.Location = new System.Drawing.Point(0, 0);
            this.panel_GLControl.Name = "panel_GLControl";
            this.panel_GLControl.Size = new System.Drawing.Size(402, 543);
            this.panel_GLControl.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 543);
            this.Controls.Add(this.panel_GLControl);
            this.Controls.Add(this.panel_Settings);
            this.MinimumSize = new System.Drawing.Size(816, 582);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Colocalization";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel_Settings.ResumeLayout(false);
            this.groupBox_Results.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox_Settings.ResumeLayout(false);
            this.groupBox_Settings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ThreshCh2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ThreshCh1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_TimeCh2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_TimeCh1)).EndInit();
            this.panel_Exporters.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_Settings;
        private System.Windows.Forms.FlowLayoutPanel panel_GLControl;
        private System.Windows.Forms.Panel panel_Exporters;
        private System.Windows.Forms.GroupBox groupBox_Results;
        private System.Windows.Forms.GroupBox groupBox_Settings;
        private System.Windows.Forms.Button button_SaveResults;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_LockFrames;
        private System.Windows.Forms.CheckBox checkBox_UseBinary;
        private System.Windows.Forms.CheckBox checkBox_UseROIs;
        private System.Windows.Forms.ComboBox comboBox_Ch2;
        private System.Windows.Forms.ComboBox comboBox_Ch1;
        private System.Windows.Forms.ComboBox comboBox_Zoom;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar trackBar_ThreshCh2;
        private System.Windows.Forms.TrackBar trackBar_ThreshCh1;
        private System.Windows.Forms.TrackBar trackBar_TimeCh2;
        private System.Windows.Forms.TrackBar trackBar_TimeCh1;
        private System.Windows.Forms.TextBox textBox_ThreshCh2;
        private System.Windows.Forms.TextBox textBox_ThreshCh1;
        private System.Windows.Forms.TextBox textBox_TCh2;
        private System.Windows.Forms.TextBox textBox_TCh1;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}