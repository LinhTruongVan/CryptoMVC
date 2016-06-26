using System;
using CryptoMVC.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CryptoMVC.Services;

namespace CryptoMVC.Controllers
{
    [Authorize]
    public class DownloadController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly GeneticCipherService _geneticCipherService = new GeneticCipherService();
        private readonly CryptoHelper _cryptoHelper = new CryptoHelper();
        public ActionResult Index()
        {
            var downloadViewModel = new DownloadViewModel();
            if (User.IsInRole(RoleName.Admin) || User.IsInRole(RoleName.Teacher))
            {
                downloadViewModel.Documents = GetSuitableDocumentsForUser(null);
            }
            else if (User.IsInRole(RoleName.Student))
            {
                var documentTypes = new List<DocumentType> { DocumentType.General };
                if (User.IsInRole(RoleName.GradeSixStudent))
                {
                    documentTypes.Add(DocumentType.GradeSix);
                }
                else if (User.IsInRole(RoleName.GradeSevenStudent))
                {
                    documentTypes.Add(DocumentType.GradeSeven);
                }
                else if (User.IsInRole(RoleName.GradeEightStudent))
                {
                    documentTypes.Add(DocumentType.GradeEight);
                }
                else if (User.IsInRole(RoleName.GradeNineStudent))
                {
                    documentTypes.Add(DocumentType.GradeNine);
                }
                downloadViewModel.Documents = GetSuitableDocumentsForUser(documentTypes);
            }
            else
            {
                downloadViewModel.Documents = GetSuitableDocumentsForUser(new List<DocumentType> { DocumentType.General });
            }
            return View(downloadViewModel);
        }
        public FileResult DownloadFile(string filePath, string fileName)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("Document not found.");
            }
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [Authorize(Roles = RoleName.Admin)]
        [HttpPost]
        public ActionResult DeleteFile(int fileId)
        {
            var deletedDocument = _context.Documents.FirstOrDefault(d => d.Id == fileId);
            if (deletedDocument != null)
            {
                var filePath = Path.Combine(Server.MapPath("~/Documents"), deletedDocument.Id + Path.GetExtension(deletedDocument.Name));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _context.Documents.Remove(deletedDocument);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = RoleName.Admin)]
        [HttpPost]
        public FileResult DecryptFile(int fileId)
        {
            var document = _context.Documents.FirstOrDefault(d => d.Id == fileId);
            if (document != null)
            {
                var filePath = Path.Combine(Server.MapPath("~/Documents"),
                    document.Id + Path.GetExtension(document.Name));
                if (!System.IO.File.Exists(filePath))
                {
                    throw new Exception("Document not found.");
                }
                var fileBytes = _geneticCipherService.Decrypt(System.IO.File.ReadAllBytes(filePath),
                    _cryptoHelper.Decrypt(document.Key));
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, document.Name);
            }
            else
            {
                throw new Exception("Document not found.");
            }
        }

        private IEnumerable<DocumentViewModel> GetSuitableDocumentsForUser(IEnumerable<DocumentType> documentTypes)
        {
            IEnumerable<DocumentViewModel> documents;
            if (documentTypes == null)
            {
                documents = _context.Documents
                    .OrderByDescending(d => d.UploadedDate)
                    .Include(d => d.ApplicationUser)
                    .Select(d => new DocumentViewModel
                    {
                        Id = d.Id,
                        AuthorName = d.ApplicationUser.UserName,
                        FileName = d.Name,
                        FileType = d.DocumentType.ToString(),
                        FilePath = d.Id.ToString(),
                        UploadedDate = d.UploadedDate
                    }).ToList();
            }
            else
            {
                documents = _context.Documents
                    .Where(d => documentTypes.Contains(d.DocumentType))
                    .OrderByDescending(d => d.UploadedDate)
                    .Include(d => d.ApplicationUser)
                    .Select(d => new DocumentViewModel
                    {
                        Id = d.Id,
                        AuthorName = d.ApplicationUser.UserName,
                        FileName = d.Name,
                        FileType = d.DocumentType.ToString(),
                        FilePath = d.Id.ToString(),
                        UploadedDate = d.UploadedDate
                    }).ToList();
            }
            foreach (var d in documents)
            {
                d.FilePath = Path.Combine(Server.MapPath("~/Documents"), d.FilePath + Path.GetExtension(d.FileName));
            }
            return documents;
        }
    }
}