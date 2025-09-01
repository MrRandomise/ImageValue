
using System;
using YamlDotNet.Core;

namespace ImageValue
{
    public partial class Main : Form
    {
        private DirectoryInfo screenDir;
        private DirectoryInfo jsonDir;
        private FileInfo[] screen;
        private int pictureIndex = -1;
        private Image image;
        private ResizeImage resize;
        private Renderer draw;
        private int layer = 999;
        private RecObj rectObj;
        private List<RecObj> rectObjList = new List<RecObj>();
        private bool activeNewRec = false;
        private bool activeDraw = false;
        private bool activeResize = false;
        private RecValue recValue = new RecValue();
        private int activeCorner = -1;
        private Rectangle currentNewRectangle;
        private RecToJson recToJson;
        private Point newStart;
        private YamlManager manager = new YamlManager();
        private string JsonConfig;

        public Main()
        {
            InitializeComponent();
            DoubleBuffered = true;
            draw = new Renderer();
            resize = new ResizeImage();
            recToJson = new RecToJson();
            //search = new SearchClass();
            //search.ImageFileChecker(@"C:\Users\dmitr\OneDrive\Desktop\DataSet\DataSet\Cards\combined_result.json");
            string appPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Configs";
            saveFileDialog1.InitialDirectory = appPath;
            openFileDialog1.InitialDirectory = appPath;
            saveFileDialog1.FileName = "";
            openFileDialog1.FileName = "";
            ///pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; // ???? ? ????? ?????? ??????
        }


