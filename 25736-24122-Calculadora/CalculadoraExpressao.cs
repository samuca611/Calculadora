using System.Globalization;
using System.Text;

namespace _25736_24122_Calculadora;

public class ResultadoExpressao
{
    public string InfixaComLetras { get; set; } = "";
    public string Posfixa { get; set; } = "";
    public double ValorFinal { get; set; }
    public string ValoresFormatados { get; set; } = "";
}

public static class CalculadoraExpressao
{
    private static readonly char[] Operadores = { '+', '-', '*', '/', '^' };

    public static ResultadoExpressao Calcular(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new InvalidOperationException("Digite uma expressao para calcular.");

        var valores = new List<double>();
        string infixa = GerarInfixaComLetras(texto, valores);
        string posfixa = ConverterInfixaParaPosfixa(infixa);
        double valorFinal = CalcularPosfixa(posfixa, valores);

        var resultado = new ResultadoExpressao();
        resultado.InfixaComLetras = infixa;
        resultado.Posfixa = posfixa;
        resultado.ValorFinal = valorFinal;
        resultado.ValoresFormatados = MontarTextoValores(valores);

        return resultado;
    }

    private static string GerarInfixaComLetras(string texto, List<double> valores)
    {
        var infixa = new StringBuilder();
        int i = 0;

        while (i < texto.Length)
        {
            char atual = texto[i];

            if (char.IsWhiteSpace(atual))
            {
                i++;
                continue;
            }

            if (EhInicioDeNumero(texto, i))
            {
                string numeroTexto = LerNumero(texto, ref i);

                if (valores.Count >= 26)
                    throw new InvalidOperationException("Esta calculadora aceita ate 26 valores por expressao.");

                if (!double.TryParse(numeroTexto.Replace(',', '.'), NumberStyles.Number,
                    CultureInfo.InvariantCulture, out double numero))
                {
                    throw new InvalidOperationException($"Numero invalido: {numeroTexto}");
                }

                valores.Add(numero);
                infixa.Append((char)('A' + valores.Count - 1));
                continue;
            }

            if (atual == '(' || atual == ')' || EhOperador(atual))
            {
                infixa.Append(atual);
                i++;
                continue;
            }

            throw new InvalidOperationException("Caractere invalido encontrado: " + atual);
        }

        return infixa.ToString();
    }

    private static bool EhInicioDeNumero(string texto, int indice)
    {
        char atual = texto[indice];

        if (char.IsDigit(atual) || atual == ',' || atual == '.')
            return true;

        if ((atual == '+' || atual == '-') && indice + 1 < texto.Length &&
            (char.IsDigit(texto[indice + 1]) || texto[indice + 1] == ',' || texto[indice + 1] == '.'))
        {
            if (indice == 0)
                return true;

            char anterior = ProcurarAnteriorUtil(texto, indice);
            return anterior == '\0' || anterior == '(' || EhOperador(anterior);
        }

        return false;
    }

    private static char ProcurarAnteriorUtil(string texto, int indice)
    {
        for (int i = indice - 1; i >= 0; i--)
        {
            if (!char.IsWhiteSpace(texto[i]))
                return texto[i];
        }

        return '\0';
    }

    private static string LerNumero(string texto, ref int indice)
    {
        var numero = new StringBuilder();
        bool temSeparador = false;

        if (texto[indice] == '+' || texto[indice] == '-')
        {
            numero.Append(texto[indice]);
            indice++;
        }

        while (indice < texto.Length)
        {
            char atual = texto[indice];

            if (char.IsDigit(atual))
            {
                numero.Append(atual);
                indice++;
            }
            else if (atual == ',' || atual == '.')
            {
                if (temSeparador)
                    throw new InvalidOperationException("Numero com mais de um separador decimal.");

                temSeparador = true;
                numero.Append(atual);
                indice++;
            }
            else
            {
                break;
            }
        }

        string textoNumero = numero.ToString();
        if (textoNumero == "+" || textoNumero == "-" ||
            textoNumero == "," || textoNumero == "." ||
            textoNumero == "+," || textoNumero == "-," ||
            textoNumero == "+." || textoNumero == "-.")
        {
            throw new InvalidOperationException("Numero invalido: " + textoNumero);
        }

        return textoNumero;
    }

