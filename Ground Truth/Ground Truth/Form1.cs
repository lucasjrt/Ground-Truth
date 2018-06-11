using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ground_Truth
{
    public partial class Form1 : Form
    {
        Image imagemOriginal, newImage;
        Bitmap bitImage;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        string file;

        private void btnProcurar_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file = openFileDialog1.FileName;
                imagemOriginal = Image.FromFile(file);
                txtFile.Text = file;
                btnAbrir.Enabled = true;
            }
        }

        private Image resizeImage(Image img, Rectangle area)
        {
            bitImage = new Bitmap(img);
            return bitImage.Clone(area, bitImage.PixelFormat);
        }

        private void picImagem_Click(object sender, EventArgs e) {}

        private void btnAbrir_Click(object sender, EventArgs e) {
            newImage = resizeImage(imagemOriginal, new Rectangle(0, 0, (int)imagemOriginal.Width - (imagemOriginal.Width % Convert.ToInt32(txtTamanho.Text)), (int) imagemOriginal.Height - (imagemOriginal.Height % Convert.ToInt32(txtTamanho.Text))));
            picImagem.Image = newImage;
            btnZoomIn.Enabled = true;
            btnZoomOut.Enabled = true;
            drawGrid(imagemOriginal);
        }
        
        private void drawLine(Image image)
        {

        }

        private void drawGrid(Image image)
        {

        }

        private void btnZoomIn_Click(object sender, EventArgs e) {
            if(Convert.ToInt32(txtTamanho.Text) < imagemOriginal.Height && Convert.ToInt32(txtTamanho.Text) < imagemOriginal.Width)
               txtTamanho.Text =Convert.ToString(Convert.ToInt32(txtTamanho.Text) + 1);
            newImage = resizeImage(imagemOriginal, new Rectangle(0, 0, (int)imagemOriginal.Width - (imagemOriginal.Width % Convert.ToInt32(txtTamanho.Text)), (int)imagemOriginal.Height - (imagemOriginal.Height % Convert.ToInt32(txtTamanho.Text))));
            picImagem.Image = newImage;
        }



        private void btnZoomOut_Click(object sender, EventArgs e) {
            if(Convert.ToInt32(txtTamanho.Text) > 1)
                txtTamanho.Text = Convert.ToString(Convert.ToInt32(txtTamanho.Text) - 1);
            newImage = resizeImage(imagemOriginal, new Rectangle(0, 0, (int)imagemOriginal.Width - (imagemOriginal.Width % Convert.ToInt32(txtTamanho.Text)), (int)imagemOriginal.Height - (imagemOriginal.Height % Convert.ToInt32(txtTamanho.Text))));
            picImagem.Image = newImage;
        }
    }
}
