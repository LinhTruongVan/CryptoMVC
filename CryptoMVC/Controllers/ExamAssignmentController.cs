using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
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
                .Where(ea => ea.ApplicationUserId == studentId && ea.Finished == false && DateTime.Compare(ea.Exam.EndTime, now) > 0 && DateTime.Compare(ea.Exam.StartTime, now) < 0)
                .Include(ea => ea.Exam)
                .Include(ea => ea.Exam.ApplicationUser)
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
                .Include(ea => ea.Exam)
                .FirstOrDefault(ea => ea.Id == id && ea.Finished == false && DateTime.Compare(ea.Exam.EndTime, DateTime.Now) > 0);
            if (examAssignment == null)
            {
                return HttpNotFound();
            }

            var examFilePath = Path.Combine(Server.MapPath("~/Exams"), examAssignment.Exam.Id + Path.GetExtension(examAssignment.Exam.Name));
            var descryptionKey = _cryptoHelper.Decrypt(examAssignment.Exam.Key);
            if (System.IO.File.Exists(examFilePath) == false)
            {
                throw new Exception("Exam not found.");
            }

            var fileData = System.IO.File.ReadAllBytes(examFilePath);
            var plainTextAsBytes = _geneticCipherService.Decrypt(fileData, descryptionKey);
            var timeSpan = examAssignment.Exam.EndTime - DateTime.Now;
            ViewBag.RemainingMinutes = (int) timeSpan.TotalMinutes;
            var viewModel = new ExamAssignmentDetailViewModel
            {
                Content = Encoding.UTF8.GetString(plainTextAsBytes),
                Deadline = examAssignment.Exam.EndTime,
                ExamAssignmentId = examAssignment.Id
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Submit(ExamAssignmentDetailViewModel viewModel)
        {
            var examAssignment = _context.ExamAssignments
                .Include(ea=>ea.Exam)
                .FirstOrDefault(ea => ea.Id == viewModel.ExamAssignmentId);
            if (examAssignment != null)
            {
                examAssignment.Finished = true;
            }
            if (DateTime.Compare(DateTime.Now, viewModel.Deadline) > 0)
            {
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            var answerName = "answer.txt";
            var randomKey = Guid.NewGuid().ToString();
            var answer = new Document
            {
                ApplicationUserId = User.Identity.GetUserId(),
                Name = answerName,
                Key = _cryptoHelper.Encrypt(randomKey),
                DocumentType = DocumentTypeService.GetAnswerTypeFromExamType(examAssignment.Exam.DocumentType),
                UploadedDate = DateTime.Now,
                IsAnswer = true
            };

            examAssignment.DocumentId = answer.Id;
            _context.Documents.Add(answer);
            _context.SaveChanges();

            var filePath = Path.Combine(Server.MapPath("~/Documents"), answer.Id + Path.GetExtension(answerName));
            var encryptedFileData = _geneticCipherService.Encrypt(Encoding.UTF8.GetBytes(viewModel.Content), randomKey);
            System.IO.File.WriteAllBytes(filePath, encryptedFileData);

            return RedirectToAction("Index");
        }
    }
}