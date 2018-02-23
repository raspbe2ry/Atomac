using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Atomac.Models;
using Atomac.DTO;
using AutoMapper;

namespace Atomac
{
    public class AutoMapperProfile: Profile
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
            //CreateMap<Artifact, DTOArtifactMini > ();
            CreateMap<Game, DTOGameMini>();
            CreateMap<ApplicationUser, DTOAppUserMini>();
            CreateMap<Artifact, DTOArtifactMini>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetType().Name));
        }
    }
}