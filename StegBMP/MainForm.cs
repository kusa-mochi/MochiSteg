using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

using StegBMP.Properties;

// TODO: 操作説明実装

namespace StegBMP
{
    public partial class MainForm : Form
    {
        #region Data Member

        /// <summary>
        /// メインフォームの各コントロールの状態を決めるためのフラグ
        /// </summary>
        private FormState _formState;

        /// <summary>
        /// BMP画像に記録できるデータの最大サイズ
        /// </summary>
        private long _maxDataSize;

        private long _totalFilesSize;

        private long _totalFileNameSize;

        private int _securityLevel;

        /// <summary>
        /// 画像ファイルのパス
        /// </summary>
        private string _imageFilePath;

        private Bitmap _bitmap;

        /// <summary>
        /// 画像に書き込むファイルのパス
        /// </summary>
        private string[] _filePaths;

        /// <summary>
        /// .NET Frameworkのバグに対応するためのフラグ。
        /// コンボボックスの値をコード内で変えたときになぜかイベントが2回発生するので，
        /// そのうちの1回だけを拾ってメッセージを表示できるように，このフラグで調整する。
        /// </summary>
        private int _comboBoxChangeIndexFlag;

        #endregion

        #region Constructor

        public MainForm()
        {
            InitializeComponent();

            this.comboBox_HidingLevel.SelectedIndex = 0;
            _securityLevel = 1;

            _comboBoxChangeIndexFlag = 0;

            _formState = FormState.INITIALIZED;
            this.SetControlState(_formState);
        }

        #endregion

        #region Private Method

        private void pictureBox_BMPMonitor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void pictureBox_BMPMonitor_DragDrop(object sender, DragEventArgs e)
        {
            // ドロップされたファイルの名前を取得する。
            string[] dropedFiles = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (null == dropedFiles) return;

            // リストボックスの中身を空にする。
            this.listBox_Files.Items.Clear();

            // ファイルのパスリストを空にする。
            _filePaths = null;

            // データ抽出モードから遷移してきた場合
            if (FormState.SHOWING_FILE_LIST == _formState)
            {
                // ロックされている画像ファイルのハンドルを解放する。
                _bitmap.Dispose();
            }

            try
            {
                _bitmap = (new Utility()).ImageToBitmap(dropedFiles[0]);
            }
            catch (ArgumentNullException exception)
            {
                MessageBox.Show(exception.Message, string.Format(Resources.FORMATED_MESSAGE_IS_NULL, @"dropedFiles[0]"));
                return;
            }
            catch (InvalidExtentionException)
            {
                MessageBox.Show(Resources.MESSAGE_INVALID_EXTENTION, Resources.CAPTION_INVALID_EXTENTION, MessageBoxButtons.OK);
                return;
            }
            catch
            {
                MessageBox.Show(Resources.MESSAGE_UNKNOWN_ERROR, Resources.CAPTION_UNKNOWN_ERROR, MessageBoxButtons.OK);
                return;
            }

            // インスタンス変数に画像ファイルのパスを格納する。
            _imageFilePath = dropedFiles[0];

            // 画像をピクチャボックスに表示する。
            this.pictureBox_BMPMonitor.ImageLocation = _imageFilePath;

            // 画像のファイル名を表示する。
            this.label_BMPFileName.Text = Path.GetFileName(_imageFilePath);

            // プログレスバーをリセットする。
            this.progressBar_DataVolume.Value = 0;

            using (FastBitmap fb = new FastBitmap(_bitmap))
            {
                // 画像の中にファイルが格納されている場合
                if (fb.HasData)
                {
                    _formState = FormState.SHOWING_FILE_LIST;

                    // 画像に記録されている全ファイルの名前を取得する。
                    fb.GetFileNames(ref _filePaths, fb.SecurityLevel);

                    // リストボックスに全ファイルの名前を追加する。
                    this.listBox_Files.Items.Clear();
                    foreach (string path in _filePaths)
                    {
                        this.listBox_Files.Items.Add(path);
                    }

                    // ファイルの合計容量を表示する。
                    NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
                    numberFormatInfo.NumberDecimalDigits = 0;
                    this.label_DataVolume.Text = fb.GetTotalFileSize().ToString("N", numberFormatInfo) + " Byte";

                    // 記録容量は表示する必要がないので値をゼロにする。
                    this.progressBar_DataVolume.Value = 0;
                }
                // BMPの中にファイルが格納されていない場合
                else
                {
                    fb.SecurityLevel = this.comboBox_HidingLevel.SelectedIndex + 1;

                    // ファイルを記録できる容量を表示する。
                    _maxDataSize = fb.GetFreeSpaceOnImage();
                    NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
                    numberFormatInfo.NumberDecimalDigits = 0;
                    this.label_DataVolume.Text = "残り容量：" + _maxDataSize.ToString("N", numberFormatInfo) + " Byte";

                    _totalFilesSize = 0L;

                    _formState = FormState.FILE_DROP_AVAILABLE;
                }
            }

            this.SetControlState(_formState);
        }

