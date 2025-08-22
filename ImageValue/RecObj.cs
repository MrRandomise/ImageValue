using Newtonsoft.Json;
using System.Collections.Generic;

public class RecObj
{
    // Публичные свойства сохраняемые в JSON
    public int X; // глобальные координаты левого верхнего угла
    public int Y;
    public int Width;
    public int Height;
    public string Name = "Rectangle";
    public string Value = "";
    public int Layer;
    public int HitRadius = 6;
    public bool NotShow = false;

    // Иерархия: Parent не сериализуем, Children сериализуемы как обычный список
    [JsonIgnore]
    public RecObj Parent;

    public List<RecObj> Children = new List<RecObj>();

    // Визуальные поля, не сериализуемые
    [JsonIgnore]
    public bool Active = false;
    [JsonIgnore]
    public Color Color = Color.Green;

    // Вспомогательные поля при операции перемещения/ресайза
    [JsonIgnore]
    private int startMouseX, startMouseY, startX, startY, startW, startH;

    [JsonProperty("Color")]
    public string ColorString
    {
        get { return ColorToString(Color); }
        set { Color = StringToColor(value); }
    }

    public RecObj(int x = 0, int y = 0, int w = 0, int h = 0, int layer = 0)
    {
        X = x; Y = y; Width = w; Height = h; Layer = layer;
    }

    // Возвращает прямоугольник в глобальной системе координат
    public Rectangle GetBounds() => new Rectangle(X, Y, Width, Height);

    // Попадание точки (глобальные координаты)
    public bool HitTest(Point p)
    {
        var r = GetBounds();
        return p.X >= r.Left && p.X <= r.Right && p.Y >= r.Top && p.Y <= r.Bottom;
    }

    // Проверка, попали ли в угол; возвращает индекс угла или -1
    public int HitCorner(Point p)
    {
        Point[] corners = new Point[]
        {
        new Point(X, Y),                     // top-left (0)
        new Point(X + Width, Y),             // top-right (1)
        new Point(X, Y + Height),            // bottom-left (2)
        new Point(X + Width, Y + Height)     // bottom-right (3)
        };
        for (int i = 0; i < corners.Length; i++)
        {
            int dx = p.X - corners[i].X;
            int dy = p.Y - corners[i].Y;
            if(dx * dx + dy * dy <= HitRadius * HitRadius) return i;
        }
    return -1;
    }

    // Начало операции (сохраняем стартовые значения)
    public void BeginOperation(int mouseX, int mouseY)
    {
        startMouseX = mouseX; startMouseY = mouseY;
        startX = X; startY = Y; startW = Width; startH = Height;
    }

    // Перемещение — смещаем сам прямоугольник и всех потомков в глобальных координатах.
    // Координаты детей остаются глобальными, поэтому при смещении родителя мы также смещаем детей на тот же дельта.
    public void MoveBy(int mouseX, int mouseY)
    {
        int dx = mouseX - startMouseX;
        int dy = mouseY - startMouseY;
        int newX = startX + dx;
        int newY = startY + dy;
        int shiftX = newX - X;
        int shiftY = newY - Y;

        // обновляем нашу позицию
        X = newX;
        Y = newY;

        // смещаем всех потомков — их координаты сохраняются глобальными
        foreach (var child in Children)
        {
            child.ShiftRecursive(shiftX, shiftY);
        }
    }

    // Рекурсивное смещение (используется при перемещении родителя)
    public void ShiftRecursive(int dx, int dy)
    {
        X += dx; Y += dy;
        foreach (var ch in Children) ch.ShiftRecursive(dx, dy);
    }

    // Изменение размера по углу в глобальных координатах
    public void ResizeBy(int mouseX, int mouseY, int corner)
    {
        int dx = mouseX - startMouseX;
        int dy = mouseY - startMouseY;
        const int minSize = 8;

        switch (corner)
        {
            case 0: // top-left
                X = startX + dx;
                Y = startY + dy;
                Width = startW - dx;
                Height = startH - dy;
                break;
            case 1: // top-right
                Y = startY + dy;
                Width = startW + dx;
                Height = startH - dy;
                break;
            case 2: // bottom-left
                X = startX + dx;
                Width = startW - dx;
                Height = startH + dy;
                break;
            case 3: // bottom-right
                Width = startW + dx;
                Height = startH + dy;
                break;
        }

        // Защита от минимального размера
        if (Width < minSize) Width = minSize;
        if (Height < minSize) Height = minSize;

        // Корректировка позиции при ограничении размеров (чтобы левая/верхняя стороны "прилипали")
        if ((corner == 0 || corner == 2) && Width == minSize)
            X = startX + (startW - minSize);
        if ((corner == 0 || corner == 1) && Height == minSize)
            Y = startY + (startH - minSize);

        // Примечание: при изменении размера родителя дочерние прямоугольники остаются в глобальных координатах
        // Если вы хотите, чтобы дочерние масштабировались вместе с родителем — нужно дополнительно пересчитать
        // позиции и размеры детей относительно новых размеров родителя.
    }

    // Добавление ребёнка — child.Position остаётся глобальным; child.Parent = this
    public void AddChild(RecObj child)
    {
        if (child == null) return;
        child.Parent = this;
        Children.Add(child);
    }

    // Удаление ребёнка
    public void RemoveChild(RecObj child)
    {
        if (child == null) return;
        child.Parent = null;
        Children.Remove(child);
    }

    // Проверка: полностью ли r2 находится внутри r1 (используется при создании дочернего)
    public bool ContainsFully(RecObj r2)
    {
        var a = this.GetBounds();
        var b = r2.GetBounds();
        return a.Left <= b.Left && a.Top <= b.Top && a.Right >= b.Right && a.Bottom >= b.Bottom;
    }

    // Конвертация цвета в строку и обратно
    private static string ColorToString(Color c) => $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
    private static Color StringToColor(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return Color.Empty;
        s = s.TrimStart('#');
        if (s.Length == 8)
        {
            byte a = Convert.ToByte(s.Substring(0, 2), 16);
            byte r = Convert.ToByte(s.Substring(2, 2), 16);
            byte g = Convert.ToByte(s.Substring(4, 2), 16);
            byte b = Convert.ToByte(s.Substring(6, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }
        else if (s.Length == 6)
        {
            byte r = Convert.ToByte(s.Substring(0, 2), 16);
            byte g = Convert.ToByte(s.Substring(2, 2), 16);
            byte b = Convert.ToByte(s.Substring(4, 2), 16);
            return Color.FromArgb(255, r, g, b);
        }
        throw new FormatException("Color string must be in '#AARRGGBB' or '#RRGGBB' format.");
    }

    public RecObj DeepClone()
    {
        var clone = new RecObj(this.X, this.Y, this.Width, this.Height, this.Layer)
        {
            Name = this.Name,
            Value = this.Value,
            HitRadius = this.HitRadius,
            Active = false,
            Color = this.Color
        };

        // Не копируем Parent — он будет установлен при вставке в иерархию
        clone.Parent = null;
        clone.Children = new List<RecObj>();
        foreach (var ch in this.Children)
        {
            var chClone = ch.DeepClone();
            chClone.Parent = clone;
            clone.Children.Add(chClone);
        }
        return clone;
    }
}