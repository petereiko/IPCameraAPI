using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Interfaces;
using IPCameraAPI.Business.Modules.Execute;
using IPCameraAPI.Data;
using IPCameraAPI.Data.DataObjects.Domains;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Implementations
{
    public class ClientRecordService:IClientRecordService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExecuteService _executeService;
        private readonly object _lock = new object();
        public ClientRecordService(ApplicationDbContext context, IExecuteService executeService)
        {
            _context = context;
            _executeService = executeService;
        }

        private async Task<List<ApplicationUser>> FetchAllApplicationUsers(bool? isActive)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            if(isActive.HasValue &&  isActive.Value)
            {
                users = await _context.ApplicationUsers.AsNoTracking().Include(x => x.SubscriptionPlan)
                .Where(x => x.IsActive == isActive.Value)
                .ToListAsync();
            }
            else
            {
                users = await _context.ApplicationUsers.AsNoTracking().Include(x => x.SubscriptionPlan)
                .ToListAsync();
            }
            return users;
        }



        public async Task Execute(string baseDirectory, int videoDuration)
        {
            var users = await FetchAllApplicationUsers(true);

            Parallel.ForEach(users, (u) =>
            {
                string fileName = Guid.NewGuid().ToString() + ".mp4";
                string directory = $"{baseDirectory}/";
                string midPath = $"{u.Email.Replace(".", "")}/";
                string TotalDirectory = Path.Combine(directory, midPath);
                if(!Directory.Exists(TotalDirectory))
                    Directory.CreateDirectory(TotalDirectory);
                DateTime StartDate = DateTime.Now;
                var videoResult = _executeService.Run(new LoginDto
                {
                    IPAddress = u.CameraIpAddress,
                    Password = u.CameraPassword,
                    Port = u.CameraPort,
                    Username = u.CameraUsername
                }, TotalDirectory, fileName, videoDuration).GetAwaiter().GetResult();
                DateTime EndDate = DateTime.Now;

                VideoRecord record = new()
                {
                    ApplicationUserId = u.Id.ToString(),
                    CreatedBy = u.Id,
                    CreatedDate = DateTime.Now,
                    FileName = $"{midPath}{fileName}",
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    IsDeleted = false,
                    StartDate = StartDate,
                    EndDate = EndDate
                };

                _context.VideoRecords.Add(record);
                lock (_lock)
                {
                    _context.SaveChanges();
                }
                
            });
        }

        
    }
}
