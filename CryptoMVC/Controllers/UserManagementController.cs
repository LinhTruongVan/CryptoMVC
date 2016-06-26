using System.Linq;
using System.Web.Mvc;
using CryptoMVC.Models;
using Microsoft.AspNet.Identity;

namespace CryptoMVC.Controllers
{
    [Authorize(Roles = RoleName.Admin)]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public ActionResult Index()
        {
            string adminId = User.Identity.GetUserId();
            var viewModel = new UserManagementViewModel
            {
                Users = _context.Users.Where(u=>u.Id != adminId)
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Name = u.UserName
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DeleteUser(string userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            var examAssignments = _context.ExamAssignments.Where(ea => ea.ApplicationUserId == user.Id).ToList();
            _context.ExamAssignments.RemoveRange(examAssignments);
            _context.Users.Remove(user);

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}