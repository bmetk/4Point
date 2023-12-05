using System.Collections;
using BlazorBootstrap;

namespace WebService.Extensions;

public static class IChartDatasetExtensions
{
    public static IChartDataset GetMeasurementDatasetForList(
        this List<double> list, 
        string label,
        List<string> color)
    {
        return new LineChartDataset
        {
            Label = label,
            Data = list,
            BackgroundColor = color,
            BorderColor = color,
            BorderWidth = new() { 2.0 },
            HoverBorderWidth = new() { 4.0 },
            PointBorderColor = color,
            PointRadius = new List<int> { 0 },
            PointHoverRadius = new List<int> { 4 }
        };
    }

    public static IChartDataset GetMeasurementDatasetForList(
        this IEnumerable<double> list,
        string label,
        List<string> color) => 
        GetMeasurementDatasetForList(list.ToList(), label, color);
}