
namespace ImageValue
{
    public partial class RecValue : Form
    {
        public RecObj recObj;

        public RecValue()
        {
            InitializeComponent();
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
            DialogResult = DialogResult.OK;
        }

        private void RecValue_Shown(object sender, EventArgs e)
        {
            NameText.Text = recObj.Name;
            Value.Text = recObj.Value;
            ColorBtn.ForeColor = recObj.Color;
        }

        private void ColorBtn_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            ColorBtn.BackColor = colorDialog1.Color;
        }
    }
}
