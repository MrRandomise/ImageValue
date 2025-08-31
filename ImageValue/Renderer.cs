public class Renderer
{
    private readonly Pen penNormal = new Pen(Color.Green, 2);
    private readonly Pen penActive = new Pen(Color.Red, 3);
    private readonly Brush brushCorner = new SolidBrush(Color.Blue);

    /// <summary>
    /// Конвертирует прямоугольник из координат изображения в координаты контрола (PictureBox).
    /// </summary>
    private Rectangle RectToControl(Rectangle imageRect, Size imageSize, Size controlSize)
    {
        if (imageSize.Width == 0 || imageSize.Height == 0) return Rectangle.Empty;

        float scaleX = (float)controlSize.Width / imageSize.Width;
        float scaleY = (float)controlSize.Height / imageSize.Height;

        int cX = (int)(imageRect.X * scaleX);
        int cY = (int)(imageRect.Y * scaleY);
        // Важно: ширину и высоту тоже масштабируем
        int cW = (int)(imageRect.Width * scaleX);
        int cH = (int)(imageRect.Height * scaleY);

        return new Rectangle(cX, cY, cW, cH);
    }

    public void Render(Graphics g, Image image, List<RecObj> rectangles, Size controlSize)
    {
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

        using (var font = new Font("Arial", 10, FontStyle.Bold))
        using (var brushText = new SolidBrush(Color.Black))
        using (var brushBackground = new SolidBrush(Color.FromArgb(200, Color.White)))
        {
            var sortedRects = rectangles.OrderBy(r => r.Layer).ThenBy(r => r.Active ? 1 : 0);

            foreach (var rect in sortedRects)
            {
                if (rect.NotShow) continue;

                // Получаем прямоугольник в координатах изображения
                var r_image = rect.GetBounds();
                // Конвертируем его в координаты PictureBox для отрисовки
                var r_control = RectToControl(r_image, image.Size, controlSize);

                // 1. Рисуем сам прямоугольник
                g.DrawRectangle(rect.Active ? penActive : penNormal, r_control);

                // 2. Рисуем маркеры по углам для активного прямоугольника
                if (rect.Active)
                {
                    int s = 8;
                    int s_half = s / 2;
                    g.FillRectangle(brushCorner, r_control.Left - s_half, r_control.Top - s_half, s, s);
                    g.FillRectangle(brushCorner, r_control.Right - s_half, r_control.Top - s_half, s, s);
                    g.FillRectangle(brushCorner, r_control.Left - s_half, r_control.Bottom - s_half, s, s);
                    g.FillRectangle(brushCorner, r_control.Right - s_half, r_control.Bottom - s_half, s, s);
                }

                // 3. Рисуем название объекта
                var text = rect.Name;
                if (rect.Active && !string.IsNullOrEmpty(text))
                {
                    SizeF textSize = g.MeasureString(text, font);
                    PointF textPosition = new PointF(r_control.Left, r_control.Top - textSize.Height - 2);

                    if (textPosition.Y < 0)
                    {
                        textPosition.Y = r_control.Top + 2;
                    }

                    g.FillRectangle(brushBackground, textPosition.X, textPosition.Y, textSize.Width, textSize.Height);
                    g.DrawString(text, font, brushText, textPosition);
                }
            }
        }
    }
}