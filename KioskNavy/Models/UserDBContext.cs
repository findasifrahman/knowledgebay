using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KioskNavy.Models
{
    public class UserDBContext : DbContext
    {
        static UserDBContext()
        {
            Database.SetInitializer<UserDBContext>(new DropCreateDatabaseIfModelChanges<UserDBContext>());
            //Database.SetInitializer<AccGardeniaDBContext>(new DropCreateDatabaseAlways<AccGardeniaDBContext>());
        }
        public UserDBContext() : base("Userdb")
        {
        }
        public System.Data.Entity.DbSet<KioskNavy.Models.UserConditionModels> UserConditionModels { get; set; }
    }
}