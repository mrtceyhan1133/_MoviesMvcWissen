using _036_MoviesMvcWissen.Models.Demos.Templates;
using _036_MoviesMvcWissen.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace _036_MoviesMvcWissen.Controllers
{
    #region Razor Demos
    public static class NameUtil
    {
        public static string GetName()
        {
            return "Name: Çağıl Alsaç";
        }
    }
    #endregion

    public class DemosController : Controller
    {
        #region Razor Demos
        public ActionResult Razor1() // kodlar için view'a gidilmelidir.
        {
            return View();
        }

        public ActionResult Razor2() // kodlar için view'a gidilmelidir.
        {
            return View();
        }

        #endregion
        #region Route Values
        public string FromRoute(int id)
        {
            return id.ToString();
        }
        #endregion
        #region QueryString Values
        //public string FromQueryString(string name,string surname)
        public string FromQueryString()
        {
            var name = Request.QueryString["name"];
            var surname = Request.QueryString["surname"];

            return name + " " + surname;
        }
        #endregion
        #region Templates
        public ActionResult GetPeople()
        {
            List<PersonModel> people;
            if (Session["people"] == null)
            {
                people = new List<PersonModel>()
                {
                new PersonModel()
                {
                    Id=1,
                    FullName="Çağıl Alsaç",
                    IdentityNo="1133",
                    GraduateFromUniversity=true,
                    BirthDate=DateTime.Parse("19.06.1980")
                },
                new PersonModel()
                {
                    Id=2,
                    FullName="Leo",
                    IdentityNo="4166",
                    GraduateFromUniversity=false,
                    BirthDate=DateTime.Parse("25.05.2015")
                }

                };
                Session["people"] = people;
            }
            else
            {
                people = Session["people"] as List<PersonModel>;
            }
            return View(people);
        }

        public ActionResult GetPersonDetails(int? id)
        {
            List<PersonModel> people = Session["people"] as List<PersonModel>;
            PersonModel person = people.SingleOrDefault(e => e.Id == id);
            return View(person);
        }
        public ActionResult AddPerson()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPerson(PersonModel personModel)
        {
            List<PersonModel> people = Session["people"] as List<PersonModel>;
            personModel.Id = people.Max(e => e.Id) + 1;
            people.Add(personModel);
            Session["people"] = people;
            return RedirectToAction("GetPeopleAjax");

        }
        #endregion
        #region handleError action Filter
        [HandleError]
        public ActionResult DivideByZero()
        {
            var no1 = 14;
            var no2 = 0;
            var result = no1 / no2;
            ViewBag.Result = result;
            return View();
        }
        #endregion

        #region Ajax
        public ActionResult GetPeopleAjax()
        {
            List<PersonModel> people;
            if (Session["people"] == null)
            {
                people = new List<PersonModel>()
                {
                new PersonModel()
                {
                    Id=1,
                    FullName="Çağıl Alsaç",
                    IdentityNo="1133",
                    GraduateFromUniversity=true,
                    BirthDate=DateTime.Parse("19.06.1980")
                },
                new PersonModel()
                {
                    Id=2,
                    FullName="Leo",
                    IdentityNo="4166",
                    GraduateFromUniversity=false,
                    BirthDate=DateTime.Parse("25.05.2015")
                }

                };
                Session["people"] = people;
            }
            else
            {
                people = Session["people"] as List<PersonModel>;
            }
            DemosGetPeopleAjaxViewModel model = new DemosGetPeopleAjaxViewModel()
            {
                PeopleModel = people,
                PersonModel = new PersonModel()
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult AddPersonAjax(PersonModel personModel)
        {
            //Thread.Sleep(3000);
            List<PersonModel> people = Session["people"] as List<PersonModel>;
            if(people.Count==0)
            {
                personModel.Id = 1;
            }
            else
            {
                personModel.Id = people.Max(e => e.Id) + 1;

            }
            people.Add(personModel);
            Session["people"] = people;
           
            return PartialView("_PeopleList", people);
        }

        public ActionResult DeletePersonAjax(int id)
        {
           // Thread.Sleep(3000);
            List<PersonModel> people = Session["people"] as List<PersonModel>;
            var person = people.FirstOrDefault(e => e.Id == id);
            people.Remove(person);
            Session["people"] = people;
            return PartialView("_PeopleList", people);
        }

        public ActionResult GetPeopleJson()
        {
            if(Request.IsAjaxRequest())
            { 
            var people = new List<PersonModel>()
                {
                new PersonModel()
                {
                    Id=1,
                    FullName="Çağıl Alsaç",
                    IdentityNo="1133",
                    GraduateFromUniversity=true,
                    BirthDate=DateTime.Parse("19.06.1980")
                },
                new PersonModel()
                {
                    Id=2,
                    FullName="Leo",
                    IdentityNo="4166",
                    GraduateFromUniversity=false,
                    BirthDate=DateTime.Parse("25.05.2015")
                }

                };
            var model = people.Select(e => new PersonModelClientModel()
            {
                Id = e.Id,
                BirthDate =  e.BirthDate.HasValue?e.BirthDate.Value.ToShortDateString() : "",
                FullName = e.FullName,
                GraduateFromUniversity = e.GraduateFromUniversity,
                IdentityNo=e.IdentityNo
            });
            
            return Json(model, JsonRequestBehavior.AllowGet);
            }
            return new EmptyResult();
        }
        public RedirectResult GetPeopleHtml()
        {
            return RedirectPermanent("~/DemosPeople.html");
        }
       
        #endregion
    }


}