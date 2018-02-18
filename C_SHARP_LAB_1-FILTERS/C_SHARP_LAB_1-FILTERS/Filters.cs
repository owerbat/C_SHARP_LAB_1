using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace C_SHARP_LAB_1_FILTERS
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);

        public Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for(int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for(int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }



    class InvertFilter: Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            //throw new NotImplementedException();

            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);
            return resultColor;
        }
    };


    
    class MatrixFilter: Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            //throw new NotImplementedException();

            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;

            for(int i = -radiusY; i <= radiusY; i++)
            {
                for(int j = -radiusX; j <= radiusX; j++)
                {
                    int idX = Clamp(x + j, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + i, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultR += neighborColor.R * kernel[j + radiusX, i + radiusY];
                    resultG += neighborColor.G * kernel[j + radiusX, i + radiusY];
                    resultB += neighborColor.B * kernel[j + radiusX, i + radiusY];
                }
            }

            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }
    };



    class BlurFilter: MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;

            kernel = new float[sizeX, sizeY];

            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
        }
    };




    class GaussianFilter: MatrixFilter
    {
        public void createGaussianKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0;

            for(int i = -radius; i <= radius; i++)
            {
                for(int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            }

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }

        public GaussianFilter()
        {
            createGaussianKernel(3, 2);
        }
    };




    
    class GrayScaleFilter: Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourseColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(Clamp((int)(0.36f * sourseColor.R + 0.53 * sourseColor.G + 0.11 * sourseColor.B), 0, 255),
                                               Clamp((int)(0.36f * sourseColor.R + 0.53 * sourseColor.G + 0.11 * sourseColor.B), 0, 255),
                                               Clamp((int)(0.36f * sourseColor.R + 0.53 * sourseColor.G + 0.11 * sourseColor.B), 0, 255));
            return resultColor;
        }
    };



    class SepiaFilter: Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            float k = 10.0f;
            Color sourseColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(Clamp((int)(sourseColor.R + 2 * k), 0, 255),
                                               Clamp((int)(sourseColor.G + 0.5 * k), 0, 255),
                                               Clamp((int)(sourseColor.B - k), 0, 255));
            return resultColor;
        }
    };



    class MoreBrightnessFilter: Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            float k = 30.0f;
            Color sourseColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(Clamp((int)(sourseColor.R + k), 0, 255),
                                               Clamp((int)(sourseColor.G + k), 0, 255),
                                               Clamp((int)(sourseColor.B + k), 0, 255));
            return resultColor;
        }
    };




    class SobelFilterX: MatrixFilter
    {
        public SobelFilterX()
        {
            kernel = new float[3, 3];

            kernel[0, 0] = kernel[0, 2] = -1.0f;
            kernel[0, 1] = -2.0f;
            kernel[1, 0] = kernel[1, 1] = kernel[1, 2] = 0.0f;
            kernel[2, 0] = kernel[2, 2] = 1.0f;
            kernel[2, 1] = 2.0f;
        }
    };

    class SobelFilterY : MatrixFilter
    {
        public SobelFilterY()
        {
            kernel = new float[3, 3];

            kernel[0, 0] = kernel[2, 0] = -1.0f;
            kernel[1, 0] = -2.0f;
            kernel[0, 1] = kernel[1, 1] = kernel[2, 1] = 0.0f;
            kernel[0, 2] = kernel[2, 2] = 1.0f;
            kernel[1, 2] = 2.0f;
        }
    };




    class MoreClarityFilter: MatrixFilter
    {
        public MoreClarityFilter()
        {
            kernel = new float[3, 3];

            kernel[0, 0] = kernel[0, 2] = kernel[2, 2] = kernel[2, 0] = 0.0f;
            kernel[1, 0] = kernel[1, 2] = kernel[2, 1] = kernel[0, 1] = -1.0f;
            kernel[1, 1] = 5.0f;
        }
    };



    class MotionBlurFilter: MatrixFilter
    {
        public MotionBlurFilter()
        {
            int size = 5;

            kernel = new float[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] = 0.0f;

            for (int i = 0; i < size; i++)
                kernel[i, i] = (float)(1.0f / (float)(size));
        }
    };



    class ScharrFilterX : MatrixFilter
    {
        public ScharrFilterX()
        {
            kernel = new float[3, 3];

            kernel[0, 0] = kernel[0, 2] = 3.0f;
            kernel[0, 1] = 10.0f;
            kernel[1, 0] = kernel[1, 1] = kernel[1, 2] = 0.0f;
            kernel[2, 0] = kernel[2, 2] = -3.0f;
            kernel[2, 1] = -10.0f;
        }
    };

    class ScharrFilterY : MatrixFilter
    {
        public ScharrFilterY()
        {
            kernel = new float[3, 3];

            kernel[0, 0] = kernel[2, 0] = 3.0f;
            kernel[1, 0] = 10.0f;
            kernel[0, 1] = kernel[1, 1] = kernel[2, 1] = 0.0f;
            kernel[0, 2] = kernel[2, 2] = -3.0f;
            kernel[1, 2] = -10.0f;
        }
    };




    class PruittFilterX : MatrixFilter
    {
        public PruittFilterX()
        {
            kernel = new float[3, 3];

            kernel[0, 0] = kernel[0, 1] = kernel[0, 2] = -1.0f;
            kernel[1, 0] = kernel[1, 1] = kernel[1, 2] = 0.0f;
            kernel[2, 0] = kernel[2, 1] = kernel[2, 2] = 1.0f;
        }
    };

    class PruittFilterY : MatrixFilter
    {
        public PruittFilterY()
        {
            kernel = new float[3, 3];

            kernel[0, 0] = kernel[1, 0] = kernel[2, 0] = -1.0f;
            kernel[0, 1] = kernel[1, 1] = kernel[2, 1] = 0.0f;
            kernel[0, 2] = kernel[1, 2] = kernel[2, 2] = 1.0f;
        }
    };
}
