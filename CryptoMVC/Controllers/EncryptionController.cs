using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using CryptoMVC.Models;
using CryptoMVC.Services;
using Microsoft.AspNet.Identity;

namespace CryptoMVC.Controllers
{
    [Authorize]
    public class EncryptionController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly GeneticCipherService _geneticCipherService = new GeneticCipherService();
        private readonly CryptoHelper _cryptoHelper = new CryptoHelper();
        public ActionResult Index()
        {
            return View("Index", new EncryptionViewModel());
        }

        [HttpPost]
        public ActionResult Encrypt(EncryptionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", viewModel);
            }

            var document = new Document
            {
                ApplicationUserId = User.Identity.GetUserId(),
                Name = viewModel.File.FileName,
                Key = _cryptoHelper.Encrypt(viewModel.Key),
                DocumentType = viewModel.SelectedDocumentType,
                UploadedDate = DateTime.Now
            };
            _context.Documents.Add(document);
            _context.SaveChanges();

            var filePath = Path.Combine(Server.MapPath("~/Documents"), document.Id + Path.GetExtension(viewModel.File.FileName));
            using (var binaryReader = new BinaryReader(viewModel.File.InputStream))
            {
                var fileData = binaryReader.ReadBytes(viewModel.File.ContentLength);
                var encryptedFileData = _geneticCipherService.Encrypt(fileData, viewModel.Key);
                System.IO.File.WriteAllBytes(filePath, encryptedFileData);
                viewModel.CipherText = System.Text.Encoding.UTF8.GetString(encryptedFileData);
            }

            ModelState.Clear();
            viewModel.File = null;
            viewModel.Key = "";
            return View("Index", viewModel);
        }
    }
}