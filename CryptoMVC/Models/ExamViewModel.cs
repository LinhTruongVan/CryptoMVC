using System;

namespace CryptoMVC.Models
{
    public class ExamViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DocumentType Type { get; set; }
        public string Teacher { get; set; }
        public DateTime Deadline { get; set; }
    }
}