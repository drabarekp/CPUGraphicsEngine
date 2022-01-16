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
        public Form1()
        {
            InitializeComponent();

            //var jReader = new JSONLoader();
            //jReader.LoadJSONFile();

            presentation = new Presentation();
            
            mainPicture.Image = Draw().BaseBitmap;

            //mainPicture.Refresh();
        }
        private FastBitmap Draw()
        {
            Bitmap bitmap = new Bitmap(800, 800);
            FastBitmap fastBitmap = new FastBitmap(800, 800);
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
    }
}
