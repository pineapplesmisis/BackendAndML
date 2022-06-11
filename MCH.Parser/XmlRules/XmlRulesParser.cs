using System;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using MCH.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace MCH.XmlRules
{
    public class XmlRulesParser
    {
        private readonly string _pathToFolder;
        public XmlRulesParser(string pathToFolder)
        {
            _pathToFolder = pathToFolder;
        }
        public ParseRules getRules(int companyId)
        {
            var doc = getDoc(companyId);
            if (doc is null)
            {
                return null;
            }

            return parseDoc(doc);
        }

        private ParseRules parseDoc(XmlDocument doc)
        {
            var rules = new ParseRules();
            try
            {
                var rulesElem = doc.DocumentElement;
                foreach (XmlElement node in rulesElem)
                {
                    switch (node?.Name)
                    {
                        case "ListProductsUrl":
                            foreach (XmlElement url in node.ChildNodes)
                            {
                                rules.ListProductsUrl.Add(new Regex($@"{url.FirstChild.Value}"));
                            }

                            break;
                        case "ProductUrl":
                            foreach (XmlElement url in node.ChildNodes)
                            {
                                rules.ProductUrl.Add(new Regex(@$"{url.FirstChild.Value}"));
                            }

                            break;
                        case "UrlBase":
                            rules.UrlBase = node.FirstChild.Value;
                            break;
                        case "ProductName":
                            rules.ProductName =new()
                            {
                                expression =  XPathExpression.Compile(node.FirstChild.Value),
                                TakenAttrubute = node.Attributes?.GetNamedItem("takenAttrubute")?.Value
                            };
                            break;
                        case "ProductPrice":
                            rules.ProductPrice = new()
                            {
                                expression = XPathExpression.Compile(node.FirstChild.Value),
                                TakenAttrubute = node.Attributes?.GetNamedItem("takenAttrubute")?.Value
                            };
                            break;
                        case "ProductDescription":
                            rules.ProductDescription = new()
                            {
                                expression = XPathExpression.Compile(node.FirstChild.Value),
                                TakenAttrubute = node.Attributes?.GetNamedItem("takenAttrubute")?.Value
                            };
                            break;
                        case "ProductImage":
                            rules.ProductImage = new()
                            {
                                expression = XPathExpression.Compile(node.FirstChild.Value),
                                TakenAttrubute = node.Attributes?.GetNamedItem("takenAttrubute")?.Value
                                
                            };
                            break;
                            
                    }
                }

                return rules;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private XmlDocument getDoc(int companyId)
        {
            if (!File.Exists(Path.Combine(_pathToFolder, $"{companyId}.xml")))
            {
                return null;
            }

            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(Path.Combine(_pathToFolder, $"{companyId}.xml"));
                return xDoc;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
    }
}