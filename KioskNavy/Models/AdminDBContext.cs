using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KioskNavy.Models
{
    public class AdminDBContext : DbContext
    {
            static AdminDBContext()
            {
                //Database.SetInitializer<AdminDBContext>(new DropCreateDatabaseIfModelChanges<AdminDBContext>());
                //Database.SetInitializer<AccGardeniaDBContext>(new DropCreateDatabaseAlways<AccGardeniaDBContext>());
            }
            public AdminDBContext() : base("Admindb")
            {
            }

        public System.Data.Entity.DbSet<KioskNavy.Models.QuizNameModels> QuizNameModels { get; set; }

        public System.Data.Entity.DbSet<KioskNavy.Models.QuizQuestionModels> QuizQuestionModels { get; set; }

        public System.Data.Entity.DbSet<KioskNavy.Models.AdminSettingsModels> AdminSettingsModels { get; set; }

        public System.Data.Entity.DbSet<KioskNavy.Models.LevelNameModels> LevelNameModels { get; set; }

        public System.Data.Entity.DbSet<KioskNavy.Models.LearningModels> LearningModels { get; set; }

        public System.Data.Entity.DbSet<KioskNavy.Models.subSubjectmdels> subSubjectmdels { get; set; }

        public System.Data.Entity.DbSet<KioskNavy.Models.IndexForLearning> IndexForLearnings { get; set; }
    }
}