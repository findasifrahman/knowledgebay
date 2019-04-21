using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KioskNavy.Models
{
    public class LearningModels
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Key]
        [Column(Order = 0)]
        [Required]
        public string TopicName { get; set; }
        [Required]
        [Column(Order = 1)]
        public string SubjectName { get; set; }
        [Required]
        [Column(Order = 2)]
        public string subsubject { get; set; }
        public string noofpage { get; set; }
        public string pdfURL { get; set; }
        public string pptURL { get; set; }
        public string vidURL { get; set; }
        public string imgURL { get; set; }
        public string content { get; set; }
        public virtual List<IndexForLearning> indexForLearning { get; set; }
        public string index1 { get; set; }
        public string index2 { get; set; }
        public string index3 { get; set; }
    }
}