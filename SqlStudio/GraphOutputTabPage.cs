using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SqlStudio
{
    public class GraphOutputTabPage : TabPage
    {
        private Chart _chart = new Chart();
        ToolTip toolTip = new ToolTip();
        Point? prevPosition = null;
        public GraphOutputTabPage()
        {
            _chart.Dock = DockStyle.Fill;
            Controls.Add(_chart);

            _chart.MouseMove += chart_MouseMove;
        }

        private void chart_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.Location;
            if (prevPosition.HasValue && pos == prevPosition.Value)
                return;
            toolTip.RemoveAll();
            prevPosition = pos;
            var results = _chart.HitTest(pos.X, pos.Y, false, ChartElementType.DataPoint); // set ChartElementType.PlottingArea for full area, not only DataPoints
            foreach (var result in results)
            {
                if (result.ChartElementType == ChartElementType.DataPoint) // set ChartElementType.PlottingArea for full area, not only DataPoints
                {
                    //var yVal = result.ChartArea.AxisY.PixelPositionToValue(pos.Y);
                    //toolTip.Show(yVal.ToString("0.##"), _chart, pos.X, pos.Y - 15);

                    var point = result.Object as DataPoint;
                    if (point == null)
                        return;

                    string yVal = "";
                    for (int i = 0; i < point.YValues.Length; i++)
                    {
                        if (i > 0)
                        {
                            yVal += ", ";
                        }
                        yVal = point.YValues[i].ToString("0.##");
                    }

                    string xVal = DateTime.FromOADate(point.XValue).ToString("dd/MM/yy");

                    toolTip.Show($"{yVal} - {xVal}", _chart, pos.X, pos.Y - 15);
                }
            }
        }

        public void SetData(GraphData data)
        {
            _chart.ChartAreas.Clear();
            var chartArea = new ChartArea();
            
            chartArea.AxisX.Name = data.XLabel;
            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.CursorX.AutoScroll = true;
            chartArea.CursorX.IsUserSelectionEnabled = true;

            _chart.ChartAreas.Add(chartArea);
            var serie = new Series("series1");
            serie.ChartType = SeriesChartType.Line;

            _chart.Series.Clear();
            _chart.Series.Add(serie);

            foreach (var p in data.Data)
            {
                serie.Points.AddXY(p.XData, p.YData.ToArray());
            }

            if (data.YMin != double.MaxValue)
            {
                chartArea.AxisY.Minimum = data.YMin - 1;
            }

            if (data.YMax != double.MinValue)
            { 
                chartArea.AxisY.Maximum = data.YMax + 1; 
            }
        }
    }
}
