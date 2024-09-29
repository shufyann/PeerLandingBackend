using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;

namespace DAL.Repositories.Services.Interfaces
{
    public interface ILenderServices
    {
        Task<ResBalanceDto> GetBalanceDto(string lenderId);
        Task<string> UpdateBalance(string lenderId, ReqUpdateBalanceDto balanceDto);
        Task<string> WithdrawBalance(string id, ReqWithdrawBalanceDto withdrawDto);
        Task<ResUserDto> GetUserById(string id);
        Task<List<ResBorrowerDto>> GetBorrowersByLenderId(string lenderId);
    }
}
