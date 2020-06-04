using CsQuery;
using ShareMyPhoto.models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ShareMyPhoto.lib
{
    public class Scraper : IScraper
    {

        private string Clean(Uri u, string s)
        {
            if (s != null && s.StartsWith("//"))
                return $"{s.Replace("//", $"{u.Scheme}://")}";
            else
                return s;
        }

        public async Task<Result> FindImageSourcesAsync(string url, int width)
        {
            try
            {
                var result = new Result();

                using (var wc = new WebClient())
                {
                    var data = await wc.DownloadStringTaskAsync(new Uri(url));
                    var dom = CQ.Create(data);

                    dom["img"].Each(x =>
                    {

                        var src = x.Attributes["src"];

                        if (src.StartsWith("https://lh3.googleusercontent.com"))
                        {
                            result.Message = $"{src.Split('=')[0]}=w{width}";
                            result.Success = true;
                        }

                    });
                }

                return result;

            }
            catch (Exception e)
            {
                return new Result { Message = $"{e.Message}" };

            }

        }
    }
}
