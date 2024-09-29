using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BEPeer.Controllers
{
    [Route("rest/v1/lender/[action]")]
    [ApiController]
    public class LenderController : ControllerBase
    {
        private readonly ILenderServices _lenderServices;

        public LenderController(ILenderServices lenderServices)
        {
            _lenderServices = lenderServices;
        }

        [HttpGet]
        public async Task<ActionResult<ResBalanceDto>> GetBalance(string id)
        {
            var balance = await _lenderServices.GetBalanceDto(id);
            return Ok(balance);
        }

        [HttpPut]
        public async Task<ActionResult<string>> UpdateBalance(string id, [FromBody] ReqUpdateBalanceDto balanceDto)
        {
            var result = await _lenderServices.UpdateBalance(id, balanceDto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> WithdrawBalance(string id, [FromBody] ReqWithdrawBalanceDto withdrawDto)
        {
            if (withdrawDto.Amount <= 0)
            {
                return BadRequest("Jumlah yang ditarik harus lebih dari nol.");
            }

            var result = await _lenderServices.WithdrawBalance(id, withdrawDto);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBorrowers(string id)
        {

            var lender = await _lenderServices.GetUserById(id); 
            if (lender == null)
            {
                return NotFound(new { Message = "Lender dengan ID ini tidak ditemukan." });
            }

            var borrowers = await _lenderServices.GetBorrowersByLenderId(id);

            if (borrowers == null || borrowers.Count == 0)
            {
                return Ok(new { Message = "Tidak ada borrower yang pernah mengajukan pinjaman ke lender ini." });
            }

            return Ok(borrowers);
        }
    }
}
