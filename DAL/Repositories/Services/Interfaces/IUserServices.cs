using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IUserServices
    {
        Task<string> Register(ReqRegisterUserDto register);

        Task<List<ResUserDto>> GetAllUsers();
        Task<ResLoginDto> Login(ReqLoginDto reqLogin);

        Task<string> UpdateUser(ReqUpdateUserDto updateUserDto, string Id);

        Task<ResDeleteDto> Delete(string id);

        Task<ResGetUserIdDto> GetUserId(string Id);
    }
}
