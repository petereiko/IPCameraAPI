using Azure.Core;
using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Interfaces;
using IPCameraAPI.Business.Validations;
using IPCameraAPI.Data;
using IPCameraAPI.Data.DataObjects.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Net;
using System.Text;

namespace IPCameraAPI.Business.Implementations
{
    public class AdminUserService : IAdminUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminUserService> _logger;
        public AdminUserService(ApplicationDbContext context, ILogger<AdminUserService> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<MessageResponse> CreateAdminUser(AdminUserDto model)
        {
            MessageResponse result = new();
            try
            {
                var validator = new AdminUserDtoValidation(_context);
                var validationResult = validator.Validate(model);
                if (!validationResult.IsValid)
                {
                    result.Message = validationResult.Errors.Select(x => x.ErrorMessage).FirstOrDefault();
                    return result;
                }

                AdminUser adminUser = new AdminUser
                {
                    CreatedBy = Guid.Empty,
                    CreatedDate = DateTime.Now,
                    Email = model.Email,
                    IsActive = true,
                    IsDeleted = false,
                    Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(model.Password)),
                    Phone = model.Phone
                };
                _context.AdminUsers.Add(adminUser);
                await _context.SaveChangesAsync();
                result.Message = "User added successfully";
                result.Status = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }

        public async Task<MessageResponse> CreateSubscriptionPlan(SubscriptionPlanDto model)
        {
            MessageResponse result= new();
            try
            {
                SubscriptionPlan plan = new()
                {
                    AdminUserId = model.AdminUserId,
                    Amount = model.Amount.GetValueOrDefault(),
                    CreatedBy = model.AdminUserId,
                    CreatedDate = DateTime.Now,
                    Duration = model.Duration.GetValueOrDefault(),
                    IsActive = true,
                    IsDeleted = false,
                    Name = model.Name
                };
                _context.SubscriptionPlans.Add(plan);
                 await _context.SaveChangesAsync();
                result.Message = "Subscription Plan added successfully";
                result.Status = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetAllSubscriptionPlans()
        {
            return await _context.SubscriptionPlans.Include(x => x.AdminUser).AsNoTracking().Select(x => new SubscriptionPlanDto
            {
                AdminUserId = x.AdminUserId,
                Amount = x.Amount,
                Duration = x.Duration,
                Id = x.Id,
                IsActive = x.IsActive,
                Name = x.Name,
                AdminUser = new AdminUserDto
                {
                    Email = x.AdminUser.Email,
                    Id = x.AdminUserId,
                    Phone = x.AdminUser.Phone
                }
            }).ToListAsync();
        }

        public async Task<SubscriptionPlanDto> GetSubscriptionPlan(Guid id)
        {
            return await _context.SubscriptionPlans.Include(x => x.AdminUser).AsNoTracking().Where(x=>x.Id==id).Select(x => new SubscriptionPlanDto
            {
                AdminUserId = x.AdminUserId,
                Amount = x.Amount,
                Duration = x.Duration,
                Id = x.Id,
                IsActive = x.IsActive,
                Name = x.Name,
                AdminUser = new AdminUserDto
                {
                    Email = x.AdminUser.Email,
                    Id = x.AdminUserId,
                    Phone = x.AdminUser.Phone
                }
            }).FirstOrDefaultAsync();
        }

        public async Task<MessageResponse<AdminUserDto>> AdminLogin(AdminLoginDto model)
        {
            MessageResponse<AdminUserDto> result = new();
            try
            {
                var pwd= Convert.ToBase64String(Encoding.UTF8.GetBytes(model.Password));
                var userExist = await _context.AdminUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == pwd);
                if (userExist==null)
                {
                    result.Message = "Invalid Email/Password";
                    return result;
                }
                result.Status = true;
                result.Result = new AdminUserDto { Email = model.Email, Password = pwd, Id = userExist.Id, Phone = userExist.Phone };
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }

        public async Task<MessageResponse> UpdateAdminUser(AdminUserDto model)
        {
            MessageResponse result = new();
            try
            {
                var user = await _context.AdminUsers.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (user == null)
                    return result;

                user.Email = model.Email;
                user.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(model.Password));
                user.Phone= model.Phone;
                await _context.SaveChangesAsync();
                result.Message = "User updated successfully";
                result.Status = true;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return result;
            }
        }

        public async Task<MessageResponse> UpdateSubscriptionPlan(SubscriptionPlanDto model)
        {
            MessageResponse result = new();
            try
            {
                var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (plan == null) return result;

                plan.Duration = model.Duration.GetValueOrDefault();
                plan.Name = model.Name;
                plan.AdminUserId = model.AdminUserId;
                plan.Amount = model.Amount.GetValueOrDefault();
                plan.IsActive = model.IsActive;
                await _context.SaveChangesAsync();
                result.Message = "Plan updated successfully";
                result.Status = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }


        public async Task<MessageResponse> CreateClientUser(ApplicationUserDto model)
        {
            MessageResponse result = new();
            var user = await _context.ApplicationUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Email.ToLower() == model.Email.ToLower() || x.Phone.Trim() == model.Phone.Trim()
             );
            if (user != null)
            {
                result.Message = "User already registered";
                return result;
            }
            user = new()
            {
                Email = model.Email,
                CameraIpAddress = model.CameraIpAddress,
                CameraPassword = model.CameraPassword,
                CameraPort = model.CameraPort,
                CameraUsername = model.CameraUsername,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                Phone = model.Phone,
                SubscriptionPlanId = model.SubscriptionPlanId
            };
            _context.ApplicationUsers.Add(user);
            try
            {
                await _context.SaveChangesAsync();
                result.Message = "Success";
                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Message = "Server Error";
            }
            return result;
        }

        public async Task<MessageResponse> UpdateClientUser(ApplicationUserDto model)
        {
            MessageResponse result = new();
            var user = await _context.ApplicationUsers.Include(x => x.SubscriptionPlan).FirstOrDefaultAsync(x => x.Id == model.Id);
            if (user == null)
            {
                result.Message = "Invalid operation";
                return result;
            }
            user.SubscriptionPlanId = model.SubscriptionPlanId; 
            user.Email = model.Email;
            user.CameraIpAddress = model.CameraIpAddress;
            user.CameraPassword = model.CameraPassword;
            user.CameraPort = model.CameraPort; 
            user.CameraUsername = model.CameraUsername;
            user.SubscriptionPlanId = model.SubscriptionPlanId;
            user.LastModified = DateTime.Now;
            user.ModifiedBy = model.ModifiedBy;
            user.IsActive = model.IsActive; 
            try
            {
                await _context.SaveChangesAsync();
                result.Message = "Success";
                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Message = "Server Error";
            }
            return result;
        }

        public async Task<IEnumerable<ApplicationUserDto>> GetAllClientUsers(Pagination pagination)
        {
            var items = await _context.ApplicationUsers.Include(x=>x.SubscriptionPlan).AsNoTracking().Skip((pagination.PageIndex - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();

            return items.Select(x => new ApplicationUserDto
            { 
                Id=x.Id,
                CameraIpAddress = x.CameraIpAddress,
                CameraPassword = x.CameraPassword,
                CameraPort = x.CameraPort,
                CameraUsername = x.CameraUsername,
                Email = x.Email,
                Phone = x.Phone,
                SubscriptionPlan = new SubscriptionPlanDto
                {
                    AdminUserId = x.SubscriptionPlan.AdminUserId,
                    Id = x.SubscriptionPlan.Id,
                    Amount = x.SubscriptionPlan.Amount,
                    Duration = x.SubscriptionPlan.Duration,
                    IsActive = x.SubscriptionPlan.IsActive,
                    Name = x.SubscriptionPlan.Name
                },
                SubscriptionPlanId = x.SubscriptionPlanId.GetValueOrDefault(),
                IsActive=x.IsActive
            }).ToList();
        }

        public async Task<ApplicationUserDto> GetClientUser(Guid id)
        {
            return await _context.ApplicationUsers.AsNoTracking().Include(x => x.SubscriptionPlan).Where(x => x.Id == id)
                 .Select(x => new ApplicationUserDto
                 {
                     CameraIpAddress = x.CameraIpAddress,
                     CameraPassword = x.CameraPassword,
                     CameraPort = x.CameraPort,
                     CameraUsername = x.CameraUsername,
                     Email = x.Email,
                     Phone = x.Phone,
                     SubscriptionPlan = new SubscriptionPlanDto
                     {
                         AdminUserId = x.SubscriptionPlan.AdminUserId,
                         Id = x.SubscriptionPlan.Id,
                         Amount = x.SubscriptionPlan.Amount,
                         Duration = x.SubscriptionPlan.Duration,
                         IsActive = x.SubscriptionPlan.IsActive,
                         Name = x.SubscriptionPlan.Name
                     },
                     SubscriptionPlanId = x.SubscriptionPlanId.GetValueOrDefault(),
                     Id = x.Id,
                     IsActive = x.IsActive
                 }).FirstOrDefaultAsync();
        }
    }
}
