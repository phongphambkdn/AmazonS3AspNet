using Amazon;
using Amazon.CloudFront;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using AmazonS3AspNet.Constants;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace AmazonS3AspNet.Storages
{
    public class AmazonS3StorageManager : IAmazonS3StorageManager
    {
        private readonly IAmazonS3 _client;
        private readonly AmazonStorageSetting _s3Options;
        private readonly AmazonCloudFront _cloudFrontOptions;
        private readonly IDistributedCache _distributedCache;

        public AmazonS3StorageManager(
            IOptionsMonitor<AmazonStorageSetting> s3OptionsAccessor,
            IOptionsMonitor<AmazonCloudFront> cloudfrontOptionsAccessor,
            IDistributedCache distributedCache)
        {
            _s3Options = s3OptionsAccessor.CurrentValue;
            _cloudFrontOptions = cloudfrontOptionsAccessor.CurrentValue;
            _client = new AmazonS3Client(_s3Options.AccessKeyID, _s3Options.SecretAccessKey, RegionEndpoint.GetBySystemName(_s3Options.RegionEndpoint));
            _distributedCache = distributedCache;
        }

        public void Create(Models.File fileEntry, MemoryStream stream)
        {
            var fileTransferUtility = new TransferUtility(_client);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = fileEntry.Id.ToString(),
                BucketName = _s3Options.BucketName,                
                CannedACL = S3CannedACL.NoACL
            };

            fileTransferUtility.UploadAsync(uploadRequest).Wait();

            fileEntry.FileLocation = fileEntry.Id.ToString();
        }

        public void Delete(Models.File fileEntry)
        {
            _client.DeleteObjectAsync(_s3Options.BucketName, fileEntry.FileLocation).Wait();
        }

        public string GetCannedSignedURL(string fileName)
        {
            var cacheKey = $"s3url_{fileName}";
            var url = _distributedCache.GetString(cacheKey);
            if (string.IsNullOrEmpty(url))
            {
                url = "";
                using (var textReader = File.OpenText(_cloudFrontOptions.PrivateKeyPath))
                {
                    url = AmazonCloudFrontUrlSigner.GetCannedSignedURL(
                        AmazonCloudFrontUrlSigner.Protocol.https,
                        _cloudFrontOptions.Domain,
                        textReader,
                        fileName,
                        _cloudFrontOptions.KeypairId,
                        DateTime.Now.AddSeconds(_cloudFrontOptions.ExpiredInSecond));
                }

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(_cloudFrontOptions.ExpiredInSecond)
                };
                _distributedCache.SetString(cacheKey, url, options);
            }

            return url;
        }

        public byte[] Read(Models.File fileEntry)
        {
            var request = new GetObjectRequest
            {
                BucketName = _s3Options.BucketName,
                Key = fileEntry.FileLocation,
            };

            using (var response = _client.GetObjectAsync(request).GetAwaiter().GetResult())
            using (var responseStream = response.ResponseStream)
            using (var reader = new MemoryStream())
            {
                responseStream.CopyTo(reader);
                return reader.ToArray();
            }
        }
    }
}
