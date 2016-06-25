using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CryptoMVC.Models
{
    public class DecryptionViewModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
        [Required]
        public string Key { get; set; }
    }
}