using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KioskNavy.Models;
using System.IO;
using PagedList;

namespace KioskNavy.Controllers
{
    public class QuizQuestionModelsController : Controller
    {
        private AdminDBContext db = new AdminDBContext();

        // GET: QuizQuestionModels
        [Authorize]
        public ActionResult Index(string id,string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            ViewBag.QNname = id;
            Session["quiz"] = id;
            ///////////////////////
            ViewBag.CurrentSort = sortOrder;
            ViewBag.QuestionNumberSortParm = sortOrder == "QuestionNumber" ? "QuestionNumber_asc" : "QuestionNumber";
            ViewBag.QuizNameSortParm = sortOrder == "QuizName" ? "QuizName_asc" : "QuizName"; 
            ViewBag.LevelNameSortParm = sortOrder == "LevelName" ? "LevelName_asc" : "LevelName";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var students = from s in db.QuizQuestionModels.Where(x=> x.QuizName == id) select s;
            int ff = 0;
            try
            {
                ff = Convert.ToInt32(searchString);
            }
            catch { }
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.ID == ff || s.QuestionNumber == ff || s.Level.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "QuestionNumber_asc":
                    students = students.OrderBy(s => s.QuestionNumber);
                    break;
                case "QuestionNumber":
                    students = students.OrderByDescending(s => s.QuestionNumber);
                    break;
                case "LevelName_asc":
                    students = students.OrderBy(s => s.Level);
                    break;
                case "LevelName":
                    students = students.OrderByDescending(s => s.Level);
                    break;
                default:  // Name ascending 
                    students = students.OrderByDescending(s => s.QuestionNumber);
                    break;
            }
            int pageSize = 300;
            int pageNumber = (page ?? 1);
            /////////////////////////////////////////////////////////
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            return View(students.Take(600).ToPagedList(pageNumber, pageSize));
            ////////////////////////////////////////////////////////
            return View(db.QuizQuestionModels.ToList());
        }

        // GET: QuizQuestionModels/Details/5
        public ActionResult Details(int id,string qname, string subsubject, string level)
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            if (qname == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizQuestionModels quizQuestionModels = db.QuizQuestionModels.FirstOrDefault(m => m.QuestionNumber == id && m.QuizName == qname && m.SubSubject == subsubject && m.Level == level);
            if (quizQuestionModels == null)
            {
                return HttpNotFound();
            }
            return View(quizQuestionModels);
        }

        // GET: QuizQuestionModels/Create
        [Authorize]
        public ActionResult Create(string id,string type)
        {
            if(id== null)
            {
                return View("~/Views/admin/Index.cshtml");
            }
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            ViewBag.type = type;
            ViewBag.levels = new SelectList(db.LevelNameModels.Where(x=> x.SubjectName == id).ToList(), "Level", "Level");
            var list = db.subSubjectmdels.Where(x => x.SubjectName == id).FirstOrDefault();
            if (list == null)
            {
                List<SelectListItem> newList = new List<SelectListItem>();
                SelectListItem selListItem = new SelectListItem() {Value = "SubSubject", Text = "SubSubject" };
                //Add select list item to list of selectlistitems
                newList.Add(selListItem);
                var ll = new List<string>();
                ll.Add("nosub");
                ViewBag.subSubject = new SelectList(ll);
            }
            else {
                ViewBag.subSubject = new SelectList(db.subSubjectmdels.Where(x => x.SubjectName == id).ToList(), "SubSubject", "SubSubject");
            }
            int oo = db.QuizQuestionModels.Where(x=> x.QuizName == id).Max(p => (int?)p.QuestionNumber) ?? 0;
 
            ViewBag.maxid = (oo + 1).ToString();

            ViewBag.Qname = id;
            return View();
        }

