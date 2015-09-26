using System;
using System.Windows.Forms;
using GzsTool.ArchiveBrowser.Forms;

namespace GzsTool.ArchiveBrowser
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ArchiveBrowserForm());
        }
    }
}
