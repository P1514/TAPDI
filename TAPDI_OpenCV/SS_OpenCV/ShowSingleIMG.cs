using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emgu.CV.Structure;

namespace SS_OpenCV
{
    public partial class ShowSingleIMG : Form
    {
        public ShowSingleIMG()
        {
            InitializeComponent();
        }
        public ShowSingleIMG(Emgu.CV.IImage img1)
        {
            InitializeComponent();
            imageBox1.Image = img1;
        }
        public static void ShowIMGStatic(Emgu.CV.IImage img1)
        {
            ShowSingleIMG form = new ShowSingleIMG(img1);
            form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}