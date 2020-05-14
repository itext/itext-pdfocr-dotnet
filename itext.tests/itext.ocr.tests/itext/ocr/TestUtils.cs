using System;
using System.IO;
using System.Threading;

namespace iText.Ocr {
    public class TestUtils {
        // directory with test files
        protected internal static String testDirectory = "itext/ocr/";

        /// <summary>Return current test directory.</summary>
        /// <returns>String</returns>
        public static String GetCurrentDirectory() {
            string appRoot = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var dir = new DirectoryInfo(appRoot).Parent.Parent.Parent;
            return dir.FullName + @"\resources\" + testDirectory;
        }
    }
}
