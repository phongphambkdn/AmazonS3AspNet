﻿namespace AmazonS3AspNet.Constants
{
    public class AmazonStorageSetting
    {
        public string AccessKeyID { get; set; }

        public string SecretAccessKey { get; set; }

        public string BucketName { get; set; }

        public string RegionEndpoint { get; set; }
    }

    public class AmazonCloudFront
    {
        public string PrivateKeyPath { get; set; }

        public string Domain { get; set; }
     
        public string KeypairId { get; set; }

        public int ExpiredInSecond { get; set; }
    }
}
