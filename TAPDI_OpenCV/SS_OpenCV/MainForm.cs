using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using System.IO;
using Emgu.CV.CvEnum;

namespace SS_OpenCV
{
    public partial class MainForm : Form
    {
        Image<Bgr, byte> img = null;
        Image<Bgr, byte> imgUndo = null;
        Image<Bgr, byte> imgOriginal = null;
        Capture imgVideo = null;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Abrir uma nova imagem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Bgr, byte>(openFileDialog1.FileName);
                imgUndo = img.Copy();
                ImageApl.Image = img;
                imgOriginal = img.Copy();
                ImageApl.Refresh();
            }
        }

        /// <summary>
        /// Guardar a imagem com novo nome
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageApl.Image.Save(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Fecha a aplicação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void autoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthorsForm form = new AuthorsForm();
            form.ShowDialog();
        }

        /// <summary>
        /// repoe a ultima copia da imagem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            img = imgUndo.Copy();
            ImageApl.Image = img;
            ImageApl.Refresh();
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Altera o modo de vizualização
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // zoom
            if (autoZoomToolStripMenuItem.Checked)
            {
                ImageApl.SizeMode = PictureBoxSizeMode.Zoom;
                ImageApl.Dock = DockStyle.Fill;
            }
            else // com scroll bars
            {
                ImageApl.Dock = DockStyle.None;
                ImageApl.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        /// <summary>
        /// Efectua o negativo da imagem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Negative(img);

            ImageApl.Refresh();
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Converte a imagem para tons de cinzento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            //copy Undo Image
            imgUndo = img.Copy();
            
            ImageClass.ConvertToGray(img);

            ImageApl.Refresh();
            Cursor = Cursors.Default;
        }


        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            //copy Undo Image
            imgUndo = img.Copy();

            int[] hist = ImageClass.histogram(img);
            histForm histF = new histForm(hist, "Histograma");

            histF.ShowDialog();

            ImageApl.Refresh();
            Cursor = Cursors.Default; 

        }

        private void bWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox iForm = new InputBox();
            iForm.ShowDialog();

            int i = Convert.ToInt32(iForm.ValueTextBox.Text);

            ImageClass.ConvertToBW(img, i);

            ImageApl.Refresh();
            Cursor = Cursors.Default; 
        }

        private void otsuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            //copy Undo Image
            imgUndo = img.Copy();

            int i = ImageClass.OTSU(img);
            MessageBox.Show("Otsu: " + i.ToString());
            ImageClass.ConvertToBW(img, i);

            ImageApl.Refresh();
            Cursor = Cursors.Default; 
        }


        private void saveHistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            //copy Undo Image
            imgUndo = img.Copy();

            int[] hist = ImageClass.histogram(img);
            string[] histSrt = new string[hist.Length];
            for (int i = 0; i < hist.Length;i++)
            {
                histSrt[i]=hist[i].ToString();
            }

            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(saveFileDialog2.FileName, histSrt);
            
            }

            ImageApl.Refresh();
            Cursor = Cursors.Default; 
        }

        private void meanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            //copy Undo Image
            imgUndo = img.Copy();

            img = img.SmoothBlur(3, 3);
            ImageApl.Image = img;

            ImageApl.Refresh();
            Cursor = Cursors.Default; 


        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            TableForm.ShowTable(img[2], "Red");
            

            Cursor = Cursors.Default; 
        }


        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            TableForm.ShowTable(img[1], "Green");


            Cursor = Cursors.Default; 
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            TableForm.ShowTable(img[0], "Blue");


            Cursor = Cursors.Default; 
        }

        private void compressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (img == null)
                return;
            imgUndo = img.Copy();
            int[] hist_array = new int[img.Height];
            hist_array =  ImageClass.histogram(imgUndo);
            CompressionTableForm From_ = new CompressionTableForm(hist_array, imgUndo);

            From_.Show();
            
        }

        private void entropiaToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (img == null)
                return;
            imgUndo = img.Copy();
            double[] var = ImageClass.entropia(imgUndo);
            Console.Out.WriteLine(" BLUE: " + var[0] + " / GREEN: " + var[1] + " /  RED:" + var[2]); //blue ; green ; red

        }

        private void aula2ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void compressToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;

            Cursor = Cursors.WaitCursor; // bloquear cursor 
            imgUndo = img.Copy();
            InputBox formX = new InputBox("Compression factor for imagem");
            formX.ShowDialog();

            //int, double ou float ???
            int factor = Convert.ToInt32(formX.ValueTextBox.Text);

            Image<Bgr, byte> imgJPEG = ImageClass.CompressJPEG(img, factor);
            ShowIMG.ShowIMGStatic(imgUndo, imgJPEG);
          

            Cursor = Cursors.Default; // cursor normal
        }
    }
}