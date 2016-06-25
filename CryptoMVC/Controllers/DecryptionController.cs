using System.IO;
using System.Web.Mvc;
using CryptoMVC.Models;
using CryptoMVC.Services;

namespace CryptoMVC.Controllers
{
    [Authorize]
    public class DecryptionController : Controller
    {
        private readonly GeneticCipherService _geneticCipherService = new GeneticCipherService();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Decrypt(DecryptionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", viewModel);
            }
            
            using (var binaryReader = new BinaryReader(viewModel.File.InputStream))
            {
                var fileData = binaryReader.ReadBytes(viewModel.File.ContentLength);
                var descryptedFileData = _geneticCipherService.Decrypt(fileData, viewModel.Key);
                return File(descryptedFileData, System.Net.Mime.MediaTypeNames.Application.Octet, viewModel.File.FileName);
            }
        }
    }
}