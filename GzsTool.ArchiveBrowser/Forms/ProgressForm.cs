using System.Windows.Forms;
using GzsTool.ArchiveBrowser.Interfaces;

namespace GzsTool.ArchiveBrowser.Forms
{
    public partial class ProgressForm : Form, IOpenQarProgress
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        public void Reset()
        {
            progressTextLabel.Text = "";
            progressBar.Value = 0;
            DialogResult = DialogResult.None;
        }
        
        public void Report(double value, double maximum, double progressBase, double progressFactor)
        {
            if (this.Visible)
            {
                Invoke((MethodInvoker) (() =>
                {
                    progressBar.Value = (int) ((progressBase + value/maximum*progressFactor)*100);
                }));
            }
        }

        public void Report(string text)
        {
            if (this.Visible)
            {
                Invoke((MethodInvoker)(() =>
                {
                    progressTextLabel.Text = text;
                }));
            }
        }

        public void HideForm()
        {
            DialogResult = DialogResult.None;
            Hide();
        }
    }
}
