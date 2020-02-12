using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using _036_MoviesMvcWissen.Models.ViewModels;

namespace _036_MoviesMvcWissen.Controllers
{
   
    public class DirectorsController : Controller
    {
        private MoviesContext db = new MoviesContext();

        // GET: Directors
        public ActionResult Index()
        {
            var model = new DirectorsIndexViewModel()
            {
                DirectorsList=db.Directors.ToList()
            };
            return View(model);
            //return View(db.Directors.ToList())
        }

        // GET: Directors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Director director = db.Directors.Find(id);
            if (director == null)
            {
                return HttpNotFound();
            }
            return View(director);
        }

        // GET: Directors/Create
        public ActionResult Create()
        {
            var movies = db.Movies.ToList().Select(e => new SelectListItem() //directors listesini aldık ve listbox da kullanabilmek için multiselectliste çevirdik
            {
                Value = e.Id.ToString(),
                Text = e.Name 
            }).ToList();
            ViewData["movies"] = new MultiSelectList(movies, "Value", "Text");
            return View();
        }

        // POST: Directors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,Surname,Retired")] Director director)
        [ActionName("Create")]
        //public ActionResult CreateNew() //public ActionResult CreateNew(FormCollection formCollection)
        public ActionResult Create([Bind(Include = "Id,Name,Surname,Retired")] Director director,List<int>movieIds)
        {
            var newDirector = new Director()
            {
                Name = director.Name,
                Surname = director.Surname,
                //Name = formCollection["Name"];
                //Surname = formCollection["Surname"];
               
                
            };
            //var retired = formCollection["Retired"];
            var retired = director.Retired;
            newDirector.Retired = retired;
            newDirector.MovieDirectors = movieIds.Select(e => new MovieDirector()
            {
                MovieId = e,
                DirectorId = director.Id
            }).ToList();

            
            //if (newDirector.Equals("false"))
            //    director.Retired = false;
            //if (String.IsNullOrWhiteSpace(newDirector.Name))
            //    ModelState.AddModelError("Name", "Director Name is required!");
            //if (String.IsNullOrWhiteSpace(newDirector.Surname))
            //    ModelState.AddModelError("Surname", "Director Surname is required!");
            //if(newDirector.Name.Length>100)
            //    ModelState.AddModelError("Name","Director Name must be maximum 100 characters");
            //if(newDirector.Surname.Length>100)
            //    ModelState.AddModelError("Surname","Director Surname must be maximum 100 characters");
            if (ModelState.IsValid)
            {
                db.Directors.Add(newDirector);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(director);
        }

        // GET: Directors/Edit/5
      //1  public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Director director = db.Directors.Find(id);
        //    if (director == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    var movies = db.Movies.ToList();
        //    var movieIds = db.Directors.Find(id).MovieDirectors.Select(e => e.MovieId).ToList();
        //    ViewBag.movies = new MultiSelectList(movies, "Id","Name", movieIds);
        //    return View(director);
        //}
        public ActionResult Edit(int? id) //2
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var movies = db.Movies.Select(e => new SelectListItem()
            {
                Value = e.Id.ToString(),
                Text = e.Name
            }).ToList();
            var director = db.Directors.Find(id.Value);
            List<int> _movieIds= director.MovieDirectors.Select(e => e.MovieId).ToList();
            DirectorsEditViewModel model = new DirectorsEditViewModel();
            model.Director = director;
            model.movieIds = _movieIds;
            model.Movies = new MultiSelectList(movies,"Value","Text", model.movieIds);
            
           
            return View("Edit2" , model);
            
            
        }
        // POST: Directors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
 
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Edit(DirectorsEditViewModel directorsEditViewModel)
        {
            if(ModelState.IsValid)
            { 
            var director = db.Directors.Find(directorsEditViewModel.Director.Id);
            director.Name = directorsEditViewModel.Director.Name;
            director.Surname = directorsEditViewModel.Director.Surname;
            director.Retired = directorsEditViewModel.Director.Retired;
            var movieDirectors = db.MovieDirectors.Where(e => e.DirectorId == director.Id).ToList();
            //var movieDirectors = director.MovieDirectors;
            foreach(var movieDirector in movieDirectors)
            {
                db.MovieDirectors.Remove(movieDirector);
            }
            director.MovieDirectors = directorsEditViewModel.movieIds.Select(e => new MovieDirector()
            {
                DirectorId = director.Id,
                MovieId = e
            }).ToList();
            db.Entry(director).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
            return View(directorsEditViewModel);
        }

        //[HttpPost]//1
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Name,Surname,Retired")] Director director,List<int> movieIds)
        //{
        //    var entity = db.Directors.SingleOrDefault(e => e.Id == director.Id);
        //    entity.Name = director.Name;
        //    entity.Retired = director.Retired;
        //    entity.Surname = director.Surname;
        //    entity.MovieDirectors = new List<MovieDirector>();
        //    var movieDirectors = db.MovieDirectors.Where(e => e.DirectorId == director.Id);
        //    foreach (var movieDirector in movieDirectors)
        //    {
        //        db.MovieDirectors.Remove(movieDirector); // movienin yönetmenlerini sildik ara tabloyu bu moviele alakalı verileri boşalttık
        //    }
        //    //alttaki foreachle aynı işi yapar
        //    //entity.MovieDirectors = movieIds.Select(e => new MovieDirector() 
        //    //{
        //    //    MovieId=e,
        //    //    DirectorId=director.Id
        //    //}).ToList();
        //    foreach (var movieId in movieIds) // elimizdeki editlenecek yönetmenlerin idlerine göre her biri için movie director newleyerek entitymize ekleyip sonrasında entityi veritabanında güncelledik
        //    {
        //        var movieDirector = new MovieDirector()
        //        {
        //            DirectorId = director.Id,
        //            MovieId = movieId
        //        };
        //        entity.MovieDirectors.Add(movieDirector);
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(director);
        //}

        // GET: Directors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Director director = db.Directors.Find(id);
            if (director == null)
            {
                return HttpNotFound();
            }
            return View(director);
        }

        // POST: Directors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Director director = db.Directors.Find(id);
            db.Directors.Remove(director);
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
