using ImageHosting.Interface;
using ImageHosting.Migrations;
using ImageHosting.Models;
using Microsoft.EntityFrameworkCore;

using ImageHosting.Data;

namespace ImageHosting.Services
{
    public class UploaderService
    {
        private readonly ApplicationDbContext _context;

        public UploaderService(ApplicationDbContext context)
        {
            _context = context;
        }


    }
}