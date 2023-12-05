using BlazorBootstrap;
using Common.Classes;
using Microsoft.AspNetCore.Components;
using WebService.Extensions;
using WebService.Models;

namespace WebService.Shared;

public partial class TaktTimeChart : ComponentBase, Common.Interfaces.IObserver<ProcessedData<int>>
{
    private const string DATASET_LABEL = "Gyártás idő";
    
    private BarChartOptions _lineChartOptions = default!;
    private ChartData _chartData = default!;
    private BarChartDataset _dataset = default!;

    private async Task InitChart()
    {
        _lineChartOptions = BarChartExtensions
            .GetDefaultBarChartOptions("Takt time", "Intervallum", "Idő (s)");
        _dataset = BarChartExtensions.GetDefaultDatasetSettings(DATASET_LABEL);

        _chartData = new()
        {
            Labels = new(),
            Datasets = new() { _dataset }
        };

        await _tactChart.InitializeAsync(_chartData, _lineChartOptions);
    }

    private void AddNewDataPoint()
    {
        DateTimeOffset endTime = DateTimeOffset.Now;
        TimeSpan diff = endTime - _startTime!.Value;
        
        Logger.LogInformation("Takt time end is {endTime}", endTime);
        Logger.LogInformation("Production was {timespan} seconds long, that is {secs} s", diff, diff.TotalSeconds);
        
        string startLabel = _startTime!.Value.ToString("T");
        string endLabel = endTime.ToString("T");
        string label = $"{startLabel} - {endLabel}";
        
        _tactChart.AddDataAsync(_chartData, label, 
            new BarChartDatasetData(DATASET_LABEL, diff.TotalSeconds));
        _startTime = DateTimeOffset.Now;
    }

    public async ValueTask DisposeAsync()
    {
        await _tactChart.DisposeAsync();
        await MqttService.UnsubscribeAsync(MqttTopics.WorkInProgressTopic(Configuration), WorkInProgressDataConsumer);
    }
}