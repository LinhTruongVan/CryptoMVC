using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CryptoMVC.Models;
using CryptoMVC.Services;
using Microsoft.AspNet.Identity;

namespace CryptoMVC.Controllers
{
    [Authorize(Roles = RoleName.Admin + "," + RoleName.Teacher)]
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly GeneticCipherService _geneticCipherService = new GeneticCipherService();
        private readonly CryptoHelper _cryptoHelper = new CryptoHelper();
        public ActionResult Index()
        {
            return View(new ExamEncryptionViewModel());
        }

        [HttpPost]
        public ActionResult Encrypt(ExamEncryptionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", viewModel);
            }

            if (DateTime.Compare(viewModel.StartTime, viewModel.EndTime) > -1)
            {
                viewModel.DateTimeErrorMessage = "Start Time must be less than End Time.";
                return View("Index", viewModel);
            }

            var exam = new Exam()
            {
                ApplicationUserId = User.Identity.GetUserId(),
                Name = viewModel.File.FileName,
                Key = _cryptoHelper.Encrypt(viewModel.Key),
                DocumentType = viewModel.SelectedExamType,
                UploadedDate = DateTime.Now,
                StartTime = viewModel.StartTime,
                EndTime = viewModel.EndTime
            };

            _context.Exams.Add(exam);
            _context.SaveChanges();

            var filePath = Path.Combine(Server.MapPath("~/Exams"), exam.Id + Path.GetExtension(viewModel.File.FileName));
            using (var binaryReader = new BinaryReader(viewModel.File.InputStream))
            {
                var fileData = binaryReader.ReadBytes(viewModel.File.ContentLength);
                var encryptedFileData = _geneticCipherService.Encrypt(fileData, viewModel.Key);
                System.IO.File.WriteAllBytes(filePath, encryptedFileData);
                viewModel.CipherText = System.Text.Encoding.UTF8.GetString(encryptedFileData);
            }

            var specialRoleName = DocumentTypeService.GetRoleAccessDocument(viewModel.SelectedExamType);
            if (specialRoleName != null)
            {
                var gradeStudentRole = _context.Roles.SingleOrDefault(r => r.Name == specialRoleName);
                var userIdsInRole = _context.Users.Where(u => u.Roles.Any(r => r.RoleId == gradeStudentRole.Id)).Select(u => u.Id);
                foreach (var userId in userIdsInRole)
                {
                    _context.ExamAssignments.Add(new ExamAssignment
                    {
                        ApplicationUserId = userId,
                        ExamId = exam.Id,
                        DocumentId = null,
                        Finished = false
                    });
                }
                _context.SaveChanges();
            }

            ModelState.Clear();
            viewModel.File = null;
            viewModel.Key = "";
            viewModel.DateTimeErrorMessage = null;
            return View("Index", viewModel);
        }
    }
}