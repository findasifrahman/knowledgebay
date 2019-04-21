using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KioskNavy.Models
{
    public class AdminSettingsModels
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        
        public string BackImageUrl { get; set; }
        public string FontSize { get; set; }
        public string FontColor { get; set; }
        public string Backcolor { get; set; }
    }
}