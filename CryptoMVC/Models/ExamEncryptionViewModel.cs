using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using CryptoMVC.Services;

namespace CryptoMVC.Models
{
    public class ExamEncryptionViewModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
        [Required]
        public string Key { get; set; }
        [DisplayName("Type")]
        public DocumentType SelectedExamType { get; set; }
        [Required]
        [DisplayName("Start Time")]
        public DateTime StartTime { get; set; }
        [Required]
        [DisplayName("End Time")]
        public DateTime EndTime { get; set; }
        [DisplayName("Cipher Text")]
        [DefaultValue("")]
        public string CipherText { get; set; }

        public List<SelectListItem> ExamTypes(IPrincipal userPrincipal)
        {
            return DocumentTypeService.GetExamTypesForAdminAndTeacher();
        }
    }
}