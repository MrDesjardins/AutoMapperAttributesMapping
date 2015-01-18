using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using WebTestProject.Models;
using WebTestProject.ViewModels;

namespace WebTestProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new UserModel {FirstName = "Test", Id = 123, LastName = "TestLastName"};
            var viewModel = Mapper.Map<UserViewModel>(model);
            return View(viewModel);
        }
    }
}