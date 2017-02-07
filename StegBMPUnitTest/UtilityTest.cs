using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using StegBMP;

namespace StegBMPUnitTest
{
    [TestClass]
    public class UtilityTest
    {
        #region Data Member

        private Utility _utility = null;

        #endregion

        [TestInitialize]
        public void TestInitializeUtility()
        {
            _utility = new Utility();
        }

        [TestCleanup]
        public void TestCleanupUtility()
        {

        }

        [TestMethod]
        public void TestStringArrayRemoveAt_Remove1st()
        {
            string[] stringArray = { "aaaaa", "bbbbb", "ccccc", "ddddd", "eeeee" };
            _utility.StringArrayRemoveAt(ref stringArray, 0);
            Assert.AreEqual(stringArray.Length, 4);
            Assert.AreEqual(stringArray[0], "bbbbb");
            Assert.AreEqual(stringArray[1], "ccccc");
            Assert.AreEqual(stringArray[2], "ddddd");
            Assert.AreEqual(stringArray[3], "eeeee");
        }

        [TestMethod]
        public void TestStringArrayRemoveAt_Remove2nd()
        {
            string[] stringArray = { "aaaaa", "bbbbb", "ccccc", "ddddd", "eeeee" };
            _utility.StringArrayRemoveAt(ref stringArray, 1);
            Assert.AreEqual(stringArray.Length, 4);
            Assert.AreEqual(stringArray[0], "aaaaa");
            Assert.AreEqual(stringArray[1], "ccccc");
            Assert.AreEqual(stringArray[2], "ddddd");
            Assert.AreEqual(stringArray[3], "eeeee");
        }

        [TestMethod]
        public void TestStringArrayRemoveAt_RemoveEnd()
        {
            string[] stringArray = { "aaaaa", "bbbbb", "ccccc", "ddddd", "eeeee" };
            _utility.StringArrayRemoveAt(ref stringArray, 4);
            Assert.AreEqual(stringArray.Length, 4);
            Assert.AreEqual(stringArray[0], "aaaaa");
            Assert.AreEqual(stringArray[1], "bbbbb");
            Assert.AreEqual(stringArray[2], "ccccc");
            Assert.AreEqual(stringArray[3], "ddddd");
        }

        [TestMethod]
        public void TestSringArrayRemoveAt_RemoveAll()
        {
            string[] stringArray = { "aaaaa", "bbbbb", "ccccc", "ddddd", "eeeee" };
            _utility.StringArrayRemoveAt(ref stringArray, 0);
            Assert.AreEqual(stringArray.Length, 4);
            _utility.StringArrayRemoveAt(ref stringArray, 0);
            Assert.AreEqual(stringArray.Length, 3);
            _utility.StringArrayRemoveAt(ref stringArray, 0);
            Assert.AreEqual(stringArray.Length, 2);
            _utility.StringArrayRemoveAt(ref stringArray, 0);
            Assert.AreEqual(stringArray.Length, 1);
            _utility.StringArrayRemoveAt(ref stringArray, 0);
            Assert.IsNull(stringArray);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSringArrayRemoveAt_a_IsNull()
        {
            string[] stringArray = null;
            _utility.StringArrayRemoveAt(ref stringArray, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSringArrayRemoveAt_idx_OutOfRange1()
        {
            string[] stringArray = { "aaaaa", "bbbbb", "ccccc", "ddddd", "eeeee" };
            _utility.StringArrayRemoveAt(ref stringArray, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSringArrayRemoveAt_idx_OutOfRange2()
        {
            string[] stringArray = { "aaaaa", "bbbbb", "ccccc", "ddddd", "eeeee" };
            _utility.StringArrayRemoveAt(ref stringArray, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImageToBitmap_path_IsNull()
        {
            string path = null;
            Bitmap bitmap = null;
            bitmap = _utility.ImageToBitmap(path);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidExtentionException))]
        public void TestImageToBitmap_NoExtention()
        {
            string path = "hoge";
            Bitmap bitmap = null;
            bitmap = _utility.ImageToBitmap(path);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidExtentionException))]
        public void TestImageToBitmap_InvalidExtention()
        {
            string path = "hoge.pdf";
            Bitmap bitmap = null;
            bitmap = _utility.ImageToBitmap(path);
        }
    }
}