        // POST: QuizQuestionModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QuestionNumber,QuizName,Level,SubSubject,QuestionType,Question,QuestionHighlighted,answer,hint1,hint2,choic1,choic2,choic3,choic4,choic5,choic6,URLchoic1,URLchoic2,,URLchoic3,URLchoic4,URLchoic5,URLchoic6,IsImage,IsVideo,imageURL,vidURL,furthurInfo,furthurInfoimageURL,furthurInfoVidURL")] QuizQuestionModels quizQuestionModels, 
            HttpPostedFileBase vfile, HttpPostedFileBase file, HttpPostedFileBase furfile, HttpPostedFileBase furvfile, HttpPostedFileBase imgchoice1, HttpPostedFileBase imgchoice2, HttpPostedFileBase imgchoice3, HttpPostedFileBase imgchoice4, HttpPostedFileBase imgchoice5, HttpPostedFileBase imgchoice6)
        {
            ViewBag.Qname = quizQuestionModels.QuizName;

            int oo = db.QuizQuestionModels.Where(x => x.QuizName == quizQuestionModels.QuizName && x.Level == quizQuestionModels.Level).Max(p => (int?)p.QuestionNumber) ?? 0;

            ViewBag.maxid = (oo + 1).ToString();

            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            ViewBag.levels = new SelectList(db.LevelNameModels.Where(x => x.SubjectName == quizQuestionModels.QuizName).ToList(), "Level", "Level");
           // ViewBag.levels = new SelectList(db.LevelNameModels.Where(x => x.Level == quizQuestionModels.Level).ToList(), "Level", "Level");
            var list = db.subSubjectmdels.Where(x => x.SubjectName == quizQuestionModels.QuizName).FirstOrDefault();
            if (list == null)
            {
                List<SelectListItem> newList = new List<SelectListItem>();
                SelectListItem selListItem = new SelectListItem() { Value = "SubSubject", Text = "nosub" };
                //Add select list item to list of selectlistitems
                newList.Add(selListItem);
                var ll = new List<string>();
                ll.Add("nosub");
                ViewBag.subSubject = new SelectList(ll);
            }
            else
            {
                ViewBag.subSubject = new SelectList(db.subSubjectmdels.Where(x => x.SubjectName == quizQuestionModels.QuizName).ToList(), "SubSubject", "SubSubject");
            }
            if (ModelState.IsValid) { 
                string subPath = "~/images/" + quizQuestionModels.QuizName; // your code goes here
                string vid_subPath = "~/videos/" + quizQuestionModels.QuizName; // your code goes here
                bool vid_exists = System.IO.Directory.Exists(Server.MapPath(vid_subPath));
                if (!vid_exists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(vid_subPath));

                bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));
                if (!exists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
                if (vfile != null)
                {
                    System.Diagnostics.Debug.WriteLine("video file not ");
                    var fileName = Path.GetFileName(vfile.FileName);

                    System.Diagnostics.Debug.WriteLine("Filename video -- " + fileName);

                    var ext = Path.GetExtension(vfile.FileName); //getting the extension(ex-.jpg)  

                    string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                    string myfile = quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext; //appending the name with id  
                                                                                                                                                               // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/videos/" + quizQuestionModels.QuizName), myfile);

                    quizQuestionModels.vidURL = Path.Combine("~/videos/" + quizQuestionModels.QuizName, myfile);
                    vfile.SaveAs(path);

                }
                if (file != null)
                {
                    System.Diagnostics.Debug.WriteLine("File nt null");
                    var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
                    var fileName = Path.GetFileName(file.FileName);

                    System.Diagnostics.Debug.WriteLine("Filename baler -- " + fileName);

                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext; //appending the name with id  
                                                                                                                                                                   // store the file inside ~/project folder(Img)  
                        var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);

                        System.Diagnostics.Debug.WriteLine(" after path");
                        quizQuestionModels.imageURL = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                        file.SaveAs(path);
                    }
                }
                /////////////////////
                if (furvfile != null)
                {
                    System.Diagnostics.Debug.WriteLine("video file not emptyfljdzsf.jhds");
                    var fileName = Path.GetFileName(furvfile.FileName);

                    System.Diagnostics.Debug.WriteLine("Filename video -- " + fileName);

                    var ext = Path.GetExtension(furvfile.FileName); //getting the extension(ex-.jpg)  

                    string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                    string myfile = "fur" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext; //appending the name with id  
                                                                                                                                                                       // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/videos/" + quizQuestionModels.QuizName), myfile);

                    quizQuestionModels.furthurInfoVidURL = Path.Combine("~/videos/" + quizQuestionModels.QuizName, myfile);
                    furvfile.SaveAs(path);

                }
                if (furfile != null)
                {
                    System.Diagnostics.Debug.WriteLine("File nt null");
                    var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
                    var fileName = Path.GetFileName(furfile.FileName);

                    System.Diagnostics.Debug.WriteLine("Filename baler -- " + fileName);

                    var ext = Path.GetExtension(furfile.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = "fur" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext; //appending the name with id  
                                                                                                                                                                           // store the file inside ~/project folder(Img)  
                        var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);

                        System.Diagnostics.Debug.WriteLine(" after path");
                        quizQuestionModels.furthurInfoimageURL = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                        furfile.SaveAs(path);
                    }
                }
                ////////////////////////////////////
                if (imgchoice1 != null)
                {
                    var ext = Path.GetExtension(imgchoice1.FileName); //getting the extension(ex-.jpg)  
                    string myfile = "imgc1" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext;                                                                                                                                                         // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.URLchoic1 = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                    imgchoice1.SaveAs(path);
                }
                if (imgchoice2 != null)
                {
                    var ext = Path.GetExtension(imgchoice2.FileName); //getting the extension(ex-.jpg)  
                    string myfile = "imgc2" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext;                                                                                                                                                         // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.URLchoic2 = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                    imgchoice2.SaveAs(path);
                }
                if (imgchoice3 != null)
                {
                    var ext = Path.GetExtension(imgchoice3.FileName); //getting the extension(ex-.jpg)  
                    string myfile = "imgc3" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext;                                                                                                                                                         // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.URLchoic3 = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                    imgchoice3.SaveAs(path);
                }
                if (imgchoice4 != null)
                {
                    var ext = Path.GetExtension(imgchoice4.FileName); //getting the extension(ex-.jpg)  
                    string myfile = "imgc4" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext;                                                                                                                                                         // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.URLchoic4 = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                    imgchoice4.SaveAs(path);
                }
                if (imgchoice5 != null)
                {
                    var ext = Path.GetExtension(imgchoice5.FileName); //getting the extension(ex-.jpg)  
                    string myfile = "imgc5" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext;                                                                                                                                                         // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.URLchoic5 = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                    imgchoice5.SaveAs(path);
                }
                if (imgchoice6 != null)
                {
                    var ext = Path.GetExtension(imgchoice6.FileName); //getting the extension(ex-.jpg)  
                    string myfile = "imgc6" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext;                                                                                                                                                         // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.URLchoic6 = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                    imgchoice6.SaveAs(path);
                }
                ////////////////////////////////////

                ///model
                db.QuizQuestionModels.Add(quizQuestionModels);
                db.SaveChanges();
                ///////////////////////////////////////////////
                List<QuizQuestionModels> quizQuestionModels1 = db.QuizQuestionModels.Where(x => x.QuizName == quizQuestionModels.QuizName).ToList();
                ViewBag.QNname = quizQuestionModels.QuizName;
                return RedirectToAction("Index", new { id = quizQuestionModels.QuizName });
            }
            return View(quizQuestionModels);
        }

        // GET: QuizQuestionModels/Edit/5
        public ActionResult Edit(int id, string qname, string subsubject, string level)
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            if (qname == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ////////////////
            
            ViewBag.levels = new SelectList(db.LevelNameModels.Where(x => x.SubjectName == qname).ToList(), "Level", "Level");
            var list = db.subSubjectmdels.Where(x => x.SubjectName == qname).FirstOrDefault();
            if (list == null)
            {
                List<SelectListItem> newList = new List<SelectListItem>();
                SelectListItem selListItem = new SelectListItem() { Value = "SubSubject", Text = "SubSubject" };
                //Add select list item to list of selectlistitems
                newList.Add(selListItem);
                var ll = new List<string>();
                ll.Add("nosub");
                ViewBag.subSubject = new SelectList(ll);
            }
            else
            {
                ViewBag.subSubject = new SelectList(db.subSubjectmdels.Where(x => x.SubjectName == qname).ToList(), "SubSubject", "SubSubject");
            }
            int oo = db.QuizQuestionModels.Where(x => x.QuizName == qname).Max(p => (int?)p.QuestionNumber) ?? 0;

            ViewBag.maxid = (oo + 1).ToString();

            ViewBag.Qname = id;
            ///////////////

            QuizQuestionModels quizQuestionModels = db.QuizQuestionModels.FirstOrDefault(m => m.QuestionNumber == id && m.QuizName == qname && m.SubSubject == subsubject && m.Level == level);
            if (quizQuestionModels == null)
            {
                return HttpNotFound();
            }
            return View(quizQuestionModels);
        }

        // POST: QuizQuestionModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QuestionNumber,QuizName,Level,SubSubject,QuestionType,Question,QuestionHighlighted,answer,hint1,hint2,choic1,choic2,choic3,choic4,choic5,choic6,URLchoic1,URLchoic2,,URLchoic3,URLchoic4,URLchoic5,URLchoic6,IsImage,IsVideo,imageURL,vidURL,furthurInfo,furthurInfoimageURL,furthurInfoVidURL")] QuizQuestionModels quizQuestionModels,
            HttpPostedFileBase vfile, HttpPostedFileBase file, HttpPostedFileBase furfile, HttpPostedFileBase furvfile, HttpPostedFileBase imgchoice1, HttpPostedFileBase imgchoice2, HttpPostedFileBase imgchoice3, HttpPostedFileBase imgchoice4, HttpPostedFileBase imgchoice5, HttpPostedFileBase imgchoice6)
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            //////////////////////////////////////////////////////
            ViewBag.levels = new SelectList(db.LevelNameModels.Where(x => x.SubjectName == quizQuestionModels.QuizName).ToList(), "Level", "Level");
            // ViewBag.levels = new SelectList(db.LevelNameModels.Where(x => x.Level == quizQuestionModels.Level).ToList(), "Level", "Level");
            var list = db.subSubjectmdels.Where(x => x.SubjectName == quizQuestionModels.QuizName).FirstOrDefault();
            if (list == null)
            {
                List<SelectListItem> newList = new List<SelectListItem>();
                SelectListItem selListItem = new SelectListItem() { Value = "SubSubject", Text = "nosub" };
                //Add select list item to list of selectlistitems
                newList.Add(selListItem);
                var ll = new List<string>();
                ll.Add("nosub");
                ViewBag.subSubject = new SelectList(ll);
            }
            else
            {
                ViewBag.subSubject = new SelectList(db.subSubjectmdels.Where(x => x.SubjectName == quizQuestionModels.QuizName).ToList(), "SubSubject", "SubSubject");
            }
            if (ModelState.IsValid)
            {
                var Prevpathvid = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber &&
                            m.QuizName == quizQuestionModels.QuizName && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level)
                            .Select(m => m.vidURL).FirstOrDefault()));

                quizQuestionModels.vidURL = db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber && m.QuizName == quizQuestionModels.QuizName
                && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level).Select(m => m.vidURL).FirstOrDefault();

                if (vfile != null)
                {                  
                    if (System.IO.File.Exists(Prevpathvid))
                    {
                        System.IO.File.Delete(Prevpathvid);
                    }

                    var fileName = Path.GetFileName(vfile.FileName);
                    var ext = Path.GetExtension(vfile.FileName); 
                    string name = Path.GetFileNameWithoutExtension(fileName); 
                    string myfile = quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext; //appending the name with id                                                                                                                                                                // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/videos/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.vidURL = Path.Combine("~/videos/" + quizQuestionModels.QuizName, myfile);
                    vfile.SaveAs(path);

                }
               var Prevpathimg = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber &&
                            m.QuizName == quizQuestionModels.QuizName && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level)
                            .Select(m => m.imageURL).FirstOrDefault()));

                quizQuestionModels.imageURL = db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber && m.QuizName == quizQuestionModels.QuizName
                && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level).Select(m => m.imageURL).FirstOrDefault();

                if (file != null)
                {
                    
                    if (System.IO.File.Exists(Prevpathimg))
                    {
                        System.IO.File.Delete(Prevpathimg);
                    }
                    
                    var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
                    var fileName = Path.GetFileName(file.FileName);

                    System.Diagnostics.Debug.WriteLine("Filename baler -- " + fileName);

                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext; //appending the name with id  
                                                                                                                                                                   // store the file inside ~/project folder(Img)  
                        var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);

                        System.Diagnostics.Debug.WriteLine(" after path");
                        quizQuestionModels.imageURL = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                        file.SaveAs(path);
                    }
                }
                /////////////////////
                var Prevpathfurvid = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber &&
                            m.QuizName == quizQuestionModels.QuizName && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level)
                            .Select(m => m.furthurInfoVidURL).FirstOrDefault()));

                quizQuestionModels.furthurInfoVidURL = db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber && m.QuizName == quizQuestionModels.QuizName
                && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level).Select(m => m.furthurInfoVidURL).FirstOrDefault();

                if (furvfile != null)
                {
                    if (System.IO.File.Exists(Prevpathfurvid))
                    {
                        System.IO.File.Delete(Prevpathfurvid);
                    }
                    var fileName = Path.GetFileName(furvfile.FileName);

                    var ext = Path.GetExtension(furvfile.FileName); //getting the extension(ex-.jpg)  

                    string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                    string myfile = "fur" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext; //appending the name with id  
                                                                                                                                                                       // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/videos/" + quizQuestionModels.QuizName), myfile);

                    quizQuestionModels.furthurInfoVidURL = Path.Combine("~/videos/" + quizQuestionModels.QuizName, myfile);
                    furvfile.SaveAs(path);

                }
                /////////////////////
                var Prevpathfurimg = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber &&
                            m.QuizName == quizQuestionModels.QuizName && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level)
                            .Select(m => m.furthurInfoimageURL).FirstOrDefault()));

                quizQuestionModels.furthurInfoimageURL = db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber && m.QuizName == quizQuestionModels.QuizName
                && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level).Select(m => m.furthurInfoimageURL).FirstOrDefault();

                if (furfile != null)
                {
                    if (System.IO.File.Exists(Prevpathfurimg))
                    {
                        System.IO.File.Delete(Prevpathfurimg);
                    }
                    var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
                    var fileName = Path.GetFileName(furfile.FileName);

                    System.Diagnostics.Debug.WriteLine("Filename baler -- " + fileName);

                    var ext = Path.GetExtension(furfile.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = "fur" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext; //appending the name with id  
                                                                                                                                                                           // store the file inside ~/project folder(Img)  
                        var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);

                        System.Diagnostics.Debug.WriteLine(" after path");
                        quizQuestionModels.furthurInfoimageURL = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                        furfile.SaveAs(path);
                    }
                }
                ////////////////////////////////////
                var Prevpathimgc1 = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber &&
                            m.QuizName == quizQuestionModels.QuizName && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level)
                            .Select(m => m.URLchoic1).FirstOrDefault()));

                quizQuestionModels.URLchoic1 = db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber && m.QuizName == quizQuestionModels.QuizName
                && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level).Select(m => m.URLchoic1).FirstOrDefault();

                if (imgchoice1 != null)
                {
                    if (System.IO.File.Exists(Prevpathimgc1))
                    {
                        System.IO.File.Delete(Prevpathimgc1);
                    }
                    var ext = Path.GetExtension(imgchoice1.FileName); //getting the extension(ex-.jpg)  
                    string myfile = "imgc1" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext;                                                                                                                                                         // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.URLchoic1 = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                    imgchoice1.SaveAs(path);
                }
                var Prevpathimgc2 = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber &&
                            m.QuizName == quizQuestionModels.QuizName && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level)
                            .Select(m => m.URLchoic2).FirstOrDefault()));

                quizQuestionModels.URLchoic2 = db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber && m.QuizName == quizQuestionModels.QuizName
                && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level).Select(m => m.URLchoic2).FirstOrDefault();

                if (imgchoice2 != null)
                {
                    if (System.IO.File.Exists(Prevpathimgc2))
                    {
                        System.IO.File.Delete(Prevpathimgc2);
                    }
                    var ext = Path.GetExtension(imgchoice2.FileName); //getting the extension(ex-.jpg)  
                    string myfile = "imgc2" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext;                                                                                                                                                         // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.URLchoic2 = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                    imgchoice2.SaveAs(path);
                }
                var Prevpathimgc3 = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber &&
                            m.QuizName == quizQuestionModels.QuizName && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level)
                            .Select(m => m.URLchoic3).FirstOrDefault()));

                quizQuestionModels.URLchoic3 = db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber && m.QuizName == quizQuestionModels.QuizName
                && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level).Select(m => m.URLchoic3).FirstOrDefault();

                if (imgchoice3 != null)
                {
                    if (System.IO.File.Exists(Prevpathimgc3))
                    {
                        System.IO.File.Delete(Prevpathimgc3);
                    }
                    var ext = Path.GetExtension(imgchoice3.FileName); //getting the extension(ex-.jpg)  
                    string myfile = "imgc3" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext;                                                                                                                                                         // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.URLchoic3 = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                    imgchoice3.SaveAs(path);
                }
                var Prevpathimgc4 = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber &&
                            m.QuizName == quizQuestionModels.QuizName && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level)
                            .Select(m => m.URLchoic4).FirstOrDefault()));

                quizQuestionModels.URLchoic4 = db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber && m.QuizName == quizQuestionModels.QuizName
                && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level).Select(m => m.URLchoic4).FirstOrDefault();

                if (imgchoice4 != null)
                {
                    if (System.IO.File.Exists(Prevpathimgc4))
                    {
                        System.IO.File.Delete(Prevpathimgc4);
                    }
                    var ext = Path.GetExtension(imgchoice4.FileName); //getting the extension(ex-.jpg)  
                    string myfile = "imgc4" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext;                                                                                                                                                         // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.URLchoic4 = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                    imgchoice4.SaveAs(path);
                }
                var Prevpathimgc5 = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber &&
                            m.QuizName == quizQuestionModels.QuizName && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level)
                            .Select(m => m.URLchoic5).FirstOrDefault()));

                quizQuestionModels.URLchoic5 = db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber && m.QuizName == quizQuestionModels.QuizName
                && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level).Select(m => m.URLchoic5).FirstOrDefault();

                if (imgchoice5 != null)
                {
                    if (System.IO.File.Exists(Prevpathimgc5))
                    {
                        System.IO.File.Delete(Prevpathimgc5);
                    }
                    var ext = Path.GetExtension(imgchoice5.FileName); //getting the extension(ex-.jpg)  
                    string myfile = "imgc5" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext;                                                                                                                                                         // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.URLchoic5 = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                    imgchoice5.SaveAs(path);
                }
                var Prevpathimgc6 = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber &&
                            m.QuizName == quizQuestionModels.QuizName && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level)
                            .Select(m => m.URLchoic6).FirstOrDefault()));

                quizQuestionModels.URLchoic6 = db.QuizQuestionModels.Where(m => m.QuestionNumber == quizQuestionModels.QuestionNumber && m.QuizName == quizQuestionModels.QuizName
                && m.SubSubject == quizQuestionModels.SubSubject && m.Level == quizQuestionModels.Level).Select(m => m.URLchoic6).FirstOrDefault();

                if (imgchoice6 != null)
                {
                    if (System.IO.File.Exists(Prevpathimgc6))
                    {
                        System.IO.File.Delete(Prevpathimgc6);
                    }
                    var ext = Path.GetExtension(imgchoice6.FileName); //getting the extension(ex-.jpg)  
                    string myfile = "imgc6" + quizQuestionModels.SubSubject + "_" + quizQuestionModels.Level + "_" + quizQuestionModels.QuestionNumber.ToString() + ext;                                                                                                                                                         // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/images/" + quizQuestionModels.QuizName), myfile);
                    quizQuestionModels.URLchoic6 = Path.Combine("~/images/" + quizQuestionModels.QuizName, myfile);
                    imgchoice6.SaveAs(path);
                }
            }
            ////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////
            if (ModelState.IsValid)
            {
                db.Entry(quizQuestionModels).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", new { id = quizQuestionModels.QuizName });
                //return RedirectToAction("Index");
            }
            return View(quizQuestionModels);
        }
        [HttpPost]
        public ActionResult GetQues(string level,string sub,string quiz)
        {

            if (level != null && sub!=null)
            {
                int oo = db.QuizQuestionModels.Where(x => x.QuizName == quiz && x.Level == level && x.SubSubject==sub).Max(p => (int?)p.QuestionNumber) ?? 0;

                ViewBag.maxid = (oo + 1).ToString();
                int qn = oo + 1;

                return Json(new { Success = "true", Data = new { qn = qn } });
            }
            return Json(new { Success = "false" });
        }
        // GET: QuizQuestionModels/Delete/5
        [Authorize]
        public ActionResult Delete(int? id,string qname,string subsubject, string level)
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizQuestionModels quizQuestionModels = db.QuizQuestionModels.FirstOrDefault(m => m.QuestionNumber == id && m.QuizName == qname);
            if (quizQuestionModels == null)
            {
                return HttpNotFound();
            }
            return View(quizQuestionModels);
        }

        // POST: QuizQuestionModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id,string qname,string subsubject,string level)
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            var path = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == id && m.QuizName == qname && m.Level== level && m.SubSubject== subsubject).Select(m => m.imageURL).FirstOrDefault()));
            var vpath = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == id && m.QuizName == qname && m.Level == level && m.SubSubject == subsubject).Select(m => m.vidURL).FirstOrDefault()));

            var furpath = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == id && m.QuizName == qname && m.Level == level && m.SubSubject == subsubject).Select(m => m.furthurInfoimageURL).FirstOrDefault()));
            var furvpath = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == id && m.QuizName == qname && m.Level == level && m.SubSubject == subsubject).Select(m => m.furthurInfoVidURL).FirstOrDefault()));

            var imc1 = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == id && m.QuizName == qname && m.Level == level && m.SubSubject == subsubject).Select(m => m.URLchoic1).FirstOrDefault()));
            var imc2 = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == id && m.QuizName == qname && m.Level == level && m.SubSubject == subsubject).Select(m => m.URLchoic2).FirstOrDefault()));
            var imc3 = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == id && m.QuizName == qname && m.Level == level && m.SubSubject == subsubject).Select(m => m.URLchoic3).FirstOrDefault()));
            var imc4 = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == id && m.QuizName == qname && m.Level == level && m.SubSubject == subsubject).Select(m => m.URLchoic4).FirstOrDefault()));
            var imc5 = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == id && m.QuizName == qname && m.Level == level && m.SubSubject == subsubject).Select(m => m.URLchoic5).FirstOrDefault()));
            var imc6 = Path.Combine(Server.MapPath(db.QuizQuestionModels.Where(m => m.QuestionNumber == id && m.QuizName == qname && m.Level == level && m.SubSubject == subsubject).Select(m => m.URLchoic6).FirstOrDefault()));
            if (System.IO.File.Exists(imc1)){System.IO.File.Delete(imc1);}
            if (System.IO.File.Exists(imc2)) { System.IO.File.Delete(imc2); }
            if (System.IO.File.Exists(imc3)) { System.IO.File.Delete(imc3); }
            if (System.IO.File.Exists(imc4)) { System.IO.File.Delete(imc4); }
            if (System.IO.File.Exists(imc5)) { System.IO.File.Delete(imc5); }
            if (System.IO.File.Exists(imc6)) { System.IO.File.Delete(imc6); }

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            if (System.IO.File.Exists(vpath))
            {
                System.IO.File.Delete(vpath);
            }
            if (System.IO.File.Exists(furpath))
            {
                System.IO.File.Delete(furpath);
            }
            if (System.IO.File.Exists(furvpath))
            {
                System.IO.File.Delete(furvpath);
            }
            QuizQuestionModels itemModels = db.QuizQuestionModels.FirstOrDefault(m => m.QuestionNumber == id && m.QuizName == qname && m.Level == level && m.SubSubject == subsubject);//Find(id);
            db.QuizQuestionModels.Remove(itemModels);
            db.SaveChanges();
            ViewBag.QNname = id;
            return RedirectToAction("Index", new { id = qname });//View("~/Views/QuizQuestionModels/Index.cshtml",db.QuizQuestionModels.ToList());
            ////////////////////////////////////////////////
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
