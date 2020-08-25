using System;
using Microsoft.AspNetCore.Http;

namespace masz.Dtos.ModCase
{
    public class UploadedFile
    {
        public IFormFile File { set; get; }
    }
}