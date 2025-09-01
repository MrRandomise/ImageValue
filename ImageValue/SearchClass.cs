using System.Text.Json;
using System.Text.Json.Serialization;

namespace ImageValue
{


    public class ImageInfo
    {
        // Атрибут [JsonPropertyName] связывает свойство C# (ImagePath)
        // с полем в JSON (image_path). Это хорошая практика.
        [JsonPropertyName("image_path")]
        public string? ImagePath { get; set; }
        [JsonPropertyName("class")]
        public string ClassName { get; set; }
    }

    public class SearchClass
    {
        private Dictionary<string, string> _imageClassMap;

        public void ImageFileChecker(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException("Указанный JSON-файл не найден.", jsonFilePath);
            }

            // Читаем весь файл
            string jsonData = File.ReadAllText(jsonFilePath);

            // Десериализуем JSON в список объектов ImageInfo
            var imageInfos = JsonSerializer.Deserialize<List<ImageInfo>>(jsonData);


            // Извлекаем только имена файлов (без пути) из каждого объекта
            // и добавляем их в HashSet для быстрой проверки.
            // Path.GetFileName извлекает "file.jpg" из "C:\folder\file.jpg".
            _imageClassMap = imageInfos.ToDictionary(info => info.ImagePath, info => info.ClassName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Проверяет, совпадает ли имя файла из переданного пути с одним из имен в JSON.
        /// Сравнение происходит по точному совпадению имени файла и его расширения.
        /// </summary>
        /// <param name="imagePathToCompare">Полный или частичный путь к файлу для сравнения.</param>
        /// <returns>True, если имя файла найдено в данных JSON, иначе False.</returns>
        public string DoesFileNameExist(string imagePathToCompare)
        {
            if (string.IsNullOrEmpty(imagePathToCompare))
            {
                return null;
            }

            // Получаем только имя файла из переданной строки
            string fileName = Path.GetFileName(imagePathToCompare);

            // Пытаемся найти класс в словаре. Это очень быстрая операция.
            if (_imageClassMap.TryGetValue(fileName, out string className))
            {
                return className;
            }

            // Если ключ не найден, возвращаем null
            return null;
        }
    }
}
