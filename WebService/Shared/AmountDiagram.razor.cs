using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using WebService.Extensions;

namespace WebService.Shared;

public partial class AmountDiagram : ComponentBase
{
    public enum ManufacturedState { Successful, Faulty }
    
    private const string DATASET_LABEL = "Mennyiség";
    
    private LineChartOptions _lineChartOptions = default!;
    private ChartData _chartData = default!;
    private LineChartDataset _dataset = default!;

    private async Task InitAmountChart()
    {
        
        _lineChartOptions = LineChartExtensions
            .GetDefaultLineChartOptions("Legyártott mennyiség", "Idő", "Darabszám");
        _dataset = LineChartExtensions.GetDefaultDatasetSettings(DATASET_LABEL);

        _chartData = new ChartData()
        {
            Labels = new(),
            Datasets = new() { _dataset }
        };

        await _amountChart.InitializeAsync(_chartData, _lineChartOptions);
    }
    
    public async ValueTask DisposeAsync()
    {
        AmountService.Unsubscribe(this);
        await _amountChart.DisposeAsync();
    }
}