        private void label_DropPictureHere_DragEnter(object sender, DragEventArgs e)
        {
            this.pictureBox_BMPMonitor_DragEnter(sender, e);
        }

        private void label_DropPictureHere_DragDrop(object sender, DragEventArgs e)
        {
            this.pictureBox_BMPMonitor_DragDrop(sender, e);
        }

        private void listBox_Files_DragEnter(object sender, DragEventArgs e)
        {
            this.pictureBox_BMPMonitor_DragEnter(sender, e);
        }

        private void listBox_Files_DragDrop(object sender, DragEventArgs e)
        {
            // 現状のリストボックスの要素数を格納しておく変数
            int oldFilePathsLength;

            // ドロップされた全ファイルの合計サイズ
            long dropedFilesSize = 0L;

            // ドロップされた全ファイルの名前の合計サイズ（Unicodeでのサイズ）
            long dropedFileNamesSize = 0L;

            // ドロップされた全ファイルの名前を取得する。
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (null == s) return;

            // ドロップされた全ファイルの合計サイズを計算する。
            this.GetTotalDataSize(s, ref dropedFilesSize, ref dropedFileNamesSize);

            // 全ファイルの合計サイズが容量を超えた場合
            if (_maxDataSize - _totalFilesSize - _totalFileNameSize - (8 * this.listBox_Files.Items.Count) < dropedFilesSize + dropedFileNamesSize + (8 * s.Length))
            {
                // エラーメッセージを表示する。
                MessageBox.Show(Resources.MESSAGE_TOO_LARGE_FILE, Resources.CAPTION_TOO_LARGE_FILE, MessageBoxButtons.OK);

                // 何もせず退散
                return;
            }

            // 現状のリストボックスに何かしらの要素が存在する場合
            if (null != _filePaths)
            {
                oldFilePathsLength = _filePaths.Length;
                Array.Resize<string>(ref _filePaths, _filePaths.Length + s.Length);
                for (int iFile = 0; iFile < s.Length; iFile++)
                {
                    _filePaths[oldFilePathsLength + iFile] = s[iFile];
                    this.listBox_Files.Items.Add(Path.GetFileName(s[iFile]));
                }
            }
            else
            {
                _filePaths = new string[s.Length];
                for (int iFile = 0; iFile < _filePaths.Length; iFile++)
                {
                    _filePaths[iFile] = s[iFile];
                    this.listBox_Files.Items.Add(Path.GetFileName(s[iFile]));
                }
                oldFilePathsLength = 0;
            }

            // リストボックスに表示されている全ファイルの合計サイズを計算する。
            this.GetTotalDataSize(_filePaths, ref _totalFilesSize, ref _totalFileNameSize);

            // 記録可能な容量と現在の全ファイルの合計サイズを表示する。
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalDigits = 0;
            this.label_DataVolume.Text = "残り容量：" + (_maxDataSize - _totalFilesSize - _totalFileNameSize - (8 * _filePaths.Length)).ToString("N", numberFormatInfo) + " Byte";
            this.progressBar_DataVolume.Value = 100 * (int)(_totalFilesSize + _totalFileNameSize + (8 * _filePaths.Length)) / (int)_maxDataSize;

            if (FormState.READY_TO_WRITE_DATA != _formState)
            {
                _formState = FormState.READY_TO_WRITE_DATA;
                this.SetControlState(_formState);
            }
        }

