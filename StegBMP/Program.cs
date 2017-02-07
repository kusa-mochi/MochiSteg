using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace StegBMP
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex mutex = new Mutex(false, "MochiMutex");
            if (mutex.WaitOne(0, false) == false)
            {
                MessageBox.Show("多重起動はできません。");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            mutex.ReleaseMutex();
        }
    }
}
