using Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;

namespace Api.Controllers
{
    public class RSSController : ApiController
    {

        // GET: api/RSS
        public IEnumerable<Rss> Get() //Returns all rss flows 
        {
            //To do: Parentobj i db
            IEnumerable<Rss> rsses1 = getRssObjects("http://www.nt.se/nyheter/norrkoping/rss/", "Norrköpings Tidningar");
            IEnumerable<Rss> rsses2 = getRssObjects("http://www.expressen.se/Pages/OutboundFeedsPage.aspx?id=3642159&viewstyle=rss", "Expressen");
            IEnumerable<Rss> rsses3 = getRssObjects("https://www.svd.se/?service=rss", "Svenska Dagbladet");

            IEnumerable<Rss> rsses = rsses1.Concat(rsses2).Concat(rsses3).ToList();
            
            return rsses;

        }

        // GET: api/RSS/5
        public RssSourse Get(int id) //Returns object with rss flow for specific id
        {
            //To do: Gå på parentobj id
            List<Rss> rsses = new List<Rss>();
            RssSourse rssSourse = new RssSourse();
            string url = "";
            string sourceName = "";

            if (id == 1) //NT
            {
                url = "http://www.nt.se/nyheter/norrkoping/rss/";
                sourceName = "Norrköpings Tidningar";
            }
            else if (id == 2)// Expressen
            {
                url = "http://www.expressen.se/Pages/OutboundFeedsPage.aspx?id=3642159&viewstyle=rss";
                sourceName = "Expressen";
            }
            else if (id == 3)//SVD
            {
                url = "https://www.svd.se/?service=rss";
                sourceName = "Svenska Dagbladet";
            }

            rssSourse = GetRssSource(url);
            rssSourse.rsses = new List<Rss>();
            rssSourse.rsses = getRssObjects(url, sourceName).ToList();

            return rssSourse;
        }

        // POST: api/RSS
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/RSS/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/RSS/5
        public void Delete(int id)
        {
        }

        /// <summary>
        /// Get parent source object (parent) for rss flow.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private RssSourse GetRssSource(string url) //To do: possible refact to use link instead
        {
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

            RssSourse rssSourse = new RssSourse();
            rssSourse.Title = feed.Title.Text;
            rssSourse.Description = feed.Description.Text;
            rssSourse.Copyright = feed.Copyright.Text;
            rssSourse.Link = feed.Links[0].Uri.ToString();

            return rssSourse;
        }

        /// <summary>
        /// Returns list of Rss-objects from parameters url  
        /// </summary>
        /// <param name="url"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private IEnumerable<Rss> getRssObjects(string url, string source)
        {
            string RSSData = "";
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                RSSData = webClient.DownloadString(url);
            }

            XDocument xml = XDocument.Parse(RSSData);

            var RSSFeedData = (from x in xml.Descendants("item")
                               select new Rss
                               {
                                   Title = ((string)x.Element("title")),
                                   Link = ((string)x.Element("link")),
                                   Description = ((string)x.Element("description")),
                                   PubDate = ((string)x.Element("pubDate")),
                                   Category = ((string)x.Element("category")),
                                   Source = source,
                               }).ToList();

            foreach (var rss in RSSFeedData)
            {
                rss.Image = TakeOutImg(rss.Description);
                if (!string.IsNullOrWhiteSpace(rss.Image))
                {
                    rss.Description = RemoveImgTag(rss.Description);
                }
            }
            return RSSFeedData;
        }

        /// <summary>
        /// Returns img tag as new string if found else returns string.empty
        /// </summary>
        /// <param name="tagWithImg"></param>
        /// <returns></returns>
        private string TakeOutImg(string tagWithImg)
        {
            Match match = Regex.Match(tagWithImg, "<img[^>]+>");
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Removes img tag from string parameter and return the string. 
        /// </summary>
        /// <param name="des"></param>
        /// <returns></returns>
        private string RemoveImgTag(string des)
        {
            des = Regex.Replace(des, @"img[^>]*?>", string.Empty);
            string _des = des.Substring(1);
            return _des;
        }


    }
}


