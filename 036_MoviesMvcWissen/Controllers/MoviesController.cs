﻿using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace _036_MoviesMvcWissen.Controllers
{
    public class MoviesController : Controller
    {
        MoviesContext db = new MoviesContext();
        // GET: Movies
        public ViewResult Index()
        {
            //var model = db.Movies.ToList();
            var model = GetList();
            ViewData["count"] = model.Count;
            return View(model);
        }
        [NonAction]
        public List<Movie> GetList(bool removeSession = true)
        {
            List<Movie> entities;
            if (removeSession)
                Session.Remove("movies");
            if (Session["movies"] == null || removeSession)
            {
                entities = db.Movies.ToList();
                Session["movies"] = entities;
            }
            else
            {
                entities = Session["movies"] as List<Movie>;
            }
            return entities;
        }

        public ActionResult GetMoviesFromSession()
        {
            var model = GetList(false);
            return View("index", model);
        }
        [HttpGet]
        public ViewResult Add()
        {

            ViewBag.Message = "Please enter Movie information...";
            // return View();
            return new ViewResult();
        }


        [HttpPost]
        public RedirectToRouteResult Add(string Name, int ProductionYear, string BoxOfficeReturn)
        {
            var entity = new Movie()
            {
                Name = Name,
                ProductionYear = ProductionYear.ToString(),
                BoxOfficeReturn = Convert.ToDouble(BoxOfficeReturn.Replace(",", "."), CultureInfo.InvariantCulture)
            };
            db.Movies.Add(entity);
            db.SaveChanges();
            TempData["Info"] = "Record successfully added to database.";
            return RedirectToAction("Index");
        }

        
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required");
            var model = db.Movies.Find(id.Value);
            List<SelectListItem> years = new List<SelectListItem>();
            SelectListItem year;
            for (int i = DateTime.Now.Year; i >= 2000; i--)
            {
                year = new SelectListItem() { Value = i.ToString(), Text = i.ToString() };
                years.Add(year);
            }
            ViewBag.Years = new SelectList(years, "Value", "Text", model.ProductionYear);
            return View(model);
        }
        [HttpPost]
       
        public ActionResult Edit([Bind(Include="Id,Name,ProductionYear")]Movie movie,string BoxOfficeReturn) //public ActionResult Edit(Movie movie,string BoxOfficeReturn) - boxofficereturn virgülle girildigi zaman doublea çeviremedigi için null geliyor, string olarak ayrı alıp controllerda set edebiliriz bu şekilde
        {
            var entity = db.Movies.SingleOrDefault(e => e.Id == movie.Id);
            entity.Name = movie.Name;
            entity.ProductionYear = movie.ProductionYear;
            entity.BoxOfficeReturn = Convert.ToDouble(BoxOfficeReturn.Replace(",","."),CultureInfo.InvariantCulture);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Info"] = "Record successfully updated in database.";
            return RedirectToRoute(new { controller = "Movies", action = "Index" });

        }
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!");
            var model = db.Movies.FirstOrDefault(e => e.Id == id.Value);
            return View(model);
        }
        [ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(int? id)
        {
            var entity = db.Movies.Find(id);
            db.Movies.Remove(entity);
            db.SaveChanges();
            TempData["Info"] = "Record successfully deleted from database.";
            return RedirectToAction("Index");
        }
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!");
            var model = db.Movies.Find(id.Value);
            return View(model);
        }
        public ActionResult Welcome()
        {
            var result = "Welcome to Movies MVC";
            //return Content(result);
            return PartialView("_Welcome",result);
        }
    }
}