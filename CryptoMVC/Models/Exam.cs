using System;

namespace CryptoMVC.Models
{
    public class Exam : BaseDocument
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}