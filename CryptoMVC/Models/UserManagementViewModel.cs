using System.Collections.Generic;

namespace CryptoMVC.Models
{
    public class UserManagementViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; } 
    }
}