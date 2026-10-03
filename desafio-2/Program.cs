using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

class Program
{
    static void Main()
    {
        string jsonEstoque = @"{ 
            ""estoque"": [ 
                { ""codigoProduto"": 101, ""descricaoProduto"": ""Caneta Azul"", ""estoque"": 150 }, 
                { ""codigoProduto"": 102, ""descricaoProduto"": ""Caderno Universitário"", ""estoque"": 75 }, 
                { ""codigoProduto"": 103, ""descricaoProduto"": ""Borracha Branca"", ""estoque"": 200 }, 
                { ""codigoProduto"": 104, ""descricaoProduto"": ""Lápis Preto HB"", ""estoque"": 320 }, 
                { ""codigoProduto"": 105, ""descricaoProduto"": ""Marcador de Texto Amarelo"", ""estoque"": 90 } 
            ] 
        }";

        var dados = JsonSerializer.Deserialize<RespostaEstoque>(jsonEstoque);
        if (dados?.Produtos == null) return;

        List<Produto> listaEstoque = dados.Produtos;
        List<Movimentacao> historico = new();
        int contadorIdMovimentacao = 1;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== SISTEMA DE CONTROLE DE ESTOQUE ===");
            ExibirEstoqueAtual(listaEstoque);

            Console.WriteLine("\nOpções:");
            Console.WriteLine("1. Lançar Movimentação (Entrada/Saída)");
            Console.WriteLine("2. Sair");
            Console.Write("Escolha uma opção: ");

            string? opcao = Console.ReadLine();
            if (opcao == "2") break;
            if (opcao != "1") continue;

            // 1. selecao de produto
            Console.Write("\nDigite o Código do Produto: ");
            if (!int.TryParse(Console.ReadLine(), out int codigoProduto))
            {
                ExibirMensagemErro("Código inválido!");
                continue;
            }

            Produto? produto = listaEstoque.FirstOrDefault(p => p.CodigoProduto == codigoProduto);
            if (produto == null)
            {
                ExibirMensagemErro("Produto não encontrado!");
                continue;
            }

            // 2. Entrada ou saida
            Console.Write("Tipo de Movimentação [E = Entrada / S = Saída]: ");
            string? tipoInput = Console.ReadLine()?.Trim().ToUpper();

            if (tipoInput != "E" && tipoInput != "S")
            {
                ExibirMensagemErro("Tipo de movimentação inválido! Use 'E' ou 'S'.");
                continue;
            }

            TipoMovimentacao tipo = tipoInput == "E" ? TipoMovimentacao.Entrada : TipoMovimentacao.Saida;

            // 3. Descricao da Movimentacao
            Console.Write("Descrição/Motivo da movimentação: ");
            string descricao = Console.ReadLine() ?? "Sem descrição";

            // 4. Quantidade
            Console.Write("Quantidade: ");
            if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
            {
                ExibirMensagemErro("A quantidade deve ser um número inteiro maior que zero!");
                continue;
            }

            // Validacao de estoque para saídas
            if (tipo == TipoMovimentacao.Saida && produto.QuantidadeEstoque < quantidade)
            {
                ExibirMensagemErro($"Estoque insuficiente! Saldo atual: {produto.QuantidadeEstoque} unidades.");
                continue;
            }

            // 5. Processamento da Movimentacao
            if (tipo == TipoMovimentacao.Entrada)
                produto.QuantidadeEstoque += quantidade;
            else
                produto.QuantidadeEstoque -= quantidade;

            // Criacao do registro de movimentacao com ID unico
            Movimentacao mov = new()
            {
                Id = contadorIdMovimentacao++,
                CodigoProduto = produto.CodigoProduto,
                DescricaoProduto = produto.DescricaoProduto,
                Tipo = tipo,
                DescricaoMovimentacao = descricao,
                Quantidade = quantidade,
                EstoqueResultante = produto.QuantidadeEstoque,
                DataHora = DateTime.Now
            };

            historico.Add(mov);

            // 6. Retorno ao Usuário com Dados Atualizados
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n===========================================");
            Console.WriteLine(" MOVIMENTAÇÃO REALIZADA COM SUCESSO!");
            Console.WriteLine("===========================================");
            Console.WriteLine($"ID Único da Movimentação : {mov.Id}");
            Console.WriteLine($"Produto                 : {mov.DescricaoProduto} (Cód: {mov.CodigoProduto})");
            Console.WriteLine($"Tipo / Motivo           : {mov.Tipo} - {mov.DescricaoMovimentacao}");
            Console.WriteLine($"Quantidade Movimentada  : {mov.Quantidade}");
            Console.WriteLine($"ESTOQUE FINAL ATUALIZADO: {mov.EstoqueResultante} unidades");
            Console.WriteLine("===========================================");
            Console.ResetColor();

            Console.WriteLine("\nPressione ENTER para continuar...");
            Console.ReadLine();
        }
    }

    static void ExibirEstoqueAtual(List<Produto> produtos)
    {
        Console.WriteLine("\n--- ESTOQUE ATUAL ---");
        Console.WriteLine($"{"Cód",-6} | {"Produto",-30} | {"Qtd Estoque",-12}");
        Console.WriteLine(new string('-', 54));
        foreach (var p in produtos)
        {
            Console.WriteLine($"{p.CodigoProduto,-6} | {p.DescricaoProduto,-30} | {p.QuantidadeEstoque,-12}");
        }
    }

    static void ExibirMensagemErro(string mensagem)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERRO] {mensagem}");
        Console.ResetColor();
        Console.WriteLine("Pressione ENTER para tentar novamente...");
        Console.ReadLine();
    }
}

// Enum para categorizar o tipo de movimentacao
public enum TipoMovimentacao
{
    Entrada,
    Saida
}

// Representa uma movimentacao registrada no sistema
public class Movimentacao
{
    public int Id { get; set; }
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = string.Empty;
    public TipoMovimentacao Tipo { get; set; }
    public string DescricaoMovimentacao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public int EstoqueResultante { get; set; }
    public DateTime DataHora { get; set; }
}

// Mapeamento dos objetos JSON
public class RespostaEstoque
{
    [JsonPropertyName("estoque")]
    public List<Produto> Produtos { get; set; } = new();
}

public class Produto
{
    [JsonPropertyName("codigoProduto")]
    public int CodigoProduto { get; set; }

    [JsonPropertyName("descricaoProduto")]
    public string DescricaoProduto { get; set; } = string.Empty;

    [JsonPropertyName("estoque")]
    public int QuantidadeEstoque { get; set; }
}