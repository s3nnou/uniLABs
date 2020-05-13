using System;
using System.IO;
using System.Windows.Forms;


namespace lab4
{
    public partial class Form1 : Form
    {
        private string parentFolderPath;

        public Form1()
        {
            InitializeComponent();
            this.dirView.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
        }

        private void PopulateTreeView()
        {
                MonitorDirectory(parentFolderPath);
                TreeNode rootNode;

                DirectoryInfo info = new DirectoryInfo(parentFolderPath);

                if (info.Exists)
                {
                    rootNode = new TreeNode(info.Name);
                    rootNode.Tag = info;
                    GetDirectories(info.GetDirectories(), rootNode);
                    dirView.Nodes.Add(rootNode);
                }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            dirItemsList.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem item = null;

            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.FullName, 1);
                if(item.ToString().Contains(".txt"))
                    dirItemsList.Items.Add(item);
            }

            dirItemsList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            txtShow.Show();
            string[] lines = File.ReadAllLines(dirItemsList.SelectedItems[0].Text);

            foreach (string item in lines)
            {
                txtShow.Items.Add(item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dirView.BeginUpdate();
            TreeNode node = dirView.SelectedNode;
            
            string path = null;
            if (node.Parent != null)
            {
                Object anode = node.Tag;
                path = node.Parent.Tag.ToString() + "\\" + dirView.SelectedNode.Tag.ToString();
            }
            else
            {
                Object aNode = dirView.SelectedNode.Tag;
                path = aNode.ToString();
            }

            string path1 = Path.Combine(path, textBox1.Text);
            Directory.CreateDirectory(path1);
            dirView.Nodes.Clear();

            PopulateTreeView();
            dirView.EndUpdate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dirView.BeginUpdate();

            TreeNode node = dirView.SelectedNode;

            string path = null;

            if (node.Parent != null)
            {
                Object anode = node.Tag;
                path = node.Parent.Tag.ToString() + "\\" + dirView.SelectedNode.Tag.ToString();
            }
            else
            {
                Object aNode = dirView.SelectedNode.Tag;
                path = aNode.ToString();
            }

            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            Directory.Delete(path);

            dirView.Nodes.Clear();
            PopulateTreeView();
            //
            dirView.EndUpdate();
        }

        private static void MonitorDirectory(string path)
        {
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

            fileSystemWatcher.Path = path;

            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;

            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.EnableRaisingEvents = true;


        }

        private static void FileSystemWatcher_Renamed(object sender, FileSystemEventArgs e)
        {
            MessageBox.Show(text: $"File: {e.Name} renamed to {e.FullPath}");
            
        }  

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = dialog.SelectedPath;
                parentFolderPath = dialog.SelectedPath;
            }

            PopulateTreeView();
        }
    }
}