        private void DirButton_Click(object sender, EventArgs e)
        {
            if (screen != null)
            {
                Array.Clear(screen, 0, screen.Length);
            }
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    screenDir = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                    screen = screenDir.GetFiles("*.png", SearchOption.AllDirectories);
                    NextPicture(1);
                    NextImgBtn.Enabled = true;
                    AddRec.Enabled = true;
                    SaveCfgBtn.Enabled = true;
                    LoadCfgBtn.Enabled = true;
                    ImgCnt.Enabled = true;
                    LoadImgCnt.Enabled = true;
                    label2.Text = "из " + screen.Count().ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Â óêàçàííîì ïàïêå íåò èçîáðàæåíèé");
                    NextImgBtn.Enabled = false;
                    AddRec.Enabled = false;
                    SaveCfgBtn.Enabled = false;
                    LoadCfgBtn.Enabled = false;
                }
            }
        }

        private void NextPicture(int index, bool changeIndex = false)
        {
            if(!changeIndex)
            {
                pictureIndex += index;
            }
            

            if (pictureIndex <= screen.Length - 1 && pictureIndex >= 0)
            {
                //var img = resize.ResizeBitmap(Image.FromFile(screen[pictureIndex].FullName), 970, 700);
                pictureBox1.Image = Image.FromFile(screen[pictureIndex].FullName);
                image = pictureBox1.Image;
                var jsonPath = Path.ChangeExtension(jsonDir + "\\" + screen[pictureIndex].Name, ".json");
                ImgCnt.Text = pictureIndex.ToString();
                Text = "Value Image - " + screen[pictureIndex].Name;
                if (File.Exists(jsonPath))
                {
                    rectObjList.Clear();
                    var text = File.ReadAllText(jsonPath);
                    rectObjList = recToJson.LoadFromCustomJson(jsonPath);
                }
                else if (JsonConfig != null)
                {
                    loadJsonConfig();
                    //var name = search.DoesFileNameExist(screen[pictureIndex].Name);
                    //rectObjList[0].Name = name;
                }
                else
                {
                    rectObjList.Clear();
                }
                var roots = rectObjList.Where(x => x.Parent == null).ToList();
                FillTreeViewFromHierarchy(roots);
                pictureBox1.Invalidate();
            }
        }

        private void PrevImgBtn_Click(object sender, EventArgs e)
        {
            NextPicture(-1);
            if (pictureIndex == 0)
            {
                PrevImgBtn.Enabled = false;
            }
            else
            {
                PrevImgBtn.Enabled = true;
            }
            NextImgBtn.Enabled = true;
        }

        private void NextImgBtn_Click(object sender, EventArgs e)
        {
            NextPicture(1);
            if (pictureIndex == screen.Length)
            {
                NextImgBtn.Enabled = false;
            }
            else
            {
                NextImgBtn.Enabled = true;
            }
            PrevImgBtn.Enabled = true;
        }

        private void AddRec_Click(object sender, EventArgs e)
        {
            activeNewRec = true;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            // Передаем размер pictureBox1 в метод Render
            if (image != null) // Добавим проверку, что изображение есть
            {
                draw.Render(g, image, rectObjList, pictureBox1.ClientSize);
            }

            // Рисуем новый прямоугольник, пока создаем его
            if (activeNewRec && currentNewRectangle != Rectangle.Empty)
            {
                using (var pen = new Pen(Color.Blue, 2))
                {
                    g.DrawRectangle(pen, currentNewRectangle);
                }
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // Конвертируем координаты мыши в координаты изображения
            var imagePoint = PointToImage(e.Location);

            if (activeNewRec)
            {
                // Для нового прямоугольника стартовая точка в координатах PictureBox
                newStart = e.Location;
                currentNewRectangle = new Rectangle(newStart, Size.Empty);
                activeDraw = true;
                return;
            }

            RecObj found = null;
            // Ищем объект по координатам изображения
            for (int i = rectObjList.Count - 1; i >= 0; i--)
            {
                if (rectObjList[i].HitTest(imagePoint))
                {
                    found = rectObjList[i];
                    break;
                }
            }

            if (found != null)
            {
                if (rectObj != null) rectObj.Active = false;
                rectObj = found;
                rectObj.Active = true;
                // Передаем в объект уже сконвертированные координаты
                rectObj.BeginOperation(imagePoint.X, imagePoint.Y);
                activeDraw = true;
                activeCorner = rectObj.HitCorner(imagePoint);
                activeResize = activeCorner != -1;
                Cursor = activeResize ? Cursors.SizeAll : Cursors.Hand;
            }
            else
            {
                if (rectObj != null) { rectObj.Active = false; rectObj = null; }
                activeDraw = false;
                activeResize = false;
                activeCorner = -1;
                Cursor = Cursors.Default;
            }

            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Конвертируем координаты мыши в координаты изображения
            var imagePoint = PointToImage(e.Location);

            if (activeNewRec && activeDraw)
            {
                // Временный прямоугольник рисуем в координатах PictureBox
                currentNewRectangle = CreateRect(newStart.X, newStart.Y, e.X, e.Y);
                pictureBox1.Invalidate();
                return;
            }

            if (!activeDraw || rectObj == null) return;

            if (activeResize && activeCorner != -1)
            {
                // Передаем в объект уже сконвертированные координаты
                rectObj.ResizeBy(imagePoint.X, imagePoint.Y, activeCorner);
            }
            else
            {
                // Передаем в объект уже сконвертированные координаты
                rectObj.MoveBy(imagePoint.X, imagePoint.Y);
            }

            // Проверка попадания в угол для смены курсора
            int cornerUnder = rectObj.HitCorner(imagePoint);
            Cursor = cornerUnder != -1 ? Cursors.SizeAll : Cursors.Hand;

            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (activeNewRec && activeDraw)
            {
                activeNewRec = false;
                activeDraw = false;
                Cursor = Cursors.Default;

                // Конвертируем финальный прямоугольник из координат PictureBox в координаты изображения
                var imageRect = RectangleToImage(currentNewRectangle);

                if (imageRect.Width >= 4 && imageRect.Height >= 4)
                {
                    // Создаем RecObj с координатами изображения
                    var newObj = new RecObj(imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height, layer--);

                    RecObj parent = null;
                    foreach (var c in rectObjList)
                    {
                        if (c.ContainsFully(newObj))
                        {
                            if (parent == null || (parent.Width * parent.Height) > (c.Width * c.Height))
                            {
                                parent = c;
                            }
                        }
                    }

                    if (parent != null)
                    {
                        parent.AddChild(newObj);
                    }

                    rectObjList.Add(newObj);
                    var roots = rectObjList.Where(x => x.Parent == null).ToList();
                    FillTreeViewFromHierarchy(roots);
                }

                currentNewRectangle = Rectangle.Empty;
                pictureBox1.Invalidate();
                return;
            }

            activeDraw = false;
            activeResize = false;
            activeCorner = -1;
            Cursor = Cursors.Default;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (rectObj != null)
            {
                recValue.recObj = rectObj;
                if (recValue.ShowDialog() == DialogResult.OK)
                {
                    rectObj = recValue.recObj;
                    var roots = rectObjList.Where(x => x.Parent == null).ToList();
                    FillTreeViewFromHierarchy(roots);
                }
            }
        }

        private Rectangle CreateRect(int x1, int y1, int x2, int y2)
        {
            int left = Math.Min(x1, x2);
            int top = Math.Min(y1, y2);
            int w = Math.Abs(x2 - x1);
            int h = Math.Abs(y2 - y1);

            return new Rectangle(left, top, w, h);
        }

        private void SaveCfgBtn_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                recToJson.SaveConfigFile(saveFileDialog1.FileName, rectObjList);
            }
        }

        private void LoadCfgBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                JsonConfig = openFileDialog1.FileName;
                loadJsonConfig();
            }
        }

        private void loadJsonConfig()
        {
            rectObjList.Clear();
            rectObjList = recToJson.LoadConfigFile(JsonConfig);
            pictureBox1.Invalidate();
            var roots = rectObjList.Where(x => x.Parent == null).ToList();
            FillTreeViewFromHierarchy(roots);
        }
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (rectObj != null)
                {
                    RemoveRecursive(rectObj);
                    DeleteSelectedFromTree();
                    var roots = rectObjList.Where(x => x.Parent == null).ToList();
                    FillTreeViewFromHierarchy(roots);
                    pictureBox1.Invalidate();
                }
            }
            if (e.KeyCode == Keys.D && e.Control)
            {
                if (rectObj != null)
                {
                    // ??? ???? ??? ???
                    var cloneRoot = rectObj.DeepClone();
                    // ???? ?? ???? ??? ????? ??? ???? (???????)
                    const int offset = 10;
                    cloneRoot.ShiftRecursive(offset, offset);

                    // ????? ?? ? ????
                    if (rectObj.Parent != null)
                    {
                        // ????? ? ????? ?? ?? ??? ???
                        rectObj.Parent.AddChild(cloneRoot);
                    }


                    // ????? ???? ?? ??? ????? ? rectObjList (?? ??? ?? ???? ? ???)
                    void AddAllToList(RecObj r)
                    {
                        if (!rectObjList.Contains(r))
                            rectObjList.Add(r);
                        foreach (var ch in r.Children) AddAllToList(ch);
                    }
                    AddAllToList(cloneRoot);

                    // ???? ??? ?? ?????
                    if (rectObj != null) rectObj.Active = false;
                    rectObj = cloneRoot;
                    rectObj.Active = true;

                    // ???? TreeView
                    var roots = rectObjList.Where(x => x.Parent == null).ToList();
                    FillTreeViewFromHierarchy(roots);

                    pictureBox1.Invalidate();

                    // ????, ?? ????? ?????
                    e.Handled = true;
                }
            }
        }

        private void FillTreeViewFromHierarchy(List<RecObj> roots)
        {
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();

            foreach (var root in roots)
            {
                var rootNode = CreateNodeRecursive(root);
                treeView1.Nodes.Add(rootNode);
            }

            treeView1.EndUpdate();
            treeView1.ExpandAll();
        }

        private TreeNode CreateNodeRecursive(RecObj rect)
        {
            var node = new TreeNode(rect.Name) { Tag = rect };
            foreach (var child in rect.Children)
            {
                node.Nodes.Add(CreateNodeRecursive(child));
            }
            return node;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var rect = e.Node.Tag as RecObj;
            if (rect != null)
            {
                if (rectObj != null)
                {
                    rectObj.Active = false;
                }
                rectObj = rect;
                rectObj.Active = true;
                activeResize = activeCorner != -1;
                pictureBox1.Invalidate();
            }
        }

        private void DeleteSelectedFromTree()
        {
            var node = treeView1.SelectedNode;
            if (node == null) return;
            var rect = node.Tag as RecObj;
            if (rect == null) return;

            RemoveRecursive(rect);
            if (rect.Parent != null) rect.Parent.Children.Remove(rect);

            pictureBox1.Invalidate();
        }

        void RemoveRecursive(RecObj r)
        {
            foreach (var ch in r.Children.ToList())
                RemoveRecursive(ch);
            rectObjList.Remove(r);
            if (r.Parent != null)
                r.Parent.Children.Remove(r);
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (rectObj != null)
            {
                recValue.recObj = rectObj;
                if (recValue.ShowDialog() == DialogResult.OK)
                {
                    rectObj = recValue.recObj;
                    var roots = rectObjList.Where(x => x.Parent == null).ToList();
                    FillTreeViewFromHierarchy(roots);
                }
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            var rect = new RecObj();
            recValue.recObj = rect;
            if (recValue.ShowDialog() == DialogResult.OK)
            {
                if (rectObj != null)
                {
                    rectObj.AddChild(recValue.recObj);
                }
                else
                {
                    rectObjList.Add(rect);
                }
                var roots = rectObjList.Where(x => x.Parent == null).ToList();
                FillTreeViewFromHierarchy(roots);
            }
        }

        private void SaveJsn_Click(object sender, EventArgs e)
        {
            //// ?? ? ????? ???
            string imgPath = null;
            string imgName = null;
            Size? imgSize = null;
            var jsonPath = Path.ChangeExtension(jsonDir + "\\" + screen[pictureIndex].Name, ".json");
            if (screen != null && pictureIndex >= 0 && pictureIndex < screen.Length)
            {
                imgPath = screen[pictureIndex].FullName;
                imgName = screen[pictureIndex].Name;
                try
                {
                    using (var img = Image.FromFile(imgPath))
                    {
                        imgSize = new Size(img.Width, img.Height);
                    }
                }
                catch
                {
                    // ?? ? ???? ? ???? null
                }
            }
            recToJson.SaveToJson(jsonPath, rectObjList, imgPath, imgName, imgSize);
            manager.SaveOrUpdateNames(rectObjList, screenDir.FullName + "\\data.yaml");
            recToJson.SaveToTxtYml(rectObjList, screenDir.FullName, imgName, imgSize);
        }

        private void LoadJsnDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                jsonDir = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                SaveJsn.Enabled = true;
            }

        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var node = e.Item as TreeNode;
            if (node == null) return;
            // Запускаем перетаскивание самого RecObj
            DoDragDrop(node, DragDropEffects.Move);
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            // Поддерживаем визуальную подсказку — вычисляем узел под курсором
            Point pt = treeView1.PointToClient(new Point(e.X, e.Y));
            TreeNode nodeUnder = treeView1.GetNodeAt(pt);
            if (nodeUnder != null)
            {
                treeView1.SelectedNode = nodeUnder;
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                // Если отпускают в пустую область — это значит, делаем корневым
                treeView1.SelectedNode = null;
                e.Effect = DragDropEffects.Move;
            }
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TreeNode))) return;
            Point pt = treeView1.PointToClient(new Point(e.X, e.Y));
            TreeNode nodeSource = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            TreeNode nodeTarget = treeView1.GetNodeAt(pt); // может быть null — тогда перенос в корень

            if (nodeSource == null) return;

            var srcRect = nodeSource.Tag as RecObj;
            if (srcRect == null) return;

            // Если перетаскиваем на самого себя или на потомка — запрещаем
            if (nodeTarget != null)
            {
                var tgtRect = nodeTarget.Tag as RecObj;
                if (tgtRect != null)
                {
                    // Проверяем, не является ли target потомком source (чтобы не создавать цикл)
                    var p = tgtRect;
                    while (p != null)
                    {
                        if (p == srcRect)
                        {
                            // нельзя переместить в своего потомка — просто выход
                            return;
                        }
                        p = p.Parent;
                    }
                }
            }

            // Удаляем связь с прежним родителем (но не удаляем детей)
            if (srcRect.Parent != null)
            {
                srcRect.Parent.Children.Remove(srcRect);
                srcRect.Parent = null;
            }

            // Если был корневым — он находился в rectObjList; при переносе вниз (в дочерние) нужно удалить из rectObjList
            if (rectObjList.Contains(srcRect))
            {
                // Если дропнули на узел (nodeTarget != null) — делаем дочерним
                if (nodeTarget != null)
                {
                    rectObjList.Remove(srcRect);
                }
                else
                {
                    // дропнули в пустую область — остаётся корнем (ничего не делаем)
                }
            }

            // Если цель — узел, то добавляем как ребёнка
            if (nodeTarget != null)
            {
                var tgtRect = nodeTarget.Tag as RecObj;
                if (tgtRect != null)
                {
                    tgtRect.AddChild(srcRect);
                }
            }
            else
            {
                // Перенесли в корень TreeView (восстанавливаем в rectObjList)
                if (!rectObjList.Contains(srcRect))
                {
                    rectObjList.Add(srcRect);
                }
                srcRect.Parent = null;
            }

            // Обновляем представление TreeView
            var roots = rectObjList.Where(x => x.Parent == null).ToList();
            FillTreeViewFromHierarchy(roots);

            pictureBox1.Invalidate();
        }

        private void ImgCnt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(ImgCnt.Text) <= 0)
                {
                    ImgCnt.Text = "1";
                    PrevImgBtn.Enabled = false;
                    NextImgBtn.Enabled = true;
                }
                if (int.Parse(ImgCnt.Text) > screen.Count())
                {
                    var i = screen.Count() - 1;
                    ImgCnt.Text = i.ToString();
                    NextImgBtn.Enabled = false;
                    PrevImgBtn.Enabled = true;
                }
                if(int.Parse(ImgCnt.Text) > 1)
                {
                    PrevImgBtn.Enabled = true;
                }
                if (int.Parse(ImgCnt.Text) < screen.Count())
                {
                    NextImgBtn.Enabled = true;
                }
            }
            catch
            {
                ImgCnt.Text = "1";
                PrevImgBtn.Enabled = false;
                NextImgBtn.Enabled = true;
            }
        }

        private void ImgCnt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // Запрещаем ввод  
            }
        }

        private void LoadImgCnt_Click(object sender, EventArgs e)
        {
            try
            {
                pictureIndex = int.Parse(ImgCnt.Text);
                NextPicture(pictureIndex, true);
            }
            catch
            {
                pictureIndex = 1;
                NextPicture(pictureIndex, true);
            }
        }
        private Point PointToImage(Point controlPoint)
        {
            if (pictureBox1.Image == null) return controlPoint;

            var pbSize = pictureBox1.ClientSize;
            var imgSize = pictureBox1.Image.Size;

            // Защита от деления на ноль, если PictureBox или Image имеют нулевой размер
            if (pbSize.Width == 0 || pbSize.Height == 0) return Point.Empty;

            float scaleX = (float)imgSize.Width / pbSize.Width;
            float scaleY = (float)imgSize.Height / pbSize.Height;

            int imgX = (int)(controlPoint.X * scaleX);
            int imgY = (int)(controlPoint.Y * scaleY);

            return new Point(imgX, imgY);
        }

        /// <summary>
        /// Конвертирует прямоугольник из координат PictureBox в координаты изображения.
        /// </summary>
        private Rectangle RectangleToImage(Rectangle controlRect)
        {
            if (pictureBox1.Image == null) return controlRect;

            Point topLeft = PointToImage(controlRect.Location);
            Point bottomRight = PointToImage(new Point(controlRect.Right, controlRect.Bottom));

            return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        }
    }
}
