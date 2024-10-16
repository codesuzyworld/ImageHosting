using ImageHosting.Interface;
using ImageHosting.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ImageHosting.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static TagService;


public class TagService : ITagService
    {

        private readonly ApplicationDbContext _context;

        public TagService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<TagDto>> ListTags()
        {
            var tags = await _context.Tag.ToListAsync();
            return tags.Select(tag => new TagDto
            {
                TagID = tag.TagID,
                TagName = tag.TagName,
                TagColor = tag.TagColor
            });
        }

        public async Task<TagDto?> FindTag(int id)
        {
            var tag = await _context.Tag.FindAsync(id);
            if (tag == null)
            {
                return null;
            }

            return new TagDto
            {
                TagID = tag.TagID,
                TagName = tag.TagName,
                TagColor = tag.TagColor
            };
        }

        public async Task<ServiceResponse> AddTag(TagDto tagDto)
        {
            ServiceResponse response = new();
            var tag = new Tag
            {
                TagName = tagDto.TagName,
                TagColor = tagDto.TagColor
            };

            _context.Tag.Add(tag);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = tag.TagID;
            return response;
        }

        public async Task<ServiceResponse> UpdateTag(TagDto tagDto)
        {
            ServiceResponse response = new();
            var tag = await _context.Tag.FindAsync(tagDto.TagID);
            if (tag == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                return response;
            }

            tag.TagName = tagDto.TagName;
            tag.TagColor = tagDto.TagColor;

            _context.Entry(tag).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        public async Task<ServiceResponse> DeleteTag(int id)
        {
            ServiceResponse response = new();
            var tag = await _context.Tag.FindAsync(id);
            if (tag == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                return response;
            }

            _context.Tag.Remove(tag);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;

        }

        public async Task<IEnumerable<ProjectDto>> ListProjectsForTag(int id)
        {
            var tag = await _context.Tag
                .Include(t => t.Projects)
                .ThenInclude(p => p.Uploader)
                .FirstOrDefaultAsync(t => t.TagID == id);

            if (tag == null || tag.Projects == null)
            {
                return new List<ProjectDto>();
            }

            // Map projects to ProjectDto
            var projects = tag.Projects.Select(project => new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                CreatedAt = project.CreatedAt,
                UploaderId = project.UploaderId,
                UploaderName = project.Uploader.UploaderName
            }).ToList();

            return projects;
        }
}
