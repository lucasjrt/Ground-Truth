using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ground_Truth
{
    public partial class Form1 : Form
    {
        Image imagemOriginal;

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

        private void picImagem_Click(object sender, EventArgs e) {}

        private void btnAbrir_Click(object sender, EventArgs e) {
            picImagem.Image = imagemOriginal;
            btnZoomIn.Enabled = true;
            btnZoomOut.Enabled = true;
        }

        private void btnZoomIn_Click(object sender, EventArgs e) {
            if(Convert.ToInt32(txtTamanho.Text) < picImagem.Image.Height && Convert.ToInt32(txtTamanho.Text) < picImagem.Image.Width)
               txtTamanho.Text =Convert.ToString(Convert.ToInt32(txtTamanho.Text) + 1);
        }

        private void btnZoomOut_Click(object sender, EventArgs e) {
            if(Convert.ToInt32(txtTamanho.Text) > 1)
                txtTamanho.Text = Convert.ToString(Convert.ToInt32(txtTamanho.Text) - 1);
        }
    }
}
