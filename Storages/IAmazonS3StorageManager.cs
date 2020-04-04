using System.IO;

namespace AmazonS3AspNet.Storages
{
    public interface IAmazonS3StorageManager
    {
        void Create(Models.File fileEntry, MemoryStream stream);
        byte[] Read(Models.File fileEntry);
        void Delete(Models.File fileEntry);
        string GetCannedSignedURL(string fileName);
    }
}
