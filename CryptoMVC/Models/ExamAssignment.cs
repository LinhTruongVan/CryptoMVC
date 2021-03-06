﻿namespace CryptoMVC.Models
{
    public class ExamAssignment
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int ExamId { get; set; }
        public virtual Exam Exam { get; set; }
        public int? DocumentId { get; set; }
        public Document Document { get; set; }
        public bool Finished { get; set; }
    }
}