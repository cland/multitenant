using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SmartAuditAPI2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAuditAPI2.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IdentityUser, UserDTO>()
                .ForMember(r => r.roles, opt => opt.Ignore())
                .ForMember(s => s.Secret, opt => opt.Ignore())
                .ReverseMap();
        }         
    }
}
