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
        int gridSize = 32; //32 = Default
        int zoom = 1; // 1 = Default
        int w, h; // Width, Height - Dimensões da imagem redimensionada
        string file; // Diretório da imagem
        string dat_file; // Em caso de comparação de arquivos dat, aqui vai ser o endereço do arquivo *.dat
        Size zoomSize; // Tamanho da imagem redimensionada
        Graphics g;
        int increaseRatio = 155, decreaseRatio = 155; // increaseRatio / decreaseRatio - Taxa de alteração de cor

        bool ctrl_pressed = false; // Usado para colorir um retângulo grande
        int color_dragging = 0; // Decide a cor que vai ser colorida quando clicar e arrastar
        Point first; // Guarda o primeiro ponto de onde vai ser colorido o retângulo
        Point oldMouseLocation; // Usado para verificar se houve movimentação do mouse
        Point oldSquare = new Point(); // Usado para verificar se o mouse trocou de quadrado

        /*
         * Matriz de informação:
         * 0 = Não definido
         * 1 = Tem plantação
         * 2 = Ambos
         * 3 = Não tem plantação
         */
        int[,] mat;
        int isize, jsize; // Dimensões da matriz

        int[,] dat; // Matriz para comparação de consistência de classificação entre dois .dat distintos

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

        // Tratador do evento de segurar o botão na imagem
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e) {
            oldMouseLocation = e.Location;
            try {
                int i = (int)(e.Y / (gridSize * zoom)), j = (int)(e.X / (gridSize * zoom));
                if (menuComparar.Checked && mat[i,j] == dat[i,j] && ((Bitmap)picBoxImage.Image).GetPixel(j * zoom * gridSize + 2, i * zoom * gridSize + 2) == Color.FromArgb(255, 0, 0 , 0))
                    return;
                oldSquare.X = j;
                oldSquare.Y = i;
                if (e.Button == MouseButtons.Left) // Se o clique for com o botão esquerdo, troca a cor pra frente
                    mat[i, j] = (1 + mat[i, j]) % 4;
                else if (e.Button == MouseButtons.Right) // Se o clique for com o botão direito, troca a cor pra trás
                    mat[i, j] = (3 + mat[i,j]) % 4;
                color_dragging = mat[i, j];

                if (Control.ModifierKeys == Keys.Control && !ctrl_pressed) {
                    ctrl_pressed = true;
                    first = new Point(j, i);
                    PaintImageSquare(i, j);
                } else if (Control.ModifierKeys == Keys.Control && ctrl_pressed) {
                    for (int y = Math.Min(first.Y, i); y <= Math.Min(first.Y, i) + Math.Abs(first.Y - i); y++) {
                        for (int x = Math.Min(first.X, j); x <= Math.Min(first.X, j) + Math.Abs(first.X - j); x++) {
                            mat[y, x] = mat[first.Y, first.X];
                        }
                    }
                    PaintImageRectangle(Math.Min(first.Y, i), Math.Min(first.X, j), Math.Max(i, first.Y), Math.Max(j, first.X));
                    ctrl_pressed = false;
                } else {
                    ctrl_pressed = false;
                    PaintImageSquare(i, j);
                }
                GC.Collect();
            }
            catch (IndexOutOfRangeException ex) {
                Console.WriteLine(ex.StackTrace);
            } 
            catch (NullReferenceException ex) {
                Console.WriteLine(ex.StackTrace);
            }
        }

        // Tratador do evento de clicar e arrastar na imagem
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e) {
            Point newPos = e.Location;
            try {
                int i = (int)(e.Y / (gridSize * zoom)), j = (int)(e.X / (gridSize * zoom));
                if ((e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                    && (newPos.X != oldMouseLocation.X || newPos.Y != oldMouseLocation.Y)
                    && (oldSquare.X != j || oldSquare.Y != i)
                    && !ctrl_pressed) {

                    mat[i, j] = color_dragging;

                    PaintImageSquare(i, j);

                    oldSquare.X = j;
                    oldSquare.Y = i;
                    oldMouseLocation = newPos;

                }
                GC.Collect();
            } 
            catch (IndexOutOfRangeException) {} 
            catch (NullReferenceException) {}
        }

        private void PictureBox1_Mouse_up(object sender, MouseEventArgs e) {
            SaveMatrix();
        }

        // Colore um quadrado na posição (i, j)
        private void PaintImageSquare(int i, int j) {
            int r, g, b, yPos, xPos;
            Color c;
            for(int y = 1; y < gridSize * zoom; y++) {
                for (int x = 1; x < gridSize * zoom; x++) {
                    xPos = j * gridSize * zoom + x;
                    yPos = i * gridSize * zoom + y;
                    r = zoomImage.GetPixel(xPos, yPos).R;
                    g = zoomImage.GetPixel(xPos, yPos).G;
                    b = zoomImage.GetPixel(xPos, yPos).B;
                    switch (mat[i,j]) {
                        case 0: // Indefinido - Não muda a cor
                            c = zoomImage.GetPixel(xPos, yPos);
                            ((Bitmap)picBoxImage.Image).SetPixel(xPos, yPos, c);
                            break;
                        case 1: // Plantação - Verde
                            c = Color.FromArgb(255, Math.Max(r - decreaseRatio , 0) , Math.Min(g + increaseRatio, 255), Math.Max(b - decreaseRatio, 0));
                            ((Bitmap)picBoxImage.Image).SetPixel(xPos, yPos, c);
                            break;
                        case 2: // Ambos - Amarelo
                            c = Color.FromArgb(255, Math.Min(r + increaseRatio, 255), Math.Min(g + increaseRatio, 255), Math.Max(b - decreaseRatio, 0));
                            ((Bitmap)picBoxImage.Image).SetPixel(xPos, yPos, c);
                            break;
                        case 3: // Não plantação - Vermelho
                            c = Color.FromArgb(255, Math.Min(r + increaseRatio, 255), Math.Max(g - decreaseRatio, 0), Math.Max(b - decreaseRatio, 0));
                            ((Bitmap)picBoxImage.Image).SetPixel(xPos, yPos, c);
                            break;
                        default: // Apenas valores do preset são suportados
                            MessageBox.Show("Valor inválido na matriz de dados na posição [" + i + "," + j + "]: " + mat[i,j]);
                            break;
                    }
                }
            }
            picBoxImage.Refresh();
        }

        // Colore a imagem entre dois pontos
        private void PaintImageRectangle(int i0, int j0, int i1, int j1) {
            int r, g, b, xPos, yPos;
            Color c;

            for (int y = 1; y < gridSize * zoom * (Math.Abs(j1 - j0) + 1); y++) {
                for (int x = 1; x < gridSize * zoom * (Math.Abs(i1 - i0) + 1); x++) {
                    // Não muda a cor de pixels que estão com o grid desenhado
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
                            ((Bitmap)picBoxImage.Image).SetPixel(yPos, xPos, c);
                            break;
                        case 1: // Plantação - Verde
                            c = Color.FromArgb(255, Math.Max(r - decreaseRatio, 0), Math.Min(g + increaseRatio, 255), Math.Max(b - decreaseRatio, 0));
                            ((Bitmap)picBoxImage.Image).SetPixel(yPos, xPos, c);
                            break;
                        case 2: // Ambos - Amarelo
                            c = Color.FromArgb(255, Math.Min(r + increaseRatio, 255), Math.Min(g + increaseRatio, 255), Math.Max(b - decreaseRatio, 0));
                            ((Bitmap)picBoxImage.Image).SetPixel(yPos, xPos, c);
                            break;
                        case 3: // Não plantação - Vermelho
                            c = Color.FromArgb(255, Math.Min(r + increaseRatio, 255), Math.Max(g - decreaseRatio, 0), Math.Max(b - decreaseRatio, 0));
                            ((Bitmap)picBoxImage.Image).SetPixel(yPos, xPos, c);
                            break;
                        default: // Apenas valores do preset são suportados
                            MessageBox.Show("Valor inválido na matriz de dados na posição [" + i0 + "," + j0 + "]: " + mat[i0,j0]);
                            break;
                    }
                }
            }
            picBoxImage.Refresh();
        }

        // Colore a imagem se existir o arquivo de dados
        private void LoadColors(bool hardLoad) {
            int r, g, b, yPos, xPos;
            Color c;
            for (int i = 0; i < isize; i++) { // Percorrendo a matriz
                for(int j = 0; j < jsize; j++) { 
                    if (menuComparar.Checked) {
                        if (mat[i, j] == dat[i, j] || mat[i, j] == 0 || dat[i, j] == 0) {
                            for (int y = 0; y < gridSize * zoom; y++) {
                                for (int x = 0; x < gridSize * zoom; x++) {
                                    xPos = j * gridSize * zoom + x;
                                    yPos = i * gridSize * zoom + y;
                                    ((Bitmap)picBoxImage.Image).SetPixel(xPos, yPos, Color.Black);
                                }
                            }
                        }
                    } else {
                        if (mat[i, j] != 0 || hardLoad) { // Só colore o quadrado se necessário
                            for (int y = 0; y < gridSize * zoom; y++) {
                                for (int x = 0; x < gridSize * zoom; x++) {
                                    xPos = j * gridSize * zoom + x;
                                    yPos = i * gridSize * zoom + y;
                                    r = zoomImage.GetPixel(xPos, yPos).R;
                                    g = zoomImage.GetPixel(xPos, yPos).G;
                                    b = zoomImage.GetPixel(xPos, yPos).B;

                                    switch (mat[i, j]) {
                                        case 0:
                                            c = Color.FromArgb(255, r, g, b);
                                            ((Bitmap)picBoxImage.Image).SetPixel(xPos, yPos, c);
                                            break;
                                        case 1: // Plantação - Verde
                                            c = Color.FromArgb(255, Math.Max(r - decreaseRatio, 0), Math.Min(g + increaseRatio, 255), Math.Max(b - decreaseRatio, 0));
                                            ((Bitmap)picBoxImage.Image).SetPixel(xPos, yPos, c);
                                            break;
                                        case 2: // Ambos - Amarelo
                                            c = Color.FromArgb(255, Math.Min(r + increaseRatio, 255), Math.Min(g + increaseRatio, 255), Math.Max(b - decreaseRatio, 0));
                                            ((Bitmap)picBoxImage.Image).SetPixel(xPos, yPos, c);
                                            break;
                                        case 3: // Não plantação - Vermelho
                                            c = Color.FromArgb(255, Math.Min(r + increaseRatio, 255), Math.Max(g - decreaseRatio, 0), Math.Max(b - decreaseRatio, 0));
                                            ((Bitmap)picBoxImage.Image).SetPixel(xPos, yPos, c);
                                            break;
                                        default:
                                            MessageBox.Show("Valor inválido na matriz de dados na posição [" + i + "," + j + "]: " + mat[i,j]);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            picBoxImage.Refresh();
            GC.Collect();
        }

        //Tratador do OK na janela de diálogo de abrir a imagem
        private void OpenFileDialogImg_FileOk(object sender, CancelEventArgs e) {
            file = openFileDialogImg.FileName; // atribuindo a localização da imagem
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
            /* Se o tamanho da imagem original for do mesmo 
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
            menuSalvarImagem.Enabled = true;
            menuComparar.Enabled = true;
            if(StartMatrix())
                LoadColors(false); // Colore a imagem conforme a matriz de dados
            DrawGrid();
        }

        //Tratador do OK na janela de diálogo de abrir o arquivo
        private void OpenFileDialogClf_FileOk(object sender, CancelEventArgs e) {
            dat_file = openFileDialogClf.FileName;
            LoadDat();
            cbGridSize.Enabled = false;
            menuComparar.Checked = true;
            menuAtivar.Enabled = true;
            Cursor.Current = Cursors.WaitCursor;
            LoadColors(false);
            DrawGrid();
            Cursor.Current = Cursors.Arrow;
        }

        //Tratador de eventos de quando clica no botão "..."
        private void BtnSearch_Click(object sender, EventArgs e) {
            if (openFileDialogImg.ShowDialog() == DialogResult.OK) {
                txtDirectory.Text = openFileDialogImg.FileName;
            }
        }

        // Ação de comprar dois arquivos de classificação
        private void MenuAtivar_Click(object sender, EventArgs e) {
            if (openFileDialogClf.ShowDialog() == DialogResult.OK) {
                menuDesativar.Enabled = true;
                menuComparar.Checked = true;
                menuAtivar.Enabled = false;
                btnSearch.Enabled = false;
                btnOpen.Enabled = false;
            }
        }

        // Ação de desativar a comparação entre dois arquivos de classificação
        private void MenuDesativar_Click(object sender, EventArgs e) {
            menuDesativar.Enabled = false;
            menuComparar.Checked = false;
            menuAtivar.Enabled = true;
            btnSearch.Enabled = true;
            btnOpen.Enabled = true;
            cbGridSize.Enabled = true;
            Cursor.Current = Cursors.WaitCursor;
            LoadColors(true);
            DrawGrid();
            Cursor.Current = Cursors.Arrow;
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
                menuSalvarImagem.Enabled = true;
                menuComparar.Enabled = true;
                if (StartMatrix())
                    LoadColors(false);
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
                picBoxImage.Refresh();
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
                LoadColors(false);
            DrawGrid();
        }

        // Quando o zoom é alterado
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            zoom = Convert.ToInt32(Convert.ToString(cbZoom.Text[0]));
            ReloadImage();
        }

        // Recarrega a imagem
        private void ReloadImage() {
            zoomImage = mainImage;
            zoomSize = new Size((int)(w * zoom), (int)(h * zoom));
            Bitmap bmp = new Bitmap(mainImage, zoomSize);
            picBoxImage.SizeMode = PictureBoxSizeMode.Normal;
            zoomImage = bmp;
            picBoxImage.Image = zoomImage;
            picBoxImage.Size = zoomSize;
            GC.Collect();
            LoadColors(false);
            DrawGrid();
        }

        // Salva a matriz em disco
        private void SaveMatrix() {
            if(mat == null || datfile == null) {
                StartMatrix();
                MessageBox.Show("Não foi encontrado uma matriz de dados, criando uma nova matriz");
            }

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
             
        }

        // Ação de salvar em disco a imagem classificada
        private void SalvarImagemToolStripMenuItem_Click(object sender, EventArgs e) {
            if (picBoxImage.Image == null) {
                MessageBox.Show("Imagem inválida");
                return;
            }

            imagefile = file.Substring(0, file.Length - 4) + "_" + gridSize.ToString() + ".jpg";

            if (File.Exists(imagefile)) {
                File.Delete(imagefile);
            }

            picBoxImage.Image.Save(imagefile);

            MessageBox.Show("Imagem salva com sucesso em \"" + imagefile + "\"");
        }

        // Atualiza a matriz no disco
        private bool UpdateMatrix(int value, int x, int y) {
            if (mat == null || datfile == null) {
                StartMatrix();
                MessageBox.Show("Não foi encontrado uma matriz de dados, criando uma nova matriz");
            }

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
                return false; // Retorna falso se ocorrer um erro
            }
            return true; // Retorna true se finalizar a execução com sucesso
        }
        
        /* Inicializa a matriz com os valores do arquivo
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
                
            // Nome que o arquivo terá
            datfile = file.Substring(0, file.Length - 4) + "_" + gridSize.ToString() + ".dat";

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
                    string read;
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

        private void LoadDat() {
            // Abrir o arquivo file
            using (StreamReader read = new StreamReader(dat_file)) {
                // Ler tamanho i e j e comparar se são iguais ao da imagem atual
                if(Convert.ToInt32(read.ReadLine()) != isize || Convert.ToInt32(read.ReadLine()) != jsize) {
                    MessageBox.Show("A matriz do arquivo de dados é de tamanho diferente da matriz atual");
                    return;
                }
                dat = new int[isize, jsize];
                string r;
                // Para cada valor do arquivo, atribuir para a matriz
                for(int i = 0; i < isize; i++) {
                    r = read.ReadLine();
                    for(int j = 0; j < jsize; j++) {
                        dat[i, j] = r[j] - 48;
                    }
                }
            }
        }

        private void Swap(int i, int j) {int aux;aux = i;i = j;j = aux;}
    }    
}
