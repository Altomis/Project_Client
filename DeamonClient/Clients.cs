using System;
using System.Collections.Generic;
using System.Text;

namespace DeamonClient
{
    public class Clients
    {
        public int Id { get; set; }
        public string Pc_Name { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
        public string Created { get; set; }
        public bool Active { get; set; }
    }
}
