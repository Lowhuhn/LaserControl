using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LaserControl.UIWindows.Dialogs
{
    public static class Prompts
    {

        public static string ShowDialog_Text(string caption, string text, string def)
        {
            Form prompt = new Form();
            prompt.Width = 280;
            prompt.Height = 160;
            prompt.Text = caption;
            Label textLabel = new Label() { Left = 16, Top = 20, Width = 240, Text = text };
            TextBox textBox = new TextBox() { Left = 16, Top = 50, Width = 240, TabIndex = 0, TabStop = true, Text = def };
            //CheckBox ckbx = new CheckBox() { Left = 16, Top = 60, Width = 240, Text = boolStr };
            Button confirmation = new Button() { Text = "OK", Left = 16, Width = 80, Top = 88, TabIndex = 1, TabStop = true };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            //prompt.Controls.Add(ckbx);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.ShowDialog();
            return textBox.Text;
        }

        public static void ShowDialog_Text_Combo(string caption, string text, string text2 , ref string defText1, List<string> comboItems, out string selectedItem)
        {
            Form prompt = new Form();
            prompt.Width = 280;
            prompt.Height = 180;
            prompt.Text = caption;
            Label textLabel = new Label() { Left = 16, Top = 10, Width = 240, Text = text };
            TextBox textBox = new TextBox() { Left = 16, Top = 35, Width = 240, TabIndex = 0, TabStop = true, Text = defText1 };
            //CheckBox ckbx = new CheckBox() { Left = 16, Top = 60, Width = 240, Text = boolStr };

            Label textLabel2 = new Label() { Left = 16, Top = 60, Width = 240, Text = text2 };
            ComboBox cbox = new ComboBox() { Left = 16, Top = 85, Width = 240, DataSource = comboItems, TabStop = true };

            Button confirmation = new Button() { Text = "OK", Left = 16, Width = 80, Top = 110, TabIndex = 1, TabStop = true };
            confirmation.Click += (sender, e) => 
            {                
                prompt.Close(); 
            };
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);

            prompt.Controls.Add(textLabel2);
            prompt.Controls.Add(cbox);

            //prompt.Controls.Add(ckbx);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.ShowDialog();
            defText1 = textBox.Text;
            selectedItem = (string)cbox.SelectedItem;
        }

    }
}
