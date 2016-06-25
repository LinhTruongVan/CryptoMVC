using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CryptoMVC.Models;
using Microsoft.AspNet.Identity;

namespace CryptoMVC.Controllers
{
    [Authorize(Roles = RoleName.Student)]
    public class ExamAssignmentController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public ActionResult Index()
        {
            //var viewModel = new ExamAssignmentViewModel();
            //var studentId = User.Identity.GetUserId();
            //viewModel.Exams = _context.ExamAssignments
            //    .Where(ea => ea.StudentId == studentId)
            //    .Include(ea=>ea.Document)
            //    .Select(ea=> new ExamViewModel
            //    {
            //        Id = ea.Id,
            //        Name = ea.Document.Name,
            //        Type = ea.Document.DocumentType
            //    })
            //    .ToList();
            //foreach (var exam in viewModel.Exams)
            //{
            //    exam.Name = Path.GetFileNameWithoutExtension(exam.Name);
            //}
            //return View(viewModel);
            return Content("Hello");
        }
    }
}