namespace ClassScheduler
{
    partial class Demo
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxJsonFile = new System.Windows.Forms.TextBox();
            this.openJsonFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonStart = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.labelResultSummary = new System.Windows.Forms.Label();
            this.comboBoxResultBy = new System.Windows.Forms.ComboBox();
            this.textBoxResult = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "输入：";
            // 
            // textBoxJsonFile
            // 
            this.textBoxJsonFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxJsonFile.Location = new System.Drawing.Point(52, 13);
            this.textBoxJsonFile.Name = "textBoxJsonFile";
            this.textBoxJsonFile.Size = new System.Drawing.Size(408, 20);
            this.textBoxJsonFile.TabIndex = 1;
            // 
            // openJsonFileDialog
            // 
            this.openJsonFileDialog.FileName = "openFileDialog1";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(12, 50);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "开始排课";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(547, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "编辑";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenFile.Location = new System.Drawing.Point(466, 13);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(75, 23);
            this.buttonOpenFile.TabIndex = 8;
            this.buttonOpenFile.Text = "打开";
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // labelResultSummary
            // 
            this.labelResultSummary.AutoSize = true;
            this.labelResultSummary.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labelResultSummary.Location = new System.Drawing.Point(102, 55);
            this.labelResultSummary.Name = "labelResultSummary";
            this.labelResultSummary.Size = new System.Drawing.Size(87, 13);
            this.labelResultSummary.TabIndex = 11;
            this.labelResultSummary.Text = "点击启动运算。";
            // 
            // comboBoxResultBy
            // 
            this.comboBoxResultBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxResultBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxResultBy.FormattingEnabled = true;
            this.comboBoxResultBy.Items.AddRange(new object[] {
            "按班级",
            "按老师",
            "按教室"});
            this.comboBoxResultBy.Location = new System.Drawing.Point(466, 51);
            this.comboBoxResultBy.Name = "comboBoxResultBy";
            this.comboBoxResultBy.Size = new System.Drawing.Size(153, 21);
            this.comboBoxResultBy.TabIndex = 12;
            this.comboBoxResultBy.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // textBoxResult
            // 
            this.textBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResult.BackColor = System.Drawing.Color.White;
            this.textBoxResult.Location = new System.Drawing.Point(12, 78);
            this.textBoxResult.Multiline = true;
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.Size = new System.Drawing.Size(607, 380);
            this.textBoxResult.TabIndex = 13;
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 470);
            this.Controls.Add(this.textBoxResult);
            this.Controls.Add(this.comboBoxResultBy);
            this.Controls.Add(this.labelResultSummary);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonOpenFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxJsonFile);
            this.Controls.Add(this.buttonStart);
            this.Name = "Demo";
            this.Text = "Demo";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxJsonFile;
        private System.Windows.Forms.OpenFileDialog openJsonFileDialog;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonOpenFile;
        private System.Windows.Forms.Label labelResultSummary;
        private System.Windows.Forms.ComboBox comboBoxResultBy;
        private System.Windows.Forms.TextBox textBoxResult;
    }
}

