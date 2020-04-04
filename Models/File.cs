using System;

namespace AmazonS3AspNet.Models
{
    public class File
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string Description { get; set; }

        public long Size { get; set; }

        public DateTime UploadedTime { get; set; }

        public string FileLocation { get; set; }
    }
}
