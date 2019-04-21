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
    public class QuizNameModelsController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        private UserDBContext dbUser = new UserDBContext();

        // GET: QuizNameModels
        [Authorize]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
             

            ViewBag.CurrentSort = sortOrder;
            ViewBag.subjectSortParm = sortOrder == "SubjectName" ? "SubjectName_asc" : "SubjectName";
            ViewBag.dateSortParm = sortOrder == "CreateDate" ? "CreateDate_asc" : "CreateDate";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var students = from s in db.QuizNameModels select s;
            int ff = 0;
            try
            {
                ff = Convert.ToInt32(searchString);
            }
            catch { }
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.ID == ff || s.SubjectName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "SubjectName_asc":
                    students = students.OrderBy(s => s.SubjectName);
                    break;
                case "SubjectName":
                    students = students.OrderByDescending(s => s.SubjectName);
                    break;
        
                default:  // Name ascending 
                    students = students.OrderByDescending(s => s.CreateDate);
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            /////////////////////////////////////////////////////////
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            return View(students.Take(500).ToPagedList(pageNumber, pageSize));//db.AccountVoucherModels.ToList());
            ///////////////////////////////

            return View(db.QuizNameModels.ToList());
        }

        // GET: QuizNameModels/Details/5
        public ActionResult Details(string id)
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizNameModels quizNameModels = db.QuizNameModels.Find(id);
            if (quizNameModels == null)
            {
                return HttpNotFound();
            }
            return View(quizNameModels);
        }
        [Authorize]
        // GET: QuizNameModels/Create
        public ActionResult Create()
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";

            int yu = (int?) db.LevelNameModels.Count() ?? 1;
            List<string> a = new List<string>();
            for(int i =1;i<= yu; i++)
            {
                a.Add(i.ToString());
            }
            ViewBag.NumOfLevel = new SelectList(a);

            return View();
        }

        // POST: QuizNameModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        public ActionResult Create(QuizNameModels quizNameModels)
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            if (ModelState.IsValid)
            {
                using (AdminDBContext dc = new AdminDBContext())
                {
                    QuizNameModels t1 = new QuizNameModels
                    {
                        SubjectName = quizNameModels.SubjectName,
                        CreateDate = quizNameModels.CreateDate,
                        LevelDetails = quizNameModels.LevelDetails,
                        SubSubject = quizNameModels.SubSubject
                    };
                    dc.QuizNameModels.Add(t1);
                    //System.Diagnostics.Debug.WriteLine("val allready exist prevwronganswer..............--" + quizNameModels.LevelDetails);
                    try
                    {
                        //level should never be empty
                        List<LevelNameModels> tt1 = quizNameModels.LevelDetails;
                        if (tt1 != null)
                        {
                            dc.SaveChanges();
                        }
                        else
                        {
                            //dc.SaveChanges(); dc.SaveChanges();
                            return new JsonResult { Data = new { status = false } };
                        }
                    }
                    catch
                    {
                        return new JsonResult { Data = new { status = false } };
                    }

                   
                }
                return new JsonResult { Data = new { status = true } };
            }

            return View(quizNameModels);
        }

        // GET: QuizNameModels/Edit/5
        [Authorize]
        public ActionResult AddQues(string id, string sortOrder, string currentFilter, string searchString, int? page)
        {
            Session["quiz"] = id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
             ViewBag.QNname = id;
            ///////////////////////
            ViewBag.CurrentSort = sortOrder;
            ViewBag.QuestionNumberSortParm = sortOrder == "QuestionNumber" ? "QuestionNumber_asc" : "QuestionNumber";
            ViewBag.QuizNameSortParm = sortOrder == "QuizName" ? "QuizName_asc" : "QuizName";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var students = from s in db.QuizQuestionModels.Where(x=>x.QuizName == id) select s;
            int ff = 0;
            try
            {
                ff = Convert.ToInt32(searchString);
            }
            catch { }
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.ID == ff || s.QuestionNumber == ff);
            }

            switch (sortOrder)
            {
                case "QuestionNumber_asc":
                    students = students.OrderBy(s => s.QuestionNumber);
                    break;
                case "QuestionNumber":
                    students = students.OrderByDescending(s => s.QuestionNumber);
                    break;
                default:  // Name ascending 
                    students = students.OrderByDescending(s => s.QuestionNumber);
                    break;
            }
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            /////////////////////////////////////////////////////////
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            return View("~/Views/QuizQuestionModels/Index.cshtml", students.Take(500).ToPagedList(pageNumber, pageSize));
            //db.AccountVoucherModels.ToList());
            /////////////////////////
            /*ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<QuizQuestionModels> quizQuestionModels = db.QuizQuestionModels.Where(x=> x.QuizName == id).ToList();
            ViewBag.QNname = id;*/

            //return View("~/Views/QuizQuestionModels/Index.cshtml", quizQuestionModels);
        }

        // GET: QuizNameModels/Delete/5
        [Authorize]
        public ActionResult Delete(string id)
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizNameModels quizNameModels = db.QuizNameModels.FirstOrDefault(m => m.SubjectName == id);
            if (quizNameModels == null)
            {
                return HttpNotFound();
            }
            return View(quizNameModels);
        }

        // POST: QuizNameModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            System.Diagnostics.Debug.WriteLine("name models delete----------" + id);
            var db1 = new AdminDBContext();
            var path1 = "~/images/" + id;
            var path2 = "~/videos/" + id;
            bool exists = System.IO.Directory.Exists(Server.MapPath(path1));
            bool exists2 = System.IO.Directory.Exists(Server.MapPath(path2));
            if (exists)
            {
                System.IO.Directory.Delete(Server.MapPath(path1),true);
            }
            if (exists2)
            {
                System.IO.Directory.Delete(Server.MapPath(path2), true);
            }
            var yy = db1.QuizQuestionModels.Where(m => m.QuizName == id);
            db1.QuizQuestionModels.RemoveRange(yy);
            db1.SaveChanges();
            /////////////////////////////////////////////////////////////


            var yy1 = db1.LevelNameModels.Where(m => m.SubjectName == id);
            db1.LevelNameModels.RemoveRange(yy1);
            db1.SaveChanges();

            var yy2 = db1.subSubjectmdels.Where(m => m.SubjectName == id);
            db1.subSubjectmdels.RemoveRange(yy2);
            db1.SaveChanges();
            try
            {
                QuizNameModels quizNameModels = db.QuizNameModels.Find(id);
                db.QuizNameModels.Remove(quizNameModels);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.InnerException.ToString());
            }
            return RedirectToAction("Index");
        }
        public ActionResult Edit(string id)
        {

            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizNameModels quizNameModels = db.QuizNameModels.Find(id);
            if (quizNameModels == null)
            {
                return HttpNotFound();
            }
            return View(quizNameModels);
        }
        [HttpPost]
        public ActionResult Edit(QuizNameModels quizNameModels)
        {

            if (ModelState.IsValid)
            {

                using (AdminDBContext dc = new AdminDBContext())
                {

                    var yy2 = dc.subSubjectmdels.Where(m => m.SubjectName == quizNameModels.SubjectName);
                    dc.subSubjectmdels.RemoveRange(yy2);
                    dc.SaveChanges();

                    var yy1 = dc.LevelNameModels.Where(m => m.SubjectName == quizNameModels.SubjectName);
                    dc.LevelNameModels.RemoveRange(yy1);
                    dc.SaveChanges();

                    var myUpdate = dc.QuizNameModels.Where(x=> x.SubjectName == quizNameModels.SubjectName).FirstOrDefault();
                    TryUpdateModel(myUpdate);
                    //dc.QuizNameModels.Add(t1);

                    //dc.Entry(quizNameModels).State = EntityState.Modified;
                    
                    //System.Diagnostics.Debug.WriteLine("val allready exist prevwronganswer..............--" + quizNameModels.LevelDetails);
                    try
                    {
                        //List<LevelNameModels> tt1 = quizNameModels.LevelDetails;
                        //if (tt1 != null)
                        {
                            dc.SaveChanges();

                            return new JsonResult { Data = new { status = true } };
                        }
                        //else
                        {
                            return new JsonResult { Data = new { status = false } };
                        }
                    }
                    catch
                    {
                        return new JsonResult { Data = new { status = false } };
                    }


                }
                return new JsonResult { Data = new { status = true } };
                //////////////////////
                //db.Entry(quizNameModels).State = EntityState.Modified;
                //db.SaveChanges();

                ///////////////////
            }
            return View(quizNameModels);
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
