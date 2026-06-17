using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Linq; 

namespace PingBox
{
    public partial class FormMain : Form
    {
        private const int DefaultWindowWidth = 960;
        private const int DefaultWindowHeight = 600;
        private const int MinWindowWidth = 600;
        private const int MinWindowHeight = 360;

        public List<string> backimages = new List<string>();
        public List<ListView> listViews = new List<ListView>();
        public List<Button> buttons = new List<Button>();
        public List<ImageList> largeImageLists = new List<ImageList>();
        public List<ImageList> mediumImageLists = new List<ImageList>();
        public List<ImageList> smallImageLists = new List<ImageList>();
        public TabControl tabControl;
        public bool noexit;
        private bool goexit = false;
        public bool hideStart;
        public bool hideRun;
        public bool noReadLnk;
        public bool dClick = true;
        public int tbwidth;
        public int tbheight;
        public int lineSpacing;
        public int columnSpacing;
        public readonly string apppath;
        public readonly string appname;
        public readonly string appdir;
        private readonly string cfgFile;
        private int width;
        private int height;
        private int locationx;
        private int locationy;
        //public Color colorB1 = Color.White;
        public Color colorF1 = Color.Black;
        public Color colorB2 = Color.LightBlue;
        public Color colorF2 = Color.Black;

        private ListViewItem dragMove;

        public bool hotkeyon;
        public string hotkey1;
        public string hotkey2;
        private bool useRelativePathStorage = true;

        private ToolStripTextBox searchTextBox;
        private ToolStripDropDownButton pathModeDropDownButton;
        private ToolStripMenuItem pathModeRelativeMenuItem;
        private ToolStripMenuItem pathModeAbsoluteMenuItem;

        private SplitContainer explorerMainSplit;
        private SplitContainer explorerRightSplit;
        private ListBox explorerPageListBox;
        private ListBox explorerTypeListBox;
        private ListView explorerListView;
        private ImageList explorerLargeImageList;
        private ImageList explorerMediumImageList;
        private ImageList explorerSmallImageList;
        private bool syncingExplorerSelection;
        private bool suppressExplorerFilterEvents;

        private sealed class ExplorerRef
        {
            public int PageIndex { get; set; }
            public ListViewItem SourceItem { get; set; }
        }

        public FormMain()
        {
            apppath = Application.ExecutablePath;
            appdir = Application.StartupPath.TrimEnd('\\');
            appname = System.IO.Path.GetFileNameWithoutExtension(apppath);
            cfgFile = appdir + "\\" + appname + ".xml";
            tbwidth = 55;
            tbheight = 23;
            lineSpacing = 75;
            columnSpacing = 75;
            InitializeComponent();
            InitializeSearchAndTypeFilterUI();
            // 从可执行目录加载图标，缺失时使用系统图标避免启动崩溃
            string startupIconPath = Path.Combine(appdir, "pingbox.ico");
            if (File.Exists(startupIconPath))
            {
                try
                {
                    this.Icon = new Icon(startupIconPath);
                }
                catch
                {
                    this.Icon = SystemIcons.Application;
                }
            }
            else
            {
                this.Icon = SystemIcons.Application;
            }
            Load += FormMain_Load;
            FormClosing += FormMain_FormClosing;
            MinimumSize = new Size(MinWindowWidth, MinWindowHeight);
            panel1.Dock = DockStyle.Fill; 
            panel1.MouseDown += FormMain_MouseDown;
            panelButton.MouseDown += FormMain_MouseDown;
            Resize += FormMain_Resize;
            LocationChanged += FormMain_LocationChanged;
            tabControl = new TabControl
            {
                Alignment = TabAlignment.Bottom,
                ContextMenuStrip = contextMenuStripMain,
                //tabControl.Dock = DockStyle.Fill;
                ItemSize = new Size(48, 16),
                //Location = new Point(0, 0),
                Margin = new Padding(0),
                Padding = new Point(0, 0)
            };
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            panel1.Controls.Add(tabControl);
            InitializeExplorerLayout();
            ReadCfg();
            notifyIcon.Text = Text;
            //if (tabControl.SelectedIndex > -1)
            //{
            //    buttons[tabControl.SelectedIndex].BackColor = Color.LightBlue;
            //}
            panelButton.Visible = false;
            panelButton.Height = 0;
            string icofile = appdir + "\\" + appname + ".ico";
            if (System.IO.File.Exists(icofile))
            {
                try
                {
                    notifyIcon.Icon = new Icon(icofile);
                }
                catch { }
            }
            Icon = notifyIcon.Icon;
            FitButton();
            // panel1 是你在设计器中拖放的Panel
            panel1.AllowDrop = true;
            panel1.DragEnter += Panel_DragEnter;
            panel1.DragDrop += Panel_DragDrop;
        }

        private void InitializeSearchAndTypeFilterUI()
        {
            searchTextBox = new ToolStripTextBox
            {
                Name = "toolStripSearchTextBox",
                AutoSize = false,
                Width = 260,
                Font = new Font("Microsoft YaHei UI", 10f, FontStyle.Regular),
                ToolTipText = "输入名称、路径或参数进行搜索"
            };
            searchTextBox.TextChanged += SearchOrTypeFilter_Changed;

            menuStrip1.Items.Add(new ToolStripSeparator());
            menuStrip1.Items.Add(new ToolStripLabel("搜索"));
            menuStrip1.Items.Add(searchTextBox);

            pathModeDropDownButton = new ToolStripDropDownButton
            {
                Text = "路径: 相对"
            };
            pathModeRelativeMenuItem = new ToolStripMenuItem("存储为相对路径")
            {
                CheckOnClick = true,
                Checked = true
            };
            pathModeAbsoluteMenuItem = new ToolStripMenuItem("存储为绝对路径")
            {
                CheckOnClick = true,
                Checked = false
            };
            pathModeRelativeMenuItem.Click += (s, e) => SetPathStorageMode(true);
            pathModeAbsoluteMenuItem.Click += (s, e) => SetPathStorageMode(false);
            pathModeDropDownButton.DropDownItems.Add(pathModeRelativeMenuItem);
            pathModeDropDownButton.DropDownItems.Add(pathModeAbsoluteMenuItem);
            menuStrip1.Items.Add(new ToolStripSeparator());
            menuStrip1.Items.Add(pathModeDropDownButton);
            UpdatePathModeUI();
        }

        private void SetPathStorageMode(bool useRelative)
        {
            if (useRelativePathStorage == useRelative)
            {
                UpdatePathModeUI();
                return;
            }

            useRelativePathStorage = useRelative;
            ConvertAllItemPathsForCurrentMode();
            UpdatePathModeUI();
            ApplySearchAndTypeFilter();
            WriteCfg();
        }

        private void UpdatePathModeUI()
        {
            if (pathModeDropDownButton == null)
            {
                return;
            }

            pathModeDropDownButton.Text = useRelativePathStorage ? "路径: 相对" : "路径: 绝对";
            if (pathModeRelativeMenuItem != null)
            {
                pathModeRelativeMenuItem.Checked = useRelativePathStorage;
            }
            if (pathModeAbsoluteMenuItem != null)
            {
                pathModeAbsoluteMenuItem.Checked = !useRelativePathStorage;
            }
        }

        private void InitializeExplorerLayout()
        {
            explorerMainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 180,
                SplitterWidth = 8,
                BackColor = Color.FromArgb(205, 228, 255)
            };
            explorerMainSplit.Paint += ExplorerSplit_Paint;

            explorerRightSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 110,
                SplitterWidth = 8,
                BackColor = Color.FromArgb(205, 228, 255)
            };
            explorerRightSplit.Paint += ExplorerSplit_Paint;

            explorerLargeImageList = new ImageList
            {
                ImageSize = new Size(48, 48),
                ColorDepth = ColorDepth.Depth32Bit
            };
            explorerMediumImageList = new ImageList
            {
                ImageSize = new Size(32, 32),
                ColorDepth = ColorDepth.Depth32Bit
            };
            explorerSmallImageList = new ImageList
            {
                ImageSize = new Size(16, 16),
                ColorDepth = ColorDepth.Depth32Bit
            };

            explorerPageListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black,
                IntegralHeight = false,
                Font = new Font("Microsoft YaHei UI", 11f, FontStyle.Bold)
            };
            explorerPageListBox.SelectedIndexChanged += ExplorerPageListBox_SelectedIndexChanged;

            explorerTypeListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black,
                IntegralHeight = false,
                Font = new Font("Microsoft YaHei UI", 9.5f, FontStyle.Regular)
            };
            explorerTypeListBox.SelectedIndexChanged += ExplorerTypeListBox_SelectedIndexChanged;

            explorerListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.LargeIcon,
                LargeImageList = explorerLargeImageList,
                SmallImageList = explorerSmallImageList,
                FullRowSelect = true,
                GridLines = true,
                HideSelection = false,
                MultiSelect = false
            };
            explorerListView.Columns.Add("名称", 180, HorizontalAlignment.Left);
            explorerListView.Columns.Add("类型", 90, HorizontalAlignment.Left);
            explorerListView.Columns.Add("路径", 360, HorizontalAlignment.Left);
            explorerListView.Columns.Add("参数", 120, HorizontalAlignment.Left);
            explorerListView.Columns.Add("页面", 110, HorizontalAlignment.Left);
            explorerListView.Columns.Add("管理员", 80, HorizontalAlignment.Left);
            explorerListView.SelectedIndexChanged += ExplorerListView_SelectedIndexChanged;
            explorerListView.DoubleClick += ExplorerListView_DoubleClick;

            explorerMainSplit.Panel1.Controls.Add(explorerPageListBox);
            explorerMainSplit.Panel2.Controls.Add(explorerRightSplit);
            explorerRightSplit.Panel1.Controls.Add(explorerTypeListBox);
            explorerRightSplit.Panel2.Controls.Add(explorerListView);

            panel1.Controls.Add(explorerMainSplit);
            panel1.BackColor = Color.FromArgb(198, 222, 255);
            tabControl.Visible = false;
            tabControl.SendToBack();
            ApplyExplorerSplitRatio();
        }

        private void ExplorerPageListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!suppressExplorerFilterEvents)
            {
                RefreshExplorerView();
            }
        }

        private void ExplorerTypeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!suppressExplorerFilterEvents)
            {
                RefreshExplorerView();
            }
        }

        private void ExplorerListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (syncingExplorerSelection)
            {
                return;
            }

            if (explorerListView.SelectedItems.Count == 0)
            {
                return;
            }

            if (!(explorerListView.SelectedItems[0].Tag is ExplorerRef xref) || xref.SourceItem == null)
            {
                return;
            }

            syncingExplorerSelection = true;
            try
            {
                if (xref.PageIndex >= 0 && xref.PageIndex < listViews.Count)
                {
                    tabControl.SelectedIndex = xref.PageIndex;
                    ListView sourceView = listViews[xref.PageIndex];
                    sourceView.SelectedItems.Clear();
                    xref.SourceItem.Selected = true;
                    xref.SourceItem.Focused = true;
                    xref.SourceItem.EnsureVisible();
                }
            }
            finally
            {
                syncingExplorerSelection = false;
            }
        }

        private void ExplorerListView_DoubleClick(object sender, EventArgs e)
        {
            RunSelectedExplorerItem();
        }

        private void RunSelectedExplorerItem()
        {
            if (explorerListView == null || explorerListView.SelectedItems.Count == 0)
            {
                return;
            }

            if (!(explorerListView.SelectedItems[0].Tag is ExplorerRef xref) || xref.SourceItem == null)
            {
                return;
            }

            string rawPath = xref.SourceItem.SubItems[1].Text;
            string arg = xref.SourceItem.SubItems[2].Text;
            bool runas = xref.SourceItem.SubItems[3].Text.ToLower() == "true";

            string path = rawPath;
            try
            {
                if (!Path.IsPathRooted(path))
                {
                    path = Path.GetFullPath(Path.Combine(appdir, path));
                }
            }
            catch
            {
                path = rawPath;
            }

            if (!File.Exists(path) && !Directory.Exists(path))
            {
                MessageBox.Show($"目标不存在:\n{path}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                RunIcon(path, arg, runas);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"无法运行项目:\n{exception.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (hideRun == !(ModifierKeys == Keys.Control))
            {
                Hide();
            }
        }

        private void RefreshExplorerView()
        {
            if (explorerPageListBox == null || explorerTypeListBox == null || explorerListView == null)
            {
                return;
            }

            if (panelButton != null)
            {
                panelButton.Visible = false;
                panelButton.Height = 0;
            }

            string selectedPage = explorerPageListBox.SelectedItem as string ?? "全部页面";
            string selectedType = explorerTypeListBox.SelectedItem as string ?? "全部类型";

            suppressExplorerFilterEvents = true;
            try
            {
                explorerPageListBox.BeginUpdate();
                explorerPageListBox.Items.Clear();
                explorerPageListBox.Items.Add("全部页面");
                for (int i = 0; i < tabControl.TabPages.Count; i++)
                {
                    explorerPageListBox.Items.Add(tabControl.TabPages[i].Text);
                }
                explorerPageListBox.EndUpdate();
                if (!explorerPageListBox.Items.Contains(selectedPage))
                {
                    selectedPage = "全部页面";
                }
                explorerPageListBox.SelectedItem = selectedPage;

                if (explorerTypeListBox.Items.Count == 0)
                {
                    explorerTypeListBox.Items.AddRange(new object[]
                    {
                        "全部类型", "应用程序", "快捷方式", "文件夹", "文档", "图片", "音频", "视频", "压缩包", "脚本", "其他"
                    });
                }
                if (!explorerTypeListBox.Items.Contains(selectedType))
                {
                    selectedType = "全部类型";
                }
                explorerTypeListBox.SelectedItem = selectedType;
            }
            finally
            {
                suppressExplorerFilterEvents = false;
            }

            string keyword = (searchTextBox?.Text ?? string.Empty).Trim().ToLowerInvariant();

            explorerListView.BeginUpdate();
            explorerListView.Items.Clear();
            explorerLargeImageList.Images.Clear();
            explorerMediumImageList.Images.Clear();
            explorerSmallImageList.Images.Clear();
            foreach (int pageIndex in Enumerable.Range(0, listViews.Count))
            {
                ListView lv = listViews[pageIndex];
                string pageName = pageIndex < tabControl.TabPages.Count ? tabControl.TabPages[pageIndex].Text : "-";
                foreach (ListViewItem src in lv.Items)
                {
                    EnsureTypeSubItem(src);
                    string type = src.SubItems[4].Text;
                    bool keywordMatched = string.IsNullOrEmpty(keyword)
                        || src.Text.ToLowerInvariant().Contains(keyword)
                        || src.SubItems[1].Text.ToLowerInvariant().Contains(keyword)
                        || src.SubItems[2].Text.ToLowerInvariant().Contains(keyword)
                        || type.ToLowerInvariant().Contains(keyword);
                    bool pageMatched = selectedPage == "全部页面" || pageName == selectedPage;
                    bool typeMatched = selectedType == "全部类型" || type == selectedType;

                    if (!keywordMatched || !pageMatched || !typeMatched)
                    {
                        continue;
                    }

                    ListViewItem row = new ListViewItem(src.Text);
                    row.SubItems.Add(type);
                    row.SubItems.Add(src.SubItems[1].Text);
                    row.SubItems.Add(src.SubItems[2].Text);
                    row.SubItems.Add(pageName);
                    row.SubItems.Add(src.SubItems[3].Text.ToLower() == "true" ? "是" : "否");

                    int iconIndex = -1;
                    if (src.ImageIndex >= 0 && src.ImageIndex < largeImageLists[pageIndex].Images.Count)
                    {
                        explorerLargeImageList.Images.Add((Image)largeImageLists[pageIndex].Images[src.ImageIndex].Clone());
                        if (src.ImageIndex < mediumImageLists[pageIndex].Images.Count)
                        {
                            explorerMediumImageList.Images.Add((Image)mediumImageLists[pageIndex].Images[src.ImageIndex].Clone());
                        }
                        else
                        {
                            explorerMediumImageList.Images.Add((Image)FilesystemIcons.ICON_FILE_32x.ToBitmap().Clone());
                        }
                        if (src.ImageIndex < smallImageLists[pageIndex].Images.Count)
                        {
                            explorerSmallImageList.Images.Add((Image)smallImageLists[pageIndex].Images[src.ImageIndex].Clone());
                        }
                        else
                        {
                            explorerSmallImageList.Images.Add((Image)FilesystemIcons.ICON_FILE_16x.ToBitmap().Clone());
                        }
                        iconIndex = explorerLargeImageList.Images.Count - 1;
                    }
                    if (iconIndex >= 0)
                    {
                        row.ImageIndex = iconIndex;
                    }

                    row.Tag = new ExplorerRef { PageIndex = pageIndex, SourceItem = src };
                    explorerListView.Items.Add(row);
                }
            }
            explorerListView.EndUpdate();

            if (explorerListView.Items.Count > 0)
            {
                explorerListView.Items[0].Selected = true;
            }

            ApplyExplorerSplitRatio();
        }

        private void ApplyExplorerSplitRatio()
        {
            if (explorerMainSplit == null || explorerRightSplit == null || panel1 == null)
            {
                return;
            }

            int totalWidth = Math.Max(1, panel1.ClientSize.Width);
            int leftWidth = Math.Max(96, (int)(totalWidth * 0.06));
            int middleWidth = Math.Max(110, (int)(totalWidth * 0.06));

            if (leftWidth + middleWidth > totalWidth - 120)
            {
                leftWidth = Math.Max(90, (int)(totalWidth * 0.06));
                middleWidth = Math.Max(90, (int)(totalWidth * 0.05));
            }

            explorerMainSplit.SplitterDistance = leftWidth;
            explorerRightSplit.SplitterDistance = middleWidth;
        }

        private void ExplorerSplit_Paint(object sender, PaintEventArgs e)
        {
            SplitContainer split = sender as SplitContainer;
            if (split == null)
            {
                return;
            }

            Rectangle splitter = split.SplitterRectangle;
            using (SolidBrush splitterBrush = new SolidBrush(Color.FromArgb(184, 211, 246)))
            {
                e.Graphics.FillRectangle(splitterBrush, splitter);
            }

            int centerX = splitter.Left + splitter.Width / 2;
            int dotStartY = splitter.Top + (splitter.Height - 20) / 2;
            using (SolidBrush dotBrush = new SolidBrush(Color.FromArgb(104, 140, 194)))
            {
                for (int i = 0; i < 5; i++)
                {
                    e.Graphics.FillEllipse(dotBrush, centerX - 2, dotStartY + i * 5, 4, 4);
                }
            }
        }

        private void SetExplorerViewMode(View viewMode, bool useMediumIcon = false)
        {
            if (explorerListView == null)
            {
                return;
            }

            if (viewMode == View.LargeIcon)
            {
                explorerListView.LargeImageList = useMediumIcon ? explorerMediumImageList : explorerLargeImageList;
            }
            explorerListView.View = viewMode;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        #region Event
        private void FormMain_Load(object sender, EventArgs e)
        {
            if (hideStart)
            {
                WindowState = FormWindowState.Minimized;
                BeginInvoke(new System.Threading.ThreadStart(Hide));
            }
            BeginInvoke(new Action(() =>
            {
                RefreshExplorerView();
                ApplyExplorerSplitRatio();
            }));
            if (lineSpacing != 75 || columnSpacing != 75)
                SetIcoSpacing();
        }



        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                panel1.BackColor = Color.LightGreen; // 视觉反馈
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Panel_DragDrop(object sender, DragEventArgs e)
        {
            // 恢复背景色
            panel1.BackColor = Color.FromArgb(198, 222, 255);
            
                
         string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null && files.Length > 0)
            {
                foreach (string filePath in files)
                {
                    string directory = Path.GetDirectoryName(filePath);
                    string fileName = Path.GetFileName(filePath);
                    FileInfo fileInfo = new FileInfo(filePath);
                    
                    AddFiles(new string[] { filePath });
                    //WriteCfg();
                }

                ApplySearchAndTypeFilter();

            }

        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplySearchAndTypeFilter();
        }

        private void SearchOrTypeFilter_Changed(object sender, EventArgs e)
        {
            ApplySearchAndTypeFilter();
        }

        private void ApplySearchAndTypeFilter()
        {
            RefreshExplorerView();
        }

        private void EnsureTypeSubItem(ListViewItem item)
        {
            while (item.SubItems.Count <= 4)
            {
                item.SubItems.Add(string.Empty);
            }

            item.SubItems[4].Text = DetectFileCategory(item.SubItems[1].Text);
        }

        private string DetectFileCategory(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return "其他";
            }

            try
            {
                if (Directory.Exists(fullPath))
                {
                    return "文件夹";
                }

                string ext = Path.GetExtension(fullPath).ToLowerInvariant();
                if (ext == ".lnk") return "快捷方式";
                if (ext == ".exe" || ext == ".bat" || ext == ".cmd" || ext == ".msi") return "应用程序";
                if (ext == ".txt" || ext == ".doc" || ext == ".docx" || ext == ".pdf" || ext == ".xls" || ext == ".xlsx" || ext == ".ppt" || ext == ".pptx") return "文档";
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".bmp" || ext == ".webp" || ext == ".ico") return "图片";
                if (ext == ".mp3" || ext == ".wav" || ext == ".flac" || ext == ".aac" || ext == ".ogg") return "音频";
                if (ext == ".mp4" || ext == ".mkv" || ext == ".avi" || ext == ".mov" || ext == ".wmv") return "视频";
                if (ext == ".zip" || ext == ".rar" || ext == ".7z" || ext == ".tar" || ext == ".gz") return "压缩包";
                if (ext == ".ps1" || ext == ".vbs" || ext == ".js" || ext == ".py" || ext == ".sh") return "脚本";

                return "其他";
            }
            catch
            {
                return "其他";
            }
        }
            


        public void SetIcoSpacing()
        {
            foreach (ListView listView in listViews)
            {
                SendMessage(listView.Handle, 0x1035, 0, 0x10000 * columnSpacing + lineSpacing);
            }
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            tabControl.Width = panel1.Width + 8;
            tabControl.Height = panel1.Height + 8 + 16;
            tabControl.Location = new Point(-4, -4);
            ApplyExplorerSplitRatio();
            if (panelButton != null)
            {
                panelButton.Visible = false;
                panelButton.Height = 0;
            }
            if (WindowState == FormWindowState.Normal)
            {
                width = Width;
                height = Height;
            }
            else if (!ShowInTaskbar && WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void FormMain_LocationChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                locationx = Location.X;
                locationy = Location.Y;
            }
        }

        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x0112, 0xF010 + 0x0002, 0);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!noexit || goexit)
            {
                WriteCfg();
            }
            else
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void ListViews_MouseClick(object sender, MouseEventArgs e)
        {
            if (!dClick && e.Button == MouseButtons.Left && 
                tabControl.SelectedIndex >= 0 && 
                tabControl.SelectedIndex < listViews.Count &&
                listViews[tabControl.SelectedIndex].SelectedItems.Count == 1)
            {
                RunItem();
            }
        }

        private void RunItem()
        {
            if (explorerListView != null && explorerListView.Visible && explorerListView.SelectedItems.Count > 0)
            {
                RunSelectedExplorerItem();
                return;
            }

            // 安全检查
            if (tabControl.SelectedIndex < 0 || tabControl.SelectedIndex >= listViews.Count)
            {
                return;
            }
            
            ListView currentListView = listViews[tabControl.SelectedIndex];
            if (currentListView.SelectedItems.Count == 0)
            {
                return;
            }
            
            string relativePath = currentListView.SelectedItems[0].SubItems[1].Text;
            string arg = currentListView.SelectedItems[0].SubItems[2].Text;
            bool runas = currentListView.SelectedItems[0].SubItems[3].Text.ToLower() == "true";
            
            try
            {
                string path = Path.GetFullPath(relativePath);
                RunIcon(path, arg, runas);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"无法运行项目:\n{exception.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (hideRun == !(ModifierKeys == Keys.Control))
            {
                Hide();
            }
        }

        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            if (dClick)
            {
                RunItem();
            }
        }

        private void ListView_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = ((ListView)sender).PointToClient(MousePosition);
            ListViewItem toitem = ((ListView)sender).GetItemAt(p.X, p.Y);
            if (toitem == null)
            {
                ReleaseCapture();
                SendMessage(Handle, 0x0112, 0xF010 + 0x0002, 0);
            }
        }

        private void ListView_ItemDrag(object sender, ItemDragEventArgs e) => ((ListView)sender).DoDragDrop(e.Item, DragDropEffects.Move);

        private void ListView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            Point p = ((ListView)sender).PointToClient(MousePosition);
            ListViewItem item = ((ListView)sender).GetItemAt(p.X, p.Y);
            if (item != null && item.Selected == false)
            {
                dragMove = item;
            }
        }

        private void ListViews_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (s != null && s.Length > 0)
            {
                AddFiles(s);
            }
            else
            {
                Point p = ((ListView)sender).PointToClient(MousePosition);
                ListViewItem toitem = ((ListView)sender).GetItemAt(p.X, p.Y);
                ListViewItem fritem = ((ListView)sender).SelectedItems[0];
                if (dragMove != null && toitem != null && fritem != toitem)
                {
                    ((ListView)sender).BeginUpdate();
                    if (fritem.Index > toitem.Index)
                    {
                        ((ListView)sender).Items.RemoveAt(fritem.Index);
                        ((ListView)sender).Items.Insert(toitem.Index, fritem);
                    }
                    else if (fritem.Index < toitem.Index)
                    {
                        ((ListView)sender).Items.RemoveAt(fritem.Index);
                        ((ListView)sender).Items.Insert(toitem.Index + 1, fritem);
                    }
                    View view = ((ListView)sender).View;
                    if (view != View.Details && view != View.List)
                    {
                        ((ListView)sender).View = View.Details;
                        ((ListView)sender).View = view;
                    }
                ((ListView)sender).EndUpdate();
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            int j = tabControl.SelectedIndex;
            int bi = buttons.IndexOf((Button)sender);
            if (bi != j)
            {
                tabControl.SelectedIndex = bi;
                foreach (Button item in buttons)
                {
                    if (item == sender)
                    {
                        //item.BackColor = Color.LightBlue;
                        item.ForeColor = colorF2;
                        item.BackColor = colorB2;// Color.LightBlue;
                    }
                    else
                    {
                        item.BackColor = Color.Transparent;// Color.White;
                        item.ForeColor = colorF1;
                    }
                    item.FlatAppearance.BorderColor = colorB2;
                }
            }
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
                if (WindowState == FormWindowState.Minimized)
                {
                    WindowState = FormWindowState.Normal;
                }
                Activate();
            }
        }
        #endregion

        #region CFG
        /// <summary>
        /// 获取相对路径（兼容所有 .NET 版本）
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        /// <param name="basePath">基准路径</param>
        /// <returns>相对路径</returns>
        private string GetRelativePath(string basePath, string fullPath)
        {
            try
            {
                // 处理空值情况
                if (string.IsNullOrEmpty(fullPath) || string.IsNullOrEmpty(basePath))
                    return fullPath;

                // 如果已经是相对路径，直接返回
                if (!System.IO.Path.IsPathRooted(fullPath))
                    return fullPath;

                // 确保基准路径以目录分隔符结尾
                if (!basePath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                {
                    basePath += System.IO.Path.DirectorySeparatorChar;
                }

                // 检查是否在同一驱动器
                string fullRoot = System.IO.Path.GetPathRoot(fullPath);
                string baseRoot = System.IO.Path.GetPathRoot(basePath);
                
                if (!string.Equals(fullRoot, baseRoot, StringComparison.OrdinalIgnoreCase))
                    return fullPath; // 不同驱动器，返回绝对路径

                // 网络路径（UNC），返回绝对路径
                if (fullPath.StartsWith(@"\\"))
                    return fullPath;

                // 使用 Uri 方法计算相对路径
                Uri fullUri = new Uri(fullPath);
                Uri baseUri = new Uri(basePath);

                string relativePath = Uri.UnescapeDataString(
                    baseUri.MakeRelativeUri(fullUri).ToString()
                        .Replace('/', System.IO.Path.DirectorySeparatorChar)
                );

                // 如果相对路径为空，说明是同一目录
                if (string.IsNullOrEmpty(relativePath))
                    return "." + System.IO.Path.DirectorySeparatorChar;

                return relativePath;
            }
            catch (Exception ex)
            {
                // 转换失败，返回原路径
                System.Diagnostics.Debug.WriteLine($"GetRelativePath failed: {ex.Message}");
                return fullPath;
            }
        }

        private string BuildStoredPath(string path)
        {
            return ConvertPathForStorageMode(path, useRelativePathStorage);
        }

        private string ConvertPathForStorageMode(string path, bool useRelative)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            try
            {
                if (!Path.IsPathRooted(path))
                {
                    if (useRelative)
                    {
                        return path;
                    }

                    return Path.GetFullPath(Path.Combine(appdir, path));
                }

                if (!useRelative)
                {
                    return path;
                }

                if (path.StartsWith(@"\\"))
                {
                    return path;
                }

                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (!string.Equals(Path.GetPathRoot(path), Path.GetPathRoot(appDirectory), StringComparison.OrdinalIgnoreCase))
                {
                    return path;
                }

                string relativePath = GetRelativePath(appDirectory, path);
                if (string.IsNullOrWhiteSpace(relativePath) || relativePath == path)
                {
                    return path;
                }

                return relativePath;
            }
            catch
            {
                return path;
            }
        }

        private void ConvertAllItemPathsForCurrentMode()
        {
            foreach (ListView listView in listViews)
            {
                foreach (ListViewItem item in listView.Items)
                {
                    if (item.SubItems.Count < 2)
                    {
                        continue;
                    }

                    string convertedPath = ConvertPathForStorageMode(item.SubItems[1].Text, useRelativePathStorage);
                    item.SubItems[1].Text = convertedPath;

                    string tip = $"{item.Text}\n链接：{convertedPath}";
                    if (item.SubItems.Count > 2 && !string.IsNullOrEmpty(item.SubItems[2].Text))
                    {
                        tip += $"\n参数：{item.SubItems[2].Text}";
                    }
                    item.ToolTipText = tip;
                }
            }
        }

        private void WriteCfg()
        {
            // 备份当前配置文件（如果存在）
            if (File.Exists(cfgFile))
            {
                try
                {
                    string backupFile = GetBackupFileName();
                    File.Copy(cfgFile, backupFile, true);
                    CleanOldBackups(); // 清理旧的备份文件
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"备份配置文件失败: {ex.Message}");
                }
            }
             // 先保存到临时文件，成功后再替换原文件
            string tempFile = cfgFile + ".tmp";
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(dec);
                XmlElement cfg = doc.CreateElement("Config");
                doc.AppendChild(cfg);
                XmlElement set = doc.CreateElement("Setting");
                cfg.AppendChild(set);
                XmlElement title = doc.CreateElement("Title");
                title.InnerText = Text;
                set.AppendChild(title);
                XmlElement h = doc.CreateElement("Height");
                h.InnerText = height.ToString();
                set.AppendChild(h);
                XmlElement w = doc.CreateElement("Width");
                w.InnerText = width.ToString();
                set.AppendChild(w);
                XmlElement lx = doc.CreateElement("LocationX");
                lx.InnerText = locationx.ToString();
                set.AppendChild(lx);
                XmlElement ly = doc.CreateElement("LocationY");
                ly.InnerText = locationy.ToString();
                set.AppendChild(ly);
                XmlElement statusBar = doc.CreateElement("StatusBar");
                statusBar.InnerText = ShowInTaskbar.ToString();
                set.AppendChild(statusBar);
                XmlElement showicon = doc.CreateElement("WindowIcon");
                showicon.InnerText = ShowIcon.ToString();
                set.AppendChild(showicon);
                XmlElement hide = doc.CreateElement("HideStart");
                hide.InnerText = hideStart.ToString();
                set.AppendChild(hide);
                XmlElement hrun = doc.CreateElement("HideRun");
                hrun.InnerText = hideRun.ToString();
                set.AppendChild(hrun);
                XmlElement topmost = doc.CreateElement("TopMost");
                topmost.InnerText = TopMost.ToString();
                set.AppendChild(topmost);
                XmlElement noex = doc.CreateElement("NotExit");
                noex.InnerText = noexit.ToString();
                set.AppendChild(noex);
                XmlElement nordlnk = doc.CreateElement("NoReadLnk");
                nordlnk.InnerText = noReadLnk.ToString();
                set.AppendChild(nordlnk);
                XmlElement dcl = doc.CreateElement("DoubleClickRun");
                dcl.InnerText = dClick.ToString();
                set.AppendChild(dcl);
                XmlElement lspa = doc.CreateElement("LineSpacing");
                lspa.InnerText = lineSpacing.ToString();
                set.AppendChild(lspa);
                XmlElement cspa = doc.CreateElement("ColumnSpacing");
                cspa.InnerText = columnSpacing.ToString();
                set.AppendChild(cspa);
                XmlElement tbloc = doc.CreateElement("LabelLocation");
                tbloc.InnerText = ((int)panelButton.Dock).ToString();
                set.AppendChild(tbloc);
                XmlElement tbbak1 = doc.CreateElement("LabelBackColor1");
                tbbak1.InnerText = panelButton.BackColor.ToArgb().ToString();
                set.AppendChild(tbbak1);
                XmlElement tbbak2 = doc.CreateElement("LabelBackColor2");
                tbbak2.InnerText = colorB2.ToArgb().ToString();
                set.AppendChild(tbbak2);
                XmlElement tbfore1 = doc.CreateElement("LabelForeColor1");
                tbfore1.InnerText = colorF1.ToArgb().ToString();
                set.AppendChild(tbfore1);
                XmlElement tbfore2 = doc.CreateElement("LabelForeColor2");
                tbfore2.InnerText = colorF2.ToArgb().ToString();
                set.AppendChild(tbfore2);
                XmlElement tbindex = doc.CreateElement("PageIndex");
                tbindex.InnerText = tabControl.SelectedIndex.ToString();
                set.AppendChild(tbindex);
                XmlElement tbw = doc.CreateElement("LabelWidth");
                tbw.InnerText = tbwidth.ToString();
                set.AppendChild(tbw);
                XmlElement tbh = doc.CreateElement("LabelHeight");
                tbh.InnerText = tbheight.ToString();
                set.AppendChild(tbh);

                XmlElement hotkon = doc.CreateElement("HotKeyOn");
                hotkon.InnerText = hotkeyon.ToString();
                set.AppendChild(hotkon);
                XmlElement hotk1 = doc.CreateElement("HotKey1");
                hotk1.InnerText = hotkey1;
                set.AppendChild(hotk1);
                XmlElement hotk2 = doc.CreateElement("HotKey2");
                hotk2.InnerText = hotkey2;
                set.AppendChild(hotk2);

                XmlElement pathMode = doc.CreateElement("PathStorageMode");
                pathMode.InnerText = useRelativePathStorage ? "Relative" : "Absolute";
                set.AppendChild(pathMode);

                XmlElement datas = doc.CreateElement("Pages");
                cfg.AppendChild(datas);
                for (int i = 0; i < listViews.Count; i++)
                {
                    XmlElement tbnane = doc.CreateElement("Name");
                    tbnane.InnerText = buttons[i].Text;
                    XmlElement bk = doc.CreateElement("BackImage");
                    bk.SetAttribute("On", (listViews[i].BackgroundImage != null).ToString());
                    bk.SetAttribute("Tiled", listViews[i].BackgroundImageTiled.ToString());
                    bk.InnerText = backimages[i];
                    XmlElement fcolor = doc.CreateElement("ListForeColor");
                    fcolor.InnerText = listViews[i].ForeColor.ToArgb().ToString();
                    XmlElement bcolor = doc.CreateElement("ListBackColor");
                    bcolor.InnerText = listViews[i].BackColor.ToArgb().ToString();
                    XmlElement tab = doc.CreateElement("Page");
                    tab.AppendChild(tbnane);
                    tab.AppendChild(bk);
                    tab.AppendChild(fcolor);
                    tab.AppendChild(bcolor);
                    foreach (ListViewItem item in listViews[i].Items)
                    {
                        XmlElement data = doc.CreateElement("Data");
                        XmlElement name = doc.CreateElement("Name");
                        name.InnerText = item.Text;
                        XmlElement fullpath = doc.CreateElement("FullPath");
                        string path = item.SubItems[1].Text;
                        fullpath.InnerText = BuildStoredPath(path);

                        XmlElement arg = doc.CreateElement("Args");
                        arg.InnerText = item.SubItems[2].Text;
                        XmlElement runas = doc.CreateElement("RunAs");
                        runas.InnerText = item.SubItems[3].Text;
                        data.AppendChild(name);
                        data.AppendChild(fullpath);
                        data.AppendChild(arg);
                        data.AppendChild(runas);
                        tab.AppendChild(data);
                    }
                    datas.AppendChild(tab);
                }
                    // 先保存到临时文件
                doc.Save(tempFile);
                
                // 检查临时文件是否成功创建
                if (File.Exists(tempFile) && new FileInfo(tempFile).Length > 0)
                {
                    // 备份原文件
                    if (File.Exists(cfgFile))
                    {
                        string backupFile = cfgFile + ".bak";
                        File.Copy(cfgFile, backupFile, true);
                    }
                    
                    // 用临时文件替换原文件
                    File.Copy(tempFile, cfgFile, true);
                }
            }
            catch (Exception ex)
            {
                // 记录错误但不抛出，避免程序崩溃
                System.Diagnostics.Debug.WriteLine($"保存配置失败: {ex.Message}");
                
            }
            finally
            {
                // 清理临时文件
                if (File.Exists(tempFile))
                {
                    try { File.Delete(tempFile); } catch { }
                }
            }
        }


        /// <summary>
        /// 获取备份文件名（带时间戳）
        /// </summary>
        private string GetBackupFileName()
        {
            string backupDir = Path.Combine(appdir, "Backups");
            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }
            
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return Path.Combine(backupDir, $"{appname}_backup_{timestamp}.xml");
        }

        /// <summary>
        /// 清理旧的备份文件（只保留最近5个）
        /// </summary>
        private void CleanOldBackups()
        {
            try
            {
                string backupDir = Path.Combine(appdir, "Backups");
                if (!Directory.Exists(backupDir)) return;
                
                var backupFiles = Directory.GetFiles(backupDir, $"{appname}_backup_*.xml")
                                        .Select(f => new FileInfo(f))
                                        .OrderByDescending(f => f.CreationTime)
                                        .ToList();
                
                // 保留最近5个备份，删除更早的
                if (backupFiles.Count > 5)
                {
                    foreach (var oldFile in backupFiles.Skip(5))
                    {
                        try
                        {
                            oldFile.Delete();
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"清理备份文件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 从备份恢复配置
        /// </summary>
        private void RestoreFromBackup()
        {
            try
            {
                string backupDir = Path.Combine(appdir, "Backups");
                if (!Directory.Exists(backupDir)) return;
                
                // 找到最新的备份文件
                var backupFiles = Directory.GetFiles(backupDir, $"{appname}_backup_*.xml")
                                        .Select(f => new FileInfo(f))
                                        .OrderByDescending(f => f.CreationTime)
                                        .FirstOrDefault();
                
                if (backupFiles != null && backupFiles.Exists && backupFiles.Length > 100)
                {
                    File.Copy(backupFiles.FullName, cfgFile, true);
                    System.Diagnostics.Debug.WriteLine($"从备份恢复配置: {backupFiles.Name}");
                }
                else
                {
                    // 如果没有有效备份，创建基础配置
                    LoadDefault();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"从备份恢复失败: {ex.Message}");
                LoadDefault();
            }
        }

        private void ReadCfg()
        {
            if (!System.IO.File.Exists(cfgFile))
            {
                LoadDefault();
                return;
            }
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(cfgFile);
                Text = doc.SelectSingleNode("Config/Setting/Title").InnerText;
                Height = Math.Max(MinWindowHeight, int.Parse(doc.SelectSingleNode("Config/Setting/Height").InnerText));
                Width = Math.Max(MinWindowWidth, int.Parse(doc.SelectSingleNode("Config/Setting/Width").InnerText));
                int lx = int.Parse(doc.SelectSingleNode("Config/Setting/LocationX").InnerText);
                ShowInTaskbar = bool.Parse(doc.SelectSingleNode("Config/Setting/StatusBar").InnerText);
                ShowIcon = true;//bool.Parse(doc.SelectSingleNode("Config/Setting/WindowIcon").InnerText);
                int ly = int.Parse(doc.SelectSingleNode("Config/Setting/LocationY").InnerText);
                hideStart = bool.Parse(doc.SelectSingleNode("Config/Setting/HideStart").InnerText);
                hideRun = bool.Parse(doc.SelectSingleNode("Config/Setting/HideRun").InnerText);
                TopMost = bool.Parse(doc.SelectSingleNode("Config/Setting/TopMost").InnerText);
                noexit = bool.Parse(doc.SelectSingleNode("Config/Setting/NotExit").InnerText);
                noReadLnk = bool.Parse(doc.SelectSingleNode("Config/Setting/NoReadLnk").InnerText);
                dClick = bool.Parse(doc.SelectSingleNode("Config/Setting/DoubleClickRun").InnerText);
                lineSpacing = int.Parse(doc.SelectSingleNode("Config/Setting/LineSpacing").InnerText);
                columnSpacing = int.Parse(doc.SelectSingleNode("Config/Setting/ColumnSpacing").InnerText);
                tbwidth = int.Parse(doc.SelectSingleNode("Config/Setting/LabelWidth").InnerText);
                tbheight = int.Parse(doc.SelectSingleNode("Config/Setting/LabelHeight").InnerText);
                if (lx + Width <= 0)
                {
                    lx = 0;
                }
                if (lx >= Screen.PrimaryScreen.Bounds.Width)
                {
                    lx = Screen.PrimaryScreen.Bounds.Width - Width;
                }
                if (ly + Height <= 0)
                {
                    ly = 0;
                }
                if (ly >= Screen.PrimaryScreen.Bounds.Height)
                {
                    ly = Screen.PrimaryScreen.Bounds.Height - Height;
                }
                Location = new Point(lx, ly);
                panelButton.Dock = (DockStyle)int.Parse(doc.SelectSingleNode("Config/Setting/LabelLocation").InnerText);
                panelButton.BackColor = Color.FromArgb(int.Parse(doc.SelectSingleNode("Config/Setting/LabelBackColor1").InnerText));
                colorB2 = Color.FromArgb(int.Parse(doc.SelectSingleNode("Config/Setting/LabelBackColor2").InnerText));
                colorF1 = Color.FromArgb(int.Parse(doc.SelectSingleNode("Config/Setting/LabelForeColor1").InnerText));
                colorF2 = Color.FromArgb(int.Parse(doc.SelectSingleNode("Config/Setting/LabelForeColor2").InnerText));
                try
                {
                    hotkeyon = bool.Parse(doc.SelectSingleNode("Config/Setting/HotKeyOn").InnerText);
                    hotkey1 = doc.SelectSingleNode("Config/Setting/HotKey1").InnerText;
                    hotkey2 = doc.SelectSingleNode("Config/Setting/HotKey2").InnerText;
                }
                catch (Exception)
                {
                    DefaultHotKey();
                }

                try
                {
                    string pathModeText = doc.SelectSingleNode("Config/Setting/PathStorageMode")?.InnerText;
                    useRelativePathStorage = !string.Equals(pathModeText, "Absolute", StringComparison.OrdinalIgnoreCase);
                }
                catch
                {
                    useRelativePathStorage = true;
                }
                UpdatePathModeUI();
                using (XmlNodeList tabs = doc.SelectNodes("Config/Pages/Page"))
                {
                    for (int i = 0; i < tabs.Count; i++)
                    {
                        AddPage(tabs[i].SelectSingleNode("Name").InnerText);
                        backimages[i] = tabs[i].SelectSingleNode("BackImage").InnerText;
                        if (bool.Parse(tabs[i].SelectSingleNode("BackImage").Attributes["On"].Value) && System.IO.File.Exists(backimages[i]))
                        {
                            try
                            {
                                // 使用副本避免文件锁定
                                using (Image tempImage = Image.FromFile(backimages[i]))
                                {
                                    listViews[i].BackgroundImage = new Bitmap(tempImage);
                                }
                            }
                            catch (Exception)
                            { }
                        }
                        listViews[i].BackgroundImageTiled = bool.Parse(tabs[i].SelectSingleNode("BackImage").Attributes["Tiled"].Value);
                        listViews[i].ForeColor = Color.FromArgb(int.Parse(tabs[i].SelectSingleNode("ListForeColor").InnerText));
                        listViews[i].BackColor = Color.FromArgb(int.Parse(tabs[i].SelectSingleNode("ListBackColor").InnerText));
                        XmlNodeList items = tabs[i].SelectNodes("Data");
                        foreach (XmlNode item in items)
                        {
                            IcoFileInfo icoFileInfo = new IcoFileInfo(item.SelectSingleNode("FullPath").InnerText)
                            {
                                Name = item.SelectSingleNode("Name").InnerText,
                                Args = item.SelectSingleNode("Args").InnerText,
                                RunAsA = item.SelectSingleNode("RunAs").InnerText.ToLower() == "true"
                            };
                            AddFile(icoFileInfo, i);
                        }
                    }
                }
                // 安全设置选中的页面索引，防止越界
                int pageIndex = int.Parse(doc.SelectSingleNode("Config/Setting/PageIndex").InnerText);
                if (pageIndex >= 0 && pageIndex < tabControl.TabPages.Count)
                {
                    tabControl.SelectedIndex = pageIndex;
                }
                else if (tabControl.TabPages.Count > 0)
                {
                    tabControl.SelectedIndex = 0;
                }
            }
            catch (Exception)
            {
                RestoreFromBackup(); // 从备份恢复
                //LoadDefault();
            }
            if (hotkeyon)
            {
                OnHotKey();
            }
        }

        private void OnHotKey()
        {
            int cobk;
            switch (hotkey1)
            {
                case "Ctr":
                    cobk = 2;
                    break;
                case "Shift":
                    cobk = 4;
                    break;
                case "Alt":
                    cobk = 1;
                    break;
                case "Ctr+Shift":
                    cobk = 6;
                    break;
                case "Ctr+Alt":
                    cobk = 3;
                    break;
                case "Alt+Shift":
                    cobk = 5;
                    break;
                case "Ctr+Shift+Alt":
                    cobk = 7;
                    break;
                default:
                    cobk = 0;
                    break;
            }

            Keys hotKey = Event.GetKeys(hotkey2);
            if (!Event.RegisterHotKey(this.Handle, 1, (uint)cobk, hotKey))
            {
                MessageBox.Show("热键配置失败！可能该热键被其他应用所占用，已为您关闭热键功能。");
                hotkeyon = false;
                return;
            }
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            if (m.Msg == WM_HOTKEY)
            {
                if (m.WParam.ToString().Equals("1"))
                {
                    if (ActiveControl.Focused)
                    {
                        Hide();
                    }
                    else
                    {
                        Show();
                        if (WindowState == FormWindowState.Minimized)
                        {
                            WindowState = FormWindowState.Normal;
                        }
                        Activate();
                    }
                }
            }
            base.WndProc(ref m);
        }

        private void DefaultHotKey()
        {
            hotkeyon = false;
            hotkey1 = "Ctr";
            hotkey2 = "F2 键";
        }
        private void LoadDefault()
        {
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(DefaultWindowWidth, DefaultWindowHeight);
            Text = appname;
            notifyIcon.Text = appname;
            DefaultHotKey();
        }
        #endregion

        #region Method
        private void RunIcon(string path, string arg, bool runas)
        {
            string dir = System.IO.Path.GetDirectoryName(path);
            if (path.Contains(" "))
            {
                path = "\"" + path + "\"";
            }
            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
            {
                if (runas)
                {
                    p.StartInfo.Verb = "runas";
                }
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = path;
                p.StartInfo.Arguments = arg;
                p.StartInfo.WorkingDirectory = dir;
                p.Start();
                p.Close();
            }
        }

        private void AddLink(string item)
        {
            int i = tabControl.SelectedIndex;
            if (i < 0 || i >= listViews.Count)
            {
                return;
            }
            
            try
            {
                Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(shellType);
                dynamic shortcut = shell.CreateShortcut(item);
                string path = shortcut.TargetPath;
                string args = shortcut.Arguments;
                IcoFileInfo icoFileInfolk = new IcoFileInfo(item)
                {
                    Args = args
                };
                if (System.IO.File.Exists(path) || System.IO.Directory.Exists(path))
                {
                    IcoFileInfo icoFileInfo = new IcoFileInfo(path)
                    {
                        Name = icoFileInfolk.Name,
                        Args = args
                    };
                    AddFile(icoFileInfo, i);
                }
                else
                {
                    AddFile(icoFileInfolk, i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法读取快捷方式:\n{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddFile(IcoFileInfo icoInfoFile, int i)
        {
            if (i < 0 || i >= largeImageLists.Count)
            {
                return;
            }
            
            // 获取图标,如果为 null 则使用默认图标
            Icon largeIcon = FilesystemIcons.LargeIcon(icoInfoFile.FullName);
            Icon mediumIcon = FilesystemIcons.MediumIcon(icoInfoFile.FullName);
            Icon smallIcon = FilesystemIcons.SmallIcon(icoInfoFile.FullName);
            
            largeImageLists[i].Images.Add(largeIcon ?? FilesystemIcons.ICON_FILE_48x);
            mediumImageLists[i].Images.Add(mediumIcon ?? FilesystemIcons.ICON_FILE_32x);
            smallImageLists[i].Images.Add(smallIcon ?? FilesystemIcons.ICON_FILE_16x);
            
            ListViewItem item = new ListViewItem
            {
                Text = icoInfoFile.Name,
                ImageIndex = largeImageLists[i].Images.Count - 1
            };
            item.SubItems.Add(icoInfoFile.FullName);
            item.SubItems.Add(icoInfoFile.Args);
            item.SubItems.Add(icoInfoFile.RunAsA.ToString());
            item.SubItems.Add(DetectFileCategory(icoInfoFile.FullName));
            string tip = $"{item.Text}\n链接：{item.SubItems[1].Text}";
            if (!string.IsNullOrEmpty(item.SubItems[2].Text))
            {
                tip += $"\n参数：{item.SubItems[2].Text}";
            }
            item.ToolTipText = tip;
            listViews[i].Items.Add(item);
        }

        private void AddFile(string file)
        {
            int selectedIndex = tabControl.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= listViews.Count)
            {
                return;
            }
            
            IcoFileInfo fileinf = new IcoFileInfo(file);
            AddFile(fileinf, selectedIndex);
        }

        private void AddFiles(string[] files)
        {
            foreach (string item in files)
            {
                if (noReadLnk == (ModifierKeys == Keys.Control) && item.ToLower().EndsWith(".lnk"))
                {
                    try
                    {
                        AddLink(item);
                    }
                    catch (Exception)
                    {
                        AddFile(item);
                    }
                }
                else
                {
                    AddFile(item);
                }
            }
            ApplySearchAndTypeFilter();
            WriteCfg();
        }

        private void AddPage(string pgname)
        {
            backimages.Add("");
            ImageList large = new ImageList
            {
                ImageSize = new Size(48, 48),
                ColorDepth = ColorDepth.Depth32Bit
            };
            ImageList medium = new ImageList
            {
                ImageSize = new Size(32, 32),
                ColorDepth = ColorDepth.Depth32Bit
            };
            ImageList small = new ImageList
            {
                ImageSize = new Size(16, 16),
                ColorDepth = ColorDepth.Depth32Bit
            };
            largeImageLists.Add(large);
            mediumImageLists.Add(medium);
            smallImageLists.Add(small);
            ListView listView = new ListView
            {
                LargeImageList = large,
                //MediumImageList = medium,
                SmallImageList = small,
                AllowDrop = true,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                HideSelection = false,
                Location = new Point(0, 0),
                Margin = new Padding(0),
                Padding = new Padding(0),
                MultiSelect = false,
                ShowItemToolTips = true
                //UseCompatibleStateImageBehavior = false
            };
            listView.DragDrop += ListViews_DragDrop;
            listView.DragOver += ListView_DragOver;
            listView.ItemDrag += ListView_ItemDrag;
            listView.MouseClick += ListViews_MouseClick;
            listView.DoubleClick += ListView_DoubleClick;
            listView.MouseDown += ListView_MouseDown;
            listView.Columns.Add("名称", 100, HorizontalAlignment.Left);
            //listView.Columns.Add("类型", 60, HorizontalAlignment.Left);
            listView.Columns.Add("路径", 300, HorizontalAlignment.Left);
            listView.Columns.Add("参数", 100, HorizontalAlignment.Left);
            listView.Columns.Add("管理员权限", 100, HorizontalAlignment.Left);
            listView.Columns.Add("类型", 90, HorizontalAlignment.Left);
            //listView.ArrangeIcons();
            listViews.Add(listView);
            TabPage tabPage = new TabPage
            {
                Location = new Point(0),
                Margin = new Padding(0),
                //tabPage.Size = new Size(652, 403);
                Padding = new Padding(0),
                Text = pgname
            };
            tabPage.Controls.Add(listView);
            tabControl.TabPages.Add(tabPage);
            Button button = new Button
            {
                //BackColor = Color.Transparent,// Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0),
                Padding = new Padding(0),
                //Location = new Point(tbwidth * buttons.Count, 0),
                Size = new Size(tbwidth, tbheight),
                Text = pgname,
                UseVisualStyleBackColor = false
            };
            //button.FlatAppearance.BorderColor = Color.LightBlue;
            button.Click += new EventHandler(Button_Click);
            //button.MouseDown += new MouseEventHandler(Button_MouseDown);
            buttons.Add(button);
            panelButton.Controls.Add(button);
        }

        private void RemovePage(int i)
        {
            if (i < 0 || i >= listViews.Count)
            {
                return;
            }
            
            // 释放背景图片资源
            if (listViews[i].BackgroundImage != null)
            {
                Image bgImage = listViews[i].BackgroundImage;
                listViews[i].BackgroundImage = null;
                bgImage.Dispose();
            }
            
            // 释放图标列表资源
            if (largeImageLists[i] != null)
            {
                largeImageLists[i].Dispose();
            }
            if (mediumImageLists[i] != null)
            {
                mediumImageLists[i].Dispose();
            }
            if (smallImageLists[i] != null)
            {
                smallImageLists[i].Dispose();
            }
            
            backimages.RemoveAt(i);
            listViews.RemoveAt(i);
            largeImageLists.RemoveAt(i);
            mediumImageLists.RemoveAt(i);
            smallImageLists.RemoveAt(i);
            tabControl.TabPages.RemoveAt(i);
            
            // 移除按钮
            if (i < buttons.Count && buttons[i] != null)
            {
                buttons[i].Dispose();
                buttons.RemoveAt(i);
            }
            
            FitButton();
            WriteCfg();
        }

        public void FitButton()
        {
            panelButton.Width = tbwidth;
            panelButton.Height = tbheight;
            panelButton.Controls.Clear();
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Size = new Size(tbwidth, tbheight);
                if (panelButton.Dock == DockStyle.Top || panelButton.Dock == DockStyle.Bottom)
                {
                    buttons[i].Location = new Point(tbwidth * i, 0);
                }
                else
                {
                    buttons[i].Location = new Point(0, tbheight * i);
                }
                panelButton.Controls.Add(buttons[i]);
                if (i == tabControl.SelectedIndex)
                {
                    buttons[i].ForeColor = colorF2;
                    buttons[i].BackColor = colorB2;// Color.LightBlue;
                }
                else
                {
                    buttons[i].ForeColor = colorF1;
                    buttons[i].BackColor = Color.Transparent;// Color.White;
                }
                buttons[i].FlatAppearance.BorderColor = colorB2;
            }
            FormMain_Resize(null, null);
        }
        #endregion

        #region MENU
        private void ContextMenuStripMain_Opening(object sender, CancelEventArgs e)
        {
            bool b = listViews.Count > 0 && listViews[tabControl.SelectedIndex].SelectedItems.Count > 0;
            RunAsAdminToolStripMenuItem.Visible = b;
            RemoveToolStripMenuItem.Visible = b;
            ShowFileToolStripMenuItem.Visible = b;
            EditToolStripMenuItem.Visible = b;
            toolStripMenuItem1.Visible = tabControl.TabPages.Count > 0;
            RemovePageToolStripMenuItem.Visible = tabControl.TabPages.Count > 0;
            ViewToolStripMenuItem.Visible = tabControl.TabPages.Count > 0;
            AddToolStripMenuItem.Visible = tabControl.TabPages.Count > 0;
            ClearToolStripMenuItem.Visible = tabControl.TabPages.Count > 0 && listViews[tabControl.SelectedIndex].Items.Count > 0; ;
            EditPageToolStripMenuItem.Visible = tabControl.TabPages.Count > 0;
        }

        private void BigIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetExplorerViewMode(View.LargeIcon);
        }
        
        private void SmallIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetExplorerViewMode(View.SmallIcon);
        }
        
        private void DetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetExplorerViewMode(View.Details);
        }
        
        private void ListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetExplorerViewMode(View.List);
        }
        
        private void TileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetExplorerViewMode(View.Tile);
        }

        private void AddFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    AddFiles(openFileDialog.FileNames);
                    WriteCfg();
                }
            }
        }

        private void AddDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folder = new FolderBrowserDialog())
            {
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    AddFiles(new string[] { folder.SelectedPath });
                    WriteCfg();
                }
            }
        }

        private void AddNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (EditIco editIco = new EditIco(new IcoFileInfo("")))
            {
                editIco.TopMost = TopMost;
                if (editIco.ShowDialog() == DialogResult.OK)
                {
                    AddFile(editIco.f, tabControl.SelectedIndex);
                    WriteCfg();
                }
            }
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = tabControl.SelectedIndex;
            int j = listViews[i].SelectedItems[0].Index;
            if (listViews[i].SelectedItems.Count > 0)
            {
                if (MessageBox.Show(this, "确定要删除选择的项目吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    return;
                }
                listViews[i].Items.RemoveAt(j);
                if (j < listViews[i].Items.Count)
                {
                    listViews[i].Items[j].Selected = true;
                }
                else if (listViews[i].Items.Count > 0)
                {
                    listViews[i].Items[j - 1].Selected = true;
                }
                ApplySearchAndTypeFilter();
                WriteCfg();
            }
        }

        private void ShowFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex < 0 || tabControl.SelectedIndex >= listViews.Count)
            {
                return;
            }
            
            if (listViews[tabControl.SelectedIndex].SelectedItems.Count > 0)
            {
                try
                {
                    using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                    {
                        string ffull = listViews[tabControl.SelectedIndex].SelectedItems[0].SubItems[1].Text;
                        if (ffull.Contains(" "))
                        {
                            ffull = "\"" + ffull + "\"";
                        }
                        p.StartInfo.FileName = "Explorer.exe";
                        p.StartInfo.Arguments = "/e,/select," + ffull;
                        p.Start();
                        p.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"无法打开文件位置:\n{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex < 0 || tabControl.SelectedIndex >= listViews.Count)
            {
                return;
            }
            
            if (listViews[tabControl.SelectedIndex].Items.Count >= 1 && 
                MessageBox.Show(this, "确定要删除所有项目？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                listViews[tabControl.SelectedIndex].Items.Clear();
                largeImageLists[tabControl.SelectedIndex].Images.Clear();
                mediumImageLists[tabControl.SelectedIndex].Images.Clear();
                smallImageLists[tabControl.SelectedIndex].Images.Clear();
                WriteCfg();
            }
        }

        private void RunAsAdminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i1 = tabControl.SelectedIndex;
            if (i1 < 0 || i1 >= listViews.Count || listViews[i1].SelectedItems.Count != 1)
            {
                return;
            }
            
            int i = listViews[i1].SelectedItems[0].Index;
            string path = listViews[i1].Items[i].SubItems[1].Text;
            string arg = listViews[i1].Items[i].SubItems[2].Text;
            try
            {
                RunIcon(path, arg, true);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"无法以管理员方式运行:\n{exception.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (hideRun)
            {
                Hide();
            }
        }

        private void EditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i1 = tabControl.SelectedIndex;
            if (i1 < 0 || i1 >= listViews.Count || listViews[i1].SelectedItems.Count == 0)
            {
                return;
            }
            
            int i = listViews[i1].SelectedItems[0].Index;
            IcoFileInfo f1 = new IcoFileInfo(listViews[i1].SelectedItems[0].SubItems[1].Text)
            {
                Name = listViews[i1].SelectedItems[0].Text,
                Args = listViews[i1].SelectedItems[0].SubItems[2].Text,
                RunAsA = listViews[i1].SelectedItems[0].SubItems[3].Text.ToLower() == "true"
            };
            using (EditIco editIco = new EditIco(f1))
            {
                editIco.TopMost = TopMost;
                if (editIco.ShowDialog() == DialogResult.OK)
                {
                    if (listViews[i1].Items[i].SubItems[1].Text != editIco.f.FullName)
                    {
                        Icon largeIcon = FilesystemIcons.LargeIcon(editIco.f.FullName);
                        Icon mediumIcon = FilesystemIcons.MediumIcon(editIco.f.FullName);
                        Icon smallIcon = FilesystemIcons.SmallIcon(editIco.f.FullName);
                        
                        if (largeIcon != null)
                        {
                            largeImageLists[i1].Images[listViews[i1].Items[i].ImageIndex] = largeIcon.ToBitmap();
                        }
                        if (mediumIcon != null)
                        {
                            mediumImageLists[i1].Images[listViews[i1].Items[i].ImageIndex] = mediumIcon.ToBitmap();
                        }
                        if (smallIcon != null)
                        {
                            smallImageLists[i1].Images[listViews[i1].Items[i].ImageIndex] = smallIcon.ToBitmap();
                        }
                    }
                    listViews[i1].Items[i].Text = editIco.f.Name;
                    listViews[i1].Items[i].SubItems[1].Text = editIco.f.FullName;
                    listViews[i1].Items[i].SubItems[2].Text = editIco.f.Args;
                    listViews[i1].Items[i].SubItems[3].Text = editIco.f.RunAsA.ToString();
                    EnsureTypeSubItem(listViews[i1].Items[i]);
                    string tip = $"{editIco.f.Name}\n链接：{editIco.f.FullName}";
                    if (!string.IsNullOrEmpty(editIco.f.Args))
                    {
                        tip += $"\n参数：{editIco.f.Args}";
                    }
                    listViews[i1].Items[i].ToolTipText = tip;
                    ApplySearchAndTypeFilter();
                    WriteCfg();
                }
            }
        }

        private void EditPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (EditPage editPage = new EditPage(this, tabControl.SelectedIndex))
            {
                editPage.ShowDialog();
                WriteCfg();
            }
        }

        private void AddPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (InputName nameTab = new InputName(null))
            {
                nameTab.TopMost = TopMost;
                if (nameTab.ShowDialog() == DialogResult.OK)
                {
                    AddPage(nameTab.name);
                    tabControl.SelectedIndex = tabControl.TabPages.Count - 1;
                    FitButton();
                    ApplySearchAndTypeFilter();
                    WriteCfg();
                }
            }
        }

        private void RemovePageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = tabControl.SelectedIndex;
            if (i > -1 && MessageBox.Show(this, "确定要删除选择的页面吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                RemovePage(i);
            }
        }

        private void SettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int w1 = tbwidth;
            int h1 = tbheight;
            using (Setting setting = new Setting(this))
            {
                if (sender == SettingToolStripMenuItem)
                {
                    setting.StartPosition = FormStartPosition.CenterParent;
                }
                setting.ShowDialog();
            }
            if (w1 != tbwidth || h1 != tbheight)
            {
                FitButton();
            }
            Activate();
            WriteCfg();
            notifyIcon.Text = Text;
            Event.UnregisterHotKey(Handle, 1);
            if (hotkeyon)
            {
                OnHotKey();
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            goexit = true;
            Close();
        }
        #endregion

        private void 软件版本ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SWVersion version = new SWVersion())
            {
                version.StartPosition = FormStartPosition.CenterParent;
                version.TopMost = TopMost;
                version.ShowDialog();
            }
        }

        private void toolStripTextBox1_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show(this, "PingBox 是一款免费软件，无需激活。\n感谢您的使用！", "软件信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void 页面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (InputName nameTab = new InputName(null))
            {
                nameTab.TopMost = TopMost;
                if (nameTab.ShowDialog() == DialogResult.OK)
                {
                    AddPage(nameTab.name);
                    tabControl.SelectedIndex = tabControl.TabPages.Count - 1;
                    FitButton();
                    WriteCfg();
                }
            }
        }

        private void 文件ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    AddFiles(openFileDialog.FileNames);
                    WriteCfg();
                }
            }
        }

        private void 文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folder = new FolderBrowserDialog())
            {
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    AddFiles(new string[] { folder.SelectedPath });
                    WriteCfg();
                }
            }
        }

        private void 大图标ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetExplorerViewMode(View.LargeIcon);
        }

        private void 中图标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetExplorerViewMode(View.LargeIcon, true);
        }

        private void 小图标ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetExplorerViewMode(View.SmallIcon);
        }

        private void 详细列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetExplorerViewMode(View.Details);
        }
        
        private void 列表ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetExplorerViewMode(View.List);
        }

        private void 平铺ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetExplorerViewMode(View.Tile);
        }

    }

    public class IcoFileInfo
    {
        public IcoFileInfo(string filename)
        {
            FullName = filename ?? "";
            Args = "";
            RunAsA = false;
            
            // 安全处理文件名提取
            if (string.IsNullOrEmpty(filename))
            {
                NameWithEx = "";
                Name = "";
            }
            else
            {
                try
                {
                    NameWithEx = System.IO.Path.GetFileName(filename);
                    if (string.IsNullOrEmpty(NameWithEx))
                    {
                        Name = "";
                    }
                    else
                    {
                        // 移除扩展名
                        string nameWithoutExt = NameWithEx.Trim('.');
                        if (nameWithoutExt.Contains("."))
                        {
                            int lastDotIndex = nameWithoutExt.LastIndexOf(".");
                            Name = nameWithoutExt.Substring(0, lastDotIndex);
                        }
                        else
                        {
                            Name = nameWithoutExt;
                        }
                    }
                }
                catch
                {
                    NameWithEx = filename;
                    Name = filename;
                }
            }
        }
        public string Name { get; set; }
        public string NameWithEx { get; set; }
        public string FullName { set; get; }
        public string Args { set; get; }
        public bool RunAsA { set; get; }
    }
}
