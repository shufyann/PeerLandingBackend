using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;

namespace DAL.Repositories.Services.Interfaces
{
    public interface ILoanServices
    {
        Task<string> CreateLoan(ReqLoanDto loan);
        Task<string> UpdateLoan(ReqUpdateLoanDto updateLoanDto, string Id);

        Task<List<ResListLoanDto>> LoanList(string status = "");

    }


}
