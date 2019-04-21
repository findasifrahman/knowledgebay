using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KioskNavy.Models
{
    public class LevelNameModels
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        [Display(Name = "Level Name")]
        public string Level { get; set; }
        public string SubjectName { get; set; }
        public string LevelDescription { get; set; }
        public int certificateTreshhold { get; set; }
    }
}