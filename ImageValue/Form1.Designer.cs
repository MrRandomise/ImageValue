namespace ImageValue
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            colorDialog1 = new ColorDialog();
            LayerPanel = new Panel();
            treeView1 = new TreeView();
            contextMenuStrip1 = new ContextMenuStrip(components);
            Add = new ToolStripMenuItem();
            SettingPanel = new Panel();
            SaveJsn = new Button();
            LoadJsnDir = new Button();
            LoadCfgBtn = new Button();
            SaveCfgBtn = new Button();
            AddRec = new Button();
            NextImgBtn = new Button();
            PrevImgBtn = new Button();
            DirButton = new Button();
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            folderBrowserDialog1 = new FolderBrowserDialog();
            saveFileDialog1 = new SaveFileDialog();
            openFileDialog1 = new OpenFileDialog();
            LayerPanel.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SettingPanel.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // LayerPanel
            // 
            LayerPanel.BorderStyle = BorderStyle.Fixed3D;
            LayerPanel.Controls.Add(treeView1);
            LayerPanel.Dock = DockStyle.Right;
            LayerPanel.Location = new Point(1239, 0);
            LayerPanel.Name = "LayerPanel";
            LayerPanel.Size = new Size(274, 709);
            LayerPanel.TabIndex = 1;
            // 
            // treeView1
            // 
            treeView1.AllowDrop = true;
            treeView1.ContextMenuStrip = contextMenuStrip1;
            treeView1.Dock = DockStyle.Top;
            treeView1.ImeMode = ImeMode.On;
            treeView1.Location = new Point(0, 0);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(270, 319);
            treeView1.TabIndex = 0;
            treeView1.ItemDrag += treeView1_ItemDrag;
            treeView1.AfterSelect += treeView1_AfterSelect;
            treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
            treeView1.DragDrop += treeView1_DragDrop;
            treeView1.DragEnter += treeView1_DragEnter;
            treeView1.DragOver += treeView1_DragOver;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { Add });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(127, 26);
            // 
            // Add
            // 
            Add.Name = "Add";
            Add.Size = new Size(126, 22);
            Add.Text = "Добавить";
            Add.Click += Add_Click;
            // 
            // SettingPanel
            // 
            SettingPanel.BorderStyle = BorderStyle.Fixed3D;
            SettingPanel.Controls.Add(SaveJsn);
            SettingPanel.Controls.Add(LoadJsnDir);
            SettingPanel.Controls.Add(LoadCfgBtn);
            SettingPanel.Controls.Add(SaveCfgBtn);
            SettingPanel.Controls.Add(AddRec);
            SettingPanel.Controls.Add(NextImgBtn);
            SettingPanel.Controls.Add(PrevImgBtn);
            SettingPanel.Controls.Add(DirButton);
            SettingPanel.Dock = DockStyle.Left;
            SettingPanel.Location = new Point(0, 0);
            SettingPanel.Name = "SettingPanel";
            SettingPanel.Size = new Size(254, 709);
            SettingPanel.TabIndex = 3;
            // 
            // SaveJsn
            // 
            SaveJsn.Enabled = false;
            SaveJsn.Location = new Point(3, 467);
            SaveJsn.Name = "SaveJsn";
            SaveJsn.Size = new Size(243, 63);
            SaveJsn.TabIndex = 7;
            SaveJsn.Text = "Сохранить разметку";
            SaveJsn.UseVisualStyleBackColor = true;
            SaveJsn.Click += SaveJsn_Click;
            // 
            // LoadJsnDir
            // 
            LoadJsnDir.Location = new Point(3, 389);
            LoadJsnDir.Name = "LoadJsnDir";
            LoadJsnDir.Size = new Size(243, 63);
            LoadJsnDir.TabIndex = 6;
            LoadJsnDir.Text = "Папка для сохранения Json";
            LoadJsnDir.UseVisualStyleBackColor = false;
            LoadJsnDir.Click += LoadJsnDir_Click;
            // 
            // LoadCfgBtn
            // 
            LoadCfgBtn.Enabled = false;
            LoadCfgBtn.Location = new Point(3, 308);
            LoadCfgBtn.Name = "LoadCfgBtn";
            LoadCfgBtn.Size = new Size(243, 63);
            LoadCfgBtn.TabIndex = 5;
            LoadCfgBtn.Text = "Загрузить конфиг";
            LoadCfgBtn.UseVisualStyleBackColor = true;
            LoadCfgBtn.Click += LoadCfgBtn_Click;
            // 
            // SaveCfgBtn
            // 
            SaveCfgBtn.Enabled = false;
            SaveCfgBtn.Location = new Point(3, 228);
            SaveCfgBtn.Name = "SaveCfgBtn";
            SaveCfgBtn.Size = new Size(243, 63);
            SaveCfgBtn.TabIndex = 4;
            SaveCfgBtn.Text = "Сохранить в конфиг";
            SaveCfgBtn.UseVisualStyleBackColor = true;
            SaveCfgBtn.Click += SaveCfgBtn_Click;
            // 
            // AddRec
            // 
            AddRec.Enabled = false;
            AddRec.Location = new Point(3, 161);
            AddRec.Name = "AddRec";
            AddRec.Size = new Size(243, 45);
            AddRec.TabIndex = 3;
            AddRec.Text = "Добавить Класс";
            AddRec.UseVisualStyleBackColor = true;
            AddRec.Click += AddRec_Click;
            // 
            // NextImgBtn
            // 
            NextImgBtn.Enabled = false;
            NextImgBtn.Location = new Point(126, 92);
            NextImgBtn.Name = "NextImgBtn";
            NextImgBtn.Size = new Size(120, 50);
            NextImgBtn.TabIndex = 2;
            NextImgBtn.Text = "Вперед";
            NextImgBtn.UseVisualStyleBackColor = true;
            NextImgBtn.Click += NextImgBtn_Click;
            // 
            // PrevImgBtn
            // 
            PrevImgBtn.Enabled = false;
            PrevImgBtn.Location = new Point(3, 92);
            PrevImgBtn.Name = "PrevImgBtn";
            PrevImgBtn.Size = new Size(117, 50);
            PrevImgBtn.TabIndex = 1;
            PrevImgBtn.Text = "Назад";
            PrevImgBtn.UseVisualStyleBackColor = true;
            PrevImgBtn.Click += PrevImgBtn_Click;
            // 
            // DirButton
            // 
            DirButton.Location = new Point(3, 10);
            DirButton.Name = "DirButton";
            DirButton.Size = new Size(243, 63);
            DirButton.TabIndex = 0;
            DirButton.Text = "Открыть папку с изображениями";
            DirButton.UseVisualStyleBackColor = true;
            DirButton.Click += DirButton_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(254, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(985, 709);
            panel1.TabIndex = 4;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(985, 709);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.MouseDoubleClick += pictureBox1_MouseDoubleClick;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.Filter = "Json файл|*.json";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "Json файл|*.json";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1513, 709);
            Controls.Add(panel1);
            Controls.Add(SettingPanel);
            Controls.Add(LayerPanel);
            KeyPreview = true;
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Value Image";
            KeyDown += Main_KeyDown;
            LayerPanel.ResumeLayout(false);
            contextMenuStrip1.ResumeLayout(false);
            SettingPanel.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ColorDialog colorDialog1;
        private Panel LayerPanel;
        private Panel SettingPanel;
        private Panel panel1;
        private PictureBox pictureBox1;
        private Button NextImgBtn;
        private Button PrevImgBtn;
        private Button DirButton;
        private FolderBrowserDialog folderBrowserDialog1;
        private Button AddRec;
        private Button LoadCfgBtn;
        private Button SaveCfgBtn;
        private SaveFileDialog saveFileDialog1;
        private OpenFileDialog openFileDialog1;
        private TreeView treeView1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem Add;
        private Button SaveJsn;
        private Button LoadJsnDir;
    }
}
