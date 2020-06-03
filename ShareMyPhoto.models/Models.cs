using System;
using System.Collections;
using System.Collections.Generic;

namespace ShareMyPhoto.models
{
    public class Kvp
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class PostResult
    {
        public dynamic info { get; set; }
        public string content { get; set; }
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
        public string OriginalUrl { get; set; }
        public string IntermediateUrl { get; set; }
        public string ShareUrl { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public long SizeInBytes { get; set; }
    }
}
