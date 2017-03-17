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
        

    
    }
}
