using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using _036_MoviesMvcWissen.Models;
using _036_MoviesMvcWissen.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace _036_MoviesMvcWissen.Controllers
{
    public class MoviesController : Controller
    {
        MoviesContext db = new MoviesContext();
        // GET: Movies
        public ViewResult Index(MoviesIndexViewModel moviesIndexViewModel)
        {
            //var model = db.Movies.ToList();
            var model = GetList();
            ViewData["count"] = model.Count;

            var years = new List<SelectListItem>();
            years.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- All --"
            });
            for (int i = DateTime.Now.Year; i >= 1950; i--)
            {
                years.Add(new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                });
            }

            if (moviesIndexViewModel == null)
            {
                moviesIndexViewModel = new MoviesIndexViewModel();
            }
            if (String.IsNullOrWhiteSpace(moviesIndexViewModel.YearId))
                moviesIndexViewModel.Movies = db.Movies.ToList();
            else
            {
                moviesIndexViewModel.Movies = db.Movies.Where(e => e.ProductionYear == moviesIndexViewModel.YearId).ToList();
            }

            moviesIndexViewModel.Years = new SelectList(years, "Value", "Text", moviesIndexViewModel.YearId);
            return View(moviesIndexViewModel);
        }

        [OutputCache(Duration = 60, Location = OutputCacheLocation.ServerAndClient, NoStore = true, VaryByParam = "Name;Min;Max")]
        public ActionResult List(MoviesIndexViewModel moviesIndexViewModel)
        {
            if (moviesIndexViewModel == null)
            {
                moviesIndexViewModel = new MoviesIndexViewModel();
            }

            var movies = db.Movies.AsQueryable();
            if (!String.IsNullOrWhiteSpace(moviesIndexViewModel.Name))
            {
                movies = movies.Where(e => e.Name.Contains(moviesIndexViewModel.Name));
            }

            moviesIndexViewModel.Movies = movies.ToList();
            var years = new List<SelectListItem>();
            for (int i = DateTime.Now.Year; i >= 2000; i--)
            {
                years.Add(new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                });
            }
            if (!String.IsNullOrWhiteSpace(moviesIndexViewModel.YearId))
                movies = movies.Where(e => e.ProductionYear == moviesIndexViewModel.YearId);

            if (!String.IsNullOrWhiteSpace(moviesIndexViewModel.Min))
            {
                double minValue = 0;
                if (Double.TryParse(moviesIndexViewModel.Min.Replace(',', '.'), out minValue))
                {
                    movies = movies.Where(e => e.BoxOfficeReturn >= minValue);
                }

            }
            if (!String.IsNullOrWhiteSpace(moviesIndexViewModel.Max))
            {
                double maxValue = 0;
                if (Double.TryParse(moviesIndexViewModel.Max.Replace(',', '.'), out maxValue))
                {
                    movies = movies.Where(e => e.BoxOfficeReturn >= maxValue);
                }

            }


            moviesIndexViewModel.Movies = movies.ToList();

            moviesIndexViewModel.Years = new SelectList(years, "Value", "Text", moviesIndexViewModel.YearId);
            return View(moviesIndexViewModel);
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
        public ActionResult Add()
        {

            ViewBag.Message = "Please enter Movie information...";
            var directors = db.Directors.ToList().Select(e => new SelectListItem() //directors listesini aldık ve listbox da kullanabilmek için multiselectliste çevirdik
            {
                Value = e.Id.ToString(),
                Text = e.Name + " " + e.Surname
            }).ToList();
            ViewData["directors"] = new MultiSelectList(directors, "Value", "Text");
            return View();
            //return new ViewResult();
        }
        private string CreateFilePath(HttpPostedFileBase Image,string Name)
        {
            string filePath = null;
            if (Image != null && Image.ContentLength > 0)
            {

                var fileName = DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Millisecond.ToString().PadLeft(3, '0') + "_" + Name + Path.GetExtension(Image.FileName);
                if (Path.GetExtension(fileName).ToLower().Equals(".jpg") || Path.GetExtension(fileName).ToLower().Equals(".jpeg") || Path.GetExtension(fileName).ToLower().Equals(".jpg") || Path.GetExtension(fileName).ToLower().Equals(".png") || Path.GetExtension(fileName).ToLower().Equals(".bmp"))
                {
                    filePath = ConfigurationManager.AppSettings["FileFolder"] + "/Movies/" + fileName;
                }
            }
            return filePath;

        }

        [HttpPost]
        public RedirectToRouteResult Add(string Name, int ProductionYear, string BoxOfficeReturn, List<int> Directors, HttpPostedFileBase Image)
        {
            string filePath = CreateFilePath(Image,Name);
            //if (Image != null && Image.ContentLength > 0)
            //{

            //    var fileName = DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Millisecond.ToString().PadLeft(3, '0') + "_" + Name + Path.GetExtension(Image.FileName);
            //    if (Path.GetExtension(fileName).ToLower().Equals(".jpg") || Path.GetExtension(fileName).ToLower().Equals(".jpeg") || Path.GetExtension(fileName).ToLower().Equals(".jpg") || Path.GetExtension(fileName).ToLower().Equals(".png") || Path.GetExtension(fileName).ToLower().Equals(".bmp"))
            //    {
            //        filePath = ConfigurationManager.AppSettings["FileFolder"] + "/Movies/" + fileName;
            //    }
            //}
            
            
                var entity = new Movie()
            {
                Name = Name,
                ProductionYear = ProductionYear.ToString(),
                BoxOfficeReturn = Convert.ToDouble(BoxOfficeReturn.Replace(",", "."), CultureInfo.InvariantCulture),
                //MovieDirectors = new List<MovieDirector>()
                FilePath=filePath
        };
            entity.MovieDirectors = Directors.Select(e => new MovieDirector()
            {
                MovieId = entity.Id,
                DirectorId = e
            }).ToList();
            db.Movies.Add(entity);
            db.SaveChanges();
          if(filePath!=null)
            {
                Image.SaveAs(Server.MapPath("~/"+filePath));
            }
            Debug.WriteLine("Added Entity Id: " + entity.Id);
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
            var directors = db.Directors.Select(e => new DirectorModel()
            {
                Id = e.Id,
                FullName = e.Name + " " + e.Surname
            }).ToList();
            var directorIds = model.MovieDirectors.Select(e => e.DirectorId).ToList();

            ViewBag.directors = new MultiSelectList(directors, "Id", "FullName", directorIds);
            return View(model);
        }
        [HttpPost]

        public ActionResult Edit([Bind(Include = "Id,Name,ProductionYear")]Movie movie, string BoxOfficeReturn, List<int> directorIds,HttpPostedFileBase Image) //public ActionResult Edit(Movie movie,string BoxOfficeReturn) - boxofficereturn virgülle girildigi zaman doublea çeviremedigi için null geliyor, string olarak ayrı alıp controllerda set edebiliriz bu şekilde
        {
            string filePath = CreateFilePath(Image, movie.Name);
            var entity = db.Movies.SingleOrDefault(e => e.Id == movie.Id);
            string oldFilePath = entity.FilePath;
            entity.Name = movie.Name;
            entity.ProductionYear = movie.ProductionYear;
            entity.BoxOfficeReturn = Convert.ToDouble(BoxOfficeReturn.Replace(",", "."), CultureInfo.InvariantCulture);
            entity.MovieDirectors = new List<MovieDirector>();
            if(Image!=null && Image.ContentLength>0)
            { 
            entity.FilePath = filePath;
            }
            var movieDirectors = db.MovieDirectors.Where(e => e.MovieId == movie.Id);
            foreach (var movieDirector in movieDirectors)
            {
                db.MovieDirectors.Remove(movieDirector); // movienin yönetmenlerini sildik ara tabloyu bu moviele alakalı verileri boşalttık
            }
            foreach (var directorId in directorIds) // elimizdeki editlenecek yönetmenlerin idlerine göre her biri için movie director newleyerek entitymize ekleyip sonrasında entityi veritabanında güncelledik
            {
                var movieDirector = new MovieDirector()
                {
                    MovieId = movie.Id,
                    DirectorId = directorId
                };
                entity.MovieDirectors.Add(movieDirector);
            }
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            if(filePath != null)
            {
                if(!String.IsNullOrWhiteSpace(oldFilePath))
                {
                    if(System.IO.File.Exists(Server.MapPath("~/" + oldFilePath)))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                }
                Image.SaveAs(Server.MapPath("~/" + filePath));
            }
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
            if(entity.FilePath!= null && System.IO.File.Exists(Server.MapPath("~/" + entity.FilePath)))
            {
                System.IO.File.Delete(Server.MapPath("~/" + entity.FilePath));
            }
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
        [ChildActionOnly] //url üzerinden degil sadece htmlactionla ulaşmak için yazıyoruz
        public ActionResult Welcome()
        {
            var result = "Welcome to Movies MVC";
            //return Content(result);
            return PartialView("_Welcome", result);
        }
    }
}