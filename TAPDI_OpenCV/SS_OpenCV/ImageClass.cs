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
                Rectangle aux_rect;
                int x, y;
                Image<Ycc, byte> imgYcc;
                imgYcc = img.Convert<Ycc, byte>();
                Image<Gray, float> Y = imgYcc[0].ConvertScale<float>(1, 0);
                Image<Gray, float> Cb = imgYcc[1].ConvertScale<float>(0.5, 0);
                Image<Gray, float> Cr = imgYcc[2].ConvertScale<float>(0.5, 0);
                Image<Gray, float> Y88 = null;
                Image<Gray, float> Cb88 = null;
                Image<Gray, float> Cr88 = null;


                //percorrer a imagem original em blocos de 8x8
                for (y = 1; y < height; y++)
                {
                    for (x = 1; x < width; x++)
                    {

                        aux_rect = new Rectangle((x - 1) * 8, (y - 1) * 8, x * 8 -1, y * 8 -1);
                        Y88 = Y.Copy(aux_rect);
                        Cb88 = Cb.Copy(aux_rect);
                        Cr88 = Cr.Copy(aux_rect);

                        CvInvoke.cvDCT(Y88, Y88,Emgu.CV.CvEnum.CV_DCT_TYPE.CV_DXT_FORWARD);
                        CvInvoke.cvDCT(Cb88, Cb88, Emgu.CV.CvEnum.CV_DCT_TYPE.CV_DXT_FORWARD);
                        CvInvoke.cvDCT(Cr88, Cr88, Emgu.CV.CvEnum.CV_DCT_TYPE.CV_DXT_FORWARD);

                        CvInvoke.cvDiv(Y88, GetQuantificationMatrix(true,factor), Y88, 1);
                        CvInvoke.cvDiv(Cb88, GetQuantificationMatrix(false, factor), Y88, 1);
                        CvInvoke.cvDiv(Cr88, GetQuantificationMatrix(false, factor), Y88, 1);

                        myRound(Y88);
                        myRound(Cr88);
                        myRound(Cb88);

                        CvInvoke.cvMul(Y88, GetQuantificationMatrix(true, factor), Y88, 1);
                        CvInvoke.cvMul(Y88, GetQuantificationMatrix(false, factor), Y88, 1);
                        CvInvoke.cvMul(Y88, GetQuantificationMatrix(false, factor), Y88, 1);

                        CvInvoke.cvDCT(Y88, Y88, Emgu.CV.CvEnum.CV_DCT_TYPE.CV_DXT_INVERSE);
                        CvInvoke.cvDCT(Cb88, Cb88, Emgu.CV.CvEnum.CV_DCT_TYPE.CV_DXT_INVERSE);
                        CvInvoke.cvDCT(Cr88, Cr88, Emgu.CV.CvEnum.CV_DCT_TYPE.CV_DXT_INVERSE);

                    }
                }

                imgYcc[0] = Y88.ConvertScale<byte>(1, 0);
                imgYcc[1] = Cb88.ConvertScale<byte>(2, 0);
                imgYcc[2] = Cr88.ConvertScale<byte>(2, 0);

                return imgYcc.Convert<Bgr, byte>();

            }
        }


    }
}
