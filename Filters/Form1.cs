using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Filters
{
    public partial class Form1 : Form
    {
        private Bitmap image;
        private Bitmap imageBuff;
        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files|*.png;*.jpg;*.bmp|All files(*.*)|*.*";
            if(dialog.ShowDialog() == DialogResult.OK) 
            {
                image = new Bitmap(dialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs =
                    (System.IO.FileStream)saveFileDialog1.OpenFile();

                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        this.pictureBox1.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        this.pictureBox1.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        this.pictureBox1.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }

                fs.Close();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filter)e.Argument).processImage(image, backgroundWorker1);
            if(backgroundWorker1.CancellationPending != true) 
            {
                imageBuff = image;
                image = newImage;
            }
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

        private void отменитьДействиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imageBuff != null)
            {
                image = imageBuff;
                pictureBox1.Image = image;
                imageBuff = null;
            }
        }
       
        private void серыйМирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GrayWorldFilter filter = new GrayWorldFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void линейнаяКоррекцияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinearCorrectionFilter filter = new LinearCorrectionFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void эффектСтеклаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlassEffectFilter filter = new GlassEffectFilter(); 
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void переносToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShiftFilter filter = new ShiftFilter(); 
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SharpnessFilter filter = new SharpnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvertFilter filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BlurFilter filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрГауссаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GaussianFilter filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void greySaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GreyScaleFilter filter = new GreyScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SepiaFilter filter = new SepiaFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void повышениеЯркостиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IncreaseBrightnessFilter filter = new IncreaseBrightnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void операторСобеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SobelOperatorFilter filter = new SobelOperatorFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void увеличениеРезкостиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IncreaseSharpnessFilter filter = new IncreaseSharpnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void motionBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MotionBlurFilter filter = new MotionBlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void dilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DilationFilter filter = new DilationFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void erosionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErosionFilter filter = new ErosionFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void openingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpeningFilter filter = new OpeningFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void closingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClosingFilter filter = new ClosingFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void отзеркалитьПоXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MirrorFilter filter = new MirrorFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void квантизацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuantizationFilter filter = new QuantizationFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияВОвалеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SepiaFilterElips filter = new SepiaFilterElips();
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
}
