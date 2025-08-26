using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ImageValue
{
    public class YamlData
    {
        // Имя свойства "names" будет соответствовать ключу в YAML файле
        public Dictionary<int, string> Names { get; set; } = new Dictionary<int, string>();
    }

    public class YamlManager
    {
        /// <summary>
        /// Сохраняет или обновляет YAML-файл, добавляя имена из списка объектов.
        /// </summary>
        /// <param name="rectObjList">Список объектов для обработки.</param>
        /// <param name="filePath">Путь к файлу data.yaml.</param>
        public void SaveOrUpdateNames(List<RecObj> rectObjList, string filePath)
        {
            // 1. Фильтруем исходный список: берем только объекты не в начале координат (0,0)
            var namesFromSource = rectObjList
                .Where(r => r.X != 0 || r.Y != 0) // Условие: хотя бы одна координата не равна 0
                .Select(r => r.Name)
                .ToList();

            // 2. Чтение существующего файла (если он есть)
            var data = new YamlData();
            if (File.Exists(filePath))
            {
                try
                {
                    var yamlContent = File.ReadAllText(filePath);
                    // Проверяем, не пустой ли файл, иначе десериализатор выдаст ошибку
                    if (!string.IsNullOrWhiteSpace(yamlContent))
                    {
                        var deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();

                        var existingData = deserializer.Deserialize<YamlData>(yamlContent);
                        if (existingData != null && existingData.Names != null)
                        {
                            data = existingData;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при чтении или парсинге файла {filePath}: {ex.Message}");
                    // В случае ошибки можно прервать выполнение или начать с чистого листа
                    // Здесь мы начнем с чистого листа, создав новый объект 'data'
                    data = new YamlData();
                }
            }

            // 3. Объединение данных: добавляем только новые, уникальные имена
            var existingNamesSet = new HashSet<string>(data.Names.Values);
            int nextIndex = data.Names.Keys.Any() ? data.Names.Keys.Max() + 1 : 0;

            foreach (var name in namesFromSource)
            {
                // Добавляем, только если такого имени еще нет в файле
                if (!existingNamesSet.Contains(name))
                {
                    data.Names.Add(nextIndex, name);
                    existingNamesSet.Add(name); // Добавляем в сет, чтобы избежать дублей из самого rectObjList
                    nextIndex++;
                }
            }

            // 4. Запись обновленных данных в файл
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yamlToWrite = serializer.Serialize(data);

            // Добавляем комментарий в начало файла, как в вашем примере
            string finalContent = "# data.yaml" + Environment.NewLine + yamlToWrite;

            File.WriteAllText(filePath, finalContent);
            Console.WriteLine($"Файл {filePath} успешно сохранен/обновлен.");
        }

        public int? FindIdByNameInYaml(string filePath, string nameToFind)
        {
            // Проверяем, существует ли файл
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Ошибка: Файл не найден по пути '{filePath}'");
                return null;
            }

            // Читаем все содержимое файла
            string yamlContent = File.ReadAllText(filePath);

            // Создаем десериализатор для преобразования YAML в C# объект
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance) // Используем, если ключи в YAML в формате camelCase
                .Build();

            try
            {
                // Десериализуем YAML в наш объект DataModel
                var data = deserializer.Deserialize<YamlData>(yamlContent);

                // Проверяем, что данные и словарь имен существуют
                if (data?.Names == null)
                {
                    Console.WriteLine("Ошибка: YAML файл не содержит ожидаемой структуры 'names'.");
                    return null;
                }

                // Ищем в словаре пару "ключ-значение", где значение равно искомому имени
                foreach (var entry in data.Names)
                {
                    // Сравниваем значения. StringComparison.OrdinalIgnoreCase делает поиск нечувствительным к регистру
                    if (string.Equals(entry.Value, nameToFind, StringComparison.OrdinalIgnoreCase))
                    {
                        return entry.Key; // Найдено! Возвращаем ключ (ID).
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при парсинге YAML: {ex.Message}");
                return null;
            }

            // Если цикл завершился, а совпадение не найдено
            return null;
        }
    }
}
