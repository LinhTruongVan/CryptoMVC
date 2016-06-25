using System;
using System.Configuration;
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

            //var exam = new Exam()
            //{
            //    ApplicationUserId = User.Identity.GetUserId(),
            //    Name = viewModel.File.FileName,
            //    Key = _geneticCipherService.EncryptString(viewModel.Key, ConfigurationManager.AppSettings["DefaultKey"]),
            //    DocumentType = viewModel.SelectedExamType,
            //    UploadedDate = DateTime.Now,
            //    StartTime = new DateTime()
            //};

            return Content("");
        }
    }
}