using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace MCH.Models
{
    public class ParseRules
    {
        public  List<Regex> ListProductsUrl { get; set; }
        public  List<Regex> ProductUrl { get; set; }
        public  int MaxRecursion { get; set; }
        public  BlockRule ProductName { get; set; }
        public  BlockRule ProductPrice { get; set; }
        public  BlockRule ProductCategory { get; set; }
        public  BlockRule ProductDescription { get; set; }
        public  BlockRule ProductImage { get; set; }
        
        public  string UrlBase { get; set; }

        public ParseRules()
        {
            ListProductsUrl = new List<Regex>();
            ProductUrl = new List<Regex>();
        }
    }
}