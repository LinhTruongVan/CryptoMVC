using System;

namespace CryptoMVC.Models
{
    public class BaseDocument
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public DocumentType DocumentType { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}