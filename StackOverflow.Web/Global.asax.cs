using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using StackOverflow.Domain.Entities;
using StackOverflow.Web.Models;

namespace StackOverflow.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AutoFacConfig.Register();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.RegisterMaps();
        }
    }

    public static class AutoFacConfig
    {
        public static void Register()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterFilterProvider();
            builder.RegisterSource(new ViewRegistrationSource());
            builder.RegisterType<ReadOnlyRepository>().As<IReadOnlyRepository>();

            builder.Register(c => Mapper.Engine).As<IMappingEngine>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

        }
    }

    public static class AutoMapperConfig
    {
        public static void RegisterMaps()
        {
            Mapper.CreateMap<AccountRegisterModel, Account>().ReverseMap();
            Mapper.CreateMap<AccountLoginModel, Account>().ReverseMap();
            Mapper.CreateMap<AddNewQuestionModel, Question>().ReverseMap();
            Mapper.CreateMap<QuestionListModel, Question>().ReverseMap();
            Mapper.CreateMap<AddNewAnswerModel, Answer>().ReverseMap();
            Mapper.CreateMap<AnswerDetailModel, Answer>().ReverseMap();
        }
    }

    public class ReadOnlyRepository : IReadOnlyRepository
    {
        public Account GetById(Guid Id)
        {
            return new Account();
        }
    }

    public class ReadAndWrite : IReadAndWriteRepository
    {
        public Account GetById(Guid Id)
        {
            return new Account();
        }
    }

    public interface IReadOnlyRepository
    {
        Account GetById(Guid Id);
    }

    public interface IReadAndWriteRepository : IReadOnlyRepository
    {
        
    }
}
