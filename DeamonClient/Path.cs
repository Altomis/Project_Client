using System;
using System.Collections.Generic;
using System.Text;

namespace DeamonClient
{
    public class Path
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public int IdFTP { get; set; }
        public string Destination { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
