public class Renderer
{
    public void Render(Graphics g, Image image, List<RecObj> rectangles)
    {
        if (image == null) return;
        // Рисуем изображение
        g.DrawImage(image, 0, 0, image.Width, image.Height);

        // Рисуем прямоугольники в порядке слоя (можно сортировать по Layer)
        var ordered = rectangles.OrderBy(r => r.Layer).ToList();

        using (var penActive = new Pen(Color.Red, 2))
        using (var penNormal = new Pen(Color.Green, 2))
        using (var brushCorner = new SolidBrush(Color.Yellow))
        {
            foreach (var rect in ordered)
            {
                var r = rect.GetBounds();
                g.DrawRectangle(rect.Active ? penActive : penNormal, r);

                if (rect.Active)
                {
                    int s = 6;
                    g.FillRectangle(brushCorner, r.Left - s / 2, r.Top - s / 2, s, s);
                    g.FillRectangle(brushCorner, r.Right - s / 2, r.Top - s / 2, s, s);
                    g.FillRectangle(brushCorner, r.Left - s / 2, r.Bottom - s / 2, s, s);
                    g.FillRectangle(brushCorner, r.Right - s / 2, r.Bottom - s / 2, s, s);
                }
            }
        }
    }
}