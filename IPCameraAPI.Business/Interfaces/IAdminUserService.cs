using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Interfaces
{
    public interface IAdminUserService
    {
        Task<MessageResponse> CreateAdminUser(AdminUserDto model);
        Task<MessageResponse> UpdateAdminUser(AdminUserDto model);
        Task<MessageResponse<AdminUserDto>> AdminLogin(AdminLoginDto model);
        Task<MessageResponse> CreateSubscriptionPlan(SubscriptionPlanDto model);
        Task<MessageResponse> UpdateSubscriptionPlan(SubscriptionPlanDto model);
        Task<IEnumerable<SubscriptionPlanDto>> GetAllSubscriptionPlans();
        Task<SubscriptionPlanDto> GetSubscriptionPlan(Guid id);
        Task<MessageResponse> CreateClientUser(ApplicationUserDto model);
        Task<MessageResponse> UpdateClientUser(ApplicationUserDto model);
        Task<ApplicationUserDto> GetClientUser(Guid id);
        Task<IEnumerable<ApplicationUserDto>> GetAllClientUsers(Pagination pagination);
    }
}
