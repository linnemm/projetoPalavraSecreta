using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ProjetoPalavraSecreta
{
    public struct Palavra
    {
        public string palavra;
        public string dica;
        public int nivel;
        public int letrasVisiveis;
        public int tentativas;
        public int tempo;
        public string curiosidade;
    };

    public partial class Form1 : Form
    {
        //VARIÁVEIS
        Palavra[] dados = new Palavra[1000];
        int maxRegistros = 1000;
        int registrosLidos = 0;
        int maxChar = 30;

        string PalavraAtual;
        string Dica;
        string Curiosidade;
        int Tempo;
        int Tentativas;

        char[] auxPalavra = new char[1000];
        char[] letrasJogador = new char[1000];
        int acertos = 0;
        int chutes = 0;
        int flagJogo = -1;

        //CONSTRUTOR
        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < maxRegistros; i++)
                dados[i] = new Palavra();

            // LER DADOS DO ARQUIVO
            registrosLidos = LerDadosArquivo("..\\..\\..\\palavras.txt");

            timer1.Enabled = false;
            timer1.Interval = 1000;
        }

        // LER ARQUIVO
        public int LerDadosArquivo(string caminho)
        {
            registrosLidos = 0;
            try
            {
                using (StreamReader arq = new StreamReader(caminho, Encoding.UTF8))
                {
                    string buffer;
                    bool flag = true;
                    while (flag)
                    {
                        buffer = arq.ReadLine();
                        if (buffer == null || buffer[0] == '$')
                        {
                            flag = false;
                            break;
                        }

                        dados[registrosLidos].palavra = buffer;
                        buffer = arq.ReadLine();
                        dados[registrosLidos].dica = buffer;
                        buffer = arq.ReadLine();
                        dados[registrosLidos].nivel = int.Parse(buffer);
                        buffer = arq.ReadLine();
                        dados[registrosLidos].letrasVisiveis = int.Parse(buffer);
                        buffer = arq.ReadLine();
                        dados[registrosLidos].tentativas = int.Parse(buffer);
                        buffer = arq.ReadLine();
                        dados[registrosLidos].tempo = int.Parse(buffer);
                        buffer = arq.ReadLine();
                        dados[registrosLidos].curiosidade = buffer;

                        registrosLidos++;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Erro ao ler arquivo!");
                registrosLidos = -1;
            }

            return registrosLidos;
        }

        // BOTÃO: INICIAR 
        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if (registrosLidos <= 0)
            {
                MessageBox.Show("Nenhum registro encontrado!");
                return;
            }

            Random rd = new Random();
            int pos = rd.Next(0, registrosLidos);

            PalavraAtual = dados[pos].palavra.ToUpper();
            Dica = dados[pos].dica;
            Curiosidade = dados[pos].curiosidade;
            Tempo = dados[pos].tempo;
            Tentativas = dados[pos].tentativas;

            for (int i = 0; i < maxChar; i++)
            {
                auxPalavra[i] = ' ';
                letrasJogador[i] = ' ';
            }

            // Mostrar * no lugar das letras (com letras visíveis iniciais)
            for (int i = 0; i < PalavraAtual.Length; i++)
            {
                if (i < dados[pos].letrasVisiveis)
                    auxPalavra[i * 2] = PalavraAtual[i];
                else
                    auxPalavra[i * 2] = '*';
                auxPalavra[i * 2 + 1] = ' ';
            }

            lblPalavra.Text = new string(auxPalavra);
            lblLetras.Text = new string(letrasJogador);
            lblDica.Text = "DICA: " + Dica;
            lblTempo.Text = "TEMPO: " + Tempo.ToString() + " s";
            lblStatus.Text = "STATUS: JOGO INICIADO";
            lblTentativas.Text = "TENTATIVAS RESTANTES: " + Tentativas.ToString();

            chutes = 0;
            acertos = 0;
            flagJogo = 1;
            timer1.Enabled = true;

            pbxImagem.Image = Image.FromFile("..\\..\\..\\IMAGENS\\startgame.jpg");
        }

        // BOTÃO: VERIFICAR
        private void btnVerificar_Click(object sender, EventArgs e)
        {
            if (flagJogo <= 0)
            {
                lblStatus.Text = "STATUS: INICIE O JOGO";
                return;
            }

            if (string.IsNullOrEmpty(txbLetra.Text))
                return;

            char letra = char.ToUpper(txbLetra.Text[0]);
            bool acertou = false;

            for (int i = 0; i < PalavraAtual.Length; i++)
            {
                if (PalavraAtual[i] == letra)
                {
                    auxPalavra[i * 2] = letra;
                    acertou = true;
                    acertos++;
                }
            }

            lblPalavra.Text = new string(auxPalavra);
            letrasJogador[chutes] = letra;
            chutes++;

            if (!acertou)
                Tentativas--;

            lblTentativas.Text = "TENTATIVAS RESTANTES: " + Tentativas;
            lblLetras.Text = new string(letrasJogador);

            if (acertos == PalavraAtual.Length)
            {
                lblStatus.Text = "STATUS: VOCÊ VENCEU!";
                pbxImagem.Image = Image.FromFile("..\\..\\..\\IMAGENS\\venceu.jpg");
                flagJogo = -2;
                timer1.Enabled = false;
                MessageBox.Show("Curiosidade: " + Curiosidade);
                return;
            }

            if (Tentativas <= 0)
            {
                lblStatus.Text = "STATUS: GAME OVER";
                pbxImagem.Image = Image.FromFile("..\\..\\..\\IMAGENS\\gameover.jpg");
                flagJogo = -1;
                timer1.Enabled = false;
                MessageBox.Show("A palavra era: " + PalavraAtual + "\n\nCuriosidade: " + Curiosidade);
                return;
            }

            lblStatus.Text = acertou ? "STATUS: ACERTOU UMA LETRA!" : "STATUS: ERROU!";
            pbxImagem.Image = acertou ?
                Image.FromFile("..\\..\\..\\IMAGENS\\continueTentando.jpg") :
                Image.FromFile("..\\..\\..\\IMAGENS\\tenteNovamente.jpg");

            txbLetra.Clear();
            txbLetra.Focus();
        }

        // Timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            Tempo--;
            lblTempo.Text = "TEMPO: " + Tempo + " s";

            if (Tempo <= 0)
            {
                lblStatus.Text = "STATUS: TEMPO ESGOTADO - GAME OVER";
                pbxImagem.Image = Image.FromFile("..\\..\\..\\IMAGENS\\gameover.jpg");
                flagJogo = -1;
                timer1.Enabled = false;
                MessageBox.Show("Tempo esgotado! A palavra era: " + PalavraAtual);
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
