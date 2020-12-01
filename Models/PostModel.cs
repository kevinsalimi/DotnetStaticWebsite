using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace K1_Static_Website.Models
{
    public class PostModel
    {
        public PostModel()
        {
            this.Keywords = new List<string>(4);
        }
        public string Title { get; set; }
        public string Summary { get; set; }
        public List<string> Keywords { get; set; }
        public string CreationDate { get; set; }
        public string Author { get; set; }
        public string Link { get; set; }
        public string Address { get; set; }

    }
}
