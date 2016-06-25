using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Services.Description;
using CryptoMVC.Models;
using CryptoMVC.Services;
using Microsoft.AspNet.Identity;

namespace CryptoMVC.Controllers
{
    [Authorize(Roles = RoleName.Student)]
    public class ExamAssignmentController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly GeneticCipherService _geneticCipherService = new GeneticCipherService();
        private readonly CryptoHelper _cryptoHelper = new CryptoHelper();
        public ActionResult Index()
        {
            var viewModel = new ExamAssignmentViewModel();
            var studentId = User.Identity.GetUserId();
            var now = DateTime.Now;
            viewModel.Exams = _context.ExamAssignments
                .Where(ea => ea.ApplicationUserId == studentId && DateTime.Compare(ea.Exam.EndTime, now) > -1)
                .Include(ea => ea.Exam)
                .Include(ea=>ea.Exam.ApplicationUser)
                .Select(ea => new ExamViewModel
                {
                    Id = ea.Id,
                    Name = ea.Exam.Name,
                    Type = ea.Exam.DocumentType,
                    Teacher = ea.Exam.ApplicationUser.UserName,
                    Deadline = ea.Exam.EndTime
                })
                .ToList();
            foreach (var exam in viewModel.Exams)
            {
                exam.Name = Path.GetFileNameWithoutExtension(exam.Name);
            }
            return View(viewModel);
        }

        public ActionResult Detail(int? id)
        {
            var examAssignment = _context.ExamAssignments
                .Include(ea=>ea.Exam)
                .FirstOrDefault(ea => ea.Id == id && ea.Finished == false);
            if (examAssignment == null)
            {
                return HttpNotFound();
            }

            var examFilePath = Path.Combine(Server.MapPath("~/Exams"), examAssignment.Exam.Id + Path.GetExtension(examAssignment.Exam.Name));
            var descryptionKey = _cryptoHelper.Decrypt(examAssignment.Exam.Key);
            if (System.IO.File.Exists(examFilePath) == false)
            {
                throw new Exception("Exam file not found.");
            }

            var fileData = System.IO.File.ReadAllBytes(examFilePath);
            var plainTextAsBytes = _geneticCipherService.Decrypt(fileData, descryptionKey);
            var viewModel = new ExamAssignmentDetailViewModel
            {
                Content = System.Text.Encoding.UTF8.GetString(plainTextAsBytes),
                Deadline = examAssignment.Exam.EndTime,
                ExamAssignmentId = examAssignment.ExamId
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Submit(ExamAssignmentDetailViewModel viewModel)
        {
            return RedirectToAction("Index");
        }
    }
}