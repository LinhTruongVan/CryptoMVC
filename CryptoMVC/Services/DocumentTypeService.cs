using System.Collections.Generic;
using System.Web.Mvc;
using CryptoMVC.Models;

namespace CryptoMVC.Services
{
    public static class DocumentTypeService
    {
        public static Dictionary<DocumentType, SelectListItem> DocumentTypes = new Dictionary
            <DocumentType, SelectListItem>
        {
            {
                DocumentType.General, new SelectListItem
                {
                    Text = "General",
                    Value = ((int)DocumentType.General).ToString()
                }
            },
            {
                DocumentType.GradeSix, new SelectListItem
                {
                    Text = "Grade 6",
                    Value = ((int)DocumentType.GradeSix).ToString()
                }
            },
            {
                DocumentType.GradeSeven, new SelectListItem
                {
                    Text = "Grade 7",
                    Value = ((int)DocumentType.GradeSeven).ToString()
                }
            },
            {
                DocumentType.GradeEight, new SelectListItem
                {
                    Text = "Grade 8",
                    Value = ((int)DocumentType.GradeEight).ToString()
                }
            },
            {
                DocumentType.GradeNine, new SelectListItem
                {
                    Text = "Grade 9",
                    Value = ((int)DocumentType.GradeNine).ToString()
                }
            },
            {
                DocumentType.GradeSixExam, new SelectListItem
                {
                    Text = "Exam 6",
                    Value = ((int)DocumentType.GradeSixExam).ToString()
                }
            },
            {
                DocumentType.GradeSevenExam, new SelectListItem
                {
                    Text = "Exam 7",
                    Value = ((int)DocumentType.GradeSevenExam).ToString()
                }
            },
            {
                DocumentType.GradeEightExam, new SelectListItem
                {
                    Text = "Exam 8",
                    Value = ((int)DocumentType.GradeEightExam).ToString()
                }
            },
            {
                DocumentType.GradeNineExam, new SelectListItem
                {
                    Text = "Exam 9",
                    Value = ((int)DocumentType.GradeNineExam).ToString()
                }
            }
        };

        public static List<SelectListItem> GetExamTypesForAdminAndTeacher()
        {
            return new List<SelectListItem>
            {
                DocumentTypes[DocumentType.GradeSixExam],
                DocumentTypes[DocumentType.GradeSevenExam],
                DocumentTypes[DocumentType.GradeEightExam],
                DocumentTypes[DocumentType.GradeNineExam]
            };
        }

        public static List<SelectListItem> GetDocumentTypesForAdmin()
        {
            return new List<SelectListItem>
            {
                DocumentTypes[DocumentType.General],
                DocumentTypes[DocumentType.GradeSix],
                DocumentTypes[DocumentType.GradeSeven],
                DocumentTypes[DocumentType.GradeEight],
                DocumentTypes[DocumentType.GradeNine]
            };
        }

        public static List<SelectListItem> GetDocumentTypesForStudentGradeSix()
        {
            return new List<SelectListItem>
            {
                DocumentTypes[DocumentType.General],
                DocumentTypes[DocumentType.GradeSix]
            };
        }

        public static List<SelectListItem> GetDocumentTypesForStudentGradeSeven()
        {
            return new List<SelectListItem>
            {
                DocumentTypes[DocumentType.General],
                DocumentTypes[DocumentType.GradeSeven]
            };
        }

        public static List<SelectListItem> GetDocumentTypesForStudentGradeEight()
        {
            return new List<SelectListItem>
            {
                DocumentTypes[DocumentType.General],
                DocumentTypes[DocumentType.GradeEight]
            };
        }

        public static List<SelectListItem> GetDocumentTypesForStudentGradeNine()
        {
            return new List<SelectListItem>
            {
                DocumentTypes[DocumentType.General],
                DocumentTypes[DocumentType.GradeNine]
            };
        }

        public static string GetRoleAccessDocument(DocumentType documentType)
        {
            switch (documentType)
            {
                case DocumentType.GradeSixExam:
                    return RoleName.GradeSixStudent;
                case DocumentType.GradeSevenExam:
                    return RoleName.GradeSevenStudent;
                case DocumentType.GradeEightExam:
                    return RoleName.GradeEightStudent;
                case DocumentType.GradeNineExam:
                    return RoleName.GradeNineStudent;
                default:
                    return null;
            }
        }

        public static DocumentType GetDocumentSuitableForStudent(string gradeStudentRole)
        {
            switch (gradeStudentRole)
            {
                case RoleName.GradeSixStudent:
                    return DocumentType.GradeSix;
                case RoleName.GradeSevenStudent:
                    return DocumentType.GradeSeven;
                case RoleName.GradeEightStudent:
                    return DocumentType.GradeEight;
                case RoleName.GradeNineStudent:
                    return DocumentType.GradeNine;
                default:
                    return DocumentType.General;
            }
        }
    }
}