using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZedGraph;


namespace SS_OpenCV
{
    public partial class histForm : Form
    {
        public histForm(int [] array, string title)
        {
            InitializeComponent();

            
            // get a reference to the GraphPane
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the Titles
            myPane.Title.Text = title;
            myPane.XAxis.Title.Text = "Intensidade";
            myPane.YAxis.Title.Text = "Contagem";

            //list points
            PointPairList list1 = new PointPairList();

            for (int i = 0; i < array.Length; i++)
            {
                list1.Add(i, array[i]);
            }

            myPane.AddBar("imagem",list1, Color.Red);
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = array.Length;
            myPane.Legend.IsVisible = false;

            zedGraphControl1.AxisChange();
        }
        public histForm(double[] array)
        {
            InitializeComponent();


            // get a reference to the GraphPane
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the Titles
            myPane.Title.Text = "Histograma";
            myPane.XAxis.Title.Text = "Intensidade";
            myPane.YAxis.Title.Text = "Contagem";

            //list points
            PointPairList list1 = new PointPairList();

            for (int i = 0; i < array.Length; i++)
            {
                list1.Add(i, array[i]);
            }

            myPane.AddBar("imagem", list1, Color.Red);
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 255;

            zedGraphControl1.AxisChange();
        }
    }
}