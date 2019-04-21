using KioskNavy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace KioskNavy.Controllers
{
    public class UserLearningController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        // GET: UserLearning
        public ActionResult Index(string name,string code,string rank)
        {
            ViewBag.name = name;
            ViewBag.rank = rank;
            ViewBag.code = code;
            return View(db.QuizNameModels.ToList());
        }
        public ActionResult IndexSecondScreen(string name, string rank)
        {
            ViewBag.name = name;
            ViewBag.rank = rank;
            return View();
        }
        public ActionResult subsubject(string qname,string name, string code, string rank)
        {
            ViewBag.qname = qname;
            ViewBag.name = name;
            ViewBag.rank = rank;
            ViewBag.code = code;
            return View(db.subSubjectmdels.Where(x=> x.SubjectName == qname).ToList());
        }
        public ActionResult learninglevel(string QuizName)
        {
            ViewBag.Quizname = QuizName;
            return View(db.LevelNameModels.ToList());
        }
        public ActionResult alllearning(string QuizName,string subsubject,string name,string code,string rank)
        {
            ViewBag.qname = QuizName;
            ViewBag.name = name;
            ViewBag.rank = rank;
            ViewBag.code = code;
            ViewBag.subsubject = subsubject;
           
            return View(db.LearningModels.Where(x=> x.SubjectName == QuizName && x.subsubject == subsubject).ToList());
        }
        [HttpPost]
        public ActionResult partialIndex(string subject, string sub, string topic)
        {

            //System.Diagnostics.Debug.WriteLine("-------------------------" +subject + "------------------ -----" +sub +"----" + topic);
            var indexlist = db.IndexForLearnings.Where(x => x.SubjectName == subject && x.subsubject == sub && x.TopicName == topic);
            var pdfurl = db.LearningModels.Where(x => x.SubjectName == subject && x.subsubject == sub && x.TopicName == topic).Select(x=> x.pdfURL).FirstOrDefault();
            ViewBag.pages = db.LearningModels.Where(x => x.SubjectName == subject && x.subsubject == sub && x.TopicName == topic).Select(x => x.noofpage).FirstOrDefault();
            //var path = VirtualPathUtility.ToAbsolute(pdfurl);
            //var url = new Uri(Request.Url, path).AbsoluteUri;
            ViewBag.pdfurl = pdfurl;

            System.Diagnostics.Debug.WriteLine(pdfurl);
            return View(indexlist);
        }
        [HttpPost]
        public ActionResult gettype(string subject, string sub, string topic)
        {

            System.Diagnostics.Debug.WriteLine("gettype-------------------------" + subject + "------------------ -----" + sub + "----" + topic);
            string pdfyes = db.LearningModels.Where(x => x.SubjectName == subject && x.subsubject == sub && x.TopicName == topic).Select(x => x.pdfURL).FirstOrDefault();
            string pptyes = db.LearningModels.Where(x => x.SubjectName == subject && x.subsubject == sub && x.TopicName == topic).Select(x => x.pptURL).FirstOrDefault();
            string vid = db.LearningModels.Where(x => x.SubjectName == subject && x.subsubject == sub && x.TopicName == topic).Select(x => x.vidURL).FirstOrDefault();
            string imgyes = db.LearningModels.Where(x => x.SubjectName == subject && x.subsubject == sub && x.TopicName == topic).Select(x => x.imgURL).FirstOrDefault();
            string cont = db.LearningModels.Where(x => x.SubjectName == subject && x.subsubject == sub && x.TopicName == topic).Select(x => x.content).FirstOrDefault();
            string pages = db.LearningModels.Where(x => x.SubjectName == subject && x.subsubject == sub && x.TopicName == topic).Select(x => x.noofpage).FirstOrDefault();

            if (pdfyes != null)
            {
                return Json(new { Success = "true", Data = new { type = "pdf",url = pdfyes,pages = pages} });
            }
            else if (pptyes != null)
            {
                return Json(new { Success = "true", Data = new { type = "ppt",url = pptyes } });
            }
            else if (vid != null)
            {
                return Json(new { Success = "true", Data = new { type = "vid",url = vid } });
            }
            else if (imgyes != null)
            {
                return Json(new { Success = "true", Data = new { type = "img",url = imgyes } });
            }
            else if(cont != null)
            {
                return Json(new { Success = "true", Data = new { type = "con",url = cont } });
            }
            else
            {
                return Json(new { Success = "true", Data = new { type = "nothing", url = cont } });
            }
        }
        public ActionResult viewpdf(string subject,string subsubject, string topic,string name,string code,string rank)
        {
            ViewBag.name = name;
            ViewBag.rank = rank;
            ViewBag.code = code;

            string pdfurl = db.LearningModels.Where(x => x.SubjectName == subject && x.subsubject == subsubject && x.TopicName == topic).Select(x => x.pdfURL).First();
            string  vidurl = db.LearningModels.Where(x => x.SubjectName == subject && x.subsubject == subsubject && x.TopicName == topic).Select(x => x.vidURL).First();
            string  imgurl = db.LearningModels.Where(x => x.SubjectName == subject&& x.subsubject == subsubject && x.TopicName == topic).Select(x => x.imgURL).First();
            string ppturl = db.LearningModels.Where(x => x.SubjectName == subject && x.subsubject == subsubject && x.TopicName == topic).Select(x => x.pptURL).First();
            ViewBag.pdfurl = pdfurl;
            if (pdfurl != null)
            {
                var path = VirtualPathUtility.ToAbsolute(pdfurl);
                var url = new Uri(Request.Url, path).AbsoluteUri;
                ViewBag.pdf = url;//pdfurl;//new System.Uri(System.Web.UI.Page.Request.Url, (pdfurl)).AbsoluteUri;//pdfurl;
                return View();
                //return File(pdfurl,"application/pdf"); 
                //return File(pdfurl, "application/vnd.ms-powerpoint");
            }
            else if(ppturl != null)
            {
                var path = VirtualPathUtility.ToAbsolute(ppturl);
                var url = new Uri(Request.Url, path).AbsoluteUri;
                ViewBag.pdf = url;//pdfurl;//new System.Uri(System.Web.UI.Page.Request.Url, (pdfurl)).AbsoluteUri;//pdfurl;
                ViewBag.ppturl = ppturl;
                return Redirect("pptview?ppturl=" + ppturl);
                //return File(ppturl, "application/vnd.ms-powerpoint");
            }
            else
            {
                return View("otherlearning",db.LearningModels.Where(x=> x.SubjectName == subject && x.subsubject == subsubject && x.TopicName == topic).FirstOrDefault());
            }
        }
        public ActionResult otherlearning(string subject, string subsubject, string topic)
        {
            var nn = db.LearningModels.Where(x => x.SubjectName == subject && x.subsubject == subsubject && x.TopicName == topic).FirstOrDefault();
            return View(nn);
        }
        public ActionResult vidview(string vidurl)
        {
            ViewBag.vidurl = vidurl;
            return View();
        }
        public ActionResult pptView(string ppturl)
        {
            ViewBag.ppturl = ppturl;

            var path = VirtualPathUtility.ToAbsolute(ppturl);
            var url = new Uri(Request.Url, path).AbsoluteUri;

            return File(ppturl, "application/vnd.openxmlformats-officedocument.presentationml.slideshow");
            //return File(ppturl, "application/vnd.ms-powerpoint");
        }
        public ActionResult PdfIndexPop()
        {
            return View();
        }
        public ActionResult viewpdfpopup(string pdfurl, string pages,string sp,string ep)
        {
            ViewBag.height = (Convert.ToUInt32(pages) * 800) +  "px";
            var path = VirtualPathUtility.ToAbsolute(pdfurl);
            var url = new Uri(Request.Url, path).AbsoluteUri;
            ViewBag.pdf = url;//pdfurl;//new System.Uri(System.Web.UI.Page.Request.Url, (pdfurl)).AbsoluteUri;//pdfurl;
            ViewBag.sp = sp;
            ViewBag.ep = ep;
            return View();

            //return File(pdfurl, "application/pdf");

        }
        public ActionResult viewpptpopup(string ppturl)
        {
            var path = VirtualPathUtility.ToAbsolute(ppturl);
            var url = new Uri(Request.Url, path).AbsoluteUri;
            ViewBag.ppt = url;
            return File(ppturl, "application/vnd.openxmlformats-officedocument.presentationml.slideshow");

        }
        public ActionResult otherwithvid(string url)
        {
            ViewBag.vidurl = url;
            return View();
        }
        public ActionResult otherwithimg(string url)
        {
            ViewBag.imgurl = url;
            return View();
        }
        public ActionResult otheronlytext(string text)
        {
            ViewBag.text = text;
            return View();
        }
        public ActionResult tiew()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AutoComplete(string Prefix)
        {
            var customers = (from customer in db.LearningModels
                             where customer.TopicName.StartsWith(Prefix)
                             select new
                             {
                                 label = customer.TopicName,
                                 ssubject = customer.SubjectName,
                                 ssubsubject = customer.subsubject
                             }).ToList();//db.LearningModels.SelectMany(x => x.TopicName).ToList();
            return Json(customers);
        }

    }
}