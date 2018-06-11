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
        Pen blackPen;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            blackPen = new Pen(Color.White, 3);
        }

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        string file;

        private void BtnProcurar_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file = openFileDialog1.FileName;
                imagemOriginal = Image.FromFile(file);
                txtFile.Text = file;
                btnAbrir.Enabled = true;
            }
        }

        private Image ResizeImage(Image img, Rectangle area)
        {
            if(bitImage != null)
                bitImage.Dispose();
            GC.Collect();
            bitImage = new Bitmap(img);
            return bitImage.Clone(area, bitImage.PixelFormat);
        }

        private void PicImagem_Click(object sender, EventArgs e) {}

        private void BtnAbrir_Click(object sender, EventArgs e) {
            newImage = ResizeImage(imagemOriginal, new Rectangle(0, 0, (int)imagemOriginal.Width - (imagemOriginal.Width % Convert.ToInt32(txtTamanho.Text)), (int) imagemOriginal.Height - (imagemOriginal.Height % Convert.ToInt32(txtTamanho.Text))));
            picImagem.Image = newImage;
            btnZoomIn.Enabled = true;
            btnZoomOut.Enabled = true;
            DrawGrid(newImage);
        }
        
        private void DrawLine(Image image, int x, int y, int width, int height) {
            using (var graphics = Graphics.FromImage(image)) {
                graphics.DrawLine(blackPen, x, y, width, height);
            }
        }

        private void DrawGrid(Image image) {
            var tam = Convert.ToInt32(txtTamanho.Text);
            var w = image.Width / tam;
            var h = image.Height / tam;

            for (int i = 0; i < w; i++) {
                DrawLine(image, tam + i * tam, 0, tam + i * tam, image.Height);
            }
            for (int i = 0; i < h; i++) {
                DrawLine(image, 0, tam + i * tam, image.Width, tam + i * tam);
            }
        }

        private void BtnZoomIn_Click(object sender, EventArgs e) {
            if(Convert.ToInt32(txtTamanho.Text) < imagemOriginal.Height && Convert.ToInt32(txtTamanho.Text) < imagemOriginal.Width)
               txtTamanho.Text =Convert.ToString(Convert.ToInt32(txtTamanho.Text) + 1);
            newImage = ResizeImage(imagemOriginal, new Rectangle(0, 0, (int)imagemOriginal.Width - (imagemOriginal.Width % Convert.ToInt32(txtTamanho.Text)), (int)imagemOriginal.Height - (imagemOriginal.Height % Convert.ToInt32(txtTamanho.Text))));
            picImagem.Image = newImage;
            DrawGrid(newImage);
        }



        public void BtnZoomOut_Click(object sender, EventArgs e) {
            if(Convert.ToInt32(txtTamanho.Text) > 1)
                txtTamanho.Text = Convert.ToString(Convert.ToInt32(txtTamanho.Text) - 1);
            newImage = ResizeImage(imagemOriginal, new Rectangle(0, 0, (int)imagemOriginal.Width - (imagemOriginal.Width % Convert.ToInt32(txtTamanho.Text)), (int)imagemOriginal.Height - (imagemOriginal.Height % Convert.ToInt32(txtTamanho.Text))));
            picImagem.Image = newImage;
            DrawGrid(newImage);
        }
    }
}
