using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KioskNavy.Models;

namespace KioskNavy.Controllers
{
    public class IndexForLearningsController : Controller
    {
        private AdminDBContext db = new AdminDBContext();

        // GET: IndexForLearnings
        public ActionResult Index()
        {
            return View(db.IndexForLearnings.ToList());
        }

        // GET: IndexForLearnings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndexForLearning indexForLearning = db.IndexForLearnings.Find(id);
            if (indexForLearning == null)
            {
                return HttpNotFound();
            }
            return View(indexForLearning);
        }

        // GET: IndexForLearnings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IndexForLearnings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,IndexName,pageNo,fileName")] IndexForLearning indexForLearning)
        {
            if (ModelState.IsValid)
            {
                db.IndexForLearnings.Add(indexForLearning);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(indexForLearning);
        }

        // GET: IndexForLearnings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndexForLearning indexForLearning = db.IndexForLearnings.Find(id);
            if (indexForLearning == null)
            {
                return HttpNotFound();
            }
            return View(indexForLearning);
        }

        // POST: IndexForLearnings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,IndexName,pageNo,fileName")] IndexForLearning indexForLearning)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indexForLearning).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(indexForLearning);
        }

        // GET: IndexForLearnings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndexForLearning indexForLearning = db.IndexForLearnings.Find(id);
            if (indexForLearning == null)
            {
                return HttpNotFound();
            }
            return View(indexForLearning);
        }

        // POST: IndexForLearnings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IndexForLearning indexForLearning = db.IndexForLearnings.Find(id);
            db.IndexForLearnings.Remove(indexForLearning);
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
