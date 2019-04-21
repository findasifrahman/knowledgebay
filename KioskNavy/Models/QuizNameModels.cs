using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KioskNavy.Models
{
    public class QuizNameModels
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Key]
        public string SubjectName { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }
        public virtual List<LevelNameModels> LevelDetails { get; set; }
        public virtual List<subSubjectmdels> SubSubject { get; set; }

    }
}