using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ShareMyPhoto.lib;
using ShareMyPhoto.models;
using shortid;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace ShareMyPhoto.com.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private static IAmazonS3 _s3Client;
        private readonly IScraper _scraper;
        private readonly string _defaultBucketName;
        private readonly int _defaultWidth;
        private readonly string[] _validBucketNames;

        public PhotoController(IConfiguration config, IScraper scraper)
        {
            _scraper = scraper;
            _s3Client = new AmazonS3Client(config.GetValue<string>("awsAccessKeyId"), config.GetValue<string>("awsSecretAccessKey"), RegionEndpoint.EUWest2);
            _defaultBucketName = config.GetValue<string>("defaultBucketName");
            _defaultWidth = config.GetValue<int>("defaultWidth");
            _validBucketNames = config.GetValue<string>("validBucketNames").Split(';');
        }

        [HttpGet]
        public async Task<UserResult> Get(string url, string? bucketName, int? width)
        {
            var userResult = new UserResult(url, bucketName ?? _defaultBucketName, width ?? _defaultWidth, $"{ShortId.Generate(true, false, 8)}");
            
            if(!_validBucketNames.Contains(userResult.Input.BucketName))
            {
                userResult.ErrorMessage = "Specified bucket name does not exist.";
                return userResult;
            }                  

            if (url.StartsWith("https://photos.app.goo.gl/"))
            {
                for (int i = 0; i <= 4; i++)
                {
                    var result = await _scraper.FindImageSourcesAsync(url, userResult.Input.Width);

                    if (result.Success && result.Message.Length > 0)
                    {
                        userResult.ErrorMessage = null;
                        userResult.Input.IntermediateUrl = result.Message;
                        url = result.Message;
                        break;
                    }
                    else
                    {
                        userResult.ErrorMessage = result.Message;
                    }
                }

                if (userResult.Input.IntermediateUrl == null)
                {
                    userResult.ErrorMessage = userResult.ErrorMessage ?? "Could not obtain image path.";
                    return userResult;
                }
            }

            var downloadResult = await DownloadFileAsync(url);

            if (downloadResult.Success && downloadResult.Data.Length > 0)
            {
                var uploadResult = await UploadFileAsync(downloadResult.Data, userResult.Input.BucketName, userResult.Input.KeyName);

                if(uploadResult.Success && uploadResult.Message.Length > 0)
                {
                    userResult.Success = true;
                    userResult.Output.SizeInBytes = downloadResult.Data.Length;
                    return userResult;
                }
                else
                {
                    userResult.ErrorMessage = uploadResult.Message;
                    return userResult;
                }
            }
            else
            {
                userResult.ErrorMessage = downloadResult.Message;
                return userResult;
            }
        }


        private async Task<Result> DownloadFileAsync(string url)
        {
            try
            {
                using (var wc = new WebClient())
                {
                    var data = await wc.DownloadDataTaskAsync(new Uri(url));
                    return new Result { Data = data, Success = true };
                }
            }
            catch (Exception e)
            {
                return new Result { Message = $"{e.Message}" };
            }
        }

        private async Task<Result> UploadFileAsync(byte[] file, string bucketName, string keyName)
        {
            try
            {
                using (var fileToUpload = new MemoryStream(file))
                {
                    await new TransferUtility(_s3Client).UploadAsync(fileToUpload, bucketName, keyName);
                    return new Result { Message = keyName, Success = true };
                }
            }
            catch (Exception e)
            {
                return new Result { Message = $"{e.Message}" };
            }

        }
    }
}