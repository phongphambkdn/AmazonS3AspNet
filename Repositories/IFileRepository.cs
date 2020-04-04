using AmazonS3AspNet.Models;
using System;

namespace AmazonS3AspNet.Repositories
{
    public interface IFileRepository
    {
        File Get(Guid id);

        Guid Add(File fileEntry);

        void Update(File fileEntry);

        void Delete(Guid id);
    }
}
