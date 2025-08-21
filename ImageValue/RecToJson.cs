using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ImageValue
{
    public class RecToJson
    {
        public void SaveToFile(string path, List<RecObj> list)
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

        public List<RecObj> LoadFromFile(string path)
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

        public void SaveToJson(string fileName, List<RecObj>rects, string imageFullPath, Size? imageSize = null)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (imageFullPath == null) throw new ArgumentNullException(nameof(imageFullPath));

            // Формируем итоговый список JSON-объектов (по вашему примеру — массив объектов)
            var rootArray = new JArray();

            // Первый объект: информация об изображении
            var imageObj = new JObject
            {
                ["image_path"] = imageFullPath,
                ["image_size"] = new JObject
                {
                    ["width"] = imageSize.Value.Width,
                    ["height"] = imageSize.Value.Height
                }
            };
            rootArray.Add(imageObj);

            // Последующие объекты: записи с id, label и опционально bbox
            foreach (var r in rects)
            {
                var obj = new JObject
                {
                    ["id"] = r.Name,
                    ["label"] = r.Value ?? ""
                };

                // Добавляем bbox только если все координаты заданы (в вашем примере bbox отсутствует у некоторых)
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

                rootArray.Add(obj);
            }

            // Сериализация с отступами для читаемости
            var json = rootArray.ToString(Formatting.Indented);

            // Сохраняем в файл
            File.WriteAllText(fileName, json);
        }

        private JObject RecObjToJObject(RecObj r, int id)
        {
            var jo = new JObject
            {
                ["id"] = id,
                ["label"] = r.Name
            };

            // parse role/fields from r.Value
            string role = null;
            JObject fieldsFromValue = null;
            if (!string.IsNullOrWhiteSpace(r.Value))
            {
                try
                {
                    var parsed = JObject.Parse(r.Value);
                    if (parsed.TryGetValue("role", out var roleTok))
                    {
                        role = roleTok.ToString();
                        parsed.Remove("role");
                    }
                    fieldsFromValue = parsed;
                }
                catch
                {
                    var s = r.Value.Trim();
                    if (s.StartsWith("role:", System.StringComparison.OrdinalIgnoreCase) || s.StartsWith("role=", System.StringComparison.OrdinalIgnoreCase))
                    {
                        var split = s.Split(new[] { ':', '=' }, 2);
                        if (split.Length == 2) role = split[1].Trim();
                    }
                }
            }
            if (!string.IsNullOrEmpty(role))
                jo["role"] = role;

            // bbox
            var bbox = new JObject();
            if (r.X != 0) bbox["x"] = r.X;
            if (r.Y != 0) bbox["y"] = r.Y;
            if (r.Width != 0) bbox["w"] = r.Width;
            if (r.Height != 0) bbox["h"] = r.Height;
            if (bbox.HasValues)
                jo["bbox"] = bbox;

            // fields
            var fields = new JObject();
            if (fieldsFromValue != null)
            {
                foreach (var prop in fieldsFromValue.Properties())
                {
                    if (prop.Value.Type == JTokenType.Object)
                    {
                        var valObj = (JObject)prop.Value;
                        var fjo = new JObject();

                        if (valObj.TryGetValue("text", out var t) && t != null && t.Type != JTokenType.Null)
                        {
                            var ts = t.Type == JTokenType.String ? t.ToString().Trim() : t;
                            if (!string.IsNullOrEmpty(ts.ToString()))
                                fjo["text"] = t;
                        }

                        if (valObj.TryGetValue("bbox", out var bbTok) && bbTok != null && bbTok.Type == JTokenType.Object)
                        {
                            var bb = (JObject)bbTok;
                            var bbOut = new JObject();
                            if (bb.TryGetValue("x", out var bx) && bx != null && bx.Type == JTokenType.Integer && bx.Value<int>() != 0)
                                bbOut["x"] = bx;
                            if (bb.TryGetValue("y", out var by) && by != null && by.Type == JTokenType.Integer && by.Value<int>() != 0)
                                bbOut["y"] = by;
                            if (bb.TryGetValue("w", out var bw) && bw != null && bw.Type == JTokenType.Integer && bw.Value<int>() != 0)
                                bbOut["w"] = bw;
                            if (bb.TryGetValue("h", out var bh) && bh != null && bh.Type == JTokenType.Integer && bh.Value<int>() != 0)
                                bbOut["h"] = bh;
                            if (bbOut.HasValues) fjo["bbox"] = bbOut;
                        }

                        if (fjo.HasValues)
                            fields[prop.Name] = fjo;
                    }
                    else
                    {
                        var v = prop.Value;
                        bool add = false;
                        if (v.Type == JTokenType.Integer || v.Type == JTokenType.Float)
                        {
                            add = v.Value<double>() != 0.0;
                        }
                        else if (v.Type == JTokenType.String)
                        {
                            add = !string.IsNullOrWhiteSpace(v.ToString());
                        }
                        else
                        {
                            add = true;
                        }

                        if (add) fields[prop.Name] = v;
                    }
                }
            }

            if (fields.HasValues)
                jo["fields"] = fields;

            return jo;
        }
}
}
