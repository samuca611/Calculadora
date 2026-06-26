namespace _25736_24122_Calculadora;

public class PilhaVetor<T> : IStack<T>
{
    const int MAXIMO = 500;
    private int tamanhoFisico;
    private T[] p;
    private int topo;

    public PilhaVetor() : this(MAXIMO)
    {
    }

    public PilhaVetor(int quantasPosicoes)
    {
        tamanhoFisico = quantasPosicoes;
        p = new T[quantasPosicoes];
        topo = -1;
    }

    public int Tamanho
    {
        get { return topo + 1; }
    }

    public bool EstaVazia
    {
        get { return topo < 0; }
    }

    public List<T> Conteudo()
    {
        var resultado = new List<T>();

        for (int indice = topo; indice >= 0; indice--)
            resultado.Add(p[indice]);

        return resultado;
    }

    public T Desempilhar()
    {
        if (EstaVazia)
            throw new Exception("Pilha esvaziou");

        T dadoDoTopo = p[topo];
        p[topo] = default(T);
        topo--;
        return dadoDoTopo;
    }

    public void Empilhar(T item)
    {
        if (Tamanho == tamanhoFisico)
            throw new Exception("Pilha encheu");

        topo++;
        p[topo] = item;
    }

    public T OTopo()
    {
        if (EstaVazia)
            throw new Exception("Pilha esvaziou");

        return p[topo];
    }
}
