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
    public class LevelNameModelsController : Controller
    {
        private AdminDBContext db = new AdminDBContext();

        // GET: LevelNameModels
        public ActionResult Index()
        {
            return View(db.LevelNameModels.ToList());
        }

        // GET: LevelNameModels/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LevelNameModels levelNameModels = db.LevelNameModels.Find(id);
            if (levelNameModels == null)
            {
                return HttpNotFound();
            }
            return View(levelNameModels);
        }

        // GET: LevelNameModels/Create
        public ActionResult Create()
        {
            return View();
        }
        
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Level,ID,LevelDescription,certificateTreshhold")] LevelNameModels levelNameModels)
        {
            if (ModelState.IsValid)
            {
                db.LevelNameModels.Add(levelNameModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(levelNameModels);
        }

        // GET: LevelNameModels/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LevelNameModels levelNameModels = db.LevelNameModels.Find(id);
            if (levelNameModels == null)
            {
                return HttpNotFound();
            }
            return View(levelNameModels);
        }

        // POST: LevelNameModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Level,ID,LevelDescription,certificateTreshhold")] LevelNameModels levelNameModels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(levelNameModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(levelNameModels);
        }

        // GET: LevelNameModels/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LevelNameModels levelNameModels = db.LevelNameModels.Find(id);
            if (levelNameModels == null)
            {
                return HttpNotFound();
            }
            return View(levelNameModels);
        }

        // POST: LevelNameModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            LevelNameModels levelNameModels = db.LevelNameModels.Find(id);
            db.LevelNameModels.Remove(levelNameModels);
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
