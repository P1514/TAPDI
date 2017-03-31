using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;

namespace SS_OpenCV
{
    class ImageClass
    {
        private static TableForm myTableForm=null;


        /// <summary>
        /// Negativo
        /// </summary>
        /// <param name="img">imagem</param>
        public static void Negative(Image<Bgr, byte> img)
        {
            unsafe
            {
                // acesso directo à memoria da imagem (sequencial)
                // top left to bottom right
                MIplImage m = img.MIplImage;
                byte* dataPtr = (Byte*)m.imageData.ToPointer();
                int h = img.Height;
                int w = img.Width;
                int nC = m.nChannels;
                int lineStep = m.widthStep - m.nChannels * m.width;
                int x, y;
                for (y = 0; y < h; y++)
                {
                    for (x = 0; x < w; x++)
                    {
                        dataPtr[0] = (byte)(255 - dataPtr[0]);
                        dataPtr[1] = (byte)(255 - dataPtr[1]);
                        dataPtr[2] = (byte)(255 - dataPtr[2]);


                        // avança apontador 
                        dataPtr += nC;

                    }

                    //no fim da linha avança alinhamento
                    dataPtr += lineStep;
                }
            }
        }


        /// <summary>
        /// Conversão para Cinzento
        /// </summary>
        /// <param name="img">imagem</param>
        internal static void ConvertToGray(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                // acesso directo à memoria da imagem (sequencial)
                // top left to bottom right
                MIplImage m = img.MIplImage;
                int start = m.imageData.ToInt32();
                byte* dataPtr = (Byte*)start;
                int h = img.Height;
                int w = img.Width;
                int x, y;
                int nC = m.nChannels;
                int wStep = m.widthStep - m.nChannels * m.width;
                byte gray;

                if (nC == 3)
                {
                    for (y = 0; y < h; y++)
                    {
                        for (x = 0; x < w; x++)
                        {
                            // converte BGR para cinza 
                            gray = (byte)Math.Round(((int)dataPtr[0] + dataPtr[1] + dataPtr[2]) / 3d);

                            *dataPtr = gray;
                            *(dataPtr + 1) = gray;
                            *(dataPtr + 2) = gray;

                            // avança apontador 
                            dataPtr += nC;
                        }

                        //no fim da linha avança alinhamento
                        dataPtr += wStep;

                    }
                }
            }

        }

         internal static int[] histogram(Emgu.CV.Image<Bgr, byte> img)
        {
            int[] hist = new int[256];
            for (int i = 0; i < hist.Length; i++) hist[i] = 0;

            unsafe
            {

                // acesso directo à memoria da imagem (sequencial)
                // top left to bottom right
                MIplImage m = img.MIplImage;
                byte* dataPtr = (Byte*)m.imageData.ToPointer();

                int h = img.Height;
                int w = img.Width;
                int nC = m.nChannels;
                int lineStep = m.widthStep - m.nChannels * m.width;
                int x, y; byte gg;
                if (m.nChannels == 3)
                {
                    for (y = 0; y < h; y++)
                    {
                        for (x = 0; x < w; x++)
                        {

                            // converte para cinza
                            gg = (byte)Math.Round(((int)dataPtr[0] + dataPtr[1] + dataPtr[2]) / 3.0);


                            hist[(int)gg]++;


                            // avança apontador 
                            dataPtr += nC;
                        }
                        //no fim da linha avança alinhamento
                        dataPtr += lineStep;
                    }
                }
            }
            return hist;
        }

