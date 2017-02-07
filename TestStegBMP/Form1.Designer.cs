namespace TestStegBMP
{
    partial class Form1
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
            this.button_StartTest = new System.Windows.Forms.Button();
            this.checkedListBox_TestList = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // button_StartTest
            // 
            this.button_StartTest.Location = new System.Drawing.Point(188, 338);
            this.button_StartTest.Name = "button_StartTest";
            this.button_StartTest.Size = new System.Drawing.Size(85, 23);
            this.button_StartTest.TabIndex = 0;
            this.button_StartTest.Text = "Start Tests";
            this.button_StartTest.UseVisualStyleBackColor = true;
            this.button_StartTest.Click += new System.EventHandler(this.button_StartTest_Click);
            // 
            // checkedListBox_TestList
            // 
            this.checkedListBox_TestList.FormattingEnabled = true;
            this.checkedListBox_TestList.Items.AddRange(new object[] {
            "FastBitmap.CopyBits to nulltest",
            "FastBitmap.CopyBits from nulltest"});
            this.checkedListBox_TestList.Location = new System.Drawing.Point(12, 12);
            this.checkedListBox_TestList.Name = "checkedListBox_TestList";
            this.checkedListBox_TestList.Size = new System.Drawing.Size(261, 312);
            this.checkedListBox_TestList.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 373);
            this.Controls.Add(this.checkedListBox_TestList);
            this.Controls.Add(this.button_StartTest);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_StartTest;
        private System.Windows.Forms.CheckedListBox checkedListBox_TestList;
    }
}

