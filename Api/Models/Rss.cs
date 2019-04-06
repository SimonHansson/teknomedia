using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class Rss
    {
        //public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string PubDate { get; set; }
        public string Category { get; set; }
        public string Link { get; set; }
        public string Source { get; set; }
    }
}