using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using CryptoMVC.Services;

namespace CryptoMVC.Models
{
    public class EncryptionViewModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
        [Required]
        public string Key { get; set; }
        [DisplayName("Type")]
        public DocumentType SelectedDocumentType { get; set; }
        public List<SelectListItem> DocumentTypes(IPrincipal userPrincipal)
        {
            if (userPrincipal.IsInRole(RoleName.Admin) || userPrincipal.IsInRole(RoleName.Teacher))
            {
                return DocumentTypeService.GetDocumentTypesForAdmin();
            }
            if (userPrincipal.IsInRole(RoleName.GradeSixStudent))
            {
                return DocumentTypeService.GetDocumentTypesForStudentGradeSix();
            }
            if (userPrincipal.IsInRole(RoleName.GradeSevenStudent))
            {
                return DocumentTypeService.GetDocumentTypesForStudentGradeSeven();
            }
            if (userPrincipal.IsInRole(RoleName.GradeEightStudent))
            {
                return DocumentTypeService.GetDocumentTypesForStudentGradeEight();
            }
            if (userPrincipal.IsInRole(RoleName.GradeNineStudent))
            {
                return DocumentTypeService.GetDocumentTypesForStudentGradeNine();
            }

            return new List<SelectListItem>();
        }
    }
}