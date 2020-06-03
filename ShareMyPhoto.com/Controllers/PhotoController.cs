using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ShareMyPhoto.lib;
using ShareMyPhoto.models;
using System;
using System.Collections.Generic;
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
        private static IAmazonS3 s3Client;
        private readonly IScraper _scraper;
        private readonly string bucketName;

        public PhotoController(IConfiguration config, IScraper scraper)
        {
            _scraper = scraper;
            s3Client = new AmazonS3Client(config.GetValue<string>("awsAccessKeyId"), config.GetValue<string>("awsSecretAccessKey"), RegionEndpoint.EUWest2);
            bucketName = config.GetValue<string>("bucketName");
        }

        [HttpGet]
        public async Task<UserResult> Get(string url)
        {

            var userResult = new UserResult() { OriginalUrl = url };

            if (url.StartsWith("https://photos.app.goo.gl/"))
            {
                for (int i = 0; i <= 2; i++)
                {
                    var result = await _scraper.FindImageSourcesAsync(url);

                    if (result.Success && result.Message.Length > 0)
                    {
                        userResult.IntermediateUrl = result.Message;
                        url = result.Message;
                        break;
                    }
                    else
                    {
                        userResult.ErrorMessage = result.Message;
                    }
                }

                if (userResult.IntermediateUrl == null)
                {
                    userResult.ErrorMessage = userResult.ErrorMessage ?? "Could not obtain image path.";
                    return userResult;
                }
            }

            var downloadResult = await DownloadFileAsync(url);

            if (downloadResult.Success && downloadResult.Data.Length > 0)
            {
                var uploadResult = await UploadFileAsync(downloadResult.Data, $"{Guid.NewGuid().ToString()}-{Guid.NewGuid().ToString()}.jpg");

                if(uploadResult.Success && uploadResult.Message.Length > 0)
                {
                    userResult.Success = true;
                    userResult.ShareUrl = uploadResult.Message;
                    userResult.SizeInBytes = downloadResult.Data.Length;
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

        private async Task<Result> UploadFileAsync(byte[] file, string keyName)
        {
            try
            {
                using (var fileToUpload = new MemoryStream(file))
                {
                    await new TransferUtility(s3Client).UploadAsync(fileToUpload, bucketName, keyName);
                    return new Result { Message = $"https://{bucketName}.s3-eu-west-2.amazonaws.com/{keyName}", Success = true };
                }
            }
            catch (Exception e)
            {
                return new Result { Message = $"{e.Message}" };
            }

        }
    }
}