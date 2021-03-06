﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Windows.Forms;


namespace C_SHARP_LAB_1_FILTERS
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);

        public virtual Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
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


    

        class SobelFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            float[,] kernel = new float[3, 3];

            kernel[0, 0] = kernel[0, 2] = -1.0f;
            kernel[0, 1] = -2.0f;
            kernel[1, 0] = kernel[1, 1] = kernel[1, 2] = 0.0f;
            kernel[2, 0] = kernel[2, 2] = 1.0f;
            kernel[2, 1] = 2.0f;

            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultRX = 0;
            float resultGX = 0;
            float resultBX = 0;
            float resultRY = 0;
            float resultGY = 0;
            float resultBY = 0;

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    int idX = Clamp(x + j, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + i, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultRX += neighborColor.R * kernel[j + radiusX, i + radiusY];
                    resultGX += neighborColor.G * kernel[j + radiusX, i + radiusY];
                    resultBX += neighborColor.B * kernel[j + radiusX, i + radiusY];
                }
            }

            kernel[0, 0] = kernel[2, 0] = -1.0f;
            kernel[1, 0] = -2.0f;
            kernel[0, 1] = kernel[1, 1] = kernel[2, 1] = 0.0f;
            kernel[0, 2] = kernel[2, 2] = 1.0f;
            kernel[1, 2] = 2.0f;

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    int idX = Clamp(x + j, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + i, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultRY += neighborColor.R * kernel[j + radiusX, i + radiusY];
                    resultGY += neighborColor.G * kernel[j + radiusX, i + radiusY];
                    resultBY += neighborColor.B * kernel[j + radiusX, i + radiusY];
                }
            }

            return Color.FromArgb(Clamp((int)(Math.Sqrt((resultRX * (int)resultRX + resultRY * (int)resultRY))), 0, 255),
                                  Clamp((int)(Math.Sqrt((resultGX * (int)resultGX + resultGY * (int)resultGY))), 0, 255), 
                                  Clamp((int)(Math.Sqrt((resultBX * (int)resultBX + resultBY * (int)resultBY))), 0, 255));
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



    class ScharrFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            float[,] kernel = new float[3, 3];

            kernel[0, 0] = kernel[0, 2] = 3.0f;
            kernel[0, 1] = 10.0f;
            kernel[1, 0] = kernel[1, 1] = kernel[1, 2] = 0.0f;
            kernel[2, 0] = kernel[2, 2] = -3.0f;
            kernel[2, 1] = -10.0f;

            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultRX = 0;
            float resultGX = 0;
            float resultBX = 0;
            float resultRY = 0;
            float resultGY = 0;
            float resultBY = 0;

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    int idX = Clamp(x + j, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + i, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultRX += neighborColor.R * kernel[j + radiusX, i + radiusY];
                    resultGX += neighborColor.G * kernel[j + radiusX, i + radiusY];
                    resultBX += neighborColor.B * kernel[j + radiusX, i + radiusY];
                }
            }

            kernel[0, 0] = kernel[2, 0] = 3.0f;
            kernel[1, 0] = 10.0f;
            kernel[0, 1] = kernel[1, 1] = kernel[2, 1] = 0.0f;
            kernel[0, 2] = kernel[2, 2] = -3.0f;
            kernel[1, 2] = -10.0f;

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    int idX = Clamp(x + j, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + i, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultRY += neighborColor.R * kernel[j + radiusX, i + radiusY];
                    resultGY += neighborColor.G * kernel[j + radiusX, i + radiusY];
                    resultBY += neighborColor.B * kernel[j + radiusX, i + radiusY];
                }
            }

            return Color.FromArgb(Clamp((int)(Math.Sqrt((resultRX * (int)resultRX + resultRY * (int)resultRY))), 0, 255),
                                  Clamp((int)(Math.Sqrt((resultGX * (int)resultGX + resultGY * (int)resultGY))), 0, 255),
                                  Clamp((int)(Math.Sqrt((resultBX * (int)resultBX + resultBY * (int)resultBY))), 0, 255));
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




    class PruittFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            float[,] kernel = new float[3, 3];

            kernel[0, 0] = kernel[0, 1] = kernel[0, 2] = -1.0f;
            kernel[1, 0] = kernel[1, 1] = kernel[1, 2] = 0.0f;
            kernel[2, 0] = kernel[2, 1] = kernel[2, 2] = 1.0f;

            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultRX = 0;
            float resultGX = 0;
            float resultBX = 0;
            float resultRY = 0;
            float resultGY = 0;
            float resultBY = 0;

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    int idX = Clamp(x + j, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + i, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultRX += neighborColor.R * kernel[j + radiusX, i + radiusY];
                    resultGX += neighborColor.G * kernel[j + radiusX, i + radiusY];
                    resultBX += neighborColor.B * kernel[j + radiusX, i + radiusY];
                }
            }

            kernel[0, 0] = kernel[1, 0] = kernel[2, 0] = -1.0f;
            kernel[0, 1] = kernel[1, 1] = kernel[2, 1] = 0.0f;
            kernel[0, 2] = kernel[1, 2] = kernel[2, 2] = 1.0f;

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    int idX = Clamp(x + j, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + i, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultRY += neighborColor.R * kernel[j + radiusX, i + radiusY];
                    resultGY += neighborColor.G * kernel[j + radiusX, i + radiusY];
                    resultBY += neighborColor.B * kernel[j + radiusX, i + radiusY];
                }
            }

            return Color.FromArgb(Clamp((int)(Math.Sqrt((resultRX * (int)resultRX + resultRY * (int)resultRY))), 0, 255),
                                  Clamp((int)(Math.Sqrt((resultGX * (int)resultGX + resultGY * (int)resultGY))), 0, 255),
                                  Clamp((int)(Math.Sqrt((resultBX * (int)resultBX + resultBY * (int)resultBY))), 0, 255));
        }
    };




    class GlassFilter: Filters
    {
        Random rand = new Random();

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int newX = Clamp((int)(x + (rand.NextDouble() - 0.5) * 10), 0, sourceImage.Width - 1);
            int newY = Clamp((int)(y + (rand.NextDouble() - 0.5) * 10), 0, sourceImage.Height - 1);

            Color resultColor = sourceImage.GetPixel(newX, newY);
            return resultColor;
        }
    };

 

    class HorizontalWavesFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int newX = Clamp((int)(x + 20 * (Math.Sin(2 * y * Math.PI / 60))), 0, sourceImage.Width - 1);

            return sourceImage.GetPixel(newX, y);
        }
    };



    class VerticalWavesFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int newX = Clamp((int)(x + 20 * (Math.Sin(2 * x * Math.PI / 30))), 0, sourceImage.Width - 1);

            return sourceImage.GetPixel(newX, y);
        }
    };



    class Filter1: MatrixFilter
    {
        public Filter1()
        {
            kernel = new float[3, 3]
            {
                {0,  1,  0 },
                {1,  0, -1 },
                {0, -1,  0 }
            };
        }
    };



    class NormFilter: Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourseColor = sourceImage.GetPixel(x, y);

            double norm = Math.Sqrt(sourseColor.R * sourseColor.R + sourseColor.G * sourseColor.G + sourseColor.G * sourseColor.G);

            return Color.FromArgb(Clamp((int)(sourseColor.R / norm), 0, 255), 
                                  Clamp((int)(sourseColor.G / norm), 0, 255),
                                  Clamp((int)(sourseColor.B / norm), 0, 255));
        }
    };



    class EmbossingFilter: MatrixFilter
    {
        public EmbossingFilter()
        {
            kernel = new float[3, 3]
            {
                {0,  1,  0 },
                {1,  0, -1 },
                {0, -1,  0 }
            };
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
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

            Color color1 = Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));

            float k = 130.0f;
            Color color2 = Color.FromArgb(Clamp((int)(color1.R + k), 0, 255),
                                          Clamp((int)(color1.G + k), 0, 255),
                                          Clamp((int)(color1.B + k), 0, 255));

            return Color.FromArgb(Clamp((int)(0.36f * color2.R + 0.53 * color2.G + 0.11 * color2.B), 0, 255),
                                  Clamp((int)(0.36f * color2.R + 0.53 * color2.G + 0.11 * color2.B), 0, 255),
                                  Clamp((int)(0.36f * color2.R + 0.53 * color2.G + 0.11 * color2.B), 0, 255));
        }
    };










    class DilationFilter : MatrixFilter
    {
        public DilationFilter()
        {
            
            kernel = new float[3, 3]
            {
                {0, 1, 0},
                {1, 1, 1},
                {0, 1, 0}
            };
                
            /*kernel = new float[3, 3]
            {
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1}
            };*/

            /*kernel = new float[5, 5]
            {
                {0, 0, 1, 0, 0},
                {0, 1, 1, 1, 0},
                {1, 1, 1, 1, 1},
                {0, 1, 1, 1, 0},
                {0, 0, 1, 0, 0}
            };*/

            /*kernel = new float[5, 5]
            {
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1}
            };*/
        }

        public DilationFilter(DomainUpDown domain)
        {
            string tmp = domain.Text;

            switch (tmp)
            {
                case "3x3 - square":

                    kernel = new float[3, 3]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    break;

                case "3x3 - circle":

                    kernel = new float[3, 3]
                    {
                        {1, 1, 1},
                        {1, 1, 1},
                        {1, 1, 1}
                    };
                    break;

                case "5x5 - square":

                    kernel = new float[5, 5]
                    {
                        {0, 0, 1, 0, 0},
                        {0, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 0},
                        {0, 0, 1, 0, 0}
                    };
                    break;

                case "5x5 - circle":

                    kernel = new float[5, 5]
                    {
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1}
                    };
                    break;

                default: 
                    DialogResult res = MessageBox.Show("Kernel is incorrect\n" + domain.Items.ToString());
                    break;
            }
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            Color max = Color.FromArgb(0, 0, 0);

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    Color curr = sourceImage.GetPixel(Clamp(x + i, 0, sourceImage.Width - 1), Clamp(y + j, 0, sourceImage.Height - 1));
                    if ((kernel[j + radiusX, i + radiusY] != 0) && (Math.Sqrt(curr.R * curr.R + curr.G * curr.G + curr.B * curr.B) >
                                                                    Math.Sqrt(max.R * max.R + max.G * max.G + max.B * max.B)))
                        max = curr;
                }
            }

            return max;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = radiusX; i < sourceImage.Width - radiusX; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));

                if (worker.CancellationPending)
                    return null;

                for (int j = radiusY; j < sourceImage.Height - radiusY; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }

            return resultImage;
        }
    };

    

    class ErosionFilter : MatrixFilter
    {
        public ErosionFilter()
        {
            kernel = new float[3, 3]
            {
                {0, 1, 0},
                {1, 1, 1},
                {0, 1, 0}
            };

            /*kernel = new float[3, 3]
            {
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1}
            };*/

            /*kernel = new float[5, 5]
            {
                {0, 0, 1, 0, 0},
                {0, 1, 1, 1, 0},
                {1, 1, 1, 1, 1},
                {0, 1, 1, 1, 0},
                {0, 0, 1, 0, 0}
            };*/

            /*kernel = new float[5, 5]
            {
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1}
            };*/
        }

        public ErosionFilter(DomainUpDown domain)
        {
            string tmp = domain.Text;

            switch (tmp)
            {
                case "3x3 - square":

                    kernel = new float[3, 3]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    break;

                case "3x3 - circle":

                    kernel = new float[3, 3]
                    {
                        {1, 1, 1},
                        {1, 1, 1},
                        {1, 1, 1}
                    };
                    break;

                case "5x5 - square":

                    kernel = new float[5, 5]
                    {
                        {0, 0, 1, 0, 0},
                        {0, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 0},
                        {0, 0, 1, 0, 0}
                    };
                    break;

                case "5x5 - circle":

                    kernel = new float[5, 5]
                    {
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1}
                    };
                    break;

                default:
                    DialogResult res = MessageBox.Show("Kernel is incorrect\n" + domain.Items.ToString());
                    break;
            }
        }

        public ErosionFilter(float[,] _kernel)
        {
            kernel = _kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            Color min = Color.FromArgb(255, 255, 255);

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for(int j = -radiusX; j <= radiusX; j++)
                {
                    Color curr = sourceImage.GetPixel(Clamp(x + i, 0, sourceImage.Width - 1), Clamp(y + j, 0, sourceImage.Height - 1));
                    if ((kernel[j + radiusX, i + radiusY] != 0) && (Math.Sqrt(curr.R * curr.R + curr.G * curr.G + curr.B * curr.B) <
                                                Math.Sqrt(min.R * min.R + min.G * min.G + min.B * min.B)))
                        min = curr;
                }
            }

            return min;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = radiusX; i < sourceImage.Width - radiusX; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));

                if (worker.CancellationPending)
                    return null;

                for (int j = radiusY; j < sourceImage.Height - radiusY; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }

            return resultImage;
        }
    };



    class ClosingFilter : MatrixFilter
    {
        public ClosingFilter()
        {
            kernel = new float[3, 3]
            {
                {0, 1, 0},
                {1, 1, 1},
                {0, 1, 0}
            };

            /*kernel = new float[3, 3]
            {
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1}
            };*/

            /*kernel = new float[5, 5]
            {
                {0, 0, 1, 0, 0},
                {0, 1, 1, 1, 0},
                {1, 1, 1, 1, 1},
                {0, 1, 1, 1, 0},
                {0, 0, 1, 0, 0}
            };*/

            /*kernel = new float[5, 5]
            {
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1}
            };*/
        }

        public ClosingFilter(DomainUpDown domain)
        {
            string tmp = domain.Text;

            switch (tmp)
            {
                case "3x3 - square":

                    kernel = new float[3, 3]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    break;

                case "3x3 - circle":

                    kernel = new float[3, 3]
                    {
                        {1, 1, 1},
                        {1, 1, 1},
                        {1, 1, 1}
                    };
                    break;

                case "5x5 - square":

                    kernel = new float[5, 5]
                    {
                        {0, 0, 1, 0, 0},
                        {0, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 0},
                        {0, 0, 1, 0, 0}
                    };
                    break;

                case "5x5 - circle":

                    kernel = new float[5, 5]
                    {
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1}
                    };
                    break;

                default:
                    DialogResult res = MessageBox.Show("Kernel is incorrect\n" + domain.Items.ToString());
                    break;
            }
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            /*Filters filterD = new DilationFilter();
            Filters filterE = new ErosionFilter();

            Bitmap tmp = filterD.processImage(sourceImage, worker);
            Bitmap res = filterE.processImage(tmp, worker);

            return res;*/

            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            Bitmap tmp = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = radiusX; i < sourceImage.Width - radiusX; i++)
            {
                worker.ReportProgress((int)((float)i / tmp.Width * 50));

                if (worker.CancellationPending)
                    return null;

                for (int j = radiusY; j < sourceImage.Height - radiusY; j++)
                {
                    tmp.SetPixel(i, j, calculateNewPixelColorD(sourceImage, i, j));
                }
            }

            Bitmap resultImage = new Bitmap(tmp.Width, tmp.Height);

            for (int i = radiusX; i < tmp.Width - radiusX; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 50 + 50 * (tmp.Width -radiusX) / resultImage.Width));

                if (worker.CancellationPending)
                    return null;

                for (int j = radiusY; j < tmp.Height - radiusY; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColorE(tmp, i, j));
                }
            }

            return resultImage;
        }

        protected Color calculateNewPixelColorD(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            Color max = Color.FromArgb(0, 0, 0);

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    Color curr = sourceImage.GetPixel(Clamp(x + i, 0, sourceImage.Width - 1), Clamp(y + j, 0, sourceImage.Height - 1));
                    if ((kernel[j + radiusX, i + radiusY] != 0) && (Math.Sqrt(curr.R * curr.R + curr.G * curr.G + curr.B * curr.B) >
                                                                    Math.Sqrt(max.R * max.R + max.G * max.G + max.B * max.B)))
                        max = curr;
                }
            }

            return max;
        }

        protected Color calculateNewPixelColorE(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            Color min = Color.FromArgb(255, 255, 255);

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    Color curr = sourceImage.GetPixel(Clamp(x + i, 0, sourceImage.Width - 1), Clamp(y + j, 0, sourceImage.Height - 1));
                    if ((kernel[j + radiusX, i + radiusY] != 0) && (Math.Sqrt(curr.R * curr.R + curr.G * curr.G + curr.B * curr.B) <
                                                                    Math.Sqrt(min.R * min.R + min.G * min.G + min.B * min.B)))
                        min = curr;
                }
            }

            return min;
        }
    };




    class OpeningFilter: MatrixFilter
    {
        public OpeningFilter()
        {
           kernel = new float[3, 3]
            {
                {0, 1, 0},
                {1, 1, 1},
                {0, 1, 0}
            };

            /*kernel = new float[3, 3]
            {
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1}
            };*/

            /*kernel = new float[5, 5]
            {
                {0, 0, 1, 0, 0},
                {0, 1, 1, 1, 0},
                {1, 1, 1, 1, 1},
                {0, 1, 1, 1, 0},
                {0, 0, 1, 0, 0}
            };*/

            /*kernel = new float[5, 5]
            {
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1}
            };*/
        }

        public OpeningFilter(DomainUpDown domain)
        {
            string tmp = domain.Text;

            switch (tmp)
            {
                case "3x3 - square":

                    kernel = new float[3, 3]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    break;

                case "3x3 - circle":

                    kernel = new float[3, 3]
                    {
                        {1, 1, 1},
                        {1, 1, 1},
                        {1, 1, 1}
                    };
                    break;

                case "5x5 - square":

                    kernel = new float[5, 5]
                    {
                        {0, 0, 1, 0, 0},
                        {0, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 0},
                        {0, 0, 1, 0, 0}
                    };
                    break;

                case "5x5 - circle":

                    kernel = new float[5, 5]
                    {
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1}
                    };
                    break;

                default:
                    DialogResult res = MessageBox.Show("Kernel is incorrect\n" + domain.Items.ToString());
                    break;
            }
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            /*Filters filterD = new DilationFilter();
            Filters filterE = new ErosionFilter();

            Bitmap tmp = filterE.processImage(sourceImage, worker);
            Bitmap res = filterD.processImage(tmp, worker);

            //Bitmap res = filterE.processImage(sourceImage, worker);

            return res;*/

            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            Bitmap tmp = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = radiusX; i < sourceImage.Width - radiusX; i++)
            {
                worker.ReportProgress((int)((float)i / tmp.Width * 50));

                if (worker.CancellationPending)
                    return null;

                for (int j = radiusY; j < sourceImage.Height - radiusY; j++)
                {
                    tmp.SetPixel(i, j, calculateNewPixelColorE(sourceImage, i, j));
                }
            }

            Bitmap resultImage = new Bitmap(tmp.Width, tmp.Height);

            for (int i = radiusX; i < tmp.Width - radiusX; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 50 + 50 * (tmp.Width - radiusX) / resultImage.Width));

                if (worker.CancellationPending)
                    return null;

                for (int j = radiusY; j < tmp.Height - radiusY; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColorD(tmp, i, j));
                }
            }

            return resultImage;
        }

        protected Color calculateNewPixelColorD(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            Color max = Color.FromArgb(0, 0, 0);

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    Color curr = sourceImage.GetPixel(Clamp(x + i, 0, sourceImage.Width - 1), Clamp(y + j, 0, sourceImage.Height - 1));
                    if ((kernel[j + radiusX, i + radiusY] != 0) && (Math.Sqrt(curr.R * curr.R + curr.G * curr.G + curr.B * curr.B) >
                                                                    Math.Sqrt(max.R * max.R + max.G * max.G + max.B * max.B)))
                        max = curr;
                }
            }

            return max;
        }

        protected Color calculateNewPixelColorE(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            Color min = Color.FromArgb(255, 255, 255);

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    Color curr = sourceImage.GetPixel(Clamp(x + i, 0, sourceImage.Width - 1), Clamp(y + j, 0, sourceImage.Height - 1));
                    if ((kernel[j + radiusX, i + radiusY] != 0) && (Math.Sqrt(curr.R * curr.R + curr.G * curr.G + curr.B * curr.B) <
                                                                    Math.Sqrt(min.R * min.R + min.G * min.G + min.B * min.B)))
                        min = curr;
                }
            }

            return min;
        }
    };




    class GrayWorld: Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            return Color.FromArgb(255, 0, 0);
        }

        protected Color calculateNewPixelColor1(Bitmap sourceImage, int x, int y, double coefR, double coefG, double coefB)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            return Color.FromArgb(Clamp((int)(sourceColor.R * coefR), 0, 255), 
                                  Clamp((int)(sourceColor.G * coefG), 0, 255),
                                  Clamp((int)(sourceColor.B * coefB), 0, 255));
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            int count = sourceImage.Height * sourceImage.Width, sumR = 0, sumG = 0, sumB = 0;

            for (int i = 0; i < sourceImage.Width; i++)
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    sumR += sourceImage.GetPixel(i, j).R;
                    sumG += sourceImage.GetPixel(i, j).G;
                    sumB += sourceImage.GetPixel(i, j).B;
                }

            double averageR = (double)(sumR / count);
            double averageG = (double)(sumG / count);
            double averageB = (double)(sumB / count);

            double Avg = (double)((averageR + averageG + averageB) / 3.0);

            double coefR = (double)(Avg / averageR);
            double coefG = (double)(Avg / averageG);
            double coefB = (double)(Avg / averageB);

            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor1(sourceImage, i, j, coefR, coefG, coefB));
                }
            }
            return resultImage;
        }
    };

    class MedianFilter : Filters
    {
        int radius = 1;

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int[] masR = new int[(2 * radius + 1) * (2 * radius + 1)];
            int[] masG = new int[(2 * radius + 1) * (2 * radius + 1)];
            int[] masB = new int[(2 * radius + 1) * (2 * radius + 1)];

            int k = 0;
            for(int i = -radius; i <= radius; i++)
                for(int j = -radius; j <= radius; j++)
                {
                    masR[k] = sourceImage.GetPixel(x + i, y + j).R;
                    masG[k] = sourceImage.GetPixel(x + i, y + j).G;
                    masB[k] = sourceImage.GetPixel(x + i, y + j).B;
                    k++;
                }

            Array.Sort(masR);
            Array.Sort(masG);
            Array.Sort(masB);

            return Color.FromArgb(masR[(2 * radius + 1) * (2 * radius + 1) / 2],
                                  masG[(2 * radius + 1) * (2 * radius + 1) / 2],
                                  masB[(2 * radius + 1) * (2 * radius + 1) / 2]);
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = radius; i < sourceImage.Width - radius; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = radius; j < sourceImage.Height - radius; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }
    };




    class GradFilter: MatrixFilter
    {
        public DomainUpDown Domain;

        public GradFilter()
        {
            kernel = new float[3, 3]
            {
                {0, 1, 0},
                {1, 1, 1},
                {0, 1, 0}
            };

            /*kernel = new float[3, 3]
            {
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1}
            };*/

            /*kernel = new float[5, 5]
            {
                {0, 0, 1, 0, 0},
                {0, 1, 1, 1, 0},
                {1, 1, 1, 1, 1},
                {0, 1, 1, 1, 0},
                {0, 0, 1, 0, 0}
            };*/

            /*kernel = new float[5, 5]
            {
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1}
            };*/
        }

        public GradFilter(DomainUpDown domain)
        {
            Domain = domain;

            string tmp = domain.Text;

            switch (tmp)
            {
                case "3x3 - square":

                    kernel = new float[3, 3]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    break;

                case "3x3 - circle":

                    kernel = new float[3, 3]
                    {
                        {1, 1, 1},
                        {1, 1, 1},
                        {1, 1, 1}
                    };
                    break;

                case "5x5 - square":

                    kernel = new float[5, 5]
                    {
                        {0, 0, 1, 0, 0},
                        {0, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 0},
                        {0, 0, 1, 0, 0}
                    };
                    break;

                case "5x5 - circle":

                    kernel = new float[5, 5]
                    {
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1}
                    };
                    break;

                default:
                    DialogResult res = MessageBox.Show("Kernel is incorrect\n" + domain.Items.ToString());
                    break;
            }
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            if (Domain != null)
            {
                Filters filterD = new DilationFilter(Domain);
                Filters filterE = new ErosionFilter(Domain);

                Bitmap tmpD = new Bitmap(sourceImage.Width, sourceImage.Height);
                Bitmap tmpE = new Bitmap(sourceImage.Width, sourceImage.Height);
                Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

                BackgroundWorker tmpWorker = new BackgroundWorker();

                tmpD = filterD.processImage(sourceImage, worker);
                tmpE = filterE.processImage(sourceImage, worker);

                for (int i = 0; i < resultImage.Width; i++)
                {
                    worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                    if (worker.CancellationPending)
                        return null;
                    for (int j = 0; j < resultImage.Height; j++)
                        resultImage.SetPixel(i, j, Color.FromArgb(Clamp(tmpD.GetPixel(i, j).R - tmpE.GetPixel(i, j).R, 0, 255),
                                                                  Clamp(tmpD.GetPixel(i, j).G - tmpE.GetPixel(i, j).G, 0, 255),
                                                                  Clamp(tmpD.GetPixel(i, j).B - tmpE.GetPixel(i, j).B, 0, 255)));
                }

                return resultImage;
            }
            return null;
        }
    };




    class AutoLevels : MatrixFilter
    {
        protected int maxR, minR, maxG, minG, maxB, minB;

        public AutoLevels()
        {
            maxR = new int();
            minR = new int();
            maxG = new int();
            minG = new int();
            maxB = new int();
            minB = new int();
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            maxR = minR = sourceImage.GetPixel(0, 0).R;
            maxG = minG = sourceImage.GetPixel(0, 0).G;
            maxB = minB = sourceImage.GetPixel(0, 0).B;

            for(int i = 0; i < sourceImage.Width; i++)
                for(int j = 0; j < sourceImage.Height; j++)
                {
                    if (sourceImage.GetPixel(i, j).R > maxR)
                        maxR = sourceImage.GetPixel(i, j).R;
                    if (sourceImage.GetPixel(i, j).G > maxG)
                        maxG = sourceImage.GetPixel(i, j).G;
                    if (sourceImage.GetPixel(i, j).B > maxB)
                        maxB = sourceImage.GetPixel(i, j).B;
                    if (sourceImage.GetPixel(i, j).R < minR)
                        minR = sourceImage.GetPixel(i, j).R;
                    if (sourceImage.GetPixel(i, j).G < minG)
                        minG = sourceImage.GetPixel(i, j).G;
                    if (sourceImage.GetPixel(i, j).B < minB)
                        minB = sourceImage.GetPixel(i, j).B;
                }

            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j,
                        Color.FromArgb(Clamp((int)(255 * (sourceImage.GetPixel(i, j).R - minR) / (maxR - minR)), 0, 255),
                                       Clamp((int)(255 * (sourceImage.GetPixel(i, j).G - minG) / (maxG - minG)), 0, 255),
                                       Clamp((int)(255 * (sourceImage.GetPixel(i, j).B - minB) / (maxB - minB)), 0, 255)));
                }
            }
            return resultImage;
        }
    };
}
