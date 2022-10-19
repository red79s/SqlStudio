using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlStudio
{
    public class GraphDataPoint
    {
        public object XData { get; set; }
        public List<object> YData { get; set; } = new List<object>();
    }
}
