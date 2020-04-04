using AmazonS3AspNet.Models;
using System;
using System.Linq;

namespace AmazonS3AspNet.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly DataContext _context;

        public FileRepository(DataContext context)
        {
            _context = context;
        }

        public File Get(Guid id)
        {
            return _context.FileEntries.FirstOrDefault(x => x.Id == id);
        }

        public Guid Add(File fileEntry)
        {
            _context.FileEntries.Add(fileEntry);
            _context.SaveChanges();

            return fileEntry.Id;
        }

        public void Update(File fileEntry)
        {
            var fileInDb = _context.FileEntries.FirstOrDefault(x => x.Id == fileEntry.Id);
            if (fileInDb != null)
            {
                fileInDb.FileLocation = fileEntry.FileLocation;
                _context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            var fileInDb = _context.FileEntries.FirstOrDefault(x => x.Id == id);
            if (fileInDb != null)
            {
                _context.FileEntries.Remove(fileInDb);
                _context.SaveChanges();
            }
        }
    }
}
