using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Ground_Truth {
    public partial class Form1 : Form {
        Bitmap mainImage; //Imagem principal, não é alterada
        Pen pen; // "Caneta" usada para desenhar o grid
        int gridSize = 25; //25 = Default
        string file; // diretório da imagem
        string read; // Linha lida do arquivo
        Graphics g;

        /*
         * Matriz de informação:
         * 0 = Não definido
         * 1 = Tem plantação
         * 2 = Ambos
         * 3 = Não tem plantação
         */
        int[,] mat;
        int isize, jsize; // Dimensões da matriz

        /* Formato do arquivo:
         * Linha 1 isize // largura da matriz
         * Linha 2 jsize // altura da matriz
         * Linha 3 valores...
         * Linha n valores...
         */
        string datfile;

        //Construtor
        public Form1() {
            InitializeComponent();
        }

        //Tratador do evento de clique na imagem
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
            int i = (int) (e.Y / gridSize), j = (int) (e.X / gridSize);
            Console.WriteLine("X: " + i + ", Y: " + j);
            if(mat[i,j]  < 3) {
                mat[i, j]++;
            } else {
                mat[i, j] = 0;
            }
            picBoxImage.Image = paintImageSquare(picBoxImage.Image, gridSize, mat[i, j], j, i);
        }

        //Parâmetros: Imagem, tamanho do quadrado, cor (valor na matriz), posição (i, j)
        private Bitmap paintImageSquare(Image image, int size, int color, int i, int j) {
            Console.WriteLine("Color = " + color);
            Bitmap newImage = new Bitmap(image);
            int r, g, b, xPos, yPos, increaseRatio = 100, decreaseRatio = 100;
            Color c;

            for(int x = 1; x < size; x++) {
                for (int y = 1; y < size; y++) {
                    xPos = j * gridSize + y;
                    yPos = i * gridSize + x;
                    r = mainImage.GetPixel(yPos, xPos).R;
                    g = mainImage.GetPixel(yPos, xPos).G;
                    b = mainImage.GetPixel(yPos, xPos).B;
                    switch (color) {
                        case 0: // Indefinido - Não muda a cor
                            c = mainImage.GetPixel(yPos, xPos);
                            newImage.SetPixel(yPos, xPos, c);
                            break;
                        case 1: // Plantação - Verde
                            c = Color.FromArgb(255, Math.Max(r - decreaseRatio , 0) , Math.Min(g + increaseRatio, 255), Math.Max(b - decreaseRatio, 0));
                            newImage.SetPixel(yPos, xPos, c);
                            break;
                        case 2: // Ambos - Amarelo
                            c = Color.FromArgb(255, Math.Min(r + increaseRatio, 255), Math.Min(g + increaseRatio, 255), Math.Max(b - decreaseRatio, 0));
                            newImage.SetPixel(yPos, xPos, c);
                            break;
                        case 3: // Não plantação - Vermelho
                            c = Color.FromArgb(255, Math.Min(r + increaseRatio, 255), Math.Max(g - decreaseRatio, 0), Math.Max(b - decreaseRatio, 0));
                            newImage.SetPixel(yPos, xPos, c);
                            break;
                        default:
                            MessageBox.Show("Valor inválido na matriz de dados aqui");
                            break;
                    }
                }
            }
            return newImage;
        }

        //Função chamada quando é clicado OK na janela de diálogo de abrir o arquivo
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
            int w, h; //width, height - Largura e altura da imagem
            file = openFileDialog1.FileName; // atribuindo a localização da imagem
            mainImage = new Bitmap(Image.FromFile(file));
            w = mainImage.Width;
            h = mainImage.Height;
            try {
                gridSize = Convert.ToInt32(cbGridSize.Text); //atribui à variável global o tamanho do grid
            } catch (System.FormatException) {
                gridSize = 0; //se houver problema na atribuição, atribui 0
            }
            if (gridSize > 0) { //verifica se o grid está sendo desenhado
                w = mainImage.Width - (mainImage.Width % gridSize); //calcula a nova largura da imagem
                h = mainImage.Height - (mainImage.Height % gridSize); //calcula a nova altura da imagem
            }
            /*
             * Se o tamanho da imagem original for do mesmo 
             * tamanho da imagem redimensionada calculada, 
             * não é necessário redimensionar a imagem
             */
            picBoxImage.Enabled = true; // Habilita o clique na imagem
            // Verifica a necessidade de redimensionar a imagem
            if (w == mainImage.Width && h == mainImage.Height)
                picBoxImage.Image = new Bitmap (mainImage);
            else
                picBoxImage.Image = new Bitmap(CropImage(mainImage, new Rectangle(0, 0, w, h)));

            drawGrid();
            cbGridSize.Enabled = true;
            startMatrix(); // Inicia a matriz com o tamanho de gridSize
        }

        //Tratador de eventos de quando clica no botão "..."
        private void btnSearch_Click(object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
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
                file = txtDirectory.Text; // atribuindo a localização da imagem
                w = mainImage.Width;
                h = mainImage.Height;
                if (gridSize > 0) { // Se tiver desenhando grid, redimensionar a imagem
                    w = mainImage.Width - (mainImage.Width % gridSize); // calcula nova largura da imagem
                    h = mainImage.Height - (mainImage.Height % gridSize); //calcula nova altura da imagem
                }
                picBoxImage.Enabled = true;
                if (w == mainImage.Width && h == mainImage.Height) // Verifica se é necessário redimensionar a imagem
                    picBoxImage.Image = new Bitmap(mainImage);
                else
                    picBoxImage.Image = new Bitmap(CropImage(mainImage, new Rectangle(0, 0, w, h))); 
                drawGrid();
                GC.Collect(); //Garbage collector
                cbGridSize.Enabled = true;
                startMatrix();
            } catch (System.IO.FileNotFoundException) { //Exceção de quando não é possível encontrar o arquivo
                MessageBox.Show("Não foi possível localizar o arquivo " + txtDirectory.Text);
            } catch (System.NullReferenceException) { // Exceção de quando a imagem é inválida
                MessageBox.Show("A imagem não é válida");
            }
        }

        // Função que desenha o grid
        private void drawGrid() {
            if (gridSize <= 0) // O grid não pode ter tamanho 0 nem valores negativos
                return;
            Console.WriteLine("Drawing grid of size " + gridSize.ToString()); // Debug line
            pen = new Pen(Color.White); // Atribui a cor do grid
            g = Graphics.FromImage(picBoxImage.Image); // Onde vai ser desenhado o grid
            for (int i = 0; i < picBoxImage.Height || i < picBoxImage.Width; i += gridSize) { // Loop para desenhar o grid
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
            int w, h;
            gridSize = Convert.ToInt32(cbGridSize.Text); // Altera a variável global do tamanho do grid
            Console.WriteLine("gridSize: " + gridSize.ToString());

            w = mainImage.Width - (mainImage.Width % gridSize); // calcula nova largura da imagem
            h = mainImage.Height - (mainImage.Height % gridSize); //calcula nova altura da imagem

            if (w == mainImage.Width && h == mainImage.Height) // Verifica se é necessário redimensionar a imagem
                picBoxImage.Image = new Bitmap(mainImage);
            else
                picBoxImage.Image = new Bitmap(CropImage(mainImage, new Rectangle(0, 0, w, h)));

            drawGrid();
            startMatrix();
        }

        private void updateMatrix(int value, int x, int y) {
            if (mat == null) {
                Console.WriteLine("A matriz de dados não existia, criando uma nova matriz");
                startMatrix();
            }
            mat[x, y] = value;
            File.Delete(datfile);
            startMatrix();
        }

        private void startMatrix() {
            if(mat != null && gridSize != picBoxImage.Width / isize) {
                Console.WriteLine("Mudando o tamanho da matriz");
                mat = null;
                GC.Collect();
            }
                
            switch (gridSize) {
                case 25:
                    datfile = file.Substring(0, file.Length - 4) + "_25.dat";
                    // Se o arquivo existir, ler do arquivo, se não, criar o arquivo
                    if (File.Exists(datfile)) {
                        using (StreamReader readText = new StreamReader(datfile)) {
                            isize = Convert.ToInt32(readText.ReadLine());
                            jsize = Convert.ToInt32(readText.ReadLine());
                            mat = new int[isize, jsize];
                            for (int i = 0; i < isize; i++) {
                                read = readText.ReadLine();
                                Console.WriteLine(read);
                                for (int j = 0; j < jsize; j++) {
                                    mat[i, j] = read[j];
                                }
                            }
                        }
                    } else {
                        using (StreamWriter writeText = new StreamWriter(datfile)) {
                            isize = picBoxImage.Image.Height / 25;
                            jsize = picBoxImage.Image.Width / 25;
                            mat = new int[isize, jsize];
                            writeText.WriteLine(isize.ToString());
                            writeText.WriteLine(jsize.ToString());
                            for (int i = 0; i < isize; i++) {
                                for (int j = 0; j < jsize; j++) {
                                    writeText.Write(mat[i, j].ToString());
                                }
                                writeText.WriteLine();
                            }
                        }
                    }
                    break;
                case 50:
                    datfile = file.Substring(0, file.Length - 4) + "_50.dat";
                    if (File.Exists(datfile)) {
                        using (StreamReader readText = new StreamReader(datfile)) {
                            isize = Convert.ToInt32(readText.ReadLine());
                            jsize = Convert.ToInt32(readText.ReadLine());
                            mat = new int[isize, jsize];
                            for (int i = 0; i < isize; i++) {
                                read = readText.ReadLine();

                                for (int j = 0; j < jsize; j++) {
                                    mat[i, j] = read[j];
                                }
                            }
                        }
                    } else {
                        using (StreamWriter writeText = new StreamWriter(datfile)) {
                            isize = picBoxImage.Image.Height / 50;
                            jsize = picBoxImage.Image.Width / 50;
                            mat = new int[isize, jsize];
                            writeText.WriteLine(isize.ToString());
                            writeText.WriteLine(jsize.ToString());
                            for (int i = 0; i < isize; i++) {
                                for (int j = 0; j < jsize; j++) {
                                    writeText.Write(mat[i, j].ToString());
                                }
                                writeText.WriteLine();
                            }
                        }
                    }
                    break;
                case 80:
                    datfile = file.Substring(0, file.Length - 4) + "_80.dat";
                    if (File.Exists(datfile)) {
                        using (StreamReader readText = new StreamReader(datfile)) {
                            isize = Convert.ToInt32(readText.ReadLine());
                            jsize = Convert.ToInt32(readText.ReadLine());
                            mat = new int[isize, jsize];
                            for (int i = 0; i < isize; i++) {
                                read = readText.ReadLine();

                                for (int j = 0; j < jsize; j++) {
                                    mat[i, j] = read[j];
                                }
                            }
                        }
                    } else {
                        using (StreamWriter writeText = new StreamWriter(datfile)) {
                            isize = picBoxImage.Image.Height / 80;
                            jsize = picBoxImage.Image.Width / 80;
                            mat = new int[jsize, jsize];
                            writeText.WriteLine(isize.ToString());
                            writeText.WriteLine(jsize.ToString());
                            for (int i = 0; i < isize; i++) {
                                for (int j = 0; j < jsize; j++) {
                                    writeText.Write(mat[i, j].ToString());
                                }
                                writeText.WriteLine();
                            }
                        }
                    }
                    break;
                default:
                    MessageBox.Show("Tamanho do grid invalido");
                    break;
            }
        }
    }
        
}
