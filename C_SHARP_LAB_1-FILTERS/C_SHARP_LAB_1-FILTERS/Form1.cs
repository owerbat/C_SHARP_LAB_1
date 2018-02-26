using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace C_SHARP_LAB_1_FILTERS
{
    public partial class Form1 : Form
    {
        Bitmap image;
        Stack cashBack;
        Stack cashForward;
        Bitmap FirstImage;

        public Form1()
        {
            InitializeComponent();
            cashBack = new Stack();
            cashForward = new Stack();
            KeyPreview = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.png; *.jpg; *.bmp | All Files (*.*) | *.* ";

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
            }

            pictureBox1.Image = image;
            pictureBox1.Refresh();

            FirstImage = new Bitmap(image);
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //PictureBox
            if(pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
                image = newImage;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void blurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void gaussianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void bWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new SepiaFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void moreBrightnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new MoreBrightnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void clarityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new MoreClarityFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void motionBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new MotionBlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new ScharrFilterX();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void yToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new ScharrFilterY();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void xToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new PruittFilterX();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void yToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new PruittFilterY();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void xToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new SobelFilterX();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void yToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new SobelFilterY();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "Image files | *.png; *.jpg; *.bmp | All Files (*.*) | *.* ";
            dialog.Title = "Save an Image File";
            dialog.ShowDialog();

            if(dialog.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)dialog.OpenFile();

                switch (dialog.FilterIndex)
                {
                    case 1:
                        image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                }

                fs.Close();
            }
        }

        private void stepBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(cashBack.Count != 0)
            {
                Bitmap tmp = image;
                image = (Bitmap)cashBack.Pop();
                pictureBox1.Image = image;
                cashForward.Push(tmp);
            }
            else
            {
                DialogResult res = MessageBox.Show("You're in the beggining of the editing");
            }
        }

        private void stepForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(cashForward.Count != 0)
            {
                Bitmap tmp = image;
                image = (Bitmap)cashForward.Pop();
                pictureBox1.Image = image;
                cashBack.Push(tmp);
            }
            else
            {
                DialogResult res = MessageBox.Show("You're in the end of the editing");
            }
        }

        private void glassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { 
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new GlassFilter();
            backgroundWorker1.RunWorkerAsync(filter);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                DialogResult res = MessageBox.Show("The glass filter works incorrectly");
            }
        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void scharrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new ScharrFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void pruittToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new PruittFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void horizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new HorizontalWavesFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void verticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new VerticalWavesFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'q' || e.KeyChar == 'й') 
                stepBackToolStripMenuItem_Click(sender, e);
            if (e.KeyChar == 'w' || e.KeyChar == 'ц')
                stepForwardToolStripMenuItem_Click(sender, e);
            if (e.KeyChar == Keys.Escape.GetHashCode())
                Application.Exit();
            if (e.KeyChar == '1')
                sobelToolStripMenuItem_Click(sender, e);
            if (e.KeyChar == '2')
                scharrToolStripMenuItem_Click(sender, e);
            if (e.KeyChar == '3')
                pruittToolStripMenuItem_Click(sender, e);
            if (e.KeyChar == '4')
                embossingToolStripMenuItem_Click(sender, e);
            if (e.KeyChar == 'a' || e.KeyChar == 'ф')
                openToolStripMenuItem_Click(sender, e);
            if (e.KeyChar == 's' || e.KeyChar == 'ы')
                saveToolStripMenuItem_Click(sender, e);
        }

        private void embossingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new EmbossingFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void dilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new DilationFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void erosionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new ErosionFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void closingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*try
            {
                if (pictureBox1.Image != null)
                {
                    cashBack.Push(image);
                }
                Filters filter = new DilationFilter();
                backgroundWorker1.RunWorkerAsync(filter);
                filter = new ErosionFilter();
                backgroundWorker1.RunWorkerAsync(filter);
            }
            catch(Exception)
            {

            }*/

            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new ClosingFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void openingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*try
            {
                if (pictureBox1.Image != null)
                {
                    cashBack.Push(image);
                }
                Filters filter = new ErosionFilter();
                backgroundWorker1.RunWorkerAsync(filter);
                filter = new DilationFilter();
                backgroundWorker1.RunWorkerAsync(filter);
            }
            catch (Exception)
            {

            }*/

            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new OpeningFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void grayWorldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new GrayWorld();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void medianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                cashBack.Push(image);
            }
            Filters filter = new MedianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
    
}
