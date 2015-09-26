using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GzsTool.ArchiveBrowser.Interfaces;
using GzsTool.Common;
using GzsTool.Common.Interfaces;
using GzsTool.Fpk;
using GzsTool.Pftxs;
using GzsTool.Qar;

namespace GzsTool.ArchiveBrowser.Model
{
    public class OpenQarTask
    {
        private readonly IOpenQarProgress _progress;

        public bool Cancelled { get; set; }
        public bool IsBusy { get; set; }

        public OpenQarTask(IOpenQarProgress progress)
        {
            _progress = progress;
        }
        
        public IEnumerable<TreeNode> GetTreeNodesFromFilePaths(string[] filePaths)
        {
            IsBusy = true;

            _progress.Report("Reading dictionaries...");
            GzsTool.Program.ReadDictionaries();
            var virtualFileSystemDirectories = new List<VirtualFileSystemDirectory>();
            var inputStreams = new List<FileStream>();

            _progress.Report("Opening qar archives...");
            _progress.Report(0, filePaths.Length, 0.0, 0.1);
            int i = 0;
            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileName(filePath);
                _progress.Report("Opening " + fileName);

                FileStream input = new FileStream(filePath, FileMode.Open);
                inputStreams.Add(input);

                if (!QarFile.IsQarFile(input))
                {
                    MessageBox.Show(string.Format("{0} is not a valid QAR file.", fileName), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    try
                    {
                        VirtualFileSystemDirectory directory = ReadQarArchive(filePath, input);
                        virtualFileSystemDirectories.Add(directory);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Unable to read {0}\n{1}", fileName, ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                
                i++;
                _progress.Report(i, filePaths.Length, 0.0, 0.1);
            }

            var nodes = CreateTreeNodesFromFileSystemDirectories(virtualFileSystemDirectories);

            foreach (var inputStream in inputStreams)
            {
                inputStream.Dispose();
            }

            IsBusy = false;
            return nodes;
        }

        private VirtualFileSystemDirectory ReadQarArchive(string path, Stream input)
        {
            VirtualFileSystemDirectory outputDirectory = new VirtualFileSystemDirectory(Path.GetFileName(path));

            QarFile qarFile = QarFile.ReadQarFile(input);
            qarFile.Name = Path.GetFileName(path);
            foreach (var exportedFile in qarFile.ExportFiles(input))
            {
                outputDirectory.WriteFile(exportedFile.FileName, exportedFile.DataStream);
            }
            return outputDirectory;
        }

        private VirtualFileSystemDirectory ReadFpkArchive(string path, Stream input)
        {
            VirtualFileSystemDirectory outputDirectory = new VirtualFileSystemDirectory(path);

            FpkFile fpkFile = FpkFile.ReadFpkFile(input);
            fpkFile.Name = Path.GetFileName(path);
            foreach (var exportedFile in fpkFile.ExportFiles(input))
            {
                outputDirectory.WriteFile(exportedFile.FileName, exportedFile.DataStream);
            }
            return outputDirectory;
        }

        private VirtualFileSystemDirectory ReadPftxsArchive(string path, Stream input)
        {
            VirtualFileSystemDirectory outputDirectory = new VirtualFileSystemDirectory(path);

            PftxsFile pftxsFile = PftxsFile.ReadPftxsFile(input);
            pftxsFile.Name = Path.GetFileName(path);
            foreach (var exportedFile in pftxsFile.ExportFiles(input))
            {
                outputDirectory.WriteFile(exportedFile.FileName, exportedFile.DataStream);
            }
            return outputDirectory;
        }

        private IEnumerable<TreeNode> CreateTreeNodesFromFileSystemDirectories(List<VirtualFileSystemDirectory> qarDirectories)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            List<VirtualFileSystemDirectory> archiveDirectories = new List<VirtualFileSystemDirectory>();

            _progress.Report("Reading qar archives...");
            _progress.Report(0, qarDirectories.Count, 0.1, 0.4);
            int i = 0;
            foreach (var qarDirectory in qarDirectories)
            {
                _progress.Report("Reading qar archive " + qarDirectory.Name);
                archiveDirectories.Add(qarDirectory);
                CreateAllArchiveDirs(qarDirectory, archiveDirectories);
                i++;
                _progress.Report(i, qarDirectories.Count, 0.1, 0.4);

            }

            _progress.Report("Reading fpk, fpkd and pftxs archives");
            _progress.Report(0, archiveDirectories.Count, 0.5, 0.3);
            i = 0;
            foreach (var directory in archiveDirectories)
            {
                List<TreeNode> dirNodes = CreateDirNodesVariant2(directory, directory);
                MergeNodes(nodes, dirNodes);

                i++;
                _progress.Report(i, archiveDirectories.Count, 0.5, 0.3);
            }

            _progress.Report("Counting references...");
            _progress.Report(0, nodes.Count, 0.8, 0.1);
            i = 0;
            foreach (var node in nodes)
            {
                SetRefCount(node);
                i++;
                _progress.Report(i, nodes.Count, 0.8, 0.1);
            }

            _progress.Report("Ordering files...");
            OrderByType(nodes);

            _progress.Report("Done");
            _progress.Report(1, 1, 1, 0);
            return nodes;
        }

        private void OrderByType(List<TreeNode> nodes)
        {
            List<TreeNode> directories = new List<TreeNode>();
            List<TreeNode> files = new List<TreeNode>();

            foreach (var treeNode in nodes)
            {
                if (((NodeTag)treeNode.Tag).Entry is VirtualFileSystemDirectory)
                {
                    directories.Add(treeNode);
                }
                if (((NodeTag)treeNode.Tag).Entry is VirtualFileSystemFile)
                {
                    files.Add(treeNode);
                }
                OrderByType(treeNode.Nodes);
            }

            nodes.Clear();
            nodes.AddRange(directories.OrderBy(d => d.Text, StringComparer.InvariantCultureIgnoreCase));
            nodes.AddRange(files.OrderBy(f => f.Text, StringComparer.InvariantCultureIgnoreCase));
        }

        private void OrderByType(TreeNodeCollection nodes)
        {
            List<TreeNode> directories = new List<TreeNode>();
            List<TreeNode> files = new List<TreeNode>();

            foreach (var treeNode in nodes.Cast<TreeNode>())
            {
                if (((NodeTag)treeNode.Tag).Entry is VirtualFileSystemDirectory)
                {
                    directories.Add(treeNode);
                }
                if (((NodeTag)treeNode.Tag).Entry is VirtualFileSystemFile)
                {
                    files.Add(treeNode);
                }
                OrderByType(treeNode.Nodes);
            }
            nodes.Clear();
            nodes.AddRange(directories.OrderBy(d => d.Text, StringComparer.InvariantCultureIgnoreCase).ToArray());
            nodes.AddRange(files.OrderBy(f => f.Text, StringComparer.InvariantCultureIgnoreCase).ToArray());
        }

        private void SetRefCount(TreeNode treeNode)
        {
            treeNode.Text = treeNode.Text + " - " + ((NodeTag)treeNode.Tag).References.Count;

            foreach (var node in treeNode.Nodes.Cast<TreeNode>())
            {
                SetRefCount(node);
            }
        }

        private void MergeNodes(List<TreeNode> nodes, List<TreeNode> dirNodes)
        {
            List<TreeNode> missingNodes = new List<TreeNode>();
            foreach (var dirNode in dirNodes)
            {
                var node1 = nodes.SingleOrDefault(n => n.Text == dirNode.Text);
                if (node1 != null)
                {
                    MergeNodes(node1, dirNode);
                }
                else
                {
                    missingNodes.Add(dirNode);
                }

            }
            nodes.AddRange(missingNodes);
        }

        private void MergeNodes(TreeNode node, TreeNode dirNode)
        {
            List<TreeNode> missingNodes = new List<TreeNode>();
            ((NodeTag)node.Tag).References.AddRange(((NodeTag)dirNode.Tag).References);

            foreach (var directoryNode in dirNode.Nodes.Cast<TreeNode>())
            {
                var node1 = node.Nodes.Cast<TreeNode>().SingleOrDefault(n => n.Text == directoryNode.Text);
                if (node1 != null)
                {
                    MergeNodes(node1, directoryNode);
                }
                else
                {
                    missingNodes.Add(directoryNode);
                }
            }
            node.Nodes.AddRange(missingNodes.ToArray());
        }

        private List<TreeNode> CreateDirNodesVariant2(VirtualFileSystemDirectory outputDirectory, VirtualFileSystemDirectory archiveDirectory)
        {
            List<TreeNode> nodes = new List<TreeNode>();

            foreach (var entry in outputDirectory.Entries)
            {
                VirtualFileSystemDirectory directory = entry as VirtualFileSystemDirectory;
                if (directory != null)
                {
                    TreeNode dirNode = new TreeNode(directory.Name);
                    dirNode.Tag = new NodeTag
                    {
                        Entry = directory,
                        References = new List<IFileSystemEntry> { archiveDirectory }
                    };
                    var dirSubNodes = CreateDirNodesVariant2(directory, archiveDirectory);
                    dirNode.Nodes.AddRange(dirSubNodes.ToArray());
                    nodes.Add(dirNode);
                }
                VirtualFileSystemFile file = entry as VirtualFileSystemFile;
                if (file != null)
                {
                    TreeNode fileNode = new TreeNode(file.Name);
                    fileNode.Tag = new NodeTag
                    {
                        Entry = file,
                        References = new List<IFileSystemEntry> { archiveDirectory }
                    };
                    nodes.Add(fileNode);
                }
            }
            return nodes;
        }

        private void CreateAllArchiveDirs(VirtualFileSystemDirectory outputDirectory, List<VirtualFileSystemDirectory> dirs)
        {
            foreach (var entry in outputDirectory.Entries)
            {
                VirtualFileSystemDirectory directory = entry as VirtualFileSystemDirectory;
                if (directory != null)
                {
                    CreateAllArchiveDirs(directory, dirs);
                }
                VirtualFileSystemFile file = entry as VirtualFileSystemFile;
                if (file != null)
                {
                    if (file.Name.EndsWith(".fpk") || file.Name.EndsWith(".fpkd"))
                    {
                        var fpkDir = ReadFpkArchive(file.Name, file.ContentStream);
                        dirs.Add(fpkDir);
                        CreateAllArchiveDirs(fpkDir, dirs);
                    }
                    else if (file.Name.EndsWith(".pftxs"))
                    {
                        var pftxsDir = ReadPftxsArchive(file.Name, file.ContentStream);
                        dirs.Add(pftxsDir);
                        CreateAllArchiveDirs(pftxsDir, dirs);
                    }
                    file.ResetStreamFactoryMethod();
                }
            }

        }
    }
}