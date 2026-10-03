using System;
using System.Globalization;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== CÁLCULO DE JUROS POR ATRASO ===");

        //valor original
        Console.Write("Digite o valor original do boleto/título (R$): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal valorOriginal) || valorOriginal <= 0)
        {
            Console.WriteLine("Valor inválido!");
            return;
        }

        //data de vencimento
        Console.Write("Digite a data de vencimento (dd/MM/yyyy): ");
        string? inputData = Console.ReadLine();

        if (!DateTime.TryParseExact(inputData, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataVencimento))
        {
            Console.WriteLine("Data em formato inválido! Utilize o formato dd/MM/yyyy.");
            return;
        }

        DateTime dataHoje = DateTime.Today;

        // calculo dos dias de atraso
        int diasAtraso = (dataHoje - dataVencimento.Date).Days;

        var valorBR = new CultureInfo("pt-BR");

        Console.WriteLine("\n--- RESUMO DO CÁLCULO ---");
        Console.WriteLine($"Data de Vencimento : {dataVencimento:dd/MM/yyyy}");
        Console.WriteLine($"Data Atual        : {dataHoje:dd/MM/yyyy}");
        Console.WriteLine($"Valor Original    : {valorOriginal.ToString("C", valorBR)}");

        if (diasAtraso <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nO título está em dia. Não há cobrança de juros/multa!");
            Console.WriteLine($"Valor Final a Pagar: {valorOriginal.ToString("C", valorBR)}");
            Console.ResetColor();
        }
        else
        {
            // Taxa de 2,5% ao dia = 0.025
            const decimal taxaDiaria = 0.025m;

            // Juros em R$ = Valor * Taxa * Dias
            decimal valorJuros = valorOriginal * taxaDiaria * diasAtraso;
            decimal valorTotal = valorOriginal + valorJuros;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Dias em Atraso    : {diasAtraso} dia(s)");
            Console.WriteLine($"Taxa Diária       : 2,5% ao dia");
            Console.WriteLine($"Valor dos Juros   : {valorJuros.ToString("C", valorBR)}");
            Console.WriteLine($"-----------------------------------");
            Console.WriteLine($"VALOR TOTAL A PAGAR: {valorTotal.ToString("C", valorBR)}");
            Console.ResetColor();
        }
    }
}