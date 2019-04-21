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
using System.Data.Entity.Validation;

namespace KioskNavy.Controllers
{
    public class LearningModelsController : Controller
    {
        private AdminDBContext db = new AdminDBContext();

        // GET: LearningModels
        public ActionResult Index()
        {
            return View(db.LearningModels.ToList());
        }

        // GET: LearningModels/Details/5
        public ActionResult Details(string id,string subject,string subsubject)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LearningModels learningModels = db.LearningModels.Where(x => x.TopicName == id && x.SubjectName == subject && x.subsubject == subsubject).FirstOrDefault();
            if (learningModels == null)
            {
                return HttpNotFound();
            }
            return View(learningModels);
        }

        // GET: LearningModels/Create
        public ActionResult Create(string type)
        {
            ViewBag.type = type;
            ViewBag.SubjectName = new SelectList(db.QuizNameModels.ToList(), "SubjectName", "SubjectName");
            ViewBag.subsubject = new SelectList(db.subSubjectmdels.ToList(), "subsubject", "subsubject");
            ViewBag.LevelName = new SelectList(db.LevelNameModels.ToList(), "Level", "Level");
            return View();
        }
        public JsonResult subsubjectlist(string val)
        {
            List<String> ll = new List<string>();
            var list = db.subSubjectmdels.Where(x => x.SubjectName == val);//.SelectMany(x=>x.ItemName);
            return Json(list, JsonRequestBehavior.AllowGet);

        }
        // POST: LearningModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "TopicName,ID,SubjectName,subsubject,noofpage,pptURL,pdfURL,vidURL,imgURL,content,indexForLearning")] LearningModels learningModels,
            HttpPostedFileBase pdffile,HttpPostedFileBase vidfile,HttpPostedFileBase imgfile, HttpPostedFileBase pptfile)
        {

            ViewBag.SubjectName = new SelectList(db.QuizNameModels.ToList(), "SubjectName", "SubjectName");
            ViewBag.subsubject = new SelectList(db.subSubjectmdels.Where(x=> x.SubjectName == learningModels.SubjectName).ToList(), "subsubject", "subsubject");
            ViewBag.LevelName = new SelectList(db.LevelNameModels.ToList(), "Level", "Level");

            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("---------------------no of pages----------" +  learningModels.noofpage);
                db.LearningModels.Add(learningModels);//learningModels);
                try
                {
                    db.SaveChanges();
                    return new JsonResult { Data = new { status = true } };
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:--------------------------------",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine("-------------------------------------- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                }
            }
            //return RedirectToAction("Index");
            
            return new JsonResult { Data = new { status = false } };
        }

        public ActionResult SaveImage([Bind(Include = "TopicName,ID,SubjectName,subsubject,noofpage,pptURL,pdfURL,vidURL,imgURL,content,indexForLearning")] LearningModels learningModels,HttpPostedFileBase pdffile, HttpPostedFileBase vidfile, HttpPostedFileBase imgfile, HttpPostedFileBase pptfile)
        {
            AdminDBContext mycon = new AdminDBContext();
            LearningModels model = mycon.LearningModels.Where(x=> x.SubjectName == learningModels.SubjectName && x.subsubject == learningModels.subsubject
                && x.TopicName == learningModels.TopicName).FirstOrDefault();
            /////////////////////
            string subPath = "~/learning/" + learningModels.SubjectName; // your code goes here
                
            bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));
            if (!exists)
                System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
            if (pdffile != null)
            {
                var fileName = Path.GetFileName(pdffile.FileName);
                var ext = Path.GetExtension(pdffile.FileName); //getting the extension(ex-.jpg)  
                {
                    string myfile = learningModels.subsubject + "_" + learningModels.TopicName + ext; //appending the name with id                                                                                                                       
                    var path = Path.Combine(Server.MapPath("~/learning/" + learningModels.SubjectName), myfile);
                    learningModels.pdfURL = Path.Combine("~/learning/" + learningModels.SubjectName, myfile);
                    pdffile.SaveAs(path);
                }
            }
            if (vidfile != null)
            {
                var fileName = Path.GetFileName(vidfile.FileName);
                var ext = Path.GetExtension(vidfile.FileName); //getting the extension(ex-.jpg)  
                {
                    string myfile = learningModels.subsubject + "_" + learningModels.TopicName + ext; //appending the name with id                                                                    
                    var path = Path.Combine(Server.MapPath("~/learning/" + learningModels.SubjectName), myfile);
                    learningModels.vidURL = Path.Combine("~/learning/" + learningModels.SubjectName, myfile);
                    vidfile.SaveAs(path);
                }
            }
            if (pptfile != null)
            {
                var fileName = Path.GetFileName(pptfile.FileName);
                var ext = Path.GetExtension(pptfile.FileName); //getting the extension(ex-.jpg)  
                {
                    string myfile = learningModels.subsubject + "_" + learningModels.TopicName + ext; //appending the name with id                                                                                                                             // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/learning/" + learningModels.SubjectName), myfile);
                    learningModels.pptURL = Path.Combine("~/learning/" + learningModels.SubjectName, myfile);
                    pptfile.SaveAs(path);
                }
            }
            if (imgfile != null)
            {
                var fileName = Path.GetFileName(imgfile.FileName);
                var ext = Path.GetExtension(imgfile.FileName); //getting the extension(ex-.jpg)  
                {
                    string myfile = learningModels.subsubject + "_" + learningModels.TopicName + ext; //appending the name with id                                                                                                                             // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/learning/" + learningModels.SubjectName), myfile);
                    learningModels.imgURL = Path.Combine("~/learning/" + learningModels.SubjectName, myfile);
                    imgfile.SaveAs(path);
                }
            }
            db.Entry(learningModels).State = EntityState.Modified;
            db.SaveChanges();
                        
            ///////////////            
            return new JsonResult { Data = new { status = true } };

        }

        // GET: LearningModels/Edit/5
        public ActionResult Edit(string id,string subject,string subsubject)
        {
            ViewBag.SubjectName = new SelectList(db.QuizNameModels.ToList(), "SubjectName", "SubjectName");
            ViewBag.subsubject = new SelectList(db.subSubjectmdels.ToList(), "subsubject", "subsubject");
            ViewBag.LevelName = new SelectList(db.LevelNameModels.ToList(), "Level", "Level");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LearningModels learningModels = db.LearningModels.Where(x => x.TopicName == id && x.SubjectName == subject && x.subsubject == subsubject).FirstOrDefault();

            ViewBag.nopage = learningModels.noofpage;
            ViewBag.subject = learningModels.SubjectName;
            ViewBag.sub = learningModels.subsubject;
            ViewBag.topic = learningModels.TopicName;

            if (learningModels.pdfURL != null)
            {
                ViewBag.type = "pdf";
            }
            else if (learningModels.pptURL!=null)
            {
                ViewBag.type = "ppt";
            }
            else if (learningModels.content !=null)
            {
                ViewBag.type = "typed";
            }
            else if (learningModels.vidURL != null)
            {
                ViewBag.type = "video";
            }

            if (learningModels == null)
            {
                return HttpNotFound();
            }
            return View(learningModels);
        }

        // POST: LearningModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(LearningModels learningModels)
        {
            ViewBag.sub = learningModels.subsubject;
            ViewBag.topic = learningModels.TopicName;

            ViewBag.nopage = learningModels.noofpage;
            ViewBag.subject = learningModels.SubjectName;
            if (learningModels.pdfURL != null)
            {
                ViewBag.type = "pdf";
            }
            else if (learningModels.pptURL != null)
            {
                ViewBag.type = "ppt";
            }
            else if (learningModels.content != null)
            {
                ViewBag.type = "typed";
            }
            else if (learningModels.vidURL != null)
            {
                ViewBag.type = "video";
            }


            ViewBag.SubjectName = new SelectList(db.QuizNameModels.ToList(), "SubjectName", "SubjectName");
            ViewBag.subsubject = new SelectList(db.subSubjectmdels.ToList(), "subsubject", "subsubject");
            ViewBag.LevelName = new SelectList(db.LevelNameModels.ToList(), "Level", "Level");
            if (ModelState.IsValid)
            {

                System.Diagnostics.Debug.WriteLine("-----------------------------Model is valid-------------");
                //
                using (AdminDBContext dc = new AdminDBContext())
                {

                    var yy1 = dc.IndexForLearnings.Where(x => x.TopicName == learningModels.TopicName && x.SubjectName == learningModels.SubjectName && x.subsubject == learningModels.subsubject);
                    dc.IndexForLearnings.RemoveRange(yy1);
                    dc.SaveChanges();

                    var myUpdate = dc.LearningModels.Where(x => x.TopicName == learningModels.TopicName && x.SubjectName == learningModels.SubjectName && x.subsubject == learningModels.subsubject).FirstOrDefault();
                    TryUpdateModel(myUpdate);
                    try
                    {
                        
                            dc.SaveChanges();
                            return new JsonResult { Data = new { status = true } };
                        
                    }
                    catch
                    {

                        System.Diagnostics.Debug.WriteLine("-----------------------------Model save error-------------");
                        return new JsonResult { Data = new { status = false } };
                    }


                }
                
                //
                /*db.Entry(learningModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");*/
            }
            System.Diagnostics.Debug.WriteLine("-----------------------------Model not valid-------------");
            return new JsonResult { Data = new { status = false } };
            return View(learningModels);
        }

        // GET: LearningModels/Delete/5
        public ActionResult Delete(string id,string subject,string subsubject)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LearningModels learningModels = db.LearningModels.Where(x=> x.TopicName == id && x.SubjectName == subject && x.subsubject == subsubject).FirstOrDefault();
            if (learningModels == null)
            {
                return HttpNotFound();
            }
            return View(learningModels);
        }

        // POST: LearningModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string subject, string subsubject)
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            var path = Path.Combine(Server.MapPath(db.LearningModels.Where(m => m.TopicName == id && m.SubjectName == subject && m.subsubject == subsubject).Select(m => m.pdfURL).FirstOrDefault()));
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            path = Path.Combine(Server.MapPath(db.LearningModels.Where(m => m.TopicName == id && m.SubjectName == subject && m.subsubject == subsubject).Select(m => m.vidURL).FirstOrDefault()));
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            path = Path.Combine(Server.MapPath(db.LearningModels.Where(m => m.TopicName == id && m.SubjectName == subject && m.subsubject == subsubject).Select(m => m.imgURL).FirstOrDefault()));
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            path = Path.Combine(Server.MapPath(db.LearningModels.Where(m => m.TopicName == id && m.SubjectName == subject && m.subsubject == subsubject).Select(m => m.pptURL).FirstOrDefault()));
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            AdminDBContext db1 = new AdminDBContext();


            var yy1 = db1.IndexForLearnings.Where(m => m.TopicName == id && m.SubjectName == subject && m.subsubject == subsubject);
            db1.IndexForLearnings.RemoveRange(yy1);
            db1.SaveChanges();

            LearningModels learningModels = db.LearningModels.Where(m => m.TopicName == id && m.SubjectName == subject && m.subsubject == subsubject).FirstOrDefault();

            db.LearningModels.Remove(learningModels);
            db.SaveChanges();
            return RedirectToAction("Index");
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
