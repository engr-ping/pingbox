namespace PingBox
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.contextMenuStripMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RunAsAdminToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.大图标ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.小图标ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.详细ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.列表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.平铺ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddDirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.AddPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemovePageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.SettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripNoti = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SettingToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.panelButton = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripMenuItem();
            this.页面ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.文件ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.文件夹ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox2 = new System.Windows.Forms.ToolStripMenuItem();
            this.大图标ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.小图标ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.详细列表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.列表ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.平铺ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.软件版本ToolStripMenuItem = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.contextMenuStripMain.SuspendLayout();
            this.contextMenuStripNoti.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripMain
            // 
            this.contextMenuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RunAsAdminToolStripMenuItem,
            this.ViewToolStripMenuItem,
            this.AddToolStripMenuItem,
            this.RemoveToolStripMenuItem,
            this.EditToolStripMenuItem,
            this.ShowFileToolStripMenuItem,
            this.ClearToolStripMenuItem,
            this.EditPageToolStripMenuItem,
            this.toolStripMenuItem1,
            this.AddPageToolStripMenuItem,
            this.RemovePageToolStripMenuItem,
            this.toolStripMenuItem2,
            this.SettingToolStripMenuItem,
            this.ExitToolStripMenuItem});
            this.contextMenuStripMain.Name = "contextMenuStrip1";
            this.contextMenuStripMain.Size = new System.Drawing.Size(184, 304);
            this.contextMenuStripMain.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripMain_Opening);
            // 
            // RunAsAdminToolStripMenuItem
            // 
            this.RunAsAdminToolStripMenuItem.Name = "RunAsAdminToolStripMenuItem";
            this.RunAsAdminToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.RunAsAdminToolStripMenuItem.Text = "管理员方式运行";
            this.RunAsAdminToolStripMenuItem.Click += new System.EventHandler(this.RunAsAdminToolStripMenuItem_Click);
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.大图标ToolStripMenuItem,
            this.小图标ToolStripMenuItem,
            this.详细ToolStripMenuItem,
            this.列表ToolStripMenuItem,
            this.平铺ToolStripMenuItem});
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.ViewToolStripMenuItem.Text = "查看";
            // 
            // 大图标ToolStripMenuItem
            // 
            this.大图标ToolStripMenuItem.Name = "大图标ToolStripMenuItem";
            this.大图标ToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.大图标ToolStripMenuItem.Text = "大图标";
            this.大图标ToolStripMenuItem.Click += new System.EventHandler(this.BigIconsToolStripMenuItem_Click);
            // 
            // 小图标ToolStripMenuItem
            // 
            this.小图标ToolStripMenuItem.Name = "小图标ToolStripMenuItem";
            this.小图标ToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.小图标ToolStripMenuItem.Text = "小图标";
            this.小图标ToolStripMenuItem.Click += new System.EventHandler(this.SmallIconsToolStripMenuItem_Click);
            // 
            // 详细ToolStripMenuItem
            // 
            this.详细ToolStripMenuItem.Name = "详细ToolStripMenuItem";
            this.详细ToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.详细ToolStripMenuItem.Text = "详细";
            this.详细ToolStripMenuItem.Click += new System.EventHandler(this.DetailToolStripMenuItem_Click);
            // 
            // 列表ToolStripMenuItem
            // 
            this.列表ToolStripMenuItem.Name = "列表ToolStripMenuItem";
            this.列表ToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.列表ToolStripMenuItem.Text = "列表";
            this.列表ToolStripMenuItem.Click += new System.EventHandler(this.ListToolStripMenuItem_Click);
            // 
            // 平铺ToolStripMenuItem
            // 
            this.平铺ToolStripMenuItem.Name = "平铺ToolStripMenuItem";
            this.平铺ToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.平铺ToolStripMenuItem.Text = "平铺";
            this.平铺ToolStripMenuItem.Click += new System.EventHandler(this.TileToolStripMenuItem_Click);
            // 
            // AddToolStripMenuItem
            // 
            this.AddToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddFileToolStripMenuItem,
            this.AddDirToolStripMenuItem,
            this.AddNewToolStripMenuItem});
            this.AddToolStripMenuItem.Name = "AddToolStripMenuItem";
            this.AddToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.AddToolStripMenuItem.Text = "添加";
            // 
            // AddFileToolStripMenuItem
            // 
            this.AddFileToolStripMenuItem.Name = "AddFileToolStripMenuItem";
            this.AddFileToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.AddFileToolStripMenuItem.Text = "文件";
            this.AddFileToolStripMenuItem.Click += new System.EventHandler(this.AddFileToolStripMenuItem_Click);
            // 
            // AddDirToolStripMenuItem
            // 
            this.AddDirToolStripMenuItem.Name = "AddDirToolStripMenuItem";
            this.AddDirToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.AddDirToolStripMenuItem.Text = "目录";
            this.AddDirToolStripMenuItem.Click += new System.EventHandler(this.AddDirToolStripMenuItem_Click);
            // 
            // AddNewToolStripMenuItem
            // 
            this.AddNewToolStripMenuItem.Name = "AddNewToolStripMenuItem";
            this.AddNewToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.AddNewToolStripMenuItem.Text = "自定义";
            this.AddNewToolStripMenuItem.Click += new System.EventHandler(this.AddNewToolStripMenuItem_Click);
            // 
            // RemoveToolStripMenuItem
            // 
            this.RemoveToolStripMenuItem.Name = "RemoveToolStripMenuItem";
            this.RemoveToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.RemoveToolStripMenuItem.Text = "删除";
            this.RemoveToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItem_Click);
            // 
            // EditToolStripMenuItem
            // 
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.EditToolStripMenuItem.Text = "修改";
            this.EditToolStripMenuItem.Click += new System.EventHandler(this.EditToolStripMenuItem_Click);
            // 
            // ShowFileToolStripMenuItem
            // 
            this.ShowFileToolStripMenuItem.Name = "ShowFileToolStripMenuItem";
            this.ShowFileToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.ShowFileToolStripMenuItem.Text = "浏览文件";
            this.ShowFileToolStripMenuItem.Click += new System.EventHandler(this.ShowFileToolStripMenuItem_Click);
            // 
            // ClearToolStripMenuItem
            // 
            this.ClearToolStripMenuItem.Name = "ClearToolStripMenuItem";
            this.ClearToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.ClearToolStripMenuItem.Text = "清空";
            this.ClearToolStripMenuItem.Click += new System.EventHandler(this.ClearToolStripMenuItem_Click);
            // 
            // EditPageToolStripMenuItem
            // 
            this.EditPageToolStripMenuItem.Name = "EditPageToolStripMenuItem";
            this.EditPageToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.EditPageToolStripMenuItem.Text = "页面设置";
            this.EditPageToolStripMenuItem.Click += new System.EventHandler(this.EditPageToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(180, 6);
            // 
            // AddPageToolStripMenuItem
            // 
            this.AddPageToolStripMenuItem.Name = "AddPageToolStripMenuItem";
            this.AddPageToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.AddPageToolStripMenuItem.Text = "添加页面";
            this.AddPageToolStripMenuItem.Click += new System.EventHandler(this.AddPageToolStripMenuItem_Click);
            // 
            // RemovePageToolStripMenuItem
            // 
            this.RemovePageToolStripMenuItem.Name = "RemovePageToolStripMenuItem";
            this.RemovePageToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.RemovePageToolStripMenuItem.Text = "刪除页面";
            this.RemovePageToolStripMenuItem.Click += new System.EventHandler(this.RemovePageToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(180, 6);
            // 
            // SettingToolStripMenuItem
            // 
            this.SettingToolStripMenuItem.Name = "SettingToolStripMenuItem";
            this.SettingToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.SettingToolStripMenuItem.Text = "设置";
            this.SettingToolStripMenuItem.Click += new System.EventHandler(this.SettingToolStripMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.ExitToolStripMenuItem.Text = "退出";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // contextMenuStripNoti
            // 
            this.contextMenuStripNoti.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripNoti.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SettingToolStripMenuItem1,
            this.ExitToolStripMenuItem1});
            this.contextMenuStripNoti.Name = "contextMenuStrip2";
            this.contextMenuStripNoti.Size = new System.Drawing.Size(109, 52);
            // 
            // SettingToolStripMenuItem1
            // 
            this.SettingToolStripMenuItem1.Name = "SettingToolStripMenuItem1";
            this.SettingToolStripMenuItem1.Size = new System.Drawing.Size(108, 24);
            this.SettingToolStripMenuItem1.Text = "设置";
            this.SettingToolStripMenuItem1.Click += new System.EventHandler(this.SettingToolStripMenuItem_Click);
            // 
            // ExitToolStripMenuItem1
            // 
            this.ExitToolStripMenuItem1.Name = "ExitToolStripMenuItem1";
            this.ExitToolStripMenuItem1.Size = new System.Drawing.Size(108, 24);
            this.ExitToolStripMenuItem1.Text = "退出";
            this.ExitToolStripMenuItem1.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStripNoti;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseClick);
            // 
            // panelButton
            // 
            this.panelButton.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButton.Location = new System.Drawing.Point(0, 523);
            this.panelButton.Margin = new System.Windows.Forms.Padding(0);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(982, 30);
            this.panelButton.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(0, 32);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(982, 491);
            this.panel1.TabIndex = 3;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.toolStripComboBox1,
            this.toolStripComboBox2,
            this.toolStripMenuItem3});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(982, 28);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.页面ToolStripMenuItem,
            this.文件ToolStripMenuItem1,
            this.文件夹ToolStripMenuItem});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(53, 24);
            this.toolStripComboBox1.Text = "添加";
            // 
            // 页面ToolStripMenuItem
            // 
            this.页面ToolStripMenuItem.Name = "页面ToolStripMenuItem";
            this.页面ToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.页面ToolStripMenuItem.Text = "页面";
            this.页面ToolStripMenuItem.Click += new System.EventHandler(this.页面ToolStripMenuItem_Click);
            // 
            // 文件ToolStripMenuItem1
            // 
            this.文件ToolStripMenuItem1.Name = "文件ToolStripMenuItem1";
            this.文件ToolStripMenuItem1.Size = new System.Drawing.Size(224, 26);
            this.文件ToolStripMenuItem1.Text = "文件";
            this.文件ToolStripMenuItem1.Click += new System.EventHandler(this.文件ToolStripMenuItem1_Click);
            // 
            // 文件夹ToolStripMenuItem
            // 
            this.文件夹ToolStripMenuItem.Name = "文件夹ToolStripMenuItem";
            this.文件夹ToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.文件夹ToolStripMenuItem.Text = "文件夹";
            this.文件夹ToolStripMenuItem.Click += new System.EventHandler(this.文件夹ToolStripMenuItem_Click);
            // 
            // toolStripComboBox2
            // 
            this.toolStripComboBox2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.大图标ToolStripMenuItem1,
            this.小图标ToolStripMenuItem1,
            this.详细列表ToolStripMenuItem,
            this.列表ToolStripMenuItem1,
            this.平铺ToolStripMenuItem1});
            this.toolStripComboBox2.Name = "toolStripComboBox2";
            this.toolStripComboBox2.Size = new System.Drawing.Size(53, 24);
            this.toolStripComboBox2.Text = "查看";
            // 
            // 大图标ToolStripMenuItem1
            // 
            this.大图标ToolStripMenuItem1.Name = "大图标ToolStripMenuItem1";
            this.大图标ToolStripMenuItem1.Size = new System.Drawing.Size(224, 26);
            this.大图标ToolStripMenuItem1.Text = "大图标";
            this.大图标ToolStripMenuItem1.Click += new System.EventHandler(this.大图标ToolStripMenuItem1_Click);
            // 
            // 小图标ToolStripMenuItem1
            // 
            this.小图标ToolStripMenuItem1.Name = "小图标ToolStripMenuItem1";
            this.小图标ToolStripMenuItem1.Size = new System.Drawing.Size(224, 26);
            this.小图标ToolStripMenuItem1.Text = "小图标";
            this.小图标ToolStripMenuItem1.Click += new System.EventHandler(this.小图标ToolStripMenuItem1_Click);
            // 
            // 详细列表ToolStripMenuItem
            // 
            this.详细列表ToolStripMenuItem.Name = "详细列表ToolStripMenuItem";
            this.详细列表ToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.详细列表ToolStripMenuItem.Text = "详细";
            this.详细列表ToolStripMenuItem.Click += new System.EventHandler(this.详细列表ToolStripMenuItem_Click);
            // 
            // 列表ToolStripMenuItem1
            // 
            this.列表ToolStripMenuItem1.Name = "列表ToolStripMenuItem1";
            this.列表ToolStripMenuItem1.Size = new System.Drawing.Size(224, 26);
            this.列表ToolStripMenuItem1.Text = "列表";
            this.列表ToolStripMenuItem1.Click += new System.EventHandler(this.列表ToolStripMenuItem1_Click);
            // 
            // 平铺ToolStripMenuItem1
            // 
            this.平铺ToolStripMenuItem1.Name = "平铺ToolStripMenuItem1";
            this.平铺ToolStripMenuItem1.Size = new System.Drawing.Size(224, 26);
            this.平铺ToolStripMenuItem1.Text = "平铺";
            this.平铺ToolStripMenuItem1.Click += new System.EventHandler(this.平铺ToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.软件版本ToolStripMenuItem,
            this.toolStripTextBox1});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(53, 24);
            this.toolStripMenuItem3.Text = "帮助";
            // 
            // 软件版本ToolStripMenuItem
            // 
            this.软件版本ToolStripMenuItem.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.软件版本ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.软件版本ToolStripMenuItem.Name = "软件版本ToolStripMenuItem";
            this.软件版本ToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
            this.软件版本ToolStripMenuItem.Text = "软件版本";
            this.软件版本ToolStripMenuItem.Click += new System.EventHandler(this.软件版本ToolStripMenuItem_Click);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 27);
            this.toolStripTextBox1.Text = "软件激活";
            this.toolStripTextBox1.Click += new System.EventHandler(this.toolStripTextBox1_Click_1);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(982, 553);
            this.ContextMenuStrip = this.contextMenuStripMain;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panelButton);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.contextMenuStripMain.ResumeLayout(false);
            this.contextMenuStripNoti.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMain;
        private System.Windows.Forms.ToolStripMenuItem RunAsAdminToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ShowFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ClearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SettingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNoti;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 大图标ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 小图标ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 详细ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 列表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 平铺ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SettingToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem AddPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemovePageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem AddFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddDirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddNewToolStripMenuItem;
        public System.Windows.Forms.Panel panelButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripTextBox 软件版本ToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripMenuItem toolStripComboBox1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 文件夹ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripComboBox2;
        private System.Windows.Forms.ToolStripMenuItem 大图标ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 小图标ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 详细列表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 列表ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 平铺ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 页面ToolStripMenuItem;
    }
}

