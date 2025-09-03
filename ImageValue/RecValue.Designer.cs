namespace ImageValue
{
    partial class RecValue
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            CncBtn = new Button();
            OkBtn = new Button();
            Value = new TextBox();
            NameText = new TextBox();
            colorDialog1 = new ColorDialog();
            ColorBtn = new Button();
            NoShwBox = new CheckBox();
            listBox1 = new ListBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(59, 15);
            label1.TabIndex = 0;
            label1.Text = "Название";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 77);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 1;
            label2.Text = "Значение";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 164);
            label3.Name = "label3";
            label3.Size = new Size(33, 15);
            label3.TabIndex = 2;
            label3.Text = "Цвет";
            // 
            // CncBtn
            // 
            CncBtn.Location = new Point(139, 244);
            CncBtn.Name = "CncBtn";
            CncBtn.Size = new Size(75, 23);
            CncBtn.TabIndex = 4;
            CncBtn.Text = "Cancel";
            CncBtn.UseVisualStyleBackColor = true;
            CncBtn.Click += CncBtn_Click;
            // 
            // OkBtn
            // 
            OkBtn.Location = new Point(12, 244);
            OkBtn.Name = "OkBtn";
            OkBtn.Size = new Size(75, 23);
            OkBtn.TabIndex = 5;
            OkBtn.Text = "Ok";
            OkBtn.UseVisualStyleBackColor = true;
            OkBtn.Click += OkBtn_Click;
            // 
            // Value
            // 
            Value.Location = new Point(15, 128);
            Value.Name = "Value";
            Value.Size = new Size(202, 23);
            Value.TabIndex = 6;
            // 
            // NameText
            // 
            NameText.Location = new Point(12, 39);
            NameText.Name = "NameText";
            NameText.Size = new Size(202, 23);
            NameText.TabIndex = 7;
            // 
            // ColorBtn
            // 
            ColorBtn.BackColor = SystemColors.ButtonHighlight;
            ColorBtn.Location = new Point(15, 196);
            ColorBtn.Name = "ColorBtn";
            ColorBtn.Size = new Size(199, 23);
            ColorBtn.TabIndex = 8;
            ColorBtn.Text = "Цвет";
            ColorBtn.UseVisualStyleBackColor = false;
            ColorBtn.Click += ColorBtn_Click;
            // 
            // NoShwBox
            // 
            NoShwBox.AutoSize = true;
            NoShwBox.Location = new Point(15, 225);
            NoShwBox.Name = "NoShwBox";
            NoShwBox.Size = new Size(110, 19);
            NoShwBox.TabIndex = 9;
            NoShwBox.Text = "Не отоброжать";
            NoShwBox.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(223, 39);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(283, 229);
            listBox1.TabIndex = 10;
            listBox1.MouseDoubleClick += listBox1_MouseDoubleClick;
            // 
            // RecValue
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(516, 279);
            Controls.Add(listBox1);
            Controls.Add(NoShwBox);
            Controls.Add(ColorBtn);
            Controls.Add(NameText);
            Controls.Add(Value);
            Controls.Add(OkBtn);
            Controls.Add(CncBtn);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "RecValue";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Значения";
            Shown += RecValue_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox textBox1;
        private Button CncBtn;
        private Button OkBtn;
        private TextBox Value;
        private TextBox NameText;
        private ColorDialog colorDialog1;
        private Button ColorBtn;
        private CheckBox NoShwBox;
        private ListBox listBox1;
    }
}