using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SS_OpenCV
{
    public partial class InputBox : Form
    {
        public InputBox()
        {
            InitializeComponent();
        }

        public InputBox(string _title)
        {
            InitializeComponent();

            this.Text = _title;

        }

        public static string GetValue()
        {
            return GetValue("");
        }


        public static string GetValue(string title)
        {
            InputBox form = new InputBox();
            form.Text = title;
            if (form.ShowDialog() == DialogResult.OK)
                return form.ValueTextBox.Text;
            return null;
        }


        public static int GetIntValue(string title)
        {
            InputBox form = new InputBox();
            form.Text = title;

            form.button1.Click += form.button1_Click_1;

            if (form.ShowDialog() == DialogResult.OK)
                return Convert.ToInt32(form.ValueTextBox.Text);
            return -1;
        }

        

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid number format");
            }
        }
    }
}