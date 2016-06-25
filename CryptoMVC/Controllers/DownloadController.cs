using System;
using CryptoMVC.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CryptoMVC.Controllers
{
    [Authorize]
    public class DownloadController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public ActionResult Index()
        {
            var downloadViewModel = new DownloadViewModel();
            if (User.IsInRole(RoleName.Admin) || User.IsInRole(RoleName.Teacher))
            {
                downloadViewModel.Documents = GetSuitableDocumentsForUser(null);
            }
            else if (User.IsInRole(RoleName.Student))
            {
                var documentTypes = new List<DocumentType> {DocumentType.General};
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
        [Authorize(Roles = RoleName.Admin)]
        public FileResult DownloadFile(string filePath, string fileName)
        {
            if (!System.IO.File.Exists(filePath)) throw new Exception();

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public ActionResult DeleteFile(int fileId)
        {
            var deletedDocument = _context.Documents.FirstOrDefault(d=>d.Id == fileId);
            if (deletedDocument != null)
            {
                var filePath = Path.Combine(Server.MapPath("~/Documents"), deletedDocument.Id + Path.GetExtension(deletedDocument.Name));
                var relatedExamAssignments = _context.ExamAssignments.Where(ea=>ea.DocumentId == fileId).ToList();

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.ExamAssignments.RemoveRange(relatedExamAssignments);
                _context.Documents.Remove(deletedDocument);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        private IEnumerable<DocumentViewModel> GetSuitableDocumentsForUser(IEnumerable<DocumentType> documentTypes)
        {
            IEnumerable<DocumentViewModel> documents;
            if (documentTypes == null)
            {
                documents = _context.Documents
                    .OrderBy(d => d.DocumentType)
                    .Include(d=>d.ApplicationUser)
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
                    .OrderBy(d => d.DocumentType)
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