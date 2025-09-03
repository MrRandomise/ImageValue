
using System.Windows.Forms;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace ImageValue
{
    public partial class RecValue : Form
    {
        public RecObj recObj;
        public string FilePath;

        public RecValue()
        {
            InitializeComponent();
        }

        private void GetYamal()
        {
            try
            {
                // 1. Читаем содержимое файла
                string yamlContent = File.ReadAllText(FilePath);

                // 2. Создаем десериализатор
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                // 3. Десериализуем YAML в объект C#. 
                //    Создадим вспомогательный класс для соответствия структуре YAML.
                var yamlObject = deserializer.Deserialize<YamlData>(yamlContent);

                // 4. Очищаем ListBox перед добавлением новых элементов
                listBox1.Items.Clear();

                // 5. Проверяем, что данные были успешно загружены
                if (yamlObject != null && yamlObject.Names != null)
                {
                    // Получаем только значения (имена игроков и т.д.)
                    var namesList = yamlObject.Names.Values.ToList();

                    // Добавляем все имена в ListBox
                    listBox1.Items.AddRange(namesList.ToArray());

                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show($"Ошибка: Файл не найден по пути '{FilePath}'", "Ошибка файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при чтении или обработке YAML: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void CncBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            recObj.Name = NameText.Text;
            recObj.Value = Value.Text;
            recObj.Color = ColorBtn.BackColor;
            recObj.NotShow = NoShwBox.Checked;
            DialogResult = DialogResult.OK;
        }

        private void RecValue_Shown(object sender, EventArgs e)
        {
            GetYamal();
            NameText.Text = recObj.Name;
            Value.Text = recObj.Value;
            NoShwBox.Checked = recObj.NotShow;
            ColorBtn.ForeColor = recObj.Color;
        }

        private void ColorBtn_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            ColorBtn.BackColor = colorDialog1.Color;
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Проверяем, что индекс выбранного элемента не -1
            if (listBox1.SelectedIndex != -1)
            {
                // Если индекс корректный, значит, и SelectedItem точно не null
                NameText.Text = listBox1.SelectedItem.ToString();
            }
        }
    }
}
