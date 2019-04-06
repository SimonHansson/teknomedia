using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class RssSourse
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Copyright { get; set; }
        public List<Rss> rsses { get; set; }
    }
}