        private void label_DropFilesHere_DragEnter(object sender, DragEventArgs e)
        {
            this.listBox_Files_DragEnter(sender, e);
        }

        private void label_DropFilesHere_DragDrop(object sender, DragEventArgs e)
        {
            this.listBox_Files_DragDrop(sender, e);
        }

        private void button_FileDelete_Click(object sender, EventArgs e)
        {
            // リストボックスの選択された項目についてループ
            while (this.listBox_Files.SelectedItems.Count > 0)
            {
                int idx = -1;
                idx = this.listBox_Files.SelectedIndices[0];

                // リストボックスのidx番目の要素を削除する。
                this.listBox_Files.Items.RemoveAt(idx);

                // ファイルパスリストのidx番目の要素を削除する。
                (new Utility()).StringArrayRemoveAt(ref _filePaths, idx);

                // 全ファイルの合計サイズの表示を更新する。
                this.GetTotalDataSize(_filePaths, ref _totalFilesSize, ref _totalFileNameSize);
                NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
                numberFormatInfo.NumberDecimalDigits = 0;
                this.label_DataVolume.Text = "残り容量：" + (_maxDataSize - _totalFilesSize - _totalFileNameSize - (8 * _filePaths.Length)).ToString("N", numberFormatInfo) + " Byte";
                this.progressBar_DataVolume.Value = 100 * (int)(_totalFilesSize + _totalFileNameSize + (8 * _filePaths.Length)) / (int)_maxDataSize;
            }

            if (this.listBox_Files.Items.Count == 0)
            {
                _formState = FormState.FILE_DROP_AVAILABLE;
                SetControlState(_formState);
            }
        }

