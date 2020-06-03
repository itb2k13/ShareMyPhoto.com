using System;

namespace ShareMyPhoto.com
{
    public class Result
    {
        public byte[] Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }

    public class UserResult
    {
        public string OriginalUrl { get; set; }
        public string IntermediateUrl { get; set; }
        public int IntermediateDownloadAttempts { get; set; }
        public string ShareUrl { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }

        public long SizeInBytes { get; set; }
    }
}
