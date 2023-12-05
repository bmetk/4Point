using System.ComponentModel;
using BlazorBootstrap;

namespace WebService.Extensions;

public static class BarChartExtensions
{
    public static BarChartOptions GetDefaultBarChartOptions(string title, string xText, string yText)
    {
        return new()
        {
            Responsive = true,
            Interaction = new() { Mode = InteractionMode.Index },
            IndexAxis = "x",
            Scales = new()
            {
                X = new() { Title = new() { Text = xText, Display = true }},
                Y = new() { Title = new() { Text = yText, Display = true }}
            },
            Plugins = new()
            {
                Title = new() { Text = title, Display = true }
            }
        };
    }

    public static BarChartDataset GetDefaultDatasetSettings(string label)
    {
        return new BarChartDataset()
        {
            Label = label,
            Data = new(),
            BackgroundColor = new() { ColorBuilder.CategoricalSixColors[0].ToColor().ToRgbString() },
            BorderColor = new() { ColorBuilder.CategoricalSixColors[0].ToColor().ToRgbString() },
            BorderWidth = new() { 0 }
        };
    }
}