using System.Diagnostics;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Parser.Models.SitesForParsing;

namespace Parser
{
    class MainParser
    {
        static async Task Main()
        {
            DatabaseInitializer.Initialize();
            ISite site = new Fackel();
            var sitemapUrl = site.SitemapUrl;
            var urls = await LoadUrlsFromSitemap(sitemapUrl, site);
            await ProcessUrls(urls, site);
        }
        
        
        //метод парсинга ссылок
        static async Task<List<string>> LoadUrlsFromSitemap(string sitemapUrl, ISite site)
        {
            WebClient client = new WebClient();
            string sitemapXml = await client.DownloadStringTaskAsync(sitemapUrl);

            List<string> urls = new List<string>();

            using (XmlReader reader = XmlReader.Create(new StringReader(sitemapXml)))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "loc")
                    {
                        reader.Read();
                        var url = reader.Value;
                        if (IsProductUrl(url, site))
                        {
                            urls.Add(url);
                        }
                    }
                }
            }

            return urls;
        }
        
        //метод отправки ссылок на парсинг
        static async Task ProcessUrls(List<string> urls, ISite site)
        {
            int semaphoreCount = 25;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"Время начала парсинга {DateTime.Now}");
            var semaphore = new SemaphoreSlim(semaphoreCount); // Ограничение на 25 одновременных запросов
            try
            {
                var tasks = urls.Select(url => ProcessUrlAsync(url, semaphore,site));
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                TimeSpan elapsedTime = stopwatch.Elapsed;
                Console.WriteLine($"Время выполнения запроса: {elapsedTime}");
                
            }
            
        }
        
        //запуска кода 
        static async Task ProcessUrlAsync(string url, SemaphoreSlim semaphore, ISite site)
        {
            await semaphore.WaitAsync(); // Захватываем семафор
            Console.WriteLine(url);
            var product = await ParserAsync(url,site);
            if (product.Avaible) await Db.AddDb(product);
            
            semaphore.Release(); // Освобождаем семафор после выполнения задачи
            
        }

        //проверка ссылку на товар
        static bool IsProductUrl(string url, ISite site)
        {
            string pattern = site.IsProductPattern;
            return Regex.IsMatch(url, pattern);
        }
        
        // не больше 25 запросов в 7 секунд ~
        // парсинг данных товара
        static async Task<Product> ParserAsync(string url,ISite site)
        {
            var product = new Product();
            product.OriginUrl = url;
            var doc = await LoadHtmlDocumentAsync(url);

            if (doc != null)
            {
                ParseStock(doc, product,site);
                ParseDescription(doc, product,site);
                if (product.Avaible==false)
                { 
                    return product;
                }
                ParseCategory(doc, product,site);
                ParseName(doc, product,site);
                ParsePrice(doc, product,site);
                ParseCharacteristics(doc, product,site);
            }

            return product;
        }

        //выгрузка с сайта
        static async Task<HtmlDocument> LoadHtmlDocumentAsync(string url)
        {
            var web = new HtmlWeb();
            return await web.LoadFromWebAsync(url);
        }
        // количество на складе
        static void ParseStock(HtmlDocument doc, Product product, ISite site)
        {
            var stock = doc.DocumentNode.SelectNodes(site.StockXpath);
            if (stock != null)
            {
                int count = 0;
                foreach (var c in stock)
                {
                    string innerText = c.InnerText.Trim();
                    MatchCollection matches = Regex.Matches(innerText, @"-?\d+"); // Используем MatchCollection для нахождения всех чисел в строке
                    foreach (Match match in matches)
                    {
                        count += int.Parse(match.Value); // Суммируем все найденные числа
                    }
                }
                product.Stock = count;
                product.Avaible = count != 0;
            }
        }
        //категория
        static void ParseCategory(HtmlDocument doc, Product product, ISite site)
        {
            var category = doc.DocumentNode.SelectNodes(site.CategoryXpath);
            if (category != null)
            {
                product.Category = TextCorrector(category[0].InnerText);
            }
        }
        //название
        static void ParseName(HtmlDocument doc, Product product, ISite site)
        {
            var name = doc.DocumentNode.SelectNodes(site.NameXpath);
            if (name != null)
            {
                product.Name = TextCorrector(name[0].InnerText);
            }
        }
        
        //описание 
        static void ParseDescription(HtmlDocument doc, Product product, ISite site)
        {
            var description = doc.DocumentNode.SelectNodes(site.DescriptionXpath);
            if (description != null)
            {
                foreach (var d in description)
                {
                    product.Description = TextCorrector(d.InnerText);
                }

                if (product.Description=="")
                {
                    product.Avaible = false;
                    Console.WriteLine($"Пустая строка {product.OriginUrl}");
                }
            }
        }
        //цена
        static void ParsePrice(HtmlDocument doc, Product product, ISite site)
        {
            var price = doc.DocumentNode.SelectNodes(site.PriceXpath);
            if (price != null)
            {
                string cost = price[0].InnerText;
                cost = cost.Remove(cost.Length - 4).Replace(" ", "").Replace(".", "");
                product.Price = Convert.ToInt32(cost);
            }
        }
        //Характеристика
        static void ParseCharacteristics(HtmlDocument doc, Product product, ISite site)
        {
            string type="", text="";
            Dictionary<string, string> test = new Dictionary<string, string>();
           
            var characteristicspar = doc.DocumentNode.SelectNodes(site.CharacteristicsXpathPar);
            
            if (characteristicspar != null)
            {   
                var characteristicsparhelp = doc.DocumentNode.SelectNodes(site.CharacteristicsXpathParHelp);
                type = characteristicsparhelp[0].InnerText;
                foreach (var c in characteristicspar)
                {
                    if (IsNonStandardSize(c.InnerText)) break;
                    text += TextCorrector(c.InnerText) + " ";
                    
                }
                test.Add(type, text);
            }
            var characteristics = doc.DocumentNode.SelectNodes(site.CharacteristicsXpath);
            
            if (characteristics != null)
            {
            
                var characteristicshelp = doc.DocumentNode.SelectNodes(site.CharacteristicsXpathHelp);
                for (int i = 0; i < characteristics.Count; i++)
                {
                    type = TextCorrector(characteristics[i].InnerText);
                    text = TextCorrector(characteristicshelp[i].InnerText);
                    test.Add(type, text);
                }
            }
            var characteristicscolororequ = doc.DocumentNode.SelectSingleNode(site.CharacteristicsColOrEqu);
            
            if (characteristicscolororequ != null)
            {
                type = TextCorrector (characteristicscolororequ.InnerText);
                var characteristicscolorhelp = doc.DocumentNode.SelectNodes(site.CharacteristicsColOrEquHelp);
                foreach (var c in characteristicscolorhelp)
                {
                    text = TextCorrector(c.InnerText);
                    test.Add(type,text);
                    
                }
            }
            var characteristicscolororequ2 = doc.DocumentNode.SelectSingleNode(site.CharacteristicsColOrEqu2);
            
            if (characteristicscolororequ2 != null)
            {
                type = TextCorrector (characteristicscolororequ2.InnerText);
                var characteristicscolorhelp = doc.DocumentNode.SelectNodes(site.CharacteristicsColOrEquHelp2);
                foreach (var c in characteristicscolorhelp)
                {
                    text = TextCorrector(c.InnerText);
                    test.Add(type,text);
                   
                }
            }

            product.Characteristics = test;
        }
        
        //метод исправления текста : исправление табуляции, замена кода на символы
        static string TextCorrector(string text)
        {
            // замена множества пробелов
            string cleanedText = Regex.Replace(text, @"\s+", " ");
           
            cleanedText = cleanedText.Replace("\n", "").Replace("\t", "").Replace("&quot;", "\"").Replace("&#40;","(").Replace("&#41;",")").Replace("&nbsp;"," ").Replace("&mdash;","-").Replace("&#43;","+").Trim();
            return cleanedText;
        }

        static bool IsNonStandardSize(string text)
        {
            // отмена парсинга характеристик если идут нестанртные размеры
            return text.ToLower().Contains("нестандартные размер");
        }
    }
}