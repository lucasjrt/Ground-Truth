using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Ground_Truth {
    public partial class Form1 : Form {
        Bitmap mainImage; // Imagem principal, não é alterada
        Bitmap zoomImage; // Imagem com zoom
        Pen pen; // "Caneta" usada para desenhar o grid
        int gridSize = 25; //25 = Default
        int zoom = 1; // 1 = Default
        int w, h; // Width, Height - Dimensões da imagem redimensionada
        string file; // Diretório da imagem
        string read; // Linha lida do arquivo
        Size zoomSize; // Tamanho da imagem redimensionada
        Graphics g;
        int increaseRatio = 100, decreaseRatio = 100; // increaseRatio / decreaseRatio - Taxa de alteração de cor

        bool ctrl_pressed = false;
        Point first;

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
         * Linha 1 isize // altura da matriz
         * Linha 2 jsize // largura da matriz
         * Linha 3 valores...
         * Linha n valores...
         */
        string datfile; // Caminho do arquivo de dados
        string imagefile; // Caminho da imagem modificada

        public Form1() {
            InitializeComponent();
        }

        //Tratador do evento de clique na imagem
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e) {
            int i = (int) (e.Y / (gridSize * zoom)), j = (int) (e.X / (gridSize * zoom));

            if (e.Button == MouseButtons.Left) // Se o clique for com o botão esquerdo, troca a cor pra frente
                if (mat[i, j] < 3)
                    mat[i, j]++;
                else
                    mat[i, j] = 0;
             else if (e.Button == MouseButtons.Right) // Se o clique for com o botão direito, troca a cor pra trás
                if (mat[i, j] > 0)
                    mat[i, j]--;
                else
                    mat[i, j] = 3;

            if (Control.ModifierKeys == Keys.Control && !ctrl_pressed) {
                ctrl_pressed = true;
                first = new Point(j, i);
                PaintImageSquare(i, j);
                UpdateMatrix(mat[i, j], i, j);
            } else if (Control.ModifierKeys == Keys.Control & ctrl_pressed) {
                for(int y = Math.Min(first.Y, i); y <= Math.Min(first.Y, i) + Math.Abs(first.Y - i); y++) {
                    for(int x = Math.Min(first.X, j); x <= Math.Min(first.X, j) + Math.Abs(first.X - j); x++) {
                        UpdateMatrix(mat[first.Y, first.X], y, x);
                    }
                }
                PaintImageRectangle(Math.Min(first.Y, i), Math.Min(first.X, j), Math.Max(i, first.Y), Math.Max(j, first.X));
                ctrl_pressed = false;
            } else {
                ctrl_pressed = false;
                PaintImageSquare(i, j);
                UpdateMatrix(mat[i, j], i, j);
            }
            GC.Collect();
        }

        // Colore um quadrado da posição (i, j) até a posição (i + gridSize, j + gridSize)
        private void PaintImageSquare(int i, int j) {
            Bitmap newImage = new Bitmap(picBoxImage.Image);
            int r, g, b, xPos, yPos;
            Color c;
            for(int y = 1; y < gridSize * zoom; y++) {
                for (int x = 1; x < gridSize * zoom; x++) {
                    xPos = i * gridSize * zoom + x;
                    yPos = j * gridSize * zoom + y;
                    r = zoomImage.GetPixel(yPos, xPos).R;
                    g = zoomImage.GetPixel(yPos, xPos).G;
                    b = zoomImage.GetPixel(yPos, xPos).B;
                    switch (mat[i,j]) {
                        case 0: // Indefinido - Não muda a cor
                            c = zoomImage.GetPixel(yPos, xPos);
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
                        default: // Apenas valores do preset são suportados
                            MessageBox.Show("Valor inválido na matriz de dados na posição [" + i + "," + j + "]");
                            break;
                    }
                }
            }
            picBoxImage.Image = newImage;
        }

        // Color a imagem entre dois pontos
        private void PaintImageRectangle(int i0, int j0, int i1, int j1) {
            Bitmap newImage = new Bitmap(picBoxImage.Image);
            int r, g, b, xPos, yPos;
            Color c;

            for (int y = 1; y < gridSize * zoom * (Math.Abs(j1 - j0) + 1); y++) {
                for (int x = 1; x < gridSize * zoom * (Math.Abs(i1 - i0) + 1); x++) {
                    if (x % (gridSize * zoom) == 0 || y % (gridSize * zoom) == 0)
                        continue;
                    xPos = i0 * gridSize * zoom + x;
                    yPos = j0 * gridSize * zoom + y;
                    r = zoomImage.GetPixel(yPos, xPos).R;
                    g = zoomImage.GetPixel(yPos, xPos).G;
                    b = zoomImage.GetPixel(yPos, xPos).B;
                    switch (mat[i0, j0]) {
                        case 0: // Indefinido - Não muda a cor
                            c = zoomImage.GetPixel(yPos, xPos);
                            newImage.SetPixel(yPos, xPos, c);
                            break;
                        case 1: // Plantação - Verde
                            c = Color.FromArgb(255, Math.Max(r - decreaseRatio, 0), Math.Min(g + increaseRatio, 255), Math.Max(b - decreaseRatio, 0));
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
                        default: // Apenas valores do preset são suportados
                            MessageBox.Show("Valor inválido na matriz de dados na posição [" + i0 + "," + j0 + "]");
                            break;
                    }
                }
            }
            picBoxImage.Image = newImage;
        }

        // Colore a imagem se existir o arquivo de dados
        private void LoadColors() {
            int r, g, b, xPos, yPos, increaseRatio = 100, decreaseRatio = 100;
            Color c;
            Bitmap newImage = new Bitmap(picBoxImage.Image);
            for (int i = 0; i < isize; i++) { // Percorrendo a matriz
                for(int j = 0; j < jsize; j++) { 
                    if(mat[i,j] != 0) { // Só colore o quadrado se necessário
                        for(int y = 0; y < gridSize * zoom; y++) { // Percorrendo os pixels da imagem
                            for (int x = 0; x < gridSize * zoom; x++) {
                                xPos = i * gridSize * zoom + x;
                                yPos = j * gridSize * zoom + y;
                                r = zoomImage.GetPixel(yPos, xPos).R;
                                g = zoomImage.GetPixel(yPos, xPos).G;
                                b = zoomImage.GetPixel(yPos, xPos).B;
                                switch (mat[i,j]) {
                                    case 0: // Indefinido - Não muda a cor
                                        c = zoomImage.GetPixel(yPos, xPos);
                                        newImage.SetPixel(yPos, xPos, c);
                                        break;
                                    case 1: // Plantação - Verde
                                        c = Color.FromArgb(255, Math.Max(r - decreaseRatio, 0), Math.Min(g + increaseRatio, 255), Math.Max(b - decreaseRatio, 0));
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
                                        MessageBox.Show("Valor inválido na matriz de dados na posição [" + i + "," + j + "]");
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            picBoxImage.Image = newImage;
            GC.Collect();
        }

        //Função chamada quando é clicado OK na janela de diálogo de abrir o arquivo
        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e) {
            file = openFileDialog1.FileName; // atribuindo a localização da imagem
            mainImage = new Bitmap(Image.FromFile(file));
            zoomImage = mainImage;
            zoomSize = mainImage.Size;
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

            cbGridSize.Enabled = true;
            cbZoom.Enabled = true;
            btnSalvar.Enabled = true;
            if(StartMatrix())
                LoadColors(); // Colore a imagem conforme a matriz de dados
            DrawGrid();
        }

        //Tratador de eventos de quando clica no botão "..."
        private void BtnSearch_Click(object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                txtDirectory.Text = openFileDialog1.FileName;
            }
        }

        //Tratador de eventos de quando clica no botão "Abrir"
        private void BtnOpen_Click(object sender, EventArgs e) {
            try {
                if (mainImage != null) // Se a imagem principal já tem uma instância
                    mainImage.Dispose(); // Descarta essa instância, por motivos de memória
                mainImage = new Bitmap(Image.FromFile(txtDirectory.Text));
                zoomImage = mainImage;
                zoomSize = mainImage.Size;
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
                GC.Collect(); //Garbage collector
                cbGridSize.Enabled = true;
                cbZoom.Enabled = true;
                btnSalvar.Enabled = true;
                if (StartMatrix())
                    LoadColors();
                DrawGrid();
            } catch (System.IO.FileNotFoundException) { //Exceção de quando não é possível encontrar o arquivo
                MessageBox.Show("Não foi possível localizar o arquivo " + txtDirectory.Text);
            } catch (System.NullReferenceException) { // Exceção de quando a imagem é inválida
                MessageBox.Show("A imagem não é válida");
            }
        }

        // Função que desenha o grid
        private void DrawGrid() {
            if (gridSize <= 0) // O grid não pode ter tamanho 0 nem valores negativos
                return;
            pen = new Pen(Color.White); // Atribui a cor do grid
            g = Graphics.FromImage(picBoxImage.Image); // Onde vai ser desenhado o grid
            for (int i = 0; i < picBoxImage.Height || i < picBoxImage.Width; i += gridSize * zoom) { // Loop para desenhar o grid
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
        private void CbGridSize_SelectedIndexChanged(object sender, EventArgs e) {
            int w, h;
            gridSize = Convert.ToInt32(cbGridSize.Text); // Altera a variável global do tamanho do grid

            w = mainImage.Width - (mainImage.Width % gridSize); // calcula nova largura da imagem
            h = mainImage.Height - (mainImage.Height % gridSize); //calcula nova altura da imagem

            if (w == mainImage.Width && h == mainImage.Height) // Verifica se é necessário redimensionar a imagem
                picBoxImage.Image = new Bitmap(mainImage);
            else
                picBoxImage.Image = new Bitmap(CropImage(mainImage, new Rectangle(0, 0, w, h)));

            if (StartMatrix())
                LoadColors();
            DrawGrid();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            zoom = Convert.ToInt32(Convert.ToString(cbZoom.Text[0]));
            ReloadImage();
        }

        private void ReloadImage() {
            zoomImage = mainImage;
            zoomSize = new Size((int)(w * zoom), (int)(h * zoom));
            Bitmap bmp = new Bitmap(mainImage, zoomSize);
            picBoxImage.Size = zoomSize;
            picBoxImage.SizeMode = PictureBoxSizeMode.Normal;
            zoomImage = bmp;
            picBoxImage.Image = zoomImage;
            picBoxImage.Size = zoomSize;
            GC.Collect();
            LoadColors();
            DrawGrid();
        }

        private void btnSalvar_Click(object sender, EventArgs e) {
            if(picBoxImage.Image == null) {
                MessageBox.Show("Imagem inválida");
                return;
            }

            imagefile = file.Substring(0, file.Length - 4) + "_" + gridSize.ToString() + ".jpg";

            if(File.Exists(imagefile)) {
                File.Delete(imagefile);
            }

            picBoxImage.Image.Save(imagefile);
        }

        // Atualiza a matriz no disco
        private void UpdateMatrix(int value, int x, int y) {
            if (mat == null)
                StartMatrix();

            mat[x, y] = value;
            if (File.Exists(datfile)) {
                File.Delete(datfile);
                using (StreamWriter writeText = new StreamWriter(datfile)) {
                    writeText.WriteLine(isize);
                    writeText.WriteLine(jsize);
                    for (int i = 0; i < isize; i++) {
                        for (int j = 0; j < jsize; j++) {
                            writeText.Write(mat[i, j]);
                        }
                        writeText.WriteLine();
                    }
                }
            } else {
                MessageBox.Show("Não foi encontrado o arquivo de dados para ser atualizado, feche e abra novamente o Ground Truth");
            }
        }

        /*
         * Inicializa a matriz com os valores do arquivo
         * de dados, caso o arquivo não exista, cria-se 
         * um novo arquivo com a matriz completamente zerada
         * 
         * Retorna true caso a matriz já existe, e falso 
         * se a matriz teve que ser criada
         */
        private bool StartMatrix() {
            bool exists = false;
            gridSize = Convert.ToInt32(cbGridSize.Text);

            if(mat != null) {
                mat = null;
                GC.Collect();
            }
                
            // Verificação do nome que o arquivo terá
            switch (gridSize) {
                case 25:
                    datfile = file.Substring(0, file.Length - 4) + "_25.dat";
                    break;
                case 50:
                    datfile = file.Substring(0, file.Length - 4) + "_50.dat";
                    break;
                case 80:
                    datfile = file.Substring(0, file.Length - 4) + "_80.dat";
                    break;
            }

            // Dimensões da matriz
            isize = picBoxImage.Image.Height / gridSize; 
            jsize = picBoxImage.Image.Width/ gridSize;

            // Se o arquivo existir, ler do arquivo, se não, criar o arquivo
            if (File.Exists(datfile)) {
                exists = true;
                using (StreamReader readText = new StreamReader(datfile)) {
                    isize = Convert.ToInt32(readText.ReadLine());
                    jsize = Convert.ToInt32(readText.ReadLine());
                    mat = new int[isize, jsize];
                    for (int i = 0; i < isize; i++) {
                        read = readText.ReadLine();
                        for (int j = 0; j < jsize; j++) {
                            mat[i, j] = Convert.ToInt32(Convert.ToString(read[j]));
                        }
                    }
                }
            } else {
                using (StreamWriter writeText = new StreamWriter(datfile)) {
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
            return exists;
        }

        private void Swap(int i, int j) {
            int aux;
            aux = i;
            i = j;
            j = aux;
        }
    }    
}
