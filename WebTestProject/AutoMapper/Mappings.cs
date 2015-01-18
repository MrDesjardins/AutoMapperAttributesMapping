using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using WebTestProject.Models;
using WebTestProject.ViewModels;

namespace WebTestProject.AutoMapper
{
    public static class Mappings
    {
        public static void RegisterMappings()
        {
            Mapper.CreateMap<UserModel, UserViewModel>();
        }
    }
}