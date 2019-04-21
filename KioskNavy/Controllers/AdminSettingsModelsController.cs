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

namespace KioskNavy.Controllers
{
    public class AdminSettingsModelsController : Controller
    {
        private AdminDBContext db = new AdminDBContext();

        // GET: AdminSettingsModels
        public ActionResult Index()
        {
            return View(db.AdminSettingsModels.ToList());
        }

        // GET: AdminSettingsModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminSettingsModels adminSettingsModels = db.AdminSettingsModels.Find(id);
            if (adminSettingsModels == null)
            {
                return HttpNotFound();
            }
            return View(adminSettingsModels);
        }

        // GET: AdminSettingsModels/Create
        public ActionResult Create()
        {
            var CSS_COLOR_NAMES = new string[] { "AliceBlue", "AntiqueWhite", "Aqua", "Aquamarine", "Azure", "Beige", "Bisque", "Black", "BlanchedAlmond", "Blue", "BlueViolet", "Brown", "BurlyWood", "CadetBlue", "Chartreuse", "Chocolate", "Coral", "CornflowerBlue", "Cornsilk", "Crimson", "Cyan", "DarkBlue", "DarkCyan", "DarkGoldenRod", "DarkGray", "DarkGrey", "DarkGreen", "DarkKhaki", "DarkMagenta", "DarkOliveGreen", "Darkorange", "DarkOrchid", "DarkRed", "DarkSalmon", "DarkSeaGreen", "DarkSlateBlue", "DarkSlateGray", "DarkSlateGrey", "DarkTurquoise", "DarkViolet", "DeepPink", "DeepSkyBlue", "DimGray", "DimGrey", "DodgerBlue", "FireBrick", "FloralWhite", "ForestGreen", "Fuchsia", "Gainsboro", "GhostWhite", "Gold", "GoldenRod", "Gray", "Grey", "Green", "GreenYellow", "HoneyDew", "HotPink", "IndianRed", "Indigo", "Ivory", "Khaki", "Lavender", "LavenderBlush", "LawnGreen", "LemonChiffon", "LightBlue", "LightCoral", "LightCyan", "LightGoldenRodYellow", "LightGray", "LightGrey", "LightGreen", "LightPink", "LightSalmon", "LightSeaGreen", "LightSkyBlue", "LightSlateGray", "LightSlateGrey", "LightSteelBlue", "LightYellow", "Lime", "LimeGreen", "Linen", "Magenta", "Maroon", "MediumAquaMarine", "MediumBlue", "MediumOrchid", "MediumPurple", "MediumSeaGreen", "MediumSlateBlue", "MediumSpringGreen", "MediumTurquoise", "MediumVioletRed", "MidnightBlue", "MintCream", "MistyRose", "Moccasin", "NavajoWhite", "Navy", "OldLace", "Olive", "OliveDrab", "Orange", "OrangeRed", "Orchid", "PaleGoldenRod", "PaleGreen", "PaleTurquoise", "PaleVioletRed", "PapayaWhip", "PeachPuff", "Peru", "Pink", "Plum", "PowderBlue", "Purple", "Red", "RosyBrown", "RoyalBlue", "SaddleBrown", "Salmon", "SandyBrown", "SeaGreen", "SeaShell", "Sienna", "Silver", "SkyBlue", "SlateBlue", "SlateGray", "SlateGrey", "Snow", "SpringGreen", "SteelBlue", "Tan", "Teal", "Thistle", "Tomato", "Turquoise", "Violet", "Wheat", "White", "WhiteSmoke", "Yellow", "YellowGreen" };
            SelectList list = new SelectList(CSS_COLOR_NAMES);
            ViewBag.myList = list;
            return View();
        }

        // POST: AdminSettingsModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       
        public ActionResult Create([Bind(Include = "ID,BackImageUrl,FontSize,FontColor,Backcolor")] AdminSettingsModels adminSettingsModels, HttpPostedFileBase file)
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";

            var Prevpath = Path.Combine("~/img");
            adminSettingsModels.BackImageUrl = "~/img";
            if (System.IO.File.Exists(Prevpath) && file != null)
            {
                System.IO.File.Delete(Prevpath);
            }
            if (file != null)
            {
                var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg","gif" };
                var fileName = Path.GetFileName(file.FileName);
                var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                if (allowedExtensions.Contains(ext)) //check what type of extension  
                {
                    string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                    string myfile = "backimage" + ext; //appending the name with id  
                                                               // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(Server.MapPath("~/img"), myfile);

                    adminSettingsModels.BackImageUrl = Path.Combine("../../img/", myfile);
                    file.SaveAs(path);
                }
            }

            if (ModelState.IsValid)
            {
                db.AdminSettingsModels.Add(adminSettingsModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(adminSettingsModels);
        }

        // GET: AdminSettingsModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminSettingsModels adminSettingsModels = db.AdminSettingsModels.Find(id);
            if (adminSettingsModels == null)
            {
                return HttpNotFound();
            }
            return View(adminSettingsModels);
        }

        // POST: AdminSettingsModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,BackImageUrl,FontSize,FontColor,Backcolor")] AdminSettingsModels adminSettingsModels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adminSettingsModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(adminSettingsModels);
        }

        // GET: AdminSettingsModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminSettingsModels adminSettingsModels = db.AdminSettingsModels.Find(id);
            if (adminSettingsModels == null)
            {
                return HttpNotFound();
            }
            return View(adminSettingsModels);
        }

        // POST: AdminSettingsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AdminSettingsModels adminSettingsModels = db.AdminSettingsModels.Find(id);
            db.AdminSettingsModels.Remove(adminSettingsModels);
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
