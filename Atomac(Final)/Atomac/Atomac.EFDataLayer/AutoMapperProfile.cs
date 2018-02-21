using Atomac.EFDataLayer.DTO;
using Atomac.EFDataLayer.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomac.EFDataLayer
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<Team, DTOTeam>();
            CreateMap<ApplicationUser, DTOAppUser>();
            CreateMap<Game, DTOGame>();
            CreateMap<Message, DTOMessage>();
            CreateMap<Move, DTOMove>();
            CreateMap<Rules, DTORules>();
            CreateMap<ATable, DTOTable>();
            CreateMap<AFigure, DTOFigure>();
            CreateMap<Artifact, DTOArtifact>();
            CreateMap<Stuff, DTOStuff>();
            CreateMap<Game, DTOGameMini>();
            CreateMap<ApplicationUser, DTOAppUserMini>();
            CreateMap<Artifact, DTOArtifactMini>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetType().Name));
        }

    }
}
