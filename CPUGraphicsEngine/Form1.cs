using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hazdryx.Drawing;

namespace CPUGraphicsEngine
{
    public partial class Form1 : Form
    {
        Presentation presentation;
        Bitmap bitmap = new Bitmap(800, 800);
        FastBitmap fastBitmap = new FastBitmap(800, 800);
        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;

            //var jReader = new JSONLoader();
            //jReader.LoadJSONFile();

            presentation = new Presentation();

            mainPicture.Image = Draw().BaseBitmap;

            //mainPicture.Refresh();
        }
        private FastBitmap Draw()
        {
            /*Bitmap bitmap = new Bitmap(800, 800);
            FastBitmap fastBitmap = new FastBitmap(800, 800);*/
            presentation.Render(fastBitmap);
            return fastBitmap;

        }
        private void mainPicture_Paint(object sender, PaintEventArgs e)
        {
            //presentation.Render(e.Graphics);
            /*while (true)
            {
                Thread.Sleep(3000);
                presentation.incAlpha();
                presentation.calcModelMatrix();
                mainPicture.Image = Draw().BaseBitmap;
            }*/
        }

        private void basicButton_Click(object sender, EventArgs e)
        {
            presentation.incAlpha();
            presentation.calcModelMatrix();
            presentation.UpdateViewPoints();
            presentation.UpdateScreenPosition();
            mainPicture.Image = Draw().BaseBitmap;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                backgroundWorker1.CancelAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            bool working = false;
            BackgroundWorker worker = sender as BackgroundWorker;
            System.Timers.Timer timer = new System.Timers.Timer(50);
            //timer.Enabled = true;
            //timer.AutoReset = true;
            timer.Elapsed += (a, b) =>
            {
                if (working) return;
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    timer.Stop();
                    return;
                }
                working = true;
                presentation.Iterate(fastBitmap);
                mainPicture.Image = fastBitmap.BaseBitmap;
                working = false;
                //worker.ReportProgress(1);
            };

            timer.Start();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            mainPicture.Image = fastBitmap.BaseBitmap;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void flatRadio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked)
                {
                    presentation.ChangeShading(Utils.ShadingMode.FlatShading);
                }
            }
        }

        private void gouraudRadio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked)
                {
                    presentation.ChangeShading(Utils.ShadingMode.GouraudShading);
                }
            }
        }

        private void PhongRadio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked)
                {
                    presentation.ChangeShading(Utils.ShadingMode.PhongShading);
                }
            }
        }
    }
}
