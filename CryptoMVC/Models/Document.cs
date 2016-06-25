using System.ComponentModel;

namespace CryptoMVC.Models
{
    public class Document : BaseDocument
    {
        [DefaultValue(false)]
        public bool IsAnswer { get; set; }
    }
}