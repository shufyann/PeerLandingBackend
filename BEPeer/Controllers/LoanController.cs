using System.Text;
using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BEPeer.Controllers
{
    [Route("rest/v1/loan/[action]")]
    [ApiController]
    public class LoanController : Controller
    {
        private readonly ILoanServices _loanServices;
        public LoanController(ILoanServices loanServices)
        {
            _loanServices = loanServices;
        }

        [HttpPost]
        public async Task<IActionResult> NewLoan(ReqLoanDto loan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();
                    var errorMessage = new StringBuilder("Validation errors occured!");

                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }
                var res = await _loanServices.CreateLoan(loan);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Success add loan data",
                    Data = res
                });
            }
            catch (Exception ex) 
            {
                if (ex.Message == "email already used")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateLoan(ReqUpdateLoanDto updateLoan, string Id)
        {
            try
            {
                var result = await _loanServices.UpdateLoan(updateLoan, Id);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = result,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoanList(string status="")
        {
            try 
            {
                var result = await _loanServices.GetLoanList(status);

                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success load loan",
                    Data = result
                });
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRequestedLoans()
        {
            var loans = await _loanServices.GetLoanList("requested"); 
            return Ok(loans);
        }


    }
}
