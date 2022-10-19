using System;
using System.Collections.Generic;

namespace SqlStudio
{
    public class GraphData
    {
        public string Name { get; set; }
        public string XLabel { get; set; }
        public Type XType { get; set; }
        public List<Type> YTypes { get; set; } = new List<Type>();
        public List<string> YLabels { get; set; } = new List<string>();
        public List<GraphDataPoint> Data { get; } = new List<GraphDataPoint>();
        public double YMin { get; set; }
        public double YMax { get; set; }
        public GraphData(string name)
        {
            Name = name;
        }
    }
}
