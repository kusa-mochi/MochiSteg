namespace StegBMP
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox_BMPMonitor = new System.Windows.Forms.PictureBox();
            this.listBox_Files = new System.Windows.Forms.ListBox();
            this.label_Information = new System.Windows.Forms.Label();
            this.button_Start = new System.Windows.Forms.Button();
            this.progressBar_DataVolume = new System.Windows.Forms.ProgressBar();
            this.label_DataVolume = new System.Windows.Forms.Label();
            this.label_BMPFileName = new System.Windows.Forms.Label();
            this.label_HidingLevel = new System.Windows.Forms.Label();
            this.comboBox_HidingLevel = new System.Windows.Forms.ComboBox();
            this.button_FileDelete = new System.Windows.Forms.Button();
            this.label_DropPictureHere = new System.Windows.Forms.Label();
            this.label_DropFilesHere = new System.Windows.Forms.Label();
            this.toolTip_MainForm = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_BMPMonitor)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_BMPMonitor
            // 
            this.pictureBox_BMPMonitor.BackColor = System.Drawing.Color.Black;
            this.pictureBox_BMPMonitor.Location = new System.Drawing.Point(0, 19);
            this.pictureBox_BMPMonitor.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox_BMPMonitor.Name = "pictureBox_BMPMonitor";
            this.pictureBox_BMPMonitor.Size = new System.Drawing.Size(334, 220);
            this.pictureBox_BMPMonitor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_BMPMonitor.TabIndex = 0;
            this.pictureBox_BMPMonitor.TabStop = false;
            this.pictureBox_BMPMonitor.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox_BMPMonitor_DragDrop);
            this.pictureBox_BMPMonitor.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox_BMPMonitor_DragEnter);
            // 
            // listBox_Files
            // 
            this.listBox_Files.AllowDrop = true;
            this.listBox_Files.BackColor = System.Drawing.SystemColors.Window;
            this.listBox_Files.FormattingEnabled = true;
            this.listBox_Files.HorizontalScrollbar = true;
            this.listBox_Files.ItemHeight = 12;
            this.listBox_Files.Location = new System.Drawing.Point(0, 260);
            this.listBox_Files.Margin = new System.Windows.Forms.Padding(0);
            this.listBox_Files.Name = "listBox_Files";
            this.listBox_Files.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_Files.Size = new System.Drawing.Size(281, 184);
            this.listBox_Files.TabIndex = 2;
            this.listBox_Files.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_Files_DragDrop);
            this.listBox_Files.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox_Files_DragEnter);
            this.listBox_Files.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox_Files_KeyDown);
            // 
            // label_Information
            // 
            this.label_Information.AutoSize = true;
            this.label_Information.Location = new System.Drawing.Point(7, 244);
            this.label_Information.Name = "label_Information";
            this.label_Information.Size = new System.Drawing.Size(35, 12);
            this.label_Information.TabIndex = 3;
            this.label_Information.Text = "label1";
            // 
            // button_Start
            // 
            this.button_Start.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Start.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_Start.Location = new System.Drawing.Point(218, 443);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(116, 66);
            this.button_Start.TabIndex = 4;
            this.button_Start.Text = "button1";
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // progressBar_DataVolume
            // 
            this.progressBar_DataVolume.Location = new System.Drawing.Point(0, 490);
            this.progressBar_DataVolume.Name = "progressBar_DataVolume";
            this.progressBar_DataVolume.Size = new System.Drawing.Size(218, 19);
            this.progressBar_DataVolume.TabIndex = 5;
            // 
            // label_DataVolume
            // 
            this.label_DataVolume.AutoSize = true;
            this.label_DataVolume.Location = new System.Drawing.Point(7, 475);
            this.label_DataVolume.Name = "label_DataVolume";
            this.label_DataVolume.Size = new System.Drawing.Size(35, 12);
            this.label_DataVolume.TabIndex = 6;
            this.label_DataVolume.Text = "label1";
            // 
            // label_BMPFileName
            // 
            this.label_BMPFileName.AutoSize = true;
            this.label_BMPFileName.Location = new System.Drawing.Point(7, 5);
            this.label_BMPFileName.Name = "label_BMPFileName";
            this.label_BMPFileName.Size = new System.Drawing.Size(35, 12);
            this.label_BMPFileName.TabIndex = 7;
            this.label_BMPFileName.Text = "label1";
            // 
            // label_HidingLevel
            // 
            this.label_HidingLevel.AutoSize = true;
            this.label_HidingLevel.Location = new System.Drawing.Point(7, 452);
            this.label_HidingLevel.Name = "label_HidingLevel";
            this.label_HidingLevel.Size = new System.Drawing.Size(86, 12);
            this.label_HidingLevel.TabIndex = 8;
            this.label_HidingLevel.Text = "データ秘匿レベル";
            // 
            // comboBox_HidingLevel
            // 
            this.comboBox_HidingLevel.BackColor = System.Drawing.SystemColors.Window;
            this.comboBox_HidingLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_HidingLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox_HidingLevel.FormattingEnabled = true;
            this.comboBox_HidingLevel.Items.AddRange(new object[] {
            "8",
            "7",
            "6",
            "5",
            "4",
            "3",
            "2",
            "1"});
            this.comboBox_HidingLevel.Location = new System.Drawing.Point(99, 449);
            this.comboBox_HidingLevel.Name = "comboBox_HidingLevel";
            this.comboBox_HidingLevel.Size = new System.Drawing.Size(41, 20);
            this.comboBox_HidingLevel.TabIndex = 9;
            this.comboBox_HidingLevel.TextChanged += new System.EventHandler(this.comboBox_HidingLevel_TextChanged);
            // 
            // button_FileDelete
            // 
            this.button_FileDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_FileDelete.Location = new System.Drawing.Point(281, 260);
            this.button_FileDelete.Name = "button_FileDelete";
            this.button_FileDelete.Size = new System.Drawing.Size(53, 23);
            this.button_FileDelete.TabIndex = 10;
            this.button_FileDelete.Text = "削除";
            this.button_FileDelete.UseVisualStyleBackColor = true;
            this.button_FileDelete.Click += new System.EventHandler(this.button_FileDelete_Click);
            // 
            // label_DropPictureHere
            // 
            this.label_DropPictureHere.AllowDrop = true;
            this.label_DropPictureHere.AutoSize = true;
            this.label_DropPictureHere.BackColor = System.Drawing.Color.Black;
            this.label_DropPictureHere.ForeColor = System.Drawing.Color.White;
            this.label_DropPictureHere.Location = new System.Drawing.Point(62, 117);
            this.label_DropPictureHere.Name = "label_DropPictureHere";
            this.label_DropPictureHere.Size = new System.Drawing.Size(191, 72);
            this.label_DropPictureHere.TabIndex = 11;
            this.label_DropPictureHere.Text = "ここに画像ファイルをドラッグしてください。\r\n\r\n↓ドラッグできる画像↓\r\nBMP，JPG，PNG，TIFF\r\nまたは\r\nこのプログラムでファイルを書込済の画像";
            this.label_DropPictureHere.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_DropPictureHere.DragDrop += new System.Windows.Forms.DragEventHandler(this.label_DropPictureHere_DragDrop);
            this.label_DropPictureHere.DragEnter += new System.Windows.Forms.DragEventHandler(this.label_DropPictureHere_DragEnter);
            // 
            // label_DropFilesHere
            // 
            this.label_DropFilesHere.AllowDrop = true;
            this.label_DropFilesHere.AutoSize = true;
            this.label_DropFilesHere.BackColor = System.Drawing.Color.White;
            this.label_DropFilesHere.Location = new System.Drawing.Point(35, 332);
            this.label_DropFilesHere.Name = "label_DropFilesHere";
            this.label_DropFilesHere.Size = new System.Drawing.Size(217, 24);
            this.label_DropFilesHere.TabIndex = 12;
            this.label_DropFilesHere.Text = "ここに記録したいファイルをドラッグしてください。\r\n（複数可）";
            this.label_DropFilesHere.DragDrop += new System.Windows.Forms.DragEventHandler(this.label_DropFilesHere_DragDrop);
            this.label_DropFilesHere.DragEnter += new System.Windows.Forms.DragEventHandler(this.label_DropFilesHere_DragEnter);
            // 
            // toolTip_MainForm
            // 
            this.toolTip_MainForm.AutomaticDelay = 50;
            this.toolTip_MainForm.AutoPopDelay = 5000;
            this.toolTip_MainForm.InitialDelay = 50;
            this.toolTip_MainForm.ReshowDelay = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 509);
            this.Controls.Add(this.label_DropFilesHere);
            this.Controls.Add(this.label_DropPictureHere);
            this.Controls.Add(this.button_FileDelete);
            this.Controls.Add(this.comboBox_HidingLevel);
            this.Controls.Add(this.label_HidingLevel);
            this.Controls.Add(this.label_BMPFileName);
            this.Controls.Add(this.label_DataVolume);
            this.Controls.Add(this.progressBar_DataVolume);
            this.Controls.Add(this.button_Start);
            this.Controls.Add(this.label_Information);
            this.Controls.Add(this.listBox_Files);
            this.Controls.Add(this.pictureBox_BMPMonitor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "透かし餅";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_BMPMonitor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_BMPMonitor;
        private System.Windows.Forms.ListBox listBox_Files;
        private System.Windows.Forms.Label label_Information;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.ProgressBar progressBar_DataVolume;
        private System.Windows.Forms.Label label_DataVolume;
        private System.Windows.Forms.Label label_BMPFileName;
        private System.Windows.Forms.Label label_HidingLevel;
        private System.Windows.Forms.ComboBox comboBox_HidingLevel;
        private System.Windows.Forms.Button button_FileDelete;
        private System.Windows.Forms.Label label_DropPictureHere;
        private System.Windows.Forms.Label label_DropFilesHere;
        private System.Windows.Forms.ToolTip toolTip_MainForm;
    }
}

