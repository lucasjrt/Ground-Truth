using System;
using System.ComponentModel;
using System.Drawing
using System.Windows.Forms;

namespace Ground_Truth {
    public partial class Form1 : Form {
        Bitmap mainImage;
        Pen pen;
        Graphics g;
        int gridSize = 25; //25 = Default

        public Form1() {
            InitializeComponent();
            picBoxImage.Image = mainImage;
        }

        private void pictureBox1_Click(object sender, EventArgs e) {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
            int w, h;
            mainImage = new Bitmap(Image.FromFile(openFileDialog1.FileName));
            w = mainImage.Width;
            h = mainImage.Height;
            cbGridSize.Enabled = true;
            try {
                gridSize = Convert.ToInt32(cbGridSize.Text);
            } catch (System.FormatException) {
                gridSize = 0;
            }
            if(gridSize > 0) {
                w = mainImage.Width - (mainImage.Width % gridSize);
                h = mainImage.Height - (mainImage.Height % gridSize);
            }
            if (w == mainImage.Width || h == mainImage.Height)
                picBoxImage.Image = mainImage;
            else
                picBoxImage.Image = CropImage(mainImage, new Rectangle(0, 0, w, h));
            drawGrid();
        }
        
        private void btnSearch_Click(object sender, EventArgs e) {
            if(openFileDialog1.ShowDialog() == DialogResult.OK) {
                txtDirectory.Text = openFileDialog1.FileName;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            int w, h;
            try {
                if (mainImage != null)
                    mainImage.Dispose();
                mainImage = new Bitmap(Image.FromFile(txtDirectory.Text));
                w = mainImage.Width;
                h = mainImage.Height;
                    h = mainImage.Height - (mainImage.Height % gridSize);
                    w = mainImage.Width - (mainImage.Width % gridSize);
                mainImage = CropImage(mainImage, new Rectangle(0, 0, w, h));
                picBoxImage.Image = mainImage;
                drawGrid();
                cbGridSize.Enabled = true;
                GC.Collect();
            } catch (System.IO.FileNotFoundException ex) {
                MessageBox.Show("Não foi possível localizar o arquivo " + txtDirectory.Text);
                Console.WriteLine(ex.Message);
            } catch(System.NullReferenceException ex) {
                MessageBox.Show("A imagem não é válida");
                Console.WriteLine(ex.Message);
            }
        }

        private void drawGrid() {
            if (gridSize == 0)
                return;
            Console.Write("Drawing grid of size ");
            Console.WriteLine(gridSize);
            pen = new Pen(Color.White);
            g = Graphics.FromImage(picBoxImage.Image);
            for(int i = 0; i < picBoxImage.Height || i < picBoxImage.Width; i += gridSize) {
                g.DrawLine(pen, i, 0, i, picBoxImage.Height);
                g.DrawLine(pen, 0, i, picBoxImage.Width, i);
            }
            pen.Dispose();
            g.Dispose();
            GC.Collect();
            Console.WriteLine("Grid drawn");
        }

        private Bitmap CropImage(Bitmap img, Rectangle area) {
            return img.Clone(area, img.PixelFormat);
        }

        private void cbGridSize_SelectedIndexChanged(object sender, EventArgs e) {
            picBoxImage.Image = new Bitmap(mainImage);
            gridSize = Convert.ToInt32(cbGridSize.Text);
            drawGrid();
        }
        
    }
}
