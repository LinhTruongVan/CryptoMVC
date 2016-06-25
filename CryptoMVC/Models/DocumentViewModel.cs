using System;

namespace CryptoMVC.Models
{
    public class DocumentViewModel
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string AuthorName { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}