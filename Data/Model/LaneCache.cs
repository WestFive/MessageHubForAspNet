using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Model
{
   public class LaneCache
    {
        public string lane_code { get; set; }
        public object lane { get; set; }
    }

    public class ServerCache
    {
        public string server_code { get; set; }
        public object server { get; set; }
    }
}
