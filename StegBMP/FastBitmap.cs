using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace StegBMP
{
    public class FastBitmap : IDisposable
    {
        #region Data Member

        private Bitmap _bitmap;
        private BitmapData _bitmapData;
        private long _pos;
        private HEADER_STEGBMP _header;

        private bool _hasData;
        private int _securityLevel;

        #endregion

        #region Property

        /// <summary>
        /// 画像データにファイルが記録されているかどうか。
        /// 記録されている場合：true，記録されていない場合：false
        /// 読み取り専用。
        /// </summary>
        public bool HasData { get { return _hasData; } }

        /// <summary>
        /// 画像データにファイルが記録されている場合の，データ秘匿レベル。
        /// 画像データにファイルが記録されている場合：1～7のいずれか，
        /// 記録されていない場合：0
        /// 読み取り専用。
        /// </summary>
        public int SecurityLevel
        {
            get
            {
                return _securityLevel;
            }

            set
            {
                if (!_hasData)
                {
                    _securityLevel = value;
                }
            }
        }

        #endregion

        #region Constructor

        public FastBitmap(Bitmap bitmap)
        {
            if (null == bitmap)
            {
                throw new Exception("null == bitmap @ FastBitmap.FastBitmap()");
            }

            _bitmap = bitmap;
            _bitmapData = _bitmap.LockBits(
                                new Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
                                ImageLockMode.ReadWrite,
                                _bitmap.PixelFormat
                                );

            _header.Identifier = Const.STRING_INVALID_PROGRAM_IDENTIFIER;
            _hasData = false;
            this.HasDataInImage();

#if DEBUG
            Test_CopyBits();
#endif
        }

        #endregion

        #region Public Method

        public void Dispose()
        {
            if (null != _bitmapData)
            {
                _bitmap.UnlockBits(_bitmapData);
                _bitmapData = null;
            }
        }

        #endregion

        #region Internal Method

        /// <summary>
        /// 画像に記録されている全ファイルの名前を返す。
        /// </summary>
        /// <param name="fileNames">[o] 画像に記録されている全ファイルの名前</param>
        /// <param name="securityLevel">[i] データ秘匿レベル</param>
        /// <returns>MOCHI_STATUS</returns>
        internal MOCHI_STATUS GetFileNames(ref string[] fileNames, int securityLevel)
        {
            // まだヘッダ情報が得られていない場合
            if (Const.STRING_INVALID_PROGRAM_IDENTIFIER == _header.Identifier)
            {
                return MOCHI_STATUS.STATUS_NOT_YET;
            }

            fileNames = new string[_header.nFiles];
            for (int iFile = 0; iFile < _header.nFiles; iFile++)
            {
                fileNames[iFile] = _header.FileProperties[iFile].FileName;
            }

            return MOCHI_STATUS.STATUS_SUCCESSFUL;
        }

        /// <summary>
        /// 画像に記録されている全ファイルを抽出する。
        /// </summary>
        /// <param name="securityLevel">[i] データ秘匿レベル</param>
        /// <param name="path">[i] ファイルの出力先ディレクトリパス</param>
        /// <returns>MOCHI_STATUS</returns>
        internal MOCHI_STATUS ExtractFiles(string path)
        {
            // まだヘッダ情報が得られていない場合
            if (Const.STRING_INVALID_PROGRAM_IDENTIFIER == _header.Identifier)
            {
                return MOCHI_STATUS.STATUS_NOT_YET;
            }

            // ヘッダを読み飛ばすためにデータ位置を設定しておく。
            long posData = (2 * _header.Identifier.Length) + 2 + 2;
            long totalFileSize = 0L;
            for (int iFile = 0; iFile < _header.nFiles; iFile++)
            {
                posData += 8;   // ファイルサイズ情報
                posData += (2 * _header.FileProperties[iFile].FileName.Length) + 2; // ファイル名情報
                totalFileSize += _header.FileProperties[iFile].FileSize;
            }

            byte[] buf = new byte[posData + totalFileSize];
            this.ReadData(buf, _securityLevel);

            for (int iFile = 0; iFile < _header.nFiles; iFile++)
            {
                byte[] fileBuf = new byte[_header.FileProperties[iFile].FileSize];
                Array.Copy(buf, posData, fileBuf, 0L, _header.FileProperties[iFile].FileSize);

                using (FileStream fs = new FileStream(path + @"\" + _header.FileProperties[iFile].FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    fs.Write(fileBuf, 0, fileBuf.Length);
                    fs.Close();
                }

                posData += _header.FileProperties[iFile].FileSize;
            }

            return MOCHI_STATUS.STATUS_SUCCESSFUL;
        }

        /// <summary>
        /// 画像にファイルを記録できる容量を返す。ファイルの個数によらず必要なヘッダ容量も考慮する。
        /// </summary>
        /// <returns>容量</returns>
        internal long GetFreeSpaceOnImage()
        {
            if (
                (null == _bitmap) ||
                (null == _bitmapData) ||
                (Const.STRING_INVALID_PROGRAM_IDENTIFIER != _header.Identifier)
                )
            {
                return 0L;
            }

            long freeSpace = 0L;

            freeSpace = (3L * _bitmap.Width * _bitmap.Height * _securityLevel / 8) - 22;

            return freeSpace;
        }

        /// <summary>
        /// 画像に記録されている全ファイルの合計サイズを返す。
        /// </summary>
        /// <returns>全ファイルの合計サイズ</returns>
        internal long GetTotalFileSize()
        {
            if (
                (null == _bitmap) ||
                (null == _bitmapData) ||
                (Const.STRING_INVALID_PROGRAM_IDENTIFIER == _header.Identifier)
                )
            {
                return -1L;
            }

            long totalFileSize = 0L;

            for (int iFile = 0; iFile < _header.nFiles; iFile++)
            {
                totalFileSize += _header.FileProperties[iFile].FileSize;
            }

            return totalFileSize;
        }

        internal MOCHI_STATUS Save(string path, ImageFormat imageFormat)
        {
            MOCHI_STATUS ret = MOCHI_STATUS.STATUS_SUCCESSFUL;

            try
            {
                _bitmap.Save(path, imageFormat);
            }
            catch (ExternalException)
            {
                ret = MOCHI_STATUS.STATUS_UNSUCCESSFUL;
            }

            return ret;
        }

        #endregion

        #region Internal Unsafe Method

        internal MOCHI_STATUS WriteData(byte[] data, int nBitsToWriteInPixel)
        {
            //
            // 引数チェック
            //

            // _bitmapDataがnullの場合
            if (null == _bitmapData)
            {
                return MOCHI_STATUS.STATUS_UNSUCCESSFUL;
            }

            // dataがnullの場合
            if (null == data)
            {
                return MOCHI_STATUS.STATUS_INVALID_PARAMETER;
            }

            // nBitsToWriteInPixelの値が不正な場合
            if ((nBitsToWriteInPixel < 1) || (8 < nBitsToWriteInPixel))
            {
                return MOCHI_STATUS.STATUS_INVALID_PARAMETER;
            }

            // データ量が多すぎて書き込み切れない場合
            if ((3L * (long)_bitmapData.Width * (long)_bitmapData.Height * (long)nBitsToWriteInPixel) < data.LongLength * 8L)
            {
                return MOCHI_STATUS.STATUS_TOO_LARGE_DATA;
            }

            //
            // データ書き込み開始
            //

            // まだ書き込んでいないデータの量[bit]
            long leftDataBits = 8L * data.LongLength;
            Trace.TraceInformation("leftDataBits = {0}\n", leftDataBits);

            // 読み込み先データのバイト位置
            long posData = 0L;

            // 読み込み先データの，次に読み込み始めるビット位置(0～7)
            byte startBitInData = 0x00;

            bool written = false;

            unsafe
            {
                byte* _p = (byte*)_bitmapData.Scan0;
                for (long y = 0L; (y < (long)_bitmapData.Height) && !written; y++)
                {
                    for (long x = 0L; (x < (long)_bitmapData.Width) && !written; x++)
                    {
                        // 色についてループ
                        for (long c = 0L; c < 3L; c++)
                        {
                            _pos = (3L * x) + ((long)_bitmapData.Stride * y) + c;

                            // これから書き込むデータの量[bit]
                            int bitToWrite = ((8 - startBitInData) > nBitsToWriteInPixel) ? nBitsToWriteInPixel : (8 - startBitInData);

                            // 上位ビットから順にデータを書き込む。
                            this.CopyBits(&_p[_pos], 8 - nBitsToWriteInPixel, data[posData], (int)startBitInData, bitToWrite);

                            // 読み込み先データのバイト位置を次に進める場合
                            if ((8 - startBitInData) <= nBitsToWriteInPixel)
                            {
                                posData++;
                            }

                            // すべてのデータを書き込み終えた場合
                            if (leftDataBits <= nBitsToWriteInPixel)
                            {
                                Trace.TraceInformation("すべてのデータの書き込みが完了しました。\n");
                                written = true;
                                break;
                            }

                            // 更にデータを書き込む場合（startBitInData-nBitsToWriteInPixelビットだけ書き込める）
                            if ((8 - startBitInData) < nBitsToWriteInPixel)
                            {
                                this.CopyBits(&_p[_pos], 8 - (nBitsToWriteInPixel - 8 + startBitInData), data[posData], 0, nBitsToWriteInPixel - 8 + startBitInData);
                            }

                            // 書き込んだ分だけ残りデータ量を更新する。
                            leftDataBits -= nBitsToWriteInPixel;
                            startBitInData = (byte)((startBitInData + (byte)nBitsToWriteInPixel) % 8);
                        }
                    }
                }
            }

            return MOCHI_STATUS.STATUS_SUCCESSFUL;
        }

        internal MOCHI_STATUS ReadData(byte[] data, int nBitsToReadInPixel)
        {
            //
            // 引数チェック
            //

            // _bitmapDataがnullの場合
            if (null == _bitmapData)
            {
                return MOCHI_STATUS.STATUS_UNSUCCESSFUL;
            }

            // dataがnullの場合
            if (null == data)
            {
                return MOCHI_STATUS.STATUS_INVALID_PARAMETER;
            }

            // nBitsToReadInPixelの値が不正な場合
            if ((nBitsToReadInPixel < 1) || (8 < nBitsToReadInPixel))
            {
                return MOCHI_STATUS.STATUS_INVALID_PARAMETER;
            }

            // 何ビット分の読み込みを行うか
            long leftDataBits = 8L * data.LongLength;

            // 書き込み先バッファのバイト位置
            long posData = 0L;

            // 書き込み先バッファの，次に書き込み始めるビット位置(0～7)
            byte startBitInData = 0x00;

            bool read = false;

            unsafe
            {
                byte* _p = (byte*)_bitmapData.Scan0;
                for (long y = 0L; (y < (long)_bitmapData.Height) && !read; y++)
                {
                    for (long x = 0L; (x < (long)_bitmapData.Width) && !read; x++)
                    {
                        // 色についてループ
                        for (long c = 0L; c < 3L; c++)
                        {
                            _pos = (3L * x) + ((long)_bitmapData.Stride * y) + c;

                            // これから読み込むデータの量[bit]
                            int bitToRead = (nBitsToReadInPixel > (8 - startBitInData)) ? (8 - startBitInData) : nBitsToReadInPixel;

                            fixed (byte* tmpData = &data[posData])
                            {
                                this.CopyBits(
                                    tmpData, startBitInData,
                                    _p[_pos], 8 - nBitsToReadInPixel,
                                    bitToRead
                                    );
                            }

                            if (nBitsToReadInPixel >= (8 - startBitInData))
                            {
                                posData++;
                            }

                            // すべてのデータを書き込み終えた場合
                            if (leftDataBits <= nBitsToReadInPixel)
                            {
                                Trace.TraceInformation("すべてのデータの読み込みが完了しました。\n");
                                read = true;
                                break;
                            }

                            // 更にデータを書き込む場合（startBitInData-nBitsToWriteInPixelビットだけ書き込める）
                            if ((8 - startBitInData) < nBitsToReadInPixel)
                            {
                                int nReadBits = bitToRead;  // 先ほどのCopyBitsでコピーしたビット数
                                bitToRead = nBitsToReadInPixel - (8 - startBitInData);

                                fixed (byte* tmpData = &data[posData])
                                {
                                    this.CopyBits(
                                        tmpData, 0,
                                        _p[_pos], 8 - (nBitsToReadInPixel - 8 + startBitInData),
                                        nBitsToReadInPixel - 8 + startBitInData
                                        );
                                }
                            }

                            leftDataBits -= nBitsToReadInPixel;
                            startBitInData = (byte)((startBitInData + (byte)nBitsToReadInPixel) % 8);
                        }
                    }
                }
            }

            return MOCHI_STATUS.STATUS_SUCCESSFUL;
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 画像に本プログラムの識別子情報が含まれている場合はヘッダ情報を取得する。
        /// 識別子情報が含まれてい場合はそっとしておく。
        /// </summary>
        private void HasDataInImage()
        {
            byte[] identifier = new byte[Encoding.Unicode.GetBytes(Const.STRING_PROGRAM_IDENTIFIER).Length];
            int securityLevel = 0;

            // すべての秘匿レベルについて識別子情報の探索を行う。
            for (securityLevel = 1; securityLevel <= 8; securityLevel++)
            {
                this.ReadData(identifier, securityLevel);
#if DEBUG
                string tmpString = Encoding.Unicode.GetString(identifier);
#endif
                // 識別子情報が見つかった場合
                if (Encoding.Unicode.GetString(identifier) == Const.STRING_PROGRAM_IDENTIFIER)
                {
                    break;
                }
            }

            // 識別子情報が見つからなかった場合
            if (9 <= securityLevel)
            {
                _securityLevel = 1;
                return;
            }

            _securityLevel = securityLevel;
            _hasData = true;

            // ヘッダ情報を取得する。
            _header = this.GetHeader();

            return;
        }

        private HEADER_STEGBMP GetHeader()
        {
            if (false == _hasData)
            {
                return _header;
            }

            HEADER_STEGBMP header = new HEADER_STEGBMP();
            byte[] tmp = null;

            // ファイルの数を取得する。
            tmp = new byte[18 + 2 + 2];
            this.ReadData(tmp, _securityLevel);
            header.Identifier = Encoding.Unicode.GetString(tmp, 0, 2 * Const.STRING_PROGRAM_IDENTIFIER.Length);
            Trace.TraceInformation("header.Identifier = ", header.Identifier);
            Trace.Assert(header.Identifier == Const.STRING_PROGRAM_IDENTIFIER);
            header.Version = (int)BitConverter.ToInt16(tmp, 18);
            Trace.TraceInformation("バージョン：{0}", header.Version);
            header.nFiles = (int)BitConverter.ToInt16(tmp, 20);
            Trace.TraceInformation("この画像には{0}個のファイルが記録されているみたいですよ", header.nFiles);

            //
            // ファイル名を取得する。
            //

            header.FileProperties = new FILE_PROPERTY[header.nFiles];

            int[] fileNameSize = new int[header.nFiles];    // バイト配列のときのファイル名の大きさ[byte]
            tmp = new byte[18 + 2 + 2 + (8 * header.nFiles) + (256 * header.nFiles)];
            this.ReadData(tmp, _securityLevel);

            int posData = 22;   // 1つ目のファイルに関する情報が記録されている場所
            for (int iFile = 0; iFile < header.nFiles; iFile++)
            {
                header.FileProperties[iFile].FileSize = BitConverter.ToInt64(tmp, posData);
                posData += 8;

                int fileNameHead = posData;
                // 終端文字が見つかるまでシークする。
                while ((tmp[posData] != (byte)0x00) || (tmp[posData + 1] != (byte)0x00))
                {
                    posData += 2;
                }
                fileNameSize[iFile] = posData - fileNameHead;

                // ファイル名を取得する。
                byte[] byte_fileName = new byte[fileNameSize[iFile]];
                int totalFileNameSize = 0;
                for (int jFile = 0; jFile < iFile; jFile++)
                {
                    totalFileNameSize += fileNameSize[jFile] + 2;
                }
                Array.Copy(
                    tmp, 22 + (8 * (iFile + 1)) + totalFileNameSize,
                    byte_fileName, 0,
                    fileNameSize[iFile]);
                header.FileProperties[iFile].FileName = Encoding.Unicode.GetString(byte_fileName);

                posData += 2;  // 終端文字を読み飛ばす。
            }

            return header;
        }

        #endregion

        #region Private Unsafe Method

        /// <summary>
        /// 1バイトの領域から別の1バイトの領域へビット単位でデータをコピーする。
        /// </summary>
        /// <param name="to"></param>
        /// <param name="idxTo"></param>
        /// <param name="from"></param>
        /// <param name="idxFrom"></param>
        /// <param name="nBits"></param>
        /// <returns></returns>
        unsafe private MOCHI_STATUS CopyBits(byte* to, int idxTo, byte from, int idxFrom, int nBits)
        {
            if (
                (null == to) ||
                (idxTo < 0) ||
                (7 < idxTo) ||
                (idxFrom < 0) ||
                (7 < idxFrom) ||
                (nBits < 1) ||
                (8 < nBits) ||
                ((8 - idxTo) < nBits) ||
                ((8 - idxFrom) < nBits)
                )
            {
                return MOCHI_STATUS.STATUS_INVALID_PARAMETER;
            }

            *to = (byte)((*to & (new Const().BITMASK[8 - idxTo])) | ((byte)(from << idxFrom) >> idxTo));

            return MOCHI_STATUS.STATUS_SUCCESSFUL;
        }

        #endregion

#if DEBUG
        #region Test Method

        unsafe private void Test_CopyBits()
        {
            byte b = 0xAA;  // 2進数で 10101010
            byte a = 0x55;  // 2進数で 01010101
            Trace.Assert(MOCHI_STATUS.STATUS_INVALID_PARAMETER == this.CopyBits(null, 1, b, 3, 3));
            Trace.Assert(MOCHI_STATUS.STATUS_INVALID_PARAMETER == this.CopyBits(&a, 6, b, 3, 3));
            Trace.Assert(MOCHI_STATUS.STATUS_INVALID_PARAMETER == this.CopyBits(&a, 3, b, 6, 3));
            Trace.Assert(b == 0xAA);
            Trace.Assert(a == 0x55);
            this.CopyBits(&a, 5, b, 3, 3);
            Trace.TraceInformation("a = {0:X}", a);
            Trace.Assert(a == 0x52);
            b = 0xAA;
            a = 0x55;
            this.CopyBits(&a, 3, b, 5, 3);
            Trace.Assert(a == 0x48);
        }

        #endregion
#endif
    }
}
