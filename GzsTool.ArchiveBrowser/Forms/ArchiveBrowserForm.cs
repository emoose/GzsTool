using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using GzsTool.ArchiveBrowser.Model;
using GzsTool.Common.Interfaces;

namespace GzsTool.ArchiveBrowser.Forms
{
    public partial class ArchiveBrowserForm : Form
    {
        private readonly ProgressForm _progressForm;
        private TreeNode[] _nodes;

        public ArchiveBrowserForm()
        {
            InitializeComponent();
            _progressForm = new ProgressForm();
            _openQarTask = new OpenQarTask(_progressForm);
        }

        private void fileOpenQarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openDatFileDialog.ShowDialog() == DialogResult.OK)
            {
                _progressForm.Reset();
                openQarBackgroundWorker.RunWorkerAsync(openDatFileDialog.FileNames);
                if (_progressForm.ShowDialog() == DialogResult.Cancel)
                {
                    openQarBackgroundWorker.CancelAsync();
                }
            }
        }

        private void openQarBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _openQarTask.GetTreeNodesFromFilePaths((string[])e.Argument);
        }

        private void openQarBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                _nodes = ((IEnumerable<TreeNode>)e.Result).ToArray();
                UpdateTreeNodes(_nodes);
            }

            _progressForm.HideForm();
        }

        private void fileTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                DisplayRefs(((NodeTag)e.Node.Tag).References);
            }
        }

        private void openTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openTreeFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream file = File.Open(openTreeFileDialog.FileName, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    IEnumerable<TreeNode> nodes = formatter.Deserialize(file) as IEnumerable<TreeNode>;
                    if (nodes == null)
                    {
                        MessageBox.Show("Unable to open the tree nodes.", "Error", MessageBoxButtons.OK);
                    }
                    else
                    {
                        _nodes = nodes.ToArray();
                        UpdateTreeNodes(_nodes);
                    }
                }
            }
        }

        private void saveTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveTreeFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream file = File.Open(saveTreeFileDialog.FileName, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    // TODO: Serialize the NodeTag objects.
                    List<TreeNode> nodes = fileTreeView.Nodes.Cast<TreeNode>().ToList();
                    formatter.Serialize(file, nodes);
                    file.Flush();
                }
            }
        }

        private void copyReferenceContextMenuStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var selectedItem in referenceListBox.SelectedItems)
            {
                stringBuilder.AppendLine(selectedItem.ToString());
            }

            Clipboard.SetText(stringBuilder.ToString());
        }

        private void DisplayRefs(List<IFileSystemEntry> entries)
        {
            referenceListBox.Items.Clear();
            foreach (var fileSystemEntry in entries.OrderBy(r => r.Name))
            {
                referenceListBox.Items.Add(fileSystemEntry);
            }
        }

        private void filterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FilterNodes(filterTextBox.Text);
            }
        }

        private void filterButton_Click(object sender, EventArgs e)
        {
            FilterNodes(filterTextBox.Text);
        }

        private void clearFilterButton_Click(object sender, EventArgs e)
        {
            filterTextBox.Text = "";
            if (_nodes != null)
            {
                fileTreeView.Nodes.Clear();
                fileTreeView.Nodes.AddRange(_nodes);
            }
        }

        private void FilterNodes(string filterText)
        {
            if (_nodes != null)
            {
                UpdateTreeNodes(MatchAndCloneNodes(_nodes, filterText));
            }
        }

        private IEnumerable<TreeNode> MatchAndCloneNodes(IEnumerable<TreeNode> nodes, string filterText)
        {
            return MatchNodes(nodes.Select(node => (TreeNode)node.Clone()), filterText);
        }

        private IEnumerable<TreeNode> MatchNodes(IEnumerable<TreeNode> nodes, string filterText)
        {
            return nodes.Where(node => MatchNode(node, filterText));
        }

        private bool MatchNode(TreeNode node, string filterText)
        {
            var subNodes = MatchNodes(node.Nodes.Cast<TreeNode>(), filterText).ToArray();
            node.Nodes.Clear();
            node.Nodes.AddRange(subNodes);

            return subNodes.Any() || node.Text.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void UpdateTreeNodes(IEnumerable<TreeNode> nodes)
        {
            fileTreeView.Nodes.Clear();
            foreach (var node in nodes)
            {
                fileTreeView.Nodes.Add((TreeNode)node.Clone());
            }
        }
    }
}
