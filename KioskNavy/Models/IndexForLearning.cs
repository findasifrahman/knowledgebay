using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KioskNavy.Models
{
    public class IndexForLearning
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string IndexName { get; set; }
        public string startPage { get; set; }
        public string endPage { get; set; }
        public string TopicName { get; set; }
        public string SubjectName { get; set; }
        public string subsubject { get; set; }
    }
}