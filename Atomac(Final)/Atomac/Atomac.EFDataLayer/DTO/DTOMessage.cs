using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Atomac.EFDataLayer.Models;

namespace Atomac.EFDataLayer.DTO
{
    [Serializable]
    public class DTOMessage
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();

        public int Id { get; set; }
        public string Text { get; set; }
        public bool LinkTag { get; set; }
        public DateTime Time { get; set; }
        public string SenderId { get; set; }
        public DTOAppUser Sender { get; set; }

        public DTOMessage GetById(int id)
        {
            var message = dbContext.Messages.Find(id);
            return Mapper.Map<DTOMessage>(message);
        }
    }
}