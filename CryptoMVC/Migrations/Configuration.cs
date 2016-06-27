using System.Collections.Generic;
using CryptoMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CryptoMVC.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            InitRoles(context, roleManager);
            InitUsers(context, userManager);
        }

        private static void InitRoles(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            if (context.Roles.Any()) return;
            var roleNames = new List<string>(){
                RoleName.Admin,
                RoleName.Teacher,
                RoleName.Student,

                RoleName.GradeSixStudent,
                RoleName.GradeSevenStudent,
                RoleName.GradeEightStudent,
                RoleName.GradeNineStudent
            };

            foreach (var roleName in roleNames)
            {
                roleManager.Create(new IdentityRole { Name = roleName });
            }

            context.SaveChanges();
        }

        private static void InitUsers(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (context.Users.Any()) return;
            var userAdmin = new ApplicationUser { UserName = "Admin", Email = "admin@cryptomvc.com" };
            var defaultTeacher = new ApplicationUser { UserName = "teacher", Email = "teacher@cryptomvc.com" };
            var defaultStudent = new ApplicationUser { UserName = "student", Email = "student@cryptomvc.com" };
            var defaultStudent2 = new ApplicationUser { UserName = "student2", Email = "student2@cryptomvc.com" };

            var gradeSevenStudent = new ApplicationUser { UserName = "gradeseven", Email = "gradeseven@cryptomvc.com" };
            var gradeEightStudent = new ApplicationUser { UserName = "gradeeight", Email = "gradeseight@cryptomvc.com" };
            var gradeNineStudent = new ApplicationUser { UserName = "gradenine", Email = "gradesnine@cryptomvc.com" };

            userManager.Create(userAdmin, "Aa123!@#");
            userManager.AddToRoles(userAdmin.Id, RoleName.Admin);

            userManager.Create(defaultTeacher, "Aa123!@#");
            userManager.AddToRoles(defaultTeacher.Id, RoleName.Teacher);

            userManager.Create(defaultStudent, "Aa123!@#");
            userManager.AddToRoles(defaultStudent.Id, RoleName.Student, RoleName.GradeSixStudent);

            userManager.Create(defaultStudent2, "Aa123!@#");
            userManager.AddToRoles(defaultStudent2.Id, RoleName.Student, RoleName.GradeSixStudent);

            userManager.Create(gradeSevenStudent, "Aa123!@#");
            userManager.AddToRoles(gradeSevenStudent.Id, RoleName.Student, RoleName.GradeSevenStudent);

            userManager.Create(gradeEightStudent, "Aa123!@#");
            userManager.AddToRoles(gradeEightStudent.Id, RoleName.Student, RoleName.GradeEightStudent);

            userManager.Create(gradeNineStudent, "Aa123!@#");
            userManager.AddToRoles(gradeNineStudent.Id, RoleName.Student, RoleName.GradeNineStudent);

            context.SaveChanges();
        }
    }
}
