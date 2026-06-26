namespace _25736_24122_Calculadora;

interface IStack<T>
{
    void Empilhar(T item);
    T Desempilhar();
    T OTopo();
    int Tamanho { get; }
    bool EstaVazia { get; }
    List<T> Conteudo();
}
