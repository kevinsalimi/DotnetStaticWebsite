using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace K1_Static_Website.Models
{
    public class PostModel
    {

        public string Title { get; set; }
        public string Summary { get; set; }
        public string Keywords { get; set; }
        public string CreationDate { get; set; }
        public string Author { get; set; }
        public string Link { get; set; }
        public string Address { get; set; }
        public string ArticleId { get; set; }
        public int DisplayPeriority { get; set; }
    }
}
