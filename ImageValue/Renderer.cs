public class Renderer
{
    // Заранее создадим перья и кисти, чтобы не создавать их в цикле каждый раз
    private readonly Pen penNormal = new Pen(Color.Green, 2);
    private readonly Pen penActive = new Pen(Color.Red, 3);
    private readonly Brush brushCorner = new SolidBrush(Color.Blue);

    public void Render(Graphics g, Image image, List<RecObj> rectangles)
    {
        // Устанавливаем высокое качество сглаживания для текста и фигур
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

        // Используем using для объектов, которые нужно освобождать
        using (var font = new Font("Arial", 10, FontStyle.Bold))
        using (var brushText = new SolidBrush(Color.Black))
        using (var brushBackground = new SolidBrush(Color.FromArgb(200, Color.White))) // Полупрозрачный белый фон
        {
            // Сортируем объекты по слою, чтобы активный всегда был сверху
            var sortedRects = rectangles.OrderBy(r => r.Layer).ThenBy(r => r.Active ? 1 : 0);

            foreach (var rect in sortedRects)
            {
                if (rect.NotShow) continue; // Пропускаем скрытые объекты

                var r = rect.GetBounds();

                // 1. Рисуем сам прямоугольник (активный или обычный)
                g.DrawRectangle(rect.Active ? penActive : penNormal, r);

                // 2. Рисуем маркеры по углам для активного прямоугольника
                if (rect.Active)
                {
                    int s = 8; // Размер маркера
                    int s_half = s / 2;
                    g.FillRectangle(brushCorner, r.Left - s_half, r.Top - s_half, s, s);
                    g.FillRectangle(brushCorner, r.Right - s_half, r.Top - s_half, s, s);
                    g.FillRectangle(brushCorner, r.Left - s_half, r.Bottom - s_half, s, s);
                    g.FillRectangle(brushCorner, r.Right - s_half, r.Bottom - s_half, s, s);
                }

                // 3. Рисуем название объекта
                var text = rect.Name;
                // Рисуем название, только если объект активен и у него есть имя
                if (rect.Active && !string.IsNullOrEmpty(text))
                {
                    // Измеряем размер текста, чтобы правильно нарисовать фон
                    SizeF textSize = g.MeasureString(text, font);

                    // Позиция текста - над левым верхним углом прямоугольника
                    PointF textPosition = new PointF(r.Left, r.Top - textSize.Height - 2);

                    // Проверка, чтобы текст не уходил за верхний край изображения
                    if (textPosition.Y < 0)
                    {
                        // Если уходит, рисуем внутри прямоугольника
                        textPosition.Y = r.Top + 2;
                    }

                    // Рисуем фон для текста для лучшей читаемости
                    g.FillRectangle(brushBackground, textPosition.X, textPosition.Y, textSize.Width, textSize.Height);

                    // Рисуем сам текст
                    g.DrawString(text, font, brushText, textPosition);
                }
            }
        }
    }

    // Не забудьте освободить ресурсы в деструкторе или через IDisposable
    // В данном простом случае можно обойтись без этого, т.к. форма сама их освободит
    // ~Renderer()
    // {
    //     penNormal.Dispose();
    //     penActive.Dispose();
    //     brushCorner.Dispose();
    // }
}