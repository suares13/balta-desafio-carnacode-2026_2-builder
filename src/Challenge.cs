using System;
using System.Collections.Generic;

namespace DesignPatternChallenge
{
    // 1. O Produto (A representação final)
    public class SalesReport
    {
        // Propriedades agora têm setters privados para garantir que 
        // apenas o Builder possa modificá-las durante a criação.
        public string Title { get; internal set; }
        public string Format { get; internal set; }
        public DateTime StartDate { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public bool IncludeHeader { get; internal set; }
        public string HeaderText { get; internal set; }
        public bool IncludeFooter { get; internal set; }
        public string FooterText { get; internal set; }
        public bool IncludeCharts { get; internal set; }
        public string ChartType { get; internal set; }
        public List<string> Columns { get; internal set; } = new List<string>();
        public List<string> Filters { get; internal set; } = new List<string>();

        public void Generate()
        {
            Console.WriteLine($"\n=== Relatório Gerado: {Title} ({Format}) ===");
            Console.WriteLine($"Período: {StartDate:dd/MM/yyyy} a {EndDate:dd/MM/yyyy}");
            if (IncludeHeader) Console.WriteLine($"[Cabeçalho] {HeaderText}");
            if (IncludeCharts) Console.WriteLine($"[Gráfico] Tipo: {ChartType}");
            Console.WriteLine($"[Colunas] {string.Join(", ", Columns)}");
            if (IncludeFooter) Console.WriteLine($"[Rodapé] {FooterText}");
        }
    }

    // 2. A Interface do Builder
    public interface ISalesReportBuilder
    {
        ISalesReportBuilder SetPeriod(DateTime start, DateTime end);
        ISalesReportBuilder WithHeader(string text);
        ISalesReportBuilder WithFooter(string text);
        ISalesReportBuilder AddColumn(string columnName);
        ISalesReportBuilder AddFilter(string filter);
        ISalesReportBuilder WithCharts(string type);
        SalesReport Build(); // O método final que entrega o objeto pronto
    }

    // 3. O Builder Concreto
    public class SalesReportBuilder : ISalesReportBuilder
    {
        private SalesReport _report;

        public SalesReportBuilder(string title, string format)
        {
            // O construtor do Builder já define o que é OBRIGATÓRIO
            _report = new SalesReport { Title = title, Format = format };
        }

        public ISalesReportBuilder SetPeriod(DateTime start, DateTime end)
        {
            _report.StartDate = start;
            _report.EndDate = end;
            return this; // Retornamos 'this' para permitir o encadeamento (Fluent API)
        }

        public ISalesReportBuilder WithHeader(string text)
        {
            _report.IncludeHeader = true;
            _report.HeaderText = text;
            return this;
        }

        public ISalesReportBuilder WithFooter(string text)
        {
            _report.IncludeFooter = true;
            _report.FooterText = text;
            return this;
        }

        public ISalesReportBuilder AddColumn(string columnName)
        {
            _report.Columns.Add(columnName);
            return this;
        }

        public ISalesReportBuilder AddFilter(string filter)
        {
            _report.Filters.Add(filter);
            return this;
        }

        public ISalesReportBuilder WithCharts(string type)
        {
            _report.IncludeCharts = true;
            _report.ChartType = type;
            return this;
        }

        public SalesReport Build()
        {
            // Aqui poderíamos validar se o objeto está consistente antes de entregar
            if (_report.StartDate == default) throw new Exception("Período não definido!");
            
            var finishedReport = _report;
            _report = null; // Opcional: limpa para não reusar o mesmo builder acidentalmente
            return finishedReport;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Relatórios (Padrão Builder) ===");

            // Exemplo de criação fluente e legível
            var builder = new SalesReportBuilder("Vendas Mensais", "PDF");
            
            SalesReport report = builder
                .SetPeriod(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31))
                .WithHeader("Relatório de Vendas Corporativas")
                .AddColumn("Produto")
                .AddColumn("Quantidade")
                .AddColumn("Valor")
                .AddFilter("Status=Ativo")
                .WithCharts("Bar")
                .WithFooter("Confidencial - Uso Interno")
                .Build();

            report.Generate();

            // Criando um relatório simples com o mesmo Builder
            var excelReport = new SalesReportBuilder("Exportação Simples", "Excel")
                .SetPeriod(DateTime.Now.AddDays(-7), DateTime.Now)
                .AddColumn("Nome")
                .AddColumn("Email")
                .Build();

            excelReport.Generate();
        }
    }
}