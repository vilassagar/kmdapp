using KMDRecociliationApp.Data;
using KMDRecociliationApp.Data.Common;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Repositories
{
    public class AnnouncementRepository : MainHeaderRepo<Announcement>
    {
        ApplicationDbContext _context = null;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public AnnouncementRepository(ILogger<AnnouncementRepository> logger, ApplicationDbContext appContext)
            : base(appContext)
        {
            _context = appContext;
            _logger = logger;
        }

        public DataReturn<AnnouncementResult> GetAnnouncement(DataFilter<AnnouncementResult> filter)
        {
            var ret = new DataReturn<AnnouncementResult>();
            var objList = new List<Announcement>();
            int numberOfRecords = 0;
            var announcement = _context.Announcements.AsQueryable().AsNoTracking();
            //.Include(x => x.ApplicationUser)
            //.Include(x => x.Organisation).Include(x => x.AssociationContactDetails)
            //.Include(x => x.AssociationBankDetails);
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                objList = announcement.Search(filter.Search, "Name").ToList();
            }
            else
            {
                objList = new ObjectQuery<Announcement>().GetAllByFilter(filter.PageNumber, filter.Limit, filter.SortName
                   , filter.SortDirection, filter.Filter == null ? null : filter.Filter.GetDelta()
                   , announcement, "Announcement", out numberOfRecords).ToList();
            }
            ret.Contents = objList.Select(x => new AnnouncementResult().CopyPolicyData(x: x)).ToList();
            ret.Source = "Announcement";
            ret.ResultCount = numberOfRecords;
            ret.StatusCode = 200;
            //Paging information
            int numberOfPages = (numberOfRecords / filter.Limit) + ((numberOfRecords % filter.Limit > 0) ? 1 : 0);
            DataPaging paging = new DataPaging();
            paging.RecordsPerPage = filter.Limit;
            paging.PageNumber = filter.PageNumber;
            paging.NumberOfPages = numberOfPages;
            if (filter.PageNumber > 1)
                paging.PreviousPageNumber = filter.PageNumber - 1;
            if (numberOfPages > filter.PageNumber)
                paging.NextPageNumber = filter.PageNumber + 1;
            ret.Paging = paging;
            DataSorting sorting = new DataSorting();
            sorting.SortName = filter.SortName;
            sorting.SortDirection = filter.SortDirection;
            ret.Sorting = sorting;

            return ret;
        }

        public async Task<Announcement?> GetAnnouncementByIdAsync(int id)
        {
            return await _context.Announcements
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
        }

        public async Task<Announcement> CheckAnnouncementName(AnnouncementDTO announcement, bool update = false)
        {
            if (update)
            {
                return await _context.Announcements.AsNoTracking()
                .Where(x => x.Name == announcement.Name
                && x.Id != announcement.Id
                ).FirstOrDefaultAsync();
            }
            else
            {
                return await _context.Announcements.AsNoTracking()
                .Where(x => x.Name == announcement.Name).FirstOrDefaultAsync();
            }
        }
        public async Task<Announcement?> AddAnnouncementAsync(string name, string announcementText, string announcementLocation,/*string announcementText1,*/ AnnouncementDTO announcement, int userId)
        {
            var _announcement = new Announcement
            {
                Name = announcement.Name,
                AnnouncementText = announcement.AnnouncementText,
                AnnouncementLocation = announcement.AnnouncementLocation,
                //IsActive = announcement.IsActive,
                IsCurrent = announcement.IsCurrent,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId,
                CreatedBy = userId // Example value; replace with actual user context
            };
            _context.Announcements.Add(_announcement);
            await _context.SaveChangesAsync();

            // Return the organisation after saving
            return _announcement;
        }

        public async Task UpdateAnnouncementAsync(Announcement announcement)
        {
            var existingAnnouncement = await _context.Announcements.FindAsync(announcement.Id);

            if (existingAnnouncement != null)
            {
                // Update properties explicitly
                existingAnnouncement.Name = announcement.Name;
                existingAnnouncement.AnnouncementText = announcement.AnnouncementText;
                existingAnnouncement.AnnouncementLocation = announcement.AnnouncementLocation; // Ensure this is updated
                existingAnnouncement.IsActive = announcement.IsActive;
                existingAnnouncement.IsCurrent = announcement.IsCurrent;
                existingAnnouncement.UpdatedAt = DateTime.UtcNow;
                existingAnnouncement.UpdatedBy = announcement.UpdatedBy;

                _context.Announcements.Update(existingAnnouncement);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Announcement with ID {announcement.Id} not found.");
            }
        }


        public async Task<bool> DeleteAnnouncementAsync(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
                return false;  // Return false if the organisation is not found

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();
            return true;  // Return true if deletion is successful
        }

        public async Task<Announcement?> GetCurrentAnnouncementAsync(string location)
        {
            return await _context.Announcements
                .Where(a => a.IsCurrent == true && a.IsActive == true && a.AnnouncementLocation == location)
                .FirstOrDefaultAsync();
        }
        public async Task<List<ApplicationPage>> GetAllApplicationPagesAsync()
        {
            return await _context.ApplicationPages.ToListAsync();
        }
    }
}