    private static string ConverterInfixaParaPosfixa(string infixa)
    {
        var posfixa = new StringBuilder();
        var pilha = new Stack<char>();

        foreach (char simbolo in infixa)
        {
            if (char.IsLetter(simbolo))
            {
                posfixa.Append(simbolo);
            }
            else if (simbolo == '(')
            {
                pilha.Push(simbolo);
            }
            else if (simbolo == ')')
            {
                bool achouAbertura = false;

                while (pilha.Count > 0)
                {
                    char operador = pilha.Pop();
                    if (operador == '(')
                    {
                        achouAbertura = true;
                        break;
                    }

                    posfixa.Append(operador);
                }

                if (!achouAbertura)
                    throw new InvalidOperationException("Parenteses fechando sem abertura.");
            }
            else if (EhOperador(simbolo))
            {
                while (pilha.Count > 0 && pilha.Peek() != '(' &&
                       TemPrecedencia(pilha.Peek(), simbolo))
                {
                    posfixa.Append(pilha.Pop());
                }

                pilha.Push(simbolo);
            }
        }

        while (pilha.Count > 0)
        {
            char operador = pilha.Pop();

            if (operador == '(')
                throw new InvalidOperationException("Parenteses abrindo sem fechamento.");

            posfixa.Append(operador);
        }

        return posfixa.ToString();
    }

    private static int Prioridade(char operador)
    {
        switch (operador)
        {
            case '^':
                return 3;
            case '*':
            case '/':
                return 2;
            case '+':
            case '-':
                return 1;
            default:
                return 0;
        }
    }

    private static bool EhOperador(char simbolo)
    {
        for (int i = 0; i < Operadores.Length; i++)
        {
            if (simbolo == Operadores[i])
                return true;
        }

        return false;
    }

    private static bool TemPrecedencia(char operador1, char operador2)
    {
        if (operador1 == '(')
            return false;

        if (Prioridade(operador1) > Prioridade(operador2))
            return true;

        if (Prioridade(operador1) == Prioridade(operador2) && operador2 != '^')
            return true;

        return false;
    }

    private static double CalcularPosfixa(string posfixa, List<double> valores)
    {
        var pilha = new Stack<double>();

        foreach (char simbolo in posfixa)
        {
            if (char.IsLetter(simbolo))
            {
                pilha.Push(valores[simbolo - 'A']);
                continue;
            }

            if (pilha.Count < 2)
                throw new InvalidOperationException("Expressao incompleta.");

            double operando2 = pilha.Pop();
            double operando1 = pilha.Pop();
            pilha.Push(ValorDaSubExpressao(operando1, simbolo, operando2));
        }

        if (pilha.Count != 1)
            throw new InvalidOperationException("Expressao incompleta.");

        return pilha.Pop();
    }

    private static double ValorDaSubExpressao(double operando1, char operador, double operando2)
    {
        switch (operador)
        {
            case '+':
                return operando1 + operando2;
            case '-':
                return operando1 - operando2;
            case '*':
                return operando1 * operando2;
            case '/':
                if (operando2 == 0)
                    throw new DivideByZeroException("Nao e permitido dividir por zero.");

                return operando1 / operando2;
            case '^':
                return Math.Pow(operando1, operando2);
            default:
                throw new InvalidOperationException("Operador invalido: " + operador);
        }
    }

    private static string MontarTextoValores(List<double> valores)
    {
        var partes = new List<string>();

        for (int i = 0; i < valores.Count; i++)
        {
            char letra = (char)('A' + i);
            partes.Add(letra + "=" + valores[i].ToString("0.##########"));
        }

        return string.Join(", ", partes);
    }
}
