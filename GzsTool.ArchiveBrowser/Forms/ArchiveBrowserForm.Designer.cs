using GzsTool.ArchiveBrowser.Model;

namespace GzsTool.ArchiveBrowser.Forms
{
    partial class ArchiveBrowserForm
    {
        private System.ComponentModel.IContainer components = null;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.openDatFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.fileTreeView = new System.Windows.Forms.TreeView();
            this.referenceListBox = new System.Windows.Forms.ListBox();
            this.referenceContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyReferenceContextMenuStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentGroupBox = new System.Windows.Forms.GroupBox();
            this.referencesGroupBox = new System.Windows.Forms.GroupBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenQarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTreeFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveTreeFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openQarBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.filterButton = new System.Windows.Forms.Button();
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.clearFilterButton = new System.Windows.Forms.Button();
            this.referenceContextMenuStrip.SuspendLayout();
            this.contentGroupBox.SuspendLayout();
            this.referencesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // openDatFileDialog
            // 
            this.openDatFileDialog.Filter = "QAR-Files|*.dat";
            this.openDatFileDialog.Multiselect = true;
            // 
            // fileTreeView
            // 
            this.fileTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTreeView.Location = new System.Drawing.Point(6, 19);
            this.fileTreeView.Name = "fileTreeView";
            this.fileTreeView.Size = new System.Drawing.Size(389, 476);
            this.fileTreeView.TabIndex = 1;
            this.fileTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.fileTreeView_AfterSelect);
            // 
            // referenceListBox
            // 
            this.referenceListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.referenceListBox.ContextMenuStrip = this.referenceContextMenuStrip;
            this.referenceListBox.FormattingEnabled = true;
            this.referenceListBox.Location = new System.Drawing.Point(6, 19);
            this.referenceListBox.Name = "referenceListBox";
            this.referenceListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.referenceListBox.Size = new System.Drawing.Size(196, 459);
            this.referenceListBox.TabIndex = 2;
            // 
            // referenceContextMenuStrip
            // 
            this.referenceContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyReferenceContextMenuStripMenuItem});
            this.referenceContextMenuStrip.Name = "referenceContextMenuStrip";
            this.referenceContextMenuStrip.Size = new System.Drawing.Size(103, 26);
            // 
            // copyReferenceContextMenuStripMenuItem
            // 
            this.copyReferenceContextMenuStripMenuItem.Name = "copyReferenceContextMenuStripMenuItem";
            this.copyReferenceContextMenuStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyReferenceContextMenuStripMenuItem.Text = "Copy";
            this.copyReferenceContextMenuStripMenuItem.Click += new System.EventHandler(this.copyReferenceContextMenuStripMenuItem_Click);
            // 
            // contentGroupBox
            // 
            this.contentGroupBox.Controls.Add(this.fileTreeView);
            this.contentGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentGroupBox.Location = new System.Drawing.Point(0, 0);
            this.contentGroupBox.Name = "contentGroupBox";
            this.contentGroupBox.Size = new System.Drawing.Size(401, 500);
            this.contentGroupBox.TabIndex = 3;
            this.contentGroupBox.TabStop = false;
            this.contentGroupBox.Text = "Archive Content";
            // 
            // referencesGroupBox
            // 
            this.referencesGroupBox.Controls.Add(this.referenceListBox);
            this.referencesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.referencesGroupBox.Location = new System.Drawing.Point(0, 0);
            this.referencesGroupBox.Name = "referencesGroupBox";
            this.referencesGroupBox.Size = new System.Drawing.Size(208, 500);
            this.referencesGroupBox.TabIndex = 4;
            this.referencesGroupBox.TabStop = false;
            this.referencesGroupBox.Text = "Archive References";
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(12, 56);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.contentGroupBox);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.referencesGroupBox);
            this.splitContainer.Size = new System.Drawing.Size(613, 500);
            this.splitContainer.SplitterDistance = 401;
            this.splitContainer.TabIndex = 5;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(634, 24);
            this.menuStrip.TabIndex = 6;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileOpenQarToolStripMenuItem,
            this.openTreeToolStripMenuItem,
            this.saveTreeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // fileOpenQarToolStripMenuItem
            // 
            this.fileOpenQarToolStripMenuItem.Name = "fileOpenQarToolStripMenuItem";
            this.fileOpenQarToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.fileOpenQarToolStripMenuItem.Text = "Open .dat file(s)";
            this.fileOpenQarToolStripMenuItem.Click += new System.EventHandler(this.fileOpenQarToolStripMenuItem_Click);
            // 
            // openTreeToolStripMenuItem
            // 
            this.openTreeToolStripMenuItem.Enabled = false;
            this.openTreeToolStripMenuItem.Name = "openTreeToolStripMenuItem";
            this.openTreeToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.openTreeToolStripMenuItem.Text = "Open tree";
            this.openTreeToolStripMenuItem.Click += new System.EventHandler(this.openTreeToolStripMenuItem_Click);
            // 
            // saveTreeToolStripMenuItem
            // 
            this.saveTreeToolStripMenuItem.Enabled = false;
            this.saveTreeToolStripMenuItem.Name = "saveTreeToolStripMenuItem";
            this.saveTreeToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.saveTreeToolStripMenuItem.Text = "Save tree";
            this.saveTreeToolStripMenuItem.Click += new System.EventHandler(this.saveTreeToolStripMenuItem_Click);
            // 
            // openTreeFileDialog
            // 
            this.openTreeFileDialog.FileName = "gzsToolTree.txt";
            // 
            // openQarBackgroundWorker
            // 
            this.openQarBackgroundWorker.WorkerSupportsCancellation = true;
            this.openQarBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.openQarBackgroundWorker_DoWork);
            this.openQarBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.openQarBackgroundWorker_RunWorkerCompleted);
            // 
            // filterButton
            // 
            this.filterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterButton.Location = new System.Drawing.Point(469, 27);
            this.filterButton.Name = "filterButton";
            this.filterButton.Size = new System.Drawing.Size(75, 23);
            this.filterButton.TabIndex = 7;
            this.filterButton.Text = "Filter";
            this.filterButton.UseVisualStyleBackColor = true;
            this.filterButton.Click += new System.EventHandler(this.filterButton_Click);
            // 
            // filterTextBox
            // 
            this.filterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterTextBox.Location = new System.Drawing.Point(18, 27);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(445, 20);
            this.filterTextBox.TabIndex = 8;
            this.filterTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.filterTextBox_KeyUp);
            // 
            // clearFilterButton
            // 
            this.clearFilterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clearFilterButton.Location = new System.Drawing.Point(550, 27);
            this.clearFilterButton.Name = "clearFilterButton";
            this.clearFilterButton.Size = new System.Drawing.Size(75, 23);
            this.clearFilterButton.TabIndex = 9;
            this.clearFilterButton.Text = "Clear";
            this.clearFilterButton.UseVisualStyleBackColor = true;
            this.clearFilterButton.Click += new System.EventHandler(this.clearFilterButton_Click);
            // 
            // ArchiveBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 562);
            this.Controls.Add(this.clearFilterButton);
            this.Controls.Add(this.filterTextBox);
            this.Controls.Add(this.filterButton);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "ArchiveBrowserForm";
            this.Text = "GzsTool Archive Browser";
            this.referenceContextMenuStrip.ResumeLayout(false);
            this.contentGroupBox.ResumeLayout(false);
            this.referencesGroupBox.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openDatFileDialog;
        private System.Windows.Forms.TreeView fileTreeView;
        private System.Windows.Forms.ListBox referenceListBox;
        private System.Windows.Forms.GroupBox contentGroupBox;
        private System.Windows.Forms.GroupBox referencesGroupBox;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileOpenQarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveTreeToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openTreeFileDialog;
        private System.Windows.Forms.SaveFileDialog saveTreeFileDialog;
        private System.Windows.Forms.ContextMenuStrip referenceContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyReferenceContextMenuStripMenuItem;
        private System.ComponentModel.BackgroundWorker openQarBackgroundWorker;
        private readonly OpenQarTask _openQarTask;
        private System.Windows.Forms.Button filterButton;
        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.Button clearFilterButton;
    }
}

