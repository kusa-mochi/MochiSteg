using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using StegBMP;

namespace TestStegBMP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void TestInit()
        {
            return;
        }

        private void TestFin()
        {
            return;
        }

        private void button_StartTest_Click(object sender, EventArgs e)
        {
            string testResult = "Test Result\n\n";

            foreach (var item in this.checkedListBox_TestList.SelectedItems)
            {
                TestInit();

                testResult += item.ToString() + ": ";

                switch (item.ToString())
                {
                    case "FastBitmap.CopyBits to nulltest":
                        testResult += Test_FastBitmap_CopyBits_to_null() == true ? "(^o^)\n" : "(#- -)\n";
                        break;
                    case "FastBitmap.CopyBits from nulltest":
                        testResult += Test_FastBitmap_CopyBits_from_null() == true ? "(^o^)\n" : "(#- -)\n";
                        break;
                    default:
                        testResult += "unknown test case\n";
                        break;
                }

                TestFin();
            }

            // テスト結果を表示する。
            MessageBox.Show(testResult);
        }

        #region Test Case

        private bool Test_FastBitmap_CopyBits_to_null()
        {
            FastBitmap fastBitmap = new FastBitmap(new Bitmap(640, 480));

            return false;
        }

        private bool Test_FastBitmap_CopyBits_from_null()
        {
            return false;
        }
        
        #endregion
    }
}
