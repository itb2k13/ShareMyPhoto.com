using ShareMyPhoto.models;
using System.Threading.Tasks;

namespace ShareMyPhoto.lib
{
    public interface IScraper
    {
        Task<Result> FindImageSourcesAsync(string url);
    }
}