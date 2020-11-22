using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace masz.Services
{
    public interface IFilesHandler
    {
        byte[] ReadFile(string path);
        string GetContentType(string path);
        FileInfo[] GetFilesByDirectory(string directory);
        Task<string> SaveFile(IFormFile file, string directory);
        void DeleteFile(string path);
        void DeleteDirectory(string directory);
    }
}
