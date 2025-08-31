using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;


namespace ImageValue
{
    public class RecToJson
    {
        private YamlManager manager = new YamlManager();

        public void SaveConfigFile(string path, List<RecObj> list)
        {
            var roots = list.Where(r => r.Parent == null).ToList();

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented, // читаемый JSON
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(roots, settings);
            File.WriteAllText(path, json);
        }

        public List<RecObj> LoadConfigFile(string path)
        {
            if (!File.Exists(path)) return new List<RecObj>();
            var json = File.ReadAllText(path);
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var roots = JsonConvert.DeserializeObject<List<RecObj>>(json, settings) ?? new List<RecObj>();
            // восстановить Parent
            void SetParents(RecObj node)
            {
                foreach (var ch in node.Children)
                {
                    ch.Parent = node;
                    SetParents(ch);
                }
            }
            foreach (var r in roots) SetParents(r);
            // собрать плоский список
            var flat = new List<RecObj>();
            void Flatten(RecObj node)
            {
                flat.Add(node);
                foreach (var ch in node.Children) Flatten(ch);
            }
            foreach (var r in roots) Flatten(r);
            return flat;
        }


        public void SaveToJson(string fileName, List<RecObj> rects, string imageFullPath, string imgName, Size? imageSize)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (imageFullPath == null) throw new ArgumentNullException(nameof(imageFullPath));

            // Формируем итоговый список JSON-объектов
            var rootArray = new JArray();

            // Первый объект: информация об изображении
            var imageObj = new JObject
            {
                ["image_path"] = imgName,
                ["image_size"] = new JObject
                {
                    ["width"] = imageSize.Value.Width,
                    ["height"] = imageSize.Value.Height
                }
            };
            rootArray.Add(imageObj);

            // --- НАЧАЛО ИЗМЕНЕНИЙ ---

            // 1. Собираем все дочерние элементы в один набор для быстрой проверки.
            var allChildren = new HashSet<RecObj>(rects.SelectMany(r => r.Children ?? new List<RecObj>()));

            // 2. Отбираем только корневые элементы (те, что не являются ничьими детьми).
            var rootRects = rects.Where(r => !allChildren.Contains(r)).ToList();

            // 3. Передаем в обработку только корневые элементы.
            var obj = RecObjToJToken(rootRects); // isRoot по умолчанию true, что здесь и нужно
            rootArray.Add(obj);

            // --- КОНЕЦ ИЗМЕНЕНИЙ ---

            // Сериализация с отступами для читаемости
            var json = rootArray.ToString(Formatting.Indented);

            // Сохраняем в файл
            File.WriteAllText(fileName, json);
        }

        private JObject RecObjToJToken(List<RecObj> rects)
        {
            var container = new JObject();
            if (rects == null) return container;

            foreach (var r in rects)
            {
                var obj = new JObject();

                if (!string.IsNullOrEmpty(r.Value))
                {
                    obj["label"] = r.Value;
                }
                if (r.X != 0 && r.Y != 0 && r.Width != 0 && r.Height != 0)
                {
                    obj["bbox"] = new JObject
                    {
                        ["x"] = r.X,
                        ["y"] = r.Y,
                        ["w"] = r.Width,
                        ["h"] = r.Height
                    };
                }
                if (r.Children != null && r.Children.Count > 0)
                {
                    obj["items"] = RecObjToJToken(r.Children); // Рекурсия без флага
                }

                container[r.Name] = obj;
            }

            return container;
        }

        public void SaveToTxtYml(List<RecObj> rects, string path, string name, Size? imageSize = null)
        {
            var fullPath = path + "\\data.yaml";
            name = Path.ChangeExtension(name, ".txt");
            var file = path + "\\" + name;

            using (StreamWriter writer = new StreamWriter(file))
            {
                foreach (var r in rects)
                {
                    if (r.X != 0 && r.Y != 0 && r.Width != 0 && r.Height != 0)
                    {
                        var id = manager.FindIdByNameInYaml(fullPath, r.Name);
                        if (id != null)
                        {
                            float x_center = (r.X + r.Width / 2.0f) / imageSize.Value.Width;
                            float y_center = (r.Y + r.Height / 2.0f) / imageSize.Value.Height;
                            float width_norm = (float)r.Width / imageSize.Value.Width;
                            float height_norm = (float)r.Height / imageSize.Value.Height;

                            // Форматируем строку, используя CultureInfo.InvariantCulture для точки в качестве десятичного разделителя
                            writer.WriteLine("{0} {1:F6} {2:F6} {3:F6} {4:F6}",
                                id, x_center, y_center, width_norm, height_norm);
                        }
                    }
                }
            }
        }
        public List<RecObj> LoadFromCustomJson(string path)
        {
            if (!File.Exists(path)) return new List<RecObj>();
            var text = File.ReadAllText(path);
            JArray arr;
            try { arr = JArray.Parse(text); } catch { return new List<RecObj>(); }
            if (arr.Count < 2) return new List<RecObj>();
            var rootObj = arr[1] as JObject;
            if (rootObj == null) return new List<RecObj>();
            var roots = new List<RecObj>();
            var flat = new List<RecObj>();

            void ParseJObject(JObject j, RecObj parent)
            {
                foreach (var prop in j.Properties())
                {
                    if (!(prop.Value is JObject val)) continue;
                    var node = new RecObj();
                    node.Name = prop.Name;
                    if (val["bbox"] is JObject bb)
                    {
                        node.X = bb["x"]?.Value<int>() ?? 0;
                        node.Y = bb["y"]?.Value<int>() ?? 0;
                        node.Width = bb["w"]?.Value<int>() ?? 0;
                        node.Height = bb["h"]?.Value<int>() ?? 0;
                    }
                    if (val["label"] != null) node.Value = val["label"].Value<string>();
                    node.Parent = parent;
                    if (parent != null) parent.Children.Add(node);
                    else roots.Add(node);
                    flat.Add(node);

                    if (val["items"] is JObject itemsObj)
                    {
                        ParseJObject(itemsObj, node);
                    }
                }
            }

            ParseJObject(rootObj, null);

            // Примечание: при создании детей мы уже добавляем их в parent.Children,
            // но flat содержит все созданные объекты.
            return flat;
        }
    }
}
