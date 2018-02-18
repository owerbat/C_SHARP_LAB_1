using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C_SHARP_LAB_1_FILTERS
{
    public partial class Form1 : Form
    {
        Bitmap image;
        public Form1()
        {
            InitializeComponent();
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
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            Filters filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void gaussianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void bWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SepiaFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void moreBrightnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MoreBrightnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void clarityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MoreClarityFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void motionBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MotionBlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ScharrFilterX();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void yToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ScharrFilterY();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void xToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Filters filter = new PruittFilterX();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void yToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Filters filter = new PruittFilterY();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void xToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Filters filter = new SobelFilterX();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void yToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Filters filter = new SobelFilterY();
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
}
