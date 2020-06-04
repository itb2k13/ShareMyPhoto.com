using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ShareMyPhoto.models
{
    public class Kvp
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Image
    {
        public List<Kvp> Attributes { get; set; } = new List<Kvp>();
    }

    public class Result
    {
        public byte[] Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public List<Image> Images { get; set; } = new List<Image>();
        
    }

    public class UserResult
    {
        public class UserResultInput
        {

            public UserResultInput(string originalUrl, string bucketName, int width, string keyName)
            {
                OriginalUrl = originalUrl;
                BucketName = bucketName;
                Width = width;
                KeyName = keyName;
            }

            [JsonIgnore]
            public string BucketName { get; }
            [JsonIgnore]
            public int Width { get; }
            [JsonIgnore]
            public string KeyName { get; }
            public string OriginalUrl { get; }
            [JsonIgnore]
            public string IntermediateUrl { get; set; }
        }

        public class UserResultOutput
        {
            public string _originalUrl;
            public string _bucketName;
            public string _keyName;

            public UserResultOutput(string originalUrl, string bucketName, string keyName)
            {
                _originalUrl = originalUrl;
                _bucketName = bucketName;
                _keyName = keyName;
            }
            public string Share => SizeInBytes > 0 ? $"https://{_bucketName}/{_keyName}" : null;
            public long SizeInBytes { get; set; }
        }

        public UserResult(string originalUrl, string bucketName, int width, string keyName)
        {
            Input = new UserResultInput(originalUrl, bucketName, width, keyName);
            Output = new UserResultOutput(originalUrl, bucketName, keyName);
        }

        public UserResultInput Input { get; }
        public UserResultOutput Output { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }        
    }
}
