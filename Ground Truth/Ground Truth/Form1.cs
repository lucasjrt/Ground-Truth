using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ground_Truth {
    public partial class Form1 : Form {
        Bitmap mainImage; //Imagem principal, não é alterada
        Pen pen; // "Caneta" usada para desenhar o grid
        int gridSize = 25; //25 = Default
        Graphics g;

        //Construtor
        public Form1() {
            InitializeComponent();
        }

        //Tratador de evento quando é clicado na imagem
        private void pictureBox1_Click(object sender, EventArgs e) {

        }

        //Função chamada quando é clicado OK na janela de diálogo de abrir o arquivo
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
            int w, h; //width, height - Largura e altura da imagem
            mainImage = new Bitmap(Image.FromFile(openFileDialog1.FileName));
            w = mainImage.Width;
            h = mainImage.Height;
            try { 
                gridSize = Convert.ToInt32(cbGridSize.Text); //atribui à variável global o tamanho do grid
            } catch (System.FormatException) {
                gridSize = 0; //se houver problema na atribuição, atribui 0
            }
            if(gridSize > 0) { //verifica se o grid está sendo desenhado
                w = mainImage.Width - (mainImage.Width % gridSize); //calcula a nova largura da imagem
                h = mainImage.Height - (mainImage.Height % gridSize); //calcula a nova altura da imagem
            }
            /*
             * Se o tamanho da imagem original for do mesmo 
             * tamanho da imagem redimensionada calculada, 
             * não é necessário redimensionar a imagem
             */
            if (w == mainImage.Width && h == mainImage.Height)
                picBoxImage.Image = mainImage;
            else
                picBoxImage.Image = CropImage(mainImage, new Rectangle(0, 0, w, h));
            drawGrid();
        }
        
        //Tratador de eventos de quando clica no botão "..."
        private void btnSearch_Click(object sender, EventArgs e) {
            if(openFileDialog1.ShowDialog() == DialogResult.OK) {
                txtDirectory.Text = openFileDialog1.FileName;
            }
        }

        //Tratador de eventos de quando clica no botão "Abrir"
        private void btnOpen_Click(object sender, EventArgs e) {
            int w, h; //width, height - Largura e altura da imagem
            try {
                if (mainImage != null) // Se a imagem principal já tem uma instância
                    mainImage.Dispose(); // Descarta essa instância, por motivos de memória
                mainImage = new Bitmap(Image.FromFile(txtDirectory.Text));
                w = mainImage.Width;
                h = mainImage.Height;
                if(gridSize > 0) { // Se tiver desenhando grid, redimensionar a imagem
                    w = mainImage.Width - (mainImage.Width % gridSize); // calcula nova largura da imagem
                    h = mainImage.Height - (mainImage.Height % gridSize); //calcula nova altura da imagem
                }
                if (w == mainImage.Width && h == mainImage.Height) // Verifica se é necessário redimensionar a imagem
                    picBoxImage.Image = mainImage;
                else
                    picBoxImage.Image = CropImage(mainImage, new Rectangle(0, 0, w, h)); picBoxImage.Image = mainImage;
                drawGrid();
                GC.Collect(); //Garbage collector
            } catch (System.IO.FileNotFoundException) { //Exceção de quando não é possível encontrar o arquivo
                MessageBox.Show("Não foi possível localizar o arquivo " + txtDirectory.Text);
            } catch(System.NullReferenceException) { // Exceção de quando a imagem é inválida
                MessageBox.Show("A imagem não é válida");
            }
        }

        // Função que desenha o grid
        private void drawGrid() {
            if (gridSize <= 0) // O grid não pode ter tamanho 0 nem valores negativos
                return;
            Console.Write("Drawing grid of size " + gridSize.ToString()); // Debug line
            pen = new Pen(Color.White); // Atribui a cor do grid
            g = Graphics.FromImage(picBoxImage.Image); // Onde vai ser desenhado o grid
            for(int i = 0; i < picBoxImage.Height || i < picBoxImage.Width; i += gridSize) { // Loop para desenhar o grid
                g.DrawLine(pen, i, 0, i, picBoxImage.Height); // Linhas verticais
                g.DrawLine(pen, 0, i, picBoxImage.Width, i); // Linhas horizontais
            }
            //Destrói os objetos
            pen.Dispose();
            g.Dispose();
            GC.Collect(); // Garbage collector
        }

        // Corta a imagem
        private Bitmap CropImage(Bitmap img, Rectangle area) {
            return img.Clone(area, img.PixelFormat);
        }

        // Tratador de eventos de quando o tamanho do grid é alterado
        private void cbGridSize_SelectedIndexChanged(object sender, EventArgs e) {
            picBoxImage.Image = new Bitmap(mainImage); // Cria uma nova imagem para não alterar a imagem principal
            gridSize = Convert.ToInt32(cbGridSize.Text); // Altera a variável global do tamanho do grid
            drawGrid();
        }
        
    }
}
