using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace masz.Services
{
    public class FilesHandler : IFilesHandler
    {
        public string GetContentType(string path)
        {
            if (!File.Exists(path)) {
                return "application/octet-stream";
            }

            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(path, out contentType);
            return contentType ?? "application/octet-stream";
        }

        public FileInfo[] GetFilesByDirectory(string directory)
        {
            if (!Directory.Exists(directory)) {
                return null;
            }

            return new DirectoryInfo(directory).GetFiles();
        }

        public byte[] ReadFile(string path)
        {
            if (!File.Exists(path)) {
                return null;
            }

            return File.ReadAllBytes(path);
        }

        public async Task<string> SaveFile(IFormFile file, string directory)
        {
            Directory.CreateDirectory(directory);

            var uniqueFileName = GetUniqueFileName(file);
            var filePath = Path.Combine(directory, uniqueFileName);
            await file.CopyToAsync(new FileStream(filePath, FileMode.Create));
            return uniqueFileName;
        }

        public void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory)) {
                Directory.Delete(directory, true);
            }
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path)) {
                File.Delete(path);
            }
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        private string GetUniqueFileName(IFormFile file)
        {
            // TODO: change to hasing algorithm
            string fileName = Path.GetFileName(file.FileName);
            return  GetSHA1Hash(file)
                    + "_"
                    + Guid.NewGuid().ToString().Substring(0, 8)
                    + "_"
                    + Path.GetFileNameWithoutExtension(fileName)
                    + Path.GetExtension(fileName);
        }

        private string GetSHA1Hash(IFormFile file)
        {
            // get stream from file then convert it to a MemoryStream
            MemoryStream stream = new MemoryStream();
            file.OpenReadStream().CopyTo(stream);
            // compute md5 hash of the file's byte array.
            byte[] bytes = SHA1.Create().ComputeHash(stream.ToArray());
            stream.Close();
            return BitConverter.ToString(bytes).Replace("-",string.Empty).ToLower();
        }
    }
}