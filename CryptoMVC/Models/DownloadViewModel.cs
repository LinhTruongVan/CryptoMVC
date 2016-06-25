using System.Collections.Generic;

namespace CryptoMVC.Models
{
    public class DownloadViewModel
    {
       public IEnumerable<DocumentViewModel> Documents { get; set; }
    }
}