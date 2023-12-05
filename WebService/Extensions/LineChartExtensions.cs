using BlazorBootstrap;

namespace WebService.Extensions;

public static class LineChartExtensions
{
    public static LineChartOptions GetDefaultLineChartOptions(string title, string xText, string yText)
    {
        return new LineChartOptions
        {
            Responsive = true,
            Interaction = new Interaction { Mode = InteractionMode.Index },
            Plugins = new LineChartPlugins
            {
                Title = new ChartPluginsTitle { Text = title , Display = true }
            },
            Scales = new Scales
            {
                X = new ChartAxes { Title = new ChartAxesTitle { Text = xText, Display = true }},
                Y = new ChartAxes { Title = new ChartAxesTitle() { Text = yText, Display = true } }
            }
        };
    }

    public static LineChartDataset GetDefaultDatasetSettings(string label)
    {
        return new LineChartDataset()
        {
            Label = label,
            Data = new(),
            BackgroundColor = new List<string> { "rgb(88, 80, 141)" },
            BorderColor = new List<string> { "rgb(88, 80, 141)" },
            BorderWidth = new List<double> { 2 },
            HoverBorderWidth = new List<double> { 4 },
            PointBackgroundColor = new List<string> { "rgb(88, 80, 141)" },
            PointBorderColor = new List<string> { "rgb(88, 80, 141)" },
            PointRadius = new List<int> { 0 }, // hide points
            PointHoverRadius = new List<int> { 4 }
        };
    }

    internal static LineChartDataset WithOptions(this LineChartDataset dataset, Action<LineChartDataset> options)
    {
        options(dataset);
        return dataset;
    }
}