using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace Filters
{
    public abstract class Filter
    {
        public abstract Color calculateNewPixelColor(Bitmap sourceImage, int i , int j);
        public virtual Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker) 
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++) 
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if(worker.CancellationPending)
                {
                    return null;
                }

                for(int j = 0; j < sourceImage.Height; j++) 
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }  

        protected int Clamp(int value, int min, int max) 
        {   
            if(value < min) 
            { 
                return min;
            }
            if(value > max) 
            { 
                return max;
            }
            return value;
        }
    }

    public class InvertFilter: Filter 
    {
        public override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            Color sourceColor = sourceImage.GetPixel(i, j);
            Color resultColor = Color.FromArgb(255 - sourceColor.R,
                                                255 - sourceColor.G,
                                                255 - sourceColor.B);

            return resultColor;
        }
    }

    public class GreyScaleFilter: Filter 
    {
        public override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            Color sourceColor = sourceImage.GetPixel(i, j);
            int Intensity = (int)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B);
            return Color.FromArgb(Clamp(Intensity,0,255),
                                    Clamp(Intensity, 0, 255),
                                    Clamp(Intensity, 0, 255));
        }
    }

    public class SepiaFilter: Filter 
    {
        public override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            Color sourceColor = sourceImage.GetPixel(i, j);
            int Intensity = (int)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B);
            int k = 20;
            return Color.FromArgb(Clamp(Intensity + 2*k, 0, 255),
                                    Clamp(Intensity + k/2, 0, 255),
                                    Clamp(Intensity - k, 0, 255));
        }
    }

    public class SepiaFilterElips : Filter
    {
        public override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            if ( Math.Pow((i-sourceImage.Width / 2),2)/Math.Pow((sourceImage.Height/2),2) +
                Math.Pow((j - sourceImage.Height / 2), 2) / Math.Pow((sourceImage.Height / 2), 2) < 1)
            {
                Color sourceColor = sourceImage.GetPixel(i, j);
                int Intensity = (int)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B);
                int k = 20;
                return Color.FromArgb(Clamp(Intensity + 2 * k, 0, 255),
                                        Clamp(Intensity + k / 2, 0, 255),
                                        Clamp(Intensity - k, 0, 255));
            }
            else 
            {
                return sourceImage.GetPixel(i, j);
            }
        }
    }

    public class IncreaseBrightnessFilter: Filter 
    {
        public override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            Color sourceColor = sourceImage.GetPixel(i, j);
            int b = 50;
            return Color.FromArgb(Clamp(sourceColor.R + b, 0, 255),
                                   Clamp(sourceColor.G + b, 0, 255),
                                   Clamp(sourceColor.B + b, 0, 255));
        }
    }

    public class GrayWorldFilter : Filter
    {
        double Avg = 0;
        double avgR = 0;
        double avgG = 0;
        double avgB = 0;

        public override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            Color sourceColor = sourceImage.GetPixel(i, j);
            return Color.FromArgb(Clamp((int)((double)sourceColor.R * (Avg / avgR)), 0, 255),
                                   Clamp((int)((double)sourceColor.G * (Avg / avgG)), 0, 255),
                                   Clamp((int)((double)sourceColor.B * (Avg / avgB)), 0, 255));
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);
                    avgR += sourceColor.R;
                    avgG += sourceColor.G;
                    avgB += sourceColor.B;
                }
            }
            avgR /= sourceImage.Width * sourceImage.Height;
            avgG /= sourceImage.Height * sourceImage.Width; 
            avgB /= sourceImage.Width * sourceImage.Height;
            Avg = (avgR+avgG+avgB)/3;

            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                {
                    return null;
                }

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }
    }

    class LinearCorrectionFilter : Filter
    {
        int Rmin = 255;
        int Rmax = 0;
        int Gmin = 255;
        int Gmax = 0;
        int Bmin = 255;
        int Bmax = 0;

        public override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            Color sourceColor = sourceImage.GetPixel(i, j);
            return Color.FromArgb(Clamp((int)((double)(sourceColor.R - Rmin) * (255.0 / (double)(Rmax - Rmin))), 0, 255),
                                   Clamp((int)((double)(sourceColor.G - Gmin) * (255.0 / (double)(Gmax - Gmin))), 0, 255),
                                   Clamp((int)((double)(sourceColor.B - Bmin) * (255.0 / (double)(Bmax - Bmin))), 0, 255));
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);
                    if(sourceColor.R > Rmax) { Rmax = sourceColor.R; }
                    if (sourceColor.R < Rmin){ Rmin = sourceColor.R; }
                    if (sourceColor.G > Gmax) { Gmax = sourceColor.G; }
                    if (sourceColor.G < Gmin) { Gmin = sourceColor.G; }
                    if (sourceColor.B > Bmax) { Bmax = sourceColor.B; }
                    if (sourceColor.B < Bmin) { Bmin = sourceColor.B; }
                }
            }

            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                {
                    return null;
                }

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }
    }

    public class ShiftFilter: Filter 
    {
        public override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            int l = 30;
            if (i-l < 0) 
            {
                return Color.Black;
            }
            Color sourceColor = sourceImage.GetPixel(i-l, j);
            return Color.FromArgb(sourceColor.R,
                                    sourceColor.G,
                                    sourceColor.B);
        }
    }

    public class GlassEffectFilter: Filter
    {
        Random r = new Random();
        public override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            Color sourceColor = sourceImage.GetPixel(Clamp(i - (int)((r.NextDouble() - 0.5) * 10),
                                                     0, sourceImage.Width - 1),
                                                     Clamp(j - (int)((r.NextDouble() - 0.5) * 10),
                                                     0, sourceImage.Height - 1));
            return Color.FromArgb(sourceColor.R,
                                    sourceColor.G,
                                    sourceColor.B);
        }
    }

    public class QuantizationFilter: Filter 
    {
        public override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            Color sourceColor = sourceImage.GetPixel(i, j);
            int Intensity = (int)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B);
            Intensity = 32 * (Intensity / 32 + 1);
            return Color.FromArgb(Clamp(Intensity, 0, 255),
                                    Clamp(Intensity, 0, 255),
                                    Clamp(Intensity, 0, 255));
        }
    }

    public class MatrixFilter: Filter 
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }

        public MatrixFilter(float[,] kernel) 
        {
            this.kernel = kernel;   
        }
        public override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;  
            float resultB = 0; 
            for(int l = - radiusY; l <= radiusY; l++) 
            {
                for(int k = -radiusX; k <= radiusX; k++) 
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighbourColor = sourceImage.GetPixel(idX, idY);
                    resultR += neighbourColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighbourColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighbourColor.B * kernel[k + radiusX, l + radiusY];
                }   
            }
            return Color.FromArgb(Clamp((int)resultR, 0, 255),
                                    Clamp((int)resultG, 0, 255),
                                    Clamp((int)resultB, 0, 255));
        }
    }

    public class MirrorFilter: Filter 
    {
        public override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            if (i < sourceImage.Width/2) 
            {
                return sourceImage.GetPixel(sourceImage.Width - 1 - i, j);
            }
            return sourceImage.GetPixel(i, j);
        }
    }

    public class BlurFilter: MatrixFilter 
    {
        public BlurFilter() 
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX ,sizeY];
            for(int i = 0; i < sizeX; i++) 
            {
                for(int j = 0; j < sizeY; j++) 
                {
                    kernel[i, j] = 1.0f / ((float)sizeX * sizeY);
                }
            }
        }
    }

    public class GaussianFilter: MatrixFilter 
    {
        public void createGaussianKernel(int radius, float sigma) 
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0.0f;

            for(int i = - radius; i <= radius; i++) 
            {
                for (int j = -radius; j <= radius; j++) 
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i*i + j*j)/(2*sigma*sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            }
            
            for(int i = 0; i < size; i++) 
            {
                for(int j = 0; j < size; j++) 
                {
                    kernel[i, j] /= norm;
                }
            }
        }

        public GaussianFilter() 
        {
            createGaussianKernel(3, 2);
        }
    }

    public class SobelOperatorFilter: MatrixFilter 
    {
        float[,] kernelX;
        float[,] kernelY;
        public SobelOperatorFilter() 
        {
            kernelY = new[,] { { -1.0f, -2.0f, -1.0f }, { 0.0f, 0.0f, 0.0f }, { 1.0f, 2.0f, 1.0f } };
            kernelX = new[,] { { -1.0f, 0.0f, 1.0f }, { -2.0f, 0.0f, 2.0f }, { -1.0f, 0.0f, 1.0f } };
        }

        public override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) 
        {
            int radiusX = kernelY.GetLength(0) / 2;
            int radiusY = kernelY.GetLength(1) / 2;
            float resultRX = 0;
            float resultGX = 0;
            float resultBX = 0;
            float resultRY = 0;
            float resultGY = 0;
            float resultBY = 0;
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighbourColor = sourceImage.GetPixel(idX, idY);
                    resultRX += neighbourColor.R * kernelX[k + radiusX, l + radiusY];
                    resultGX += neighbourColor.G * kernelX[k + radiusX, l + radiusY];
                    resultBX += neighbourColor.B * kernelX[k + radiusX, l + radiusY];
                    resultRY += neighbourColor.R * kernelY[k + radiusX, l + radiusY];
                    resultGY += neighbourColor.G * kernelY[k + radiusX, l + radiusY];
                    resultBY += neighbourColor.B * kernelY[k + radiusX, l + radiusY];
                }
            }
            double resultR = Math.Sqrt(Math.Pow(resultRX, 2) + Math.Pow(resultRY, 2));
            double resultG = Math.Sqrt(Math.Pow(resultGX, 2) + Math.Pow(resultGY, 2));
            double resultB = Math.Sqrt(Math.Pow(resultBX, 2) + Math.Pow(resultBY, 2));

            return Color.FromArgb(Clamp((int)resultR, 0, 255),
                                    Clamp((int)resultG, 0, 255),
                                    Clamp((int)resultB, 0, 255));
        }
    }

    public class IncreaseSharpnessFilter: MatrixFilter 
    {
        public IncreaseSharpnessFilter() 
        {
            kernel = new float[,] { { 0.0f, -1.0f, 0.0f }, { -1.0f, 5.0f, -1.0f }, { 0.0f, -1.0f, 0.0f } };
        }
    }

    public class SharpnessFilter: MatrixFilter 
    {
        public SharpnessFilter()
        {
            kernel = new float[,] { { -1.0f, -1.0f, -1.0f }, { -1.0f, 9.0f, -1.0f }, { -1.0f, -1.0f, -1.0f } };
        }
    }

    public class MotionBlurFilter: MatrixFilter 
    {
        public MotionBlurFilter() 
        {
            int n = 7;
            kernel = new float[n, n];
            for(int i = 0; i < n; i++) 
            {
                for (int j = 0; j < n; j++) 
                {
                    if(i == j) 
                    {
                        kernel[i, j] = 1.0f / (float)n;
                    }
                }
            }
        }
    }

    public class DilationFilter: MatrixFilter
    {
        int MW = 5;
        int MH = 5;
        bool[,] mask = null;

        public DilationFilter() 
        {
            mask = new bool[MH,MW];
            for (int i = 0; i < MH; i++) 
            {
                for(int j = 0; j < MW; j++) 
                {
                    mask[i,j] = true;
                }
            }
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int y = MH / 2; y < sourceImage.Height - MH/2; y++)
            {
                worker.ReportProgress((int)((float)y / resultImage.Height * 100));
                if (worker.CancellationPending)
                {
                    return null;
                }

                for (int x = MW / 2; x < (sourceImage.Width - MW/2); x++)
	            {
                    Color max = Color.Black;
                    for (int j = -MH / 2; j < MH / 2; j++)
                    {
                        for (int i = -MW / 2; i < MW / 2; i++)
                        {
                            if ((mask[i+MW/2,j+MH/2]) && (sourceImage.GetPixel(x + i, y + j).R > max.R))
                            {
                                max = sourceImage.GetPixel(x + i, y + j);
                            }
                        }
                    }
                    resultImage.SetPixel(x, y, max);
                }
            }
            return resultImage;
        }
    }

    public class ErosionFilter : MatrixFilter 
    {
        int MW = 5;
        int MH = 5;
        bool[,] mask = null;

        public ErosionFilter()
        {
            mask = new bool[MH, MW];
            for (int i = 0; i < MH; i++)
            {
                for (int j = 0; j < MW; j++)
                {
                    mask[i, j] = true;
                }
            }
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int y = MH / 2; y < sourceImage.Height - MH / 2; y++)
            {
                worker.ReportProgress((int)((float)y / resultImage.Height * 100));
                if (worker.CancellationPending)
                {
                    return null;
                }

                for (int x = MW / 2; x < (sourceImage.Width - MW / 2); x++)
                {
                    Color min = Color.White;
                    for (int j = -MH / 2; j < MH / 2; j++)
                    {
                        for (int i = -MW / 2; i < MW / 2; i++)
                        {
                            if ((mask[i + MW / 2, j + MH / 2]) && (sourceImage.GetPixel(x + i, y + j).R < min.R))
                            {
                                min = sourceImage.GetPixel(x + i, y + j);
                            }
                        }
                    }
                    resultImage.SetPixel(x, y, min);
                }
            }
            return resultImage;
        }
    }

    public class OpeningFilter : MatrixFilter
    {
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker) 
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            ErosionFilter erosion = new ErosionFilter();
            resultImage = erosion.processImage(sourceImage, worker);
            DilationFilter dilation = new DilationFilter();
            resultImage = dilation.processImage(resultImage, worker);
            return resultImage;
        }
    }

    public class ClosingFilter : MatrixFilter
    {
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            DilationFilter dilation = new DilationFilter();
            resultImage = dilation.processImage(sourceImage, worker);
            ErosionFilter erosion = new ErosionFilter();
            resultImage = erosion.processImage(resultImage, worker);
            return resultImage;
        }
    }
}
