using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;

namespace ProjectManagement.DAL
{
    //Uses automapper
    public class GenericMapper<S, D>
        where S : class
        where D : class 
    {
        public static D GetDestination(S sourceModelObject)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<S, D>());
            var mapper = config.CreateMapper();
            var dest = mapper.Map<D>(sourceModelObject);
            return dest;
        }
        public static List<D> GetDestinationList(List<S> sourceModelList)
        {
            //Mapper.Initialize(cfg => cfg.CreateMap<ProjectMaster, ProjectMasterModel>());
            //List<ProjectMasterModel> listDest = Mapper.Map<List<ProjectMaster>, List<ProjectMasterModel>>(projectMasters);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<S, D>());
            var mapper = config.CreateMapper();
            var destinationList = mapper.Map<List<S>, List<D>>(sourceModelList);
            return destinationList;
        }
    }
}