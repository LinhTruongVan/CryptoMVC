using System;

namespace CryptoMVC.Models
{
    public class ExamAssignmentDetailViewModel
    {
        public int ExamAssignmentId { get; set; }
        public string Content { get; set; }
        public DateTime Deadline { get; set; }
    }
}