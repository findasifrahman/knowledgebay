using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KioskNavy.Models
{
    public class UserConditionModels
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]      
        public int ID { get; set; }
        public string mName { get; set; }
        [Key]
        [Column(Order = 0)]
        public string mCode { get; set; }
        public string mRank { get; set; }
        public string currenLevel { get; set; }
        [Key]
        [Column(Order = 1)]
        public string quizName { get; set; }
        [Key]
        [Column(Order = 2)]
        public string subjectLevel { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SubSubject { get; set; }

        [DataType(DataType.Date)]
        public DateTime CurrentDate { get; set; }
        public string prevlevel { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string allAnswers { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string wrongAnswers { get; set; }
        public string userXP { get; set; }
    }
}