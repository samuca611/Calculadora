namespace _25736_24122_Calculadora;

public partial class Form1 : Form
{
    private bool limpandoTexto;

    public Form1()
    {
        InitializeComponent();
    }

    private void txtVisor_KeyPress(object sender, KeyPressEventArgs e)
    {
        char tecla = e.KeyChar;

        if (char.IsControl(tecla) ||
            char.IsDigit(tecla) ||
            tecla == '+' ||
            tecla == '-' ||
            tecla == '*' ||
            tecla == '/' ||
            tecla == '^' ||
            tecla == '(' ||
            tecla == ')' ||
            tecla == ',' ||
            tecla == '.' ||
            tecla == ' ')
        {
            return;
        }

        e.Handled = true;
    }

    private void txtVisor_TextChanged(object sender, EventArgs e)
    {
        if (limpandoTexto)
            return;

        string textoValido = "";

        foreach (char caractere in txtVisor.Text)
        {
            if (char.IsDigit(caractere) ||
                caractere == '+' ||
                caractere == '-' ||
                caractere == '*' ||
                caractere == '/' ||
                caractere == '^' ||
                caractere == '(' ||
                caractere == ')' ||
                caractere == ',' ||
                caractere == '.' ||
                caractere == ' ')
            {
                textoValido += caractere;
            }
        }

        if (textoValido != txtVisor.Text)
        {
            int posicao = Math.Min(txtVisor.SelectionStart, textoValido.Length);
            limpandoTexto = true;
            txtVisor.Text = textoValido;
            txtVisor.SelectionStart = posicao;
            limpandoTexto = false;
        }
    }

    private void btnNumeroOperador_Click(object sender, EventArgs e)
    {
        var botao = sender as Button;

        if (botao != null)
        {
            txtVisor.SelectedText = botao.Text;
            txtVisor.Focus();
        }
    }

    private void btnLimpar_Click(object sender, EventArgs e)
    {
        txtVisor.Clear();
        txtResultado.Clear();
        lbSequencias.Text = "Sequencias:";
        txtVisor.Focus();
    }

    private void btnApagar_Click(object sender, EventArgs e)
    {
        if (txtVisor.SelectionLength > 0)
        {
            txtVisor.SelectedText = "";
        }
        else if (txtVisor.SelectionStart > 0)
        {
            int posicao = txtVisor.SelectionStart;
            txtVisor.Text = txtVisor.Text.Remove(posicao - 1, 1);
            txtVisor.SelectionStart = posicao - 1;
        }

        txtVisor.Focus();
    }

    private void btnIgual_Click(object sender, EventArgs e)
    {
        try
        {
            ResultadoExpressao resultado = CalculadoraExpressao.Calcular(txtVisor.Text);

            lbSequencias.Text =
                "Sequencia infixa: " + resultado.InfixaComLetras + Environment.NewLine +
                "Sequencia pos-fixa: " + resultado.Posfixa + Environment.NewLine +
                "Valores: " + resultado.ValoresFormatados;

            txtResultado.Text = resultado.ValorFinal.ToString("0.##########");
        }
        catch (Exception erro)
        {
            txtResultado.Text = "Erro";
            lbSequencias.Text = "Sequencias:";
            MessageBox.Show(erro.Message, "Expressao invalida",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