        public static int[,] Histogram_RGB(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // obter apontador do inicio da imagem
                int[,] array = new int[3, 256];
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // numero de canais 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhamento (padding)
                int x, y;

                if (nChan == 3)
                { // imagem em RGB
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {                            
                            array[0, dataPtr[0]]++;
                            array[1, dataPtr[1]]++;
                            array[2, dataPtr[2]]++;

                            dataPtr += nChan;
                        }
                        dataPtr += padding;
                    }
                }
                return array;
            }
        }

        /// <summary>
        /// Convert to Black and White
        /// </summary>
        /// <param name="img">imagem</param>
        /// <param name="threshold">Threshold value</param>
        internal static void ConvertToBW(Emgu.CV.Image<Bgr, byte> img, int threshold)
        {
            unsafe
            {

                // acesso directo à memoria da imagem (sequencial)
                // top left to bottom right
                MIplImage m = img.MIplImage;
                int start = m.imageData.ToInt32();
                byte* dataPtr = (Byte*)start;
                int h = img.Height;
                int w = img.Width;
                int x, y;
                int nC = m.nChannels;
                int wStep = m.widthStep - m.nChannels * m.width;
                byte gray;

                if (nC == 3)
                {
                    for (y = 0; y < h; y++)
                    {
                        for (x = 0; x < w; x++)
                        {
                            // converte para cinza
                            gray = (byte)Math.Round(((int)dataPtr[0] + dataPtr[1] + dataPtr[2]) / 3d);

                            if (gray < threshold)
                            {
                                dataPtr[0] = 0;
                                dataPtr[1] = 0;
                                dataPtr[2] = 0;
                            }
                            else
                            {
                                dataPtr[0] = 255;
                                dataPtr[1] = 255;
                                dataPtr[2] = 255;
                            }
                            // avança apontador 
                            dataPtr += m.nChannels;
                        }
                        dataPtr += m.widthStep - m.nChannels * m.width;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the Otsu threshold value. Computes the Grey image Histogram
        /// </summary>
        /// <param name="img"></param>
        /// <returns>Threshold Value</returns>
        internal static int OTSU(Emgu.CV.Image<Bgr, byte> img)
        {
            int[] hist = histogram(img);
            int thres = 1, idx=127;
            double u1, u2, q1, q2;
            double nextVar = 0, maxVar = 0;
            double total1 = 0, total2 = 0, ponderada = 0;
            double[] outV = new double[256];

            while (  (thres < 255)) {
                if (maxVar <= nextVar)
                {
                  idx = thres;
                  maxVar = nextVar;
                }

                total1 = 0;
                ponderada = 0;
                for ( int n = 0 ; n < thres ; n++ )  {
	                total1 += hist[n];
  	                ponderada += n*hist[n];
                }
                if (total1 == 0)
                  u1 = thres;
                else
                  u1 = ponderada / (float)total1;

                total2 = 0;
                ponderada = 0;
                for ( int n = thres ; n <= 255 ; n++ )  {
	                total2 += hist[n];
  	                ponderada += n*hist[n];
                }
                if (total2 == 0)
                  u2 = thres;
                else
                  u2 = ponderada / (float)total2 ;

                q1 = total1 /(total1 + total2);
                q2 = total2 /(total1 + total2);

                nextVar = (q1*q2) * Math.Pow(u1-u2,2);
                outV[(int)thres]=nextVar;
                thres++;

            }

            histForm ff = new histForm(outV);
            ff.ShowDialog();

            return idx;
        }

        internal static void myRound(Image<Gray, float> img)
        {
            unsafe
            {
                // acesso directo à memoria da imagem (sequencial)
                // top left to bottom right
                MIplImage m = img.MIplImage;
                int start = m.imageData.ToInt32();
                float* dataPtr = (float*)start;
                int h = img.Height;
                int w = img.Width;
                int x, y;
                int nC = m.nChannels;
                int wStep = m.widthStep - m.nChannels * m.width * sizeof(float);
                byte gray;

                for (y = 0; y < h; y++)
                {
                    for (x = 0; x < w; x++)
                    {
                        // converte BGR para cinza 
                        *dataPtr = (float)Math.Round((double)*dataPtr);
                        // avança apontador 
                        dataPtr++;
                    }
                    //no fim da linha avança alinhamento
                    dataPtr += wStep;
                }
            }
        }

        public static Image<Gray, float> GetQuantificationMatrix(bool LuminanceOrChrominance, int compfactor)
        {
            float[] LumQuant = {   16,11,10,16,24,40,51,61,
                                12,12,14,19,26,58,60,55,
                                14,13,16,24,40,57,69,56,
                                14,17,22,29,51,87,80,62,
                                18,22,37,56,68,109,103,77,
                                24,35,55,64,81,104,113,92,
                                49,64,78,87,103,121,120,101,
                                72,92,95,98,112,100,103,99};

            float[] ChrQuant = {17,18,24,47,99,99,99,99,
                                18,21,26,66,99,99,99,99,
                                24,26,56,99,99,99,99,99,
                                47,66,99,99,99,99,99,99,
                                99,99,99,99,99,99,99,99,
                                99,99,99,99,99,99,99,99,
                                99,99,99,99,99,99,99,99,
                                99,99,99,99,99,99,99,99
                                };
            
            Image<Gray, float> matrix = new Image<Gray, float>(8, 8);
            int idx = 0;
            float[] Quant = (LuminanceOrChrominance) ? LumQuant : ChrQuant;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    matrix[y, x] = new Gray(Quant[idx++] * 100 / compfactor);
                }
            }
            return matrix;
        }

        internal static double[] entropia(Emgu.CV.Image<Bgr, byte> img)
        {

            int[,] var = ImageClass.Histogram_RGB(img); //vector com valores RGB
            long n_total = img.Width * img.Height; //numero de pixeis da imagem
            double[] ent = new double[3]; // vector para a entropia de cada cor
            int aux = var.Length / 3;
            for (int i = 0; i < aux; i++)
            {
                int ch = 0;
                double p = var[ch, i] / (double)n_total; // calculo do valor de pi
                ent[ch] += p!=0 ?p * (Math.Log10(p) / Math.Log10(2)):0; //se pi |= de 0 entao calcular o log
                ch++;
                p = var[ch, i] / (double) n_total;
                ent[ch] += p!=0?p * (Math.Log10(p) / Math.Log10(2)):0;
                ch++;
                p = var[ch, i] / (double) n_total;
                ent[ch] += p!=0?p * (Math.Log10(p) / Math.Log10(2)):0;

            }
            //transformar a entropia de negativa para positiva
            ent[0] = Math.Abs(ent[0]);
            ent[1] = Math.Abs(ent[1]);
            ent[2] = Math.Abs(ent[2]);
            return ent;
        }


        internal static Image<Bgr, byte> CompressJPEG(Image<Bgr, byte> img, int factor)
        {
            unsafe
            {

                MIplImage m = img.MIplImage;
                int width = (int) Math.Floor(img.Width/(double)8);      // largura da imagem em blocos de 8
                int height = (int)Math.Floor(img.Height/(double)8);    // altura da imagem
                img.Resize(width * 8, height * 8, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);// Resize imagem original para 8x8
                int x, y;
                Image<Ycc, float> imgYcc;
                imgYcc = img.Convert<Bgr, float>().Convert<Ycc,float>();
                Image<Gray, float> Y = imgYcc[0].ConvertScale<float>(1, 0);
                Image<Gray, float> Cb = imgYcc[1].ConvertScale<float>(1, 0);
                Image<Gray, float> Cr = imgYcc[2].ConvertScale<float>(1, 0);


                Image<Gray, float> Y88 = new Image<Gray, float>(8,8);
                Image<Gray, float> Cb88 = new Image<Gray, float>(8, 8);
                Image<Gray, float> Cr88 = new Image<Gray, float>(8, 8);
                Image<Ycc, float> blockYCC = new Image<Ycc, float>(8, 8);


                //percorrer a imagem original em blocos de 8x8
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        Y88 = Y.Copy(new Rectangle(x * 8, y * 8, 8, 8));
                        Cb88 = Cb.Copy(new Rectangle(x * 8, y * 8, 8, 8));
                        Cr88 = Cr.Copy(new Rectangle(x * 8, y * 8, 8, 8));

                        CvInvoke.cvDCT(Y88, Y88, Emgu.CV.CvEnum.CV_DCT_TYPE.CV_DXT_FORWARD);
                        CvInvoke.cvDCT(Cb88, Cb88, Emgu.CV.CvEnum.CV_DCT_TYPE.CV_DXT_FORWARD);
                        CvInvoke.cvDCT(Cr88, Cr88, Emgu.CV.CvEnum.CV_DCT_TYPE.CV_DXT_FORWARD);

                        CvInvoke.cvDiv(Y88, GetQuantificationMatrix(true, factor), Y88, 1);
                        CvInvoke.cvDiv(Cb88, GetQuantificationMatrix(false, factor), Cb88, 1);
                        CvInvoke.cvDiv(Cr88, GetQuantificationMatrix(false, factor), Cr88, 1);

                        myRound(Y88);
                        myRound(Cr88);
                        myRound(Cb88);

                        CvInvoke.cvMul(Y88, GetQuantificationMatrix(true, factor), Y88, 1);
                        CvInvoke.cvMul(Cb88, GetQuantificationMatrix(false, factor), Cb88, 1);
                        CvInvoke.cvMul(Cr88, GetQuantificationMatrix(false, factor), Cr88, 1);
                        
                        CvInvoke.cvDCT(Y88, Y88, Emgu.CV.CvEnum.CV_DCT_TYPE.CV_DXT_INVERSE);
                        CvInvoke.cvDCT(Cb88, Cb88, Emgu.CV.CvEnum.CV_DCT_TYPE.CV_DXT_INVERSE);
                        CvInvoke.cvDCT(Cr88, Cr88, Emgu.CV.CvEnum.CV_DCT_TYPE.CV_DXT_INVERSE);
                      
                        //copy the processed block to the image
                        imgYcc.ROI = new Rectangle(x * 8, y * 8, 8, 8);

                        // merge individual channels into one single Ycc image
                        CvInvoke.cvMerge(Y88, Cb88, Cr88, IntPtr.Zero, blockYCC);

                        // copy to final image
                        blockYCC.CopyTo(imgYcc);
                        imgYcc.ROI = Rectangle.Empty;


                    }
                }

                return imgYcc.Convert<Bgr, float>().Convert<Bgr,byte>();

            }
        }


        /// <summary>
        /// Compute image connected components 
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        internal static Image<Gray, int> GetConnectedComponents(Image<Bgr, byte> img)
        {
            Image<Gray, byte> imgThresh = img.Convert<Gray, byte>();
            CvInvoke.cvThreshold(imgThresh, imgThresh, 0, 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY | Emgu.CV.CvEnum.THRESH.CV_THRESH_OTSU);

            ShowSingleIMG.ShowIMGStatic(imgThresh);

            Contour<Point> contours = imgThresh.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_CCOMP);
            Image<Gray, int> labelsImg = new Image<Gray, int>(imgThresh.Size);
            int count = 1;

            while (contours != null)
            {
                labelsImg.Draw(contours, new Gray(count), -1);
                labelsImg.Draw(contours, new Gray(-10), 1);
                contours = contours.HNext;
                count++;
            }

            return labelsImg;
        }



        /// <summary>
        /// Watershed with labels (Meyer)
        /// </summary>
        /// <param name="img"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        internal static Image<Gray, int> GetWatershedFromLabels(Image<Bgr, byte> img, Image<Gray, byte> labels)
        {
            Image<Gray, int> watershedAux = labels.Convert<Gray, int>();

            CvInvoke.cvWatershed(img, watershedAux);

            return watershedAux;
        }


        /// <summary>
        /// Watershed By Immersion (Vincent and Soille)
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        internal static Image<Gray, int> GetWatershedByImmersion(Image<Bgr, byte> img)
        {
            WatershedGrayscale wt = new WatershedGrayscale();
            Image<Gray, byte> wtImg = img.Convert<Gray, byte>();
            wt.ProcessFilter(wtImg.Not());
            wtImg.SetZero();
            Image<Gray, int> wtOutimg = new Image<Gray, int>(img.Size);
            wt.DrawWatershedLines(wtOutimg);

            return wtOutimg;
        }

        /// <summary>
        /// Get Gradient Path Labelling (GPL) segmnentation 
        /// </summary>
        /// <param name="img">image</param>
        /// <returns></returns>
        internal static Image<Bgr, byte> GetGPL(Image<Bgr, byte> img)
        {
            GPL_lib.GPL_lib gpl = new GPL_lib.GPL_lib(img, false);

            gpl.ShowConfigForm();

            return gpl.GetImage();
        }

        public class WatershedPixel
        {
            public int X;
            public int Y;
            public int Height;
            // labels the pixel as belonging to a unique basin or as a part of the watershed line
            public int Label;
            // Distance is a work image of distances
            public int Distance;

            public WatershedPixel()
            {
                this.X = -1;
                this.Y = -1;
                this.Height = -1000;
                this.Label = -1000;
                this.Distance = -1000;
            }

            public WatershedPixel(int x, int y)
            {
                this.X = x;
                this.Y = y;
                this.Height = -1000;
                this.Label = WatershedCommon.INIT;
                this.Distance = 0;
            }

            public WatershedPixel(int x, int y, int height)
            {
                this.X = x;
                this.Y = y;
                this.Height = height;
                this.Label = WatershedCommon.INIT;
                this.Distance = 0;
            }

            public override bool Equals(Object obj)
            {
                WatershedPixel p = (WatershedPixel)obj;
                return (X == p.X && X == p.Y);
            }

            public override int GetHashCode()
            {
                return X;
            }
            public override string ToString()
            {
                return "Height = " + Height + "; X = " + X.ToString() + ", Y = " + Y.ToString() +
                       "; Label = " + Label.ToString() + "; Distance = " + Distance.ToString();
            }
        }

        public class WatershedGrayscale
        {
            #region Variables
            private WatershedPixel FictitiousPixel = new WatershedPixel();
            private int _currentLabel = 0;
            private int _currentDistance = 0;
            private FifoQueue _fifoQueue = new FifoQueue();
            // each pixel can be accesesd from 2 places: a dictionary for faster direct lookup of neighbouring pixels 
            // or from a height ordered list
            // sorted array of pixels according to height
            private List<List<WatershedPixel>> _heightSortedList;
            // string in the form "X,Y" is used as a key for the dictionary lookup of a pixel
            private Dictionary<string, WatershedPixel> _pixelMap;
            private int _watershedPixelCount = 0;
            private int _numberOfNeighbours = 8;
            private bool _borderInWhite;
            private int _pictureWidth = 0;
            private int _pictureHeight = 0;
            #endregion

            #region Constructors
            public WatershedGrayscale()
                : this(8)
            { }

            public WatershedGrayscale(int numberOfNeighbours)
            {
                if (numberOfNeighbours != 8 && numberOfNeighbours != 4)
                    throw new Exception("Invalid number of neighbour pixels to check. Valid values are 4 and 8.");
                _borderInWhite = true;
                _numberOfNeighbours = numberOfNeighbours;
                _heightSortedList = new List<List<WatershedPixel>>(256);
                for (int i = 0; i < 256; i++)
                    _heightSortedList.Add(new List<WatershedPixel>());
            }
            #endregion

            #region Properties
            /// <summary>
            /// number of neighbours to check for each pixel. valid values are 8 and 4
            /// </summary>
            public int NumberOfNeighbours
            {
                get { return _numberOfNeighbours; }
                set
                {
                    if (value != 8 && value != 4)
                        throw new Exception("Invalid number of neighbour pixels to check. Valid values are 4 and 8.");
                    _numberOfNeighbours = value;
                }
            }

            /// <summary>
            /// Number of labels/basins found
            /// </summary>
            public int LabelCount
            {
                get { return _currentLabel; }
                set { _currentLabel = value; }
            }

            /// <summary>
            /// True: border is drawn in white. False: border is drawn in black
            /// </summary>
            /// <value></value>
            public bool BorderInWhite
            {
                get { return _borderInWhite; }
                set { _borderInWhite = value; }
            }
            #endregion

            private void CreatePixelMapAndHeightSortedArray(Image<Gray, byte> wtImg)
            {
                MIplImage data = wtImg.MIplImage;
                _pictureWidth = data.width;
                _pictureHeight = data.height;
                // pixel map holds every pixel thus size of (_pictureWidth * _pictureHeight)
                _pixelMap = new Dictionary<string, WatershedPixel>(_pictureWidth * _pictureHeight);
                unsafe
                {
                    int offset = data.widthStep - data.width;
                    byte* ptr = (byte*)(data.imageData);

                    // get histogram of all values in grey = height
                    for (int y = 0; y < data.height; y++)
                    {
                        for (int x = 0; x < data.width; x++, ptr++)
                        {
                            WatershedPixel p = new WatershedPixel(x, y, *ptr);
                            // add every pixel to the pixel map
                            _pixelMap.Add(p.X.ToString() + "," + p.Y.ToString(), p);
                            _heightSortedList[*ptr].Add(p);
                        }
                        ptr += offset;
                    }
                }
                this._currentLabel = 0;
            }

            private void Segment()
            {
                // Geodesic SKIZ (skeleton by influence zones) of each level height
                for (int h = 0; h < _heightSortedList.Count; h++)
                {
                    // get all pixels for current height
                    foreach (WatershedPixel heightSortedPixel in _heightSortedList[h])
                    {
                        heightSortedPixel.Label = WatershedCommon.MASK;
                        // for each pixel on current height get neighbouring pixels
                        List<WatershedPixel> neighbouringPixels = GetNeighbouringPixels(heightSortedPixel);
                        // initialize queue with neighbours at level h of current basins or watersheds
                        foreach (WatershedPixel neighbouringPixel in neighbouringPixels)
                        {
                            if (neighbouringPixel.Label > 0 || neighbouringPixel.Label == WatershedCommon.WSHED)
                            {
                                heightSortedPixel.Distance = 1;
                                _fifoQueue.AddToEnd(heightSortedPixel);
                                break;
                            }
                        }
                    }
                    _currentDistance = 1;
                    _fifoQueue.AddToEnd(FictitiousPixel);
                    // extend basins
                    while (true)
                    {
                        WatershedPixel p = _fifoQueue.RemoveAtFront();
                        if (p.Equals(FictitiousPixel))
                        {
                            if (_fifoQueue.IsEmpty)
                                break;
                            else
                            {
                                _fifoQueue.AddToEnd(FictitiousPixel);
                                _currentDistance++;
                                p = _fifoQueue.RemoveAtFront();
                            }
                        }
                        List<WatershedPixel> neighbouringPixels = GetNeighbouringPixels(p);
                        // labelling p by inspecting neighbours
                        foreach (WatershedPixel neighbouringPixel in neighbouringPixels)
                        {
                            // neighbouringPixel belongs to an existing basin or to watersheds
                            // in the original algorithm the condition is:
                            //   if (neighbouringPixel.Distance < currentDistance && 
                            //      (neighbouringPixel.Label > 0 || neighbouringPixel.Label == WatershedCommon.WSHED))
                            //   but this returns incomplete borders so the this one is used                        
                            if (neighbouringPixel.Distance <= _currentDistance &&
                               (neighbouringPixel.Label > 0 || neighbouringPixel.Label == WatershedCommon.WSHED))
                            {
                                if (neighbouringPixel.Label > 0)
                                {
                                    // the commented condition is also in the original algorithm 
                                    // but it also gives incomplete borders
                                    if (p.Label == WatershedCommon.MASK /*|| p.Label == WatershedCommon.WSHED*/)
                                        p.Label = neighbouringPixel.Label;
                                    else if (p.Label != neighbouringPixel.Label)
                                    {
                                        p.Label = WatershedCommon.WSHED;
                                        _watershedPixelCount++;
                                    }
                                }
                                else if (p.Label == WatershedCommon.MASK)
                                {
                                    p.Label = WatershedCommon.WSHED;
                                    _watershedPixelCount++;
                                }
                            }
                            // neighbouringPixel is plateau pixel
                            else if (neighbouringPixel.Label == WatershedCommon.MASK && neighbouringPixel.Distance == 0)
                            {
                                neighbouringPixel.Distance = _currentDistance + 1;
                                _fifoQueue.AddToEnd(neighbouringPixel);
                            }
                        }
                    }
                    // detect and process new minima at height level h
                    foreach (WatershedPixel p in _heightSortedList[h])
                    {
                        // reset distance to zero
                        p.Distance = 0;
                        // if true then p is inside a new minimum 
                        if (p.Label == WatershedCommon.MASK)
                        {
                            // create new label
                            _currentLabel++;
                            p.Label = _currentLabel;
                            _fifoQueue.AddToEnd(p);
                            while (!_fifoQueue.IsEmpty)
                            {
                                WatershedPixel q = _fifoQueue.RemoveAtFront();
                                // check neighbours of q
                                List<WatershedPixel> neighbouringPixels = GetNeighbouringPixels(q);
                                foreach (WatershedPixel neighbouringPixel in neighbouringPixels)
                                {
                                    if (neighbouringPixel.Label == WatershedCommon.MASK)
                                    {
                                        neighbouringPixel.Label = _currentLabel;
                                        _fifoQueue.AddToEnd(neighbouringPixel);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            private List<WatershedPixel> GetNeighbouringPixels(WatershedPixel centerPixel)
            {
                List<WatershedPixel> temp = new List<WatershedPixel>();
                if (_numberOfNeighbours == 8)
                {
                    /*
                    CP = Center pixel
                    (X,Y) -- get all 8 connected 
                    |-1,-1|0,-1|1,-1|
                    |-1, 0| CP |1, 0|
                    |-1,+1|0,+1|1,+1|
                    */
                    // -1, -1                
                    if ((centerPixel.X - 1) >= 0 && (centerPixel.Y - 1) >= 0)
                        temp.Add(_pixelMap[(centerPixel.X - 1).ToString() + "," + (centerPixel.Y - 1).ToString()]);
                    //  0, -1
                    if ((centerPixel.Y - 1) >= 0)
                        temp.Add(_pixelMap[centerPixel.X.ToString() + "," + (centerPixel.Y - 1).ToString()]);
                    //  1, -1
                    if ((centerPixel.X + 1) < _pictureWidth && (centerPixel.Y - 1) >= 0)
                        temp.Add(_pixelMap[(centerPixel.X + 1).ToString() + "," + (centerPixel.Y - 1).ToString()]);
                    // -1, 0
                    if ((centerPixel.X - 1) >= 0)
                        temp.Add(_pixelMap[(centerPixel.X - 1).ToString() + "," + centerPixel.Y.ToString()]);
                    //  1, 0
                    if ((centerPixel.X + 1) < _pictureWidth)
                        temp.Add(_pixelMap[(centerPixel.X + 1).ToString() + "," + centerPixel.Y.ToString()]);
                    // -1, 1
                    if ((centerPixel.X - 1) >= 0 && (centerPixel.Y + 1) < _pictureHeight)
                        temp.Add(_pixelMap[(centerPixel.X - 1).ToString() + "," + (centerPixel.Y + 1).ToString()]);
                    //  0, 1
                    if ((centerPixel.Y + 1) < _pictureHeight)
                        temp.Add(_pixelMap[centerPixel.X.ToString() + "," + (centerPixel.Y + 1).ToString()]);
                    //  1, 1
                    if ((centerPixel.X + 1) < _pictureWidth && (centerPixel.Y + 1) < _pictureHeight)
                        temp.Add(_pixelMap[(centerPixel.X + 1).ToString() + "," + (centerPixel.Y + 1).ToString()]);
                }
                else
                {
                    /*
                    CP = Center pixel, N/A = not used
                    (X,Y) -- get only 4 connected 
                    | N/A |0,-1| N/A |
                    |-1, 0| CP |+1, 0|
                    | N/A |0,+1| N/A |
                    */
                    //  -1, 0
                    if ((centerPixel.X - 1) >= 0)
                        temp.Add(_pixelMap[(centerPixel.X - 1).ToString() + "," + centerPixel.Y.ToString()]);
                    //  0, -1
                    if ((centerPixel.Y - 1) >= 0)
                        temp.Add(_pixelMap[centerPixel.X.ToString() + "," + (centerPixel.Y - 1).ToString()]);
                    //  1, 0
                    if ((centerPixel.X + 1) < _pictureWidth)
                        temp.Add(_pixelMap[(centerPixel.X + 1).ToString() + "," + centerPixel.Y.ToString()]);
                    //  0, 1
                    if ((centerPixel.Y + 1) < _pictureHeight)
                        temp.Add(_pixelMap[centerPixel.X.ToString() + "," + (centerPixel.Y + 1).ToString()]);
                }
                return temp;
            }

            public void DrawWatershedLines(Image<Gray, int> wtImg)
            {
                MIplImage data = wtImg.MIplImage;
                if (_watershedPixelCount == 0)
                    return;

                int watershedColor = -1;

                unsafe
                {
                    int offset = data.widthStep - data.width * sizeof(int);
                    int* ptr = (int*)(data.imageData);

                    for (int y = 0; y < data.height; y++)
                    {
                        for (int x = 0; x < data.width; x++, ptr++)
                        {
                            // if the pixel in our map is watershed pixel then draw it
                            if (_pixelMap[x.ToString() + "," + y.ToString()].Label == WatershedCommon.WSHED)
                                *ptr = watershedColor;
                            else
                                *ptr = _pixelMap[x.ToString() + "," + y.ToString()].Label;
                        }
                        ptr += offset;
                    }
                }
            }

            public void ProcessFilter(Image<Gray, byte> wtImg)
            {
                CreatePixelMapAndHeightSortedArray(wtImg);
                Segment();
                //DrawWatershedLines(wtImg);
            }
        }

        public class FifoQueue
        {
            List<WatershedPixel> queue = new List<WatershedPixel>();

            public int Count
            {
                get { return queue.Count; }
            }

            public void AddToEnd(WatershedPixel p)
            {
                queue.Add(p);
            }

            public WatershedPixel RemoveAtFront()
            {
                WatershedPixel temp = queue[0];
                queue.RemoveAt(0);
                return temp;
            }

            public bool IsEmpty
            {
                get { return (queue.Count == 0); }
            }

            public override string ToString()
            {
                return base.ToString() + " Count = " + queue.Count.ToString();
            }
        }

        public class WatershedCommon
        {
            #region Constants
            public const int INIT = -1;
            public const int MASK = -2;
            public const int WSHED = 0;
            #endregion
        }


    }
}