        private void listBox_Files_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                this.button_FileDelete_Click(sender, e);
            }
        }

        private void comboBox_HidingLevel_TextChanged(object sender, EventArgs e)
        {
            // ファイルをリストボックスにドロップできる状態ではない場合
            if ((_formState != FormState.FILE_DROP_AVAILABLE) && (_formState != FormState.READY_TO_WRITE_DATA))
            {
                return;
            }

            // 画像データがまだ用意されていない場合
            if (null == _bitmap)
            {
                return;
            }

            // 現在のセキュリティレベルとコンボボックスで選択されている項目が同じ場合
            if (_securityLevel == this.comboBox_HidingLevel.SelectedIndex + 1)
            {
                return;
            }

            bool success = true;

            using (FastBitmap fb = new FastBitmap(_bitmap))
            {
                fb.SecurityLevel = this.comboBox_HidingLevel.SelectedIndex + 1;
                long tmpMaxDataSize = fb.GetFreeSpaceOnImage();

                // 現在リストボックスに表示されているファイルの数
                int nFile;
                if (null == _filePaths)
                {
                    nFile = 0;
                }
                else
                {
                    nFile = _filePaths.Length;
                }

                // データ秘匿レベルを変えたことで容量が足りなくなった場合
                if (tmpMaxDataSize < _totalFilesSize + _totalFileNameSize + (8 * nFile))
                {
                    // エラーメッセージを表示する。
                    switch (_comboBoxChangeIndexFlag)
                    {
                        case 0:
                            MessageBox.Show(Resources.MESSAGE_TOO_LARGE_FILE_WITH_ADVICE, Resources.CAPTION_TOO_LARGE_FILE, MessageBoxButtons.OK);
                            _comboBoxChangeIndexFlag = 1;
                            break;
                        case 1:
                            _comboBoxChangeIndexFlag = 0;
                            break;
                        default:
                            // 何もしない。
                            break;
                    }

                    success = false;
                }
                else
                {
                    _securityLevel = this.comboBox_HidingLevel.SelectedIndex + 1;
                    _maxDataSize = tmpMaxDataSize;
                    NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
                    numberFormatInfo.NumberDecimalDigits = 0;
                    this.label_DataVolume.Text = "残り容量：" + (_maxDataSize - _totalFilesSize - (8 * nFile)).ToString("N", numberFormatInfo) + " Byte";
                    this.progressBar_DataVolume.Value = 100 * (int)(_totalFilesSize + _totalFileNameSize + (8 * nFile)) / (int)_maxDataSize;
                }
            }

            if (!success)
            {
                // コンボボックスの状態を元に戻す。
                this.comboBox_HidingLevel.SelectedIndex = _securityLevel - 1;
            }
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            switch (_formState)
            {
                case FormState.READY_TO_WRITE_DATA:

                    _formState = FormState.MAKING_BMP;
                    this.SetControlState(_formState);

                    //
                    // 画像へのバッファデータの書き込み
                    //

                    using (FastBitmap fb = new FastBitmap(_bitmap))
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = Resources.DEFAULT_FILE_NAME;
                        saveFileDialog.Filter = "BMPファイル(*.bmp)|*.bmp|PNGファイル(*.png)|*.png|TIFFファイル(*.tiff)|*.tiff";
                        saveFileDialog.FilterIndex = 1;
                        saveFileDialog.Title = Resources.MESSAGE_FILE_SAVE_DIALOG;
                        saveFileDialog.RestoreDirectory = true;
                        saveFileDialog.OverwritePrompt = true;
                        saveFileDialog.CheckPathExists = true;
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            // 書き込むファイルの数
                            int nFile = _filePaths.Length;

                            // 各ファイルのサイズ
                            long[] fileSize = new long[nFile];

                            // 各ファイルのファイル名の長さ（終端文字込み）
                            int[] fileNameSize = new int[nFile];

                            //
                            // 書き込むデータの準備
                            //

                            // プログラムの識別子
                            byte[] programIdentifier = Encoding.Unicode.GetBytes(Const.STRING_PROGRAM_IDENTIFIER);
                            Trace.Assert(programIdentifier.Length == (2 * Const.STRING_PROGRAM_IDENTIFIER.Length));

                            // プログラムのバージョン
                            byte[] version = BitConverter.GetBytes((Int16)1);
                            Trace.Assert(version.Length == 2);

                            // 記録するファイルの数
                            byte[] byte_nFile = BitConverter.GetBytes((Int16)nFile);
                            Trace.Assert(byte_nFile.Length == 2);

                            // ここまでで 22byte
                            {
                                int i = 0;
                                foreach (string path in _filePaths)
                                {
                                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                                    {
                                        // ファイルサイズを取得する。
                                        fileSize[i] = fs.Length;

                                        // ファイル名の長さを取得する。
                                        fileNameSize[i] = Encoding.Unicode.GetBytes(Path.GetFileName(path)).Length + 2; // 最後の+2は終端文字分。
                                    }
                                    i++;
                                }
                            }

                            // ここまでで 22 + (8 * nFile) + (fileNameSize * nFile) byte

                            // ファイルサイズの合計とファイル名の長さの合計をもとめる。
                            this.GetTotalDataSize(_filePaths, ref _totalFilesSize, ref _totalFileNameSize);

                            // 必要なバッファの大きさをもとめる。
                            long bufSize =
                                programIdentifier.LongLength +
                                version.LongLength +
                                byte_nFile.LongLength +
                                (8L * nFile) +
                                _totalFileNameSize +
                                _totalFilesSize;

                            //
                            // バッファへのデータの書き込み
                            //

                            // ヘッダ分も合わせてバッファを確保する。
                            byte[] dataToWrite = new byte[bufSize];

                            // バッファのポインタ代わり
                            long pDataToWrite = 0L;

                            // ヘッダ情報
                            Array.Copy(
                                programIdentifier, 0L,
                                dataToWrite, pDataToWrite,
                                programIdentifier.LongLength
                                );
                            pDataToWrite += programIdentifier.LongLength;

                            Array.Copy(
                                version, 0L,
                                dataToWrite, pDataToWrite,
                                version.LongLength
                                );
                            pDataToWrite += version.LongLength;

                            Array.Copy(
                                byte_nFile, 0L,
                                dataToWrite, pDataToWrite,
                                byte_nFile.LongLength
                                );
                            pDataToWrite += byte_nFile.LongLength;

                            // ファイルサイズとファイル名
                            for (int iFile = 0; iFile < nFile; iFile++)
                            {
                                Array.Copy(
                                    BitConverter.GetBytes(fileSize[iFile]), 0L,
                                    dataToWrite, pDataToWrite,
                                    sizeof(long)
                                    );
                                pDataToWrite += sizeof(long);

                                string fileName = Path.GetFileName(_filePaths[iFile]);
                                byte[] byte_fileName = new byte[(2 * fileName.Length) + 2];
                                Array.Copy(
                                    Encoding.Unicode.GetBytes(fileName), 0L,
                                    byte_fileName, 0L,
                                    2 * fileName.Length
                                    );
                                byte_fileName[byte_fileName.Length - 2] = (byte)0x00;
                                byte_fileName[byte_fileName.Length - 1] = (byte)0x00;
                                Array.Copy(
                                    byte_fileName, 0L,
                                    dataToWrite, pDataToWrite,
                                    byte_fileName.LongLength
                                    );
                                pDataToWrite += byte_fileName.LongLength;
                            }

                            // ファイルの中身
                            {
                                int i = 0;
                                foreach (string path in _filePaths)
                                {
                                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                                    {
                                        byte[] fileBuf = new byte[fileSize[i]];
                                        fs.Read(fileBuf, 0, fileBuf.Length);
                                        Array.Copy(
                                            fileBuf, 0L,
                                            dataToWrite, pDataToWrite,
                                            fileBuf.LongLength
                                            );
                                        pDataToWrite += fileBuf.LongLength;
                                    }
                                    i++;
                                }
                            }

                            fb.WriteData(dataToWrite, this.comboBox_HidingLevel.SelectedIndex + 1);

                            ImageFormat imageFormat = null;
                            switch (Path.GetExtension(saveFileDialog.FileName))
                            {
                                case ".bmp":
                                    imageFormat = ImageFormat.Bmp;
                                    break;
                                case ".BMP":
                                    imageFormat = ImageFormat.Bmp;
                                    break;
                                case ".png":
                                    imageFormat = ImageFormat.Png;
                                    break;
                                case ".PNG":
                                    imageFormat = ImageFormat.Png;
                                    break;
                                case ".tiff":
                                    imageFormat = ImageFormat.Tiff;
                                    break;
                                case ".TIFF":
                                    imageFormat = ImageFormat.Tiff;
                                    break;
                                default:
                                    // 何もしない
                                    break;
                            }

                            if (MOCHI_STATUS.STATUS_UNSUCCESSFUL == fb.Save(saveFileDialog.FileName, imageFormat))
                            {
                                MessageBox.Show(Resources.MESSAGE_FILE_ALREADY_OPENED, Resources.CAPTION_FILE_ALREADY_OPENED, MessageBoxButtons.OK);
                            }
                        }
                        MessageBox.Show(Resources.MESSAGE_WRITING_SUCCEEDED, Resources.CAPTION_WRITING_SUCCEEDED, MessageBoxButtons.OK);
                    }

                    break;

                case FormState.SHOWING_FILE_LIST:
                    _formState = FormState.EXTRACTING_FILES;
                    this.SetControlState(_formState);

                    // ファイルの出力先ディレクトリを指定するダイアログを表示する。
                    FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                    folderBrowserDialog.Description = Resources.MESSAGE_DIRECTORY_SELECT_DIALOG;
                    folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                    folderBrowserDialog.ShowNewFolderButton = true;
                    if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        using (FastBitmap fb = new FastBitmap(_bitmap))
                        {
                            fb.ExtractFiles(folderBrowserDialog.SelectedPath);
                        }
                        MessageBox.Show(Resources.MESSAGE_EXTRACTING_SUCCEEDED, Resources.CAPTION_EXTRACTING_SUCCEEDED, MessageBoxButtons.OK);
                    }

                    break;

                default:
                    break;
            }

            _formState = FormState.INITIALIZED;
            this.SetControlState(_formState);
        }

        private void GetTotalDataSize(string[] paths, ref long totalFileSize, ref long totalFileNameLength, long init_totalFileSize = 0L, long init_totalFileNameLength = 0L)
        {
            if (null == paths) return;

            totalFileSize = 0L;
            totalFileNameLength = 0L;
            foreach (string path in paths)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    totalFileSize += fs.Length;
                    totalFileNameLength += (2 * Path.GetFileName(path).Length) + 2;
                }
            }
        }

        private void SetControlState(FormState formState)
        {
            switch (formState)
            {
                case FormState.INITIALIZED:
                    _maxDataSize = Const.UNDEFINED_MAX_DATA_SIZE;
                    _imageFilePath = null;
                    if (null != _bitmap)
                    {
                        _bitmap.Dispose();
                    }
                    _bitmap = null;
                    _filePaths = null;
                    _totalFilesSize = 0L;
                    _totalFileNameSize = 0L;

                    this.Height = 295;
                    this.label_BMPFileName.Enabled = false;
                    this.label_BMPFileName.Text = "";
                    this.pictureBox_BMPMonitor.Enabled = true;
                    this.pictureBox_BMPMonitor.ImageLocation = null;
                    this.pictureBox_BMPMonitor.AllowDrop = true;
                    this.label_DropPictureHere.Visible = true;
                    this.label_Information.Enabled = true;
                    this.label_Information.Text = "";
                    this.listBox_Files.Enabled = false;
                    this.listBox_Files.SelectionMode = SelectionMode.None;
                    this.listBox_Files.AllowDrop = false;
                    this.label_DropFilesHere.Visible = false;
                    this.button_FileDelete.Enabled = false;
                    this.label_HidingLevel.Enabled = false;
                    this.comboBox_HidingLevel.Enabled = false;
                    this.label_DataVolume.Enabled = false;
                    this.label_DataVolume.Text = "";
                    this.progressBar_DataVolume.Enabled = false;
                    this.progressBar_DataVolume.Style = ProgressBarStyle.Blocks;
                    this.button_Start.Enabled = false;
                    this.button_Start.Text = "";
                    break;

                case FormState.FILE_DROP_AVAILABLE:
                    this.Height = 547;
                    this.label_BMPFileName.Enabled = true;
                    //this.label_BMPFileName.Text = "";
                    this.pictureBox_BMPMonitor.Enabled = true;
                    this.label_DropPictureHere.Visible = false;
                    this.label_Information.Enabled = true;
                    this.label_Information.Text = Resources.MESSAGE_DRAG_FILES_HERE;
                    this.listBox_Files.Enabled = true;
                    this.listBox_Files.SelectionMode = SelectionMode.MultiExtended;
                    this.listBox_Files.AllowDrop = true;
                    this.label_DropFilesHere.Visible = true;
                    this.button_FileDelete.Enabled = false;
                    this.label_HidingLevel.Enabled = true;
                    this.comboBox_HidingLevel.Enabled = true;
                    this.label_DataVolume.Enabled = true;
                    //this.label_DataVolume.Text = "";
                    this.progressBar_DataVolume.Enabled = true;
                    this.progressBar_DataVolume.Style = ProgressBarStyle.Blocks;
                    this.button_Start.Enabled = false;
                    this.button_Start.Text = "";
                    break;

                case FormState.READY_TO_WRITE_DATA:
                    this.Height = 547;
                    this.label_BMPFileName.Enabled = true;
                    //this.label_BMPFileName.Text = "";
                    this.pictureBox_BMPMonitor.Enabled = true;
                    this.label_DropPictureHere.Visible = false;
                    this.label_Information.Enabled = true;
                    this.label_Information.Text = Resources.MESSAGE_READY_TO_WRITE_DATA;
                    this.listBox_Files.Enabled = true;
                    this.listBox_Files.SelectionMode = SelectionMode.MultiExtended;
                    this.listBox_Files.AllowDrop = true;
                    this.label_DropFilesHere.Visible = false;
                    this.button_FileDelete.Enabled = true;
                    this.label_HidingLevel.Enabled = true;
                    this.comboBox_HidingLevel.Enabled = true;
                    this.label_DataVolume.Enabled = true;
                    //this.label_DataVolume.Text = "";
                    this.progressBar_DataVolume.Enabled = true;
                    this.progressBar_DataVolume.Style = ProgressBarStyle.Blocks;
                    this.button_Start.Enabled = true;
                    this.button_Start.Text = Resources.BUTTONTEXT_WRITE;
                    break;

                case FormState.SHOWING_FILE_LIST:
                    this.Height = 547;
                    this.label_BMPFileName.Enabled = true;
                    //this.label_BMPFileName.Text = "";
                    this.pictureBox_BMPMonitor.Enabled = true;
                    this.label_DropPictureHere.Visible = false;
                    this.label_Information.Enabled = true;
                    this.label_Information.Text = Resources.MESSAGE_IMAGE_INCLUDE_THESE_FILES;
                    this.listBox_Files.Enabled = true;
                    this.listBox_Files.SelectionMode = SelectionMode.None;
                    this.listBox_Files.AllowDrop = false;
                    this.label_DropFilesHere.Visible = false;
                    this.button_FileDelete.Enabled = false;
                    this.label_HidingLevel.Enabled = false;
                    this.comboBox_HidingLevel.Enabled = false;
                    this.label_DataVolume.Enabled = true;
                    //this.label_DataVolume.Text = "";
                    this.progressBar_DataVolume.Enabled = false;
                    this.progressBar_DataVolume.Style = ProgressBarStyle.Blocks;
                    this.button_Start.Enabled = true;
                    this.button_Start.Text = Resources.BUTTONTEXT_EXTRACT;
                    break;

                case FormState.MAKING_BMP:
                    this.Height = 547;
                    this.label_BMPFileName.Enabled = false;
                    //this.label_BMPFileName.Text = "";
                    this.pictureBox_BMPMonitor.Enabled = false;
                    this.label_DropPictureHere.Visible = false;
                    this.label_Information.Enabled = true;
                    this.label_Information.Text = Resources.MESSAGE_WRITING;
                    this.listBox_Files.Enabled = false;
                    this.listBox_Files.SelectionMode = SelectionMode.MultiExtended;
                    this.listBox_Files.AllowDrop = false;
                    this.label_DropFilesHere.Visible = false;
                    this.button_FileDelete.Enabled = false;
                    this.label_HidingLevel.Enabled = false;
                    this.comboBox_HidingLevel.Enabled = false;
                    this.label_DataVolume.Enabled = true;
                    //this.label_DataVolume.Text = "";
                    this.progressBar_DataVolume.Enabled = true;
                    this.progressBar_DataVolume.Style = ProgressBarStyle.Marquee;
                    this.button_Start.Enabled = false;
                    this.button_Start.Text = Resources.BUTTONTEXT_WRITING;
                    break;

                case FormState.EXTRACTING_FILES:
                    this.Height = 547;
                    this.label_BMPFileName.Enabled = false;
                    //this.label_BMPFileName.Text = "";
                    this.pictureBox_BMPMonitor.Enabled = false;
                    this.label_DropPictureHere.Visible = false;
                    this.label_Information.Enabled = true;
                    this.label_Information.Text = Resources.MESSAGE_EXTRACTING;
                    this.listBox_Files.Enabled = false;
                    this.listBox_Files.SelectionMode = SelectionMode.None;
                    this.listBox_Files.AllowDrop = false;
                    this.label_DropFilesHere.Visible = false;
                    this.button_FileDelete.Enabled = false;
                    this.label_HidingLevel.Enabled = false;
                    this.comboBox_HidingLevel.Enabled = false;
                    this.label_DataVolume.Enabled = true;
                    //this.label_DataVolume.Text = "";
                    this.progressBar_DataVolume.Enabled = true;
                    this.progressBar_DataVolume.Style = ProgressBarStyle.Marquee;
                    this.button_Start.Enabled = false;
                    this.button_Start.Text = Resources.BUTTONTEXT_EXTRACTING;
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}
