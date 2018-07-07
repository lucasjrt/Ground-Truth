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
        Bitmap imagemOriginal, newImage;
        Bitmap bitImage;
        Bitmap paintedImage;
        Pen pen;
        int[,] status;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            pen = new Pen(Color.White, 4);
        }

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e) {}

        string file;

        private void BtnProcurar_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file = openFileDialog1.FileName;
                imagemOriginal = new Bitmap(Image.FromFile(file));
                txtFile.Text = file;
                btnAbrir.Enabled = true;
            }
        }

        private Bitmap ResizeImage(Bitmap img, Rectangle area)
        {
            if(bitImage != null)
                bitImage.Dispose();
            GC.Collect();
            bitImage = new Bitmap(img);
            return bitImage.Clone(area, bitImage.PixelFormat);
        }

        private void PicImagem_Click(object sender, MouseEventArgs e) {
            var tam = Convert.ToInt32(txtTamanho.Text);
            var i = (int) picImagem.Width / tam;
            var j = (int) picImagem.Height / tam;
            //var i = (int) e.Location.X / tam;
            //var j = (int)e.Location.Y / tam;
            if(paintedImage == null)
                paintedImage = new Bitmap(newImage);
            //0 = indefinido, 1 = plantação, 2 = ambos, 3 = não plantação
            switch(status[i,j]) {
                case 0:
                    if (e.Button.Equals(MouseButtons.Left)) {
                        status[i, j] = 1;
                        paintImage(paintedImage, Color.Green, i, j);
                        //newImage = paintedImage;
                        picImagem.Image = paintedImage;
                    } else if (e.Button.Equals(MouseButtons.Right)) {
                        status[i, j] = 3;
                    }
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            paintedImage.Dispose();
            GC.Collect();
        }

        private void paintImage(Bitmap bmp, Color color, int i, int j) {
            var tam = Convert.ToInt32(txtTamanho.Text);
            for (int y = 0; y < tam; y++) {
                for(int x = 0; x < tam; x++) { 
                    Color gotColor = bmp.GetPixel(x, y);
                    var r = gotColor.R;
                    var g = gotColor.G;
                    var b = gotColor.B;
                    gotColor = Color.FromArgb(r, 255, b);
                    bmp.SetPixel(x, y, gotColor);
                }
            }
            GC.Collect();
        }

        private void BtnAbrir_Click(object sender, EventArgs e) {
            newImage = ResizeImage(imagemOriginal, new Rectangle(0, 0, (int)imagemOriginal.Width - (imagemOriginal.Width % Convert.ToInt32(txtTamanho.Text)), (int) imagemOriginal.Height - (imagemOriginal.Height % Convert.ToInt32(txtTamanho.Text))));
            picImagem.Image = newImage;
            var w = newImage.Width / Convert.ToInt32(txtTamanho.Text);
            var h = newImage.Height / Convert.ToInt32(txtTamanho.Text);
            btnZoomIn.Enabled = true;
            btnZoomOut.Enabled = true;
            DrawGrid(newImage);
            status = new int[w, h];
            for(int i = 0; i < w; i++) {
                for(int j = 0; j < h; j++) {
                    status[i, j] = 0;
                }

            }
        }
        
        private void DrawLine(Bitmap image, int x, int y, int width, int height) {
            using (var graphics = Graphics.FromImage(image)) {
                graphics.DrawLine(pen, x, y, width, height);
            }
        }

        private void DrawGrid(Bitmap image) {
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

        private void TxtTamanho_KeyPressed(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.Enter) {
                newImage = ResizeImage(imagemOriginal, new Rectangle(0, 0, (int)imagemOriginal.Width - (imagemOriginal.Width % Convert.ToInt32(txtTamanho.Text)), (int)imagemOriginal.Height - (imagemOriginal.Height % Convert.ToInt32(txtTamanho.Text))));
                picImagem.Image = newImage;
                DrawGrid(newImage);
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
