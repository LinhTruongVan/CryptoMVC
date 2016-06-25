using System.Collections.Generic;

namespace CryptoMVC.Models
{
    public class ExamAssignmentViewModel
    {
        public IEnumerable<ExamViewModel> Exams { get; set; } 
    }
}