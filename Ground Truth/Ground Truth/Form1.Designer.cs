using System.Drawing;

namespace Ground_Truth {
    partial class Form1 {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.openFileDialogImg = new System.Windows.Forms.OpenFileDialog();
            this.picBoxImage = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbGridSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbZoom = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuComparar = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAtivar = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDesativar = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSalvarImagem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogClf = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxImage)).BeginInit();
            this.panel1.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtDirectory
            // 
            this.txtDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDirectory.Location = new System.Drawing.Point(12, 519);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.Size = new System.Drawing.Size(350, 22);
            this.txtDirectory.TabIndex = 0;
            this.txtDirectory.Text = "C:\\";
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(368, 522);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(42, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "...";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Location = new System.Drawing.Point(416, 522);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(59, 23);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Abrir";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.BtnOpen_Click);
            // 
            // openFileDialogImg
            // 
            this.openFileDialogImg.Filter = "Imagem JPG|*.jpg|Imagem PNG|*.png";
            this.openFileDialogImg.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialogImg_FileOk);
            // 
            // picBoxImage
            // 
            this.picBoxImage.Location = new System.Drawing.Point(0, 0);
            this.picBoxImage.Name = "picBoxImage";
            this.picBoxImage.Size = new System.Drawing.Size(30, 30);
            this.picBoxImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picBoxImage.TabIndex = 6;
            this.picBoxImage.TabStop = false;
            this.picBoxImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseDown);
            this.picBoxImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseMove);
            this.picBoxImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_Mouse_up);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.picBoxImage);
            this.panel1.Location = new System.Drawing.Point(12, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(758, 472);
            this.panel1.TabIndex = 7;
            // 
            // cbGridSize
            // 
            this.cbGridSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbGridSize.Enabled = false;
            this.cbGridSize.FormattingEnabled = true;
            this.cbGridSize.Items.AddRange(new object[] {
            "16",
            "25",
            "32",
            "50"});
            this.cbGridSize.Location = new System.Drawing.Point(720, 518);
            this.cbGridSize.Name = "cbGridSize";
            this.cbGridSize.Size = new System.Drawing.Size(50, 24);
            this.cbGridSize.TabIndex = 5;
            this.cbGridSize.Text = "32";
            this.cbGridSize.SelectedIndexChanged += new System.EventHandler(this.CbGridSize_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(594, 522);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "Tamanho do grid:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(481, 522);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Zoom: ";
            // 
            // cbZoom
            // 
            this.cbZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbZoom.Enabled = false;
            this.cbZoom.FormattingEnabled = true;
            this.cbZoom.Items.AddRange(new object[] {
            "1x",
            "2x"});
            this.cbZoom.Location = new System.Drawing.Point(532, 519);
            this.cbZoom.Name = "cbZoom";
            this.cbZoom.Size = new System.Drawing.Size(49, 24);
            this.cbZoom.TabIndex = 4;
            this.cbZoom.Text = "1x";
            this.cbZoom.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Location = new System.Drawing.Point(0, 28);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(782, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuStrip2
            // 
            this.menuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(782, 28);
            this.menuStrip2.TabIndex = 12;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuComparar,
            this.menuSalvarImagem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(102, 24);
            this.toolsToolStripMenuItem.Text = "Ferramentas";
            // 
            // menuComparar
            // 
            this.menuComparar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menuComparar.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAtivar,
            this.menuDesativar});
            this.menuComparar.Enabled = false;
            this.menuComparar.Name = "menuComparar";
            this.menuComparar.ShowShortcutKeys = false;
            this.menuComparar.Size = new System.Drawing.Size(234, 26);
            this.menuComparar.Text = "Comparar classificações";
            // 
            // menuAtivar
            // 
            this.menuAtivar.Name = "menuAtivar";
            this.menuAtivar.ShowShortcutKeys = false;
            this.menuAtivar.Size = new System.Drawing.Size(146, 26);
            this.menuAtivar.Text = "Ativar";
            this.menuAtivar.Click += new System.EventHandler(this.MenuAtivar_Click);
            // 
            // menuDesativar
            // 
            this.menuDesativar.Enabled = false;
            this.menuDesativar.Name = "menuDesativar";
            this.menuDesativar.Size = new System.Drawing.Size(146, 26);
            this.menuDesativar.Text = "Desativar";
            this.menuDesativar.Click += new System.EventHandler(this.MenuDesativar_Click);
            // 
            // menuSalvarImagem
            // 
            this.menuSalvarImagem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menuSalvarImagem.Enabled = false;
            this.menuSalvarImagem.Name = "menuSalvarImagem";
            this.menuSalvarImagem.Size = new System.Drawing.Size(234, 26);
            this.menuSalvarImagem.Text = "Exportar JPG";
            this.menuSalvarImagem.Click += new System.EventHandler(this.SalvarImagemToolStripMenuItem_Click);
            // 
            // openFileDialogClf
            // 
            this.openFileDialogClf.Filter = "Arquivo de dados|*.dat";
            this.openFileDialogClf.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialogClf_FileOk);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 553);
            this.Controls.Add(this.cbZoom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbGridSize);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtDirectory);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.menuStrip2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Ground Truth";
            ((System.ComponentModel.ISupportInitialize)(this.picBoxImage)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDirectory;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.OpenFileDialog openFileDialogImg;
        private System.Windows.Forms.PictureBox picBoxImage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cbGridSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbZoom;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuComparar;
        private System.Windows.Forms.ToolStripMenuItem menuSalvarImagem;
        private System.Windows.Forms.OpenFileDialog openFileDialogClf;
        private System.Windows.Forms.ToolStripMenuItem menuAtivar;
        private System.Windows.Forms.ToolStripMenuItem menuDesativar;
    }
}

