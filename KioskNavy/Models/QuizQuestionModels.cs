using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KioskNavy.Models
{
    public class QuizQuestionModels
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Key]
        [Column(Order = 0)]
        [Display(Name = "Question Number")]
        [Required]
        public int QuestionNumber { get; set; }
        [Key]
        [Column(Order = 1)]
        [Required]
        public string QuizName { get; set; }
        [Required]
        [Key]
        [Column(Order = 2)]
        public string Level { get; set; }
        [Required]
        [Key]
        [Column(Order = 3)]
        public string SubSubject { get; set; }
        public string QuestionType { get; set; }
        [Required]
        public string Question { get; set; }
        public string QuestionHighlighted { get; set; }
        [Required]
        public string answer { get; set; }
        public string hint1 { get; set; }
        public string choic1 { get; set; }
        public string choic2 { get; set; }
        public string choic3 { get; set; }
        public string choic4 { get; set; }
        public string choic5 { get; set; }
        public string choic6 { get; set; }
        public string URLchoic1 { get; set; }
        public string URLchoic2 { get; set; }
        public string URLchoic3 { get; set; }
        public string URLchoic4 { get; set; }
        public string URLchoic5 { get; set; }
        public string URLchoic6 { get; set; }
        public bool IsImage { get; set; }
        public bool IsVideo { get; set; }

        public string imageURL { get; set; }
        public string vidURL { get; set; }
        [DataType(DataType.MultilineText)]
        public string furthurInfo { get; set; }
        public string furthurInfoimageURL { get; set; }
        public string furthurInfoVidURL { get; set; }

    }
}