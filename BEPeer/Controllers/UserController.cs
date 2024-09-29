using System.Reflection.Metadata.Ecma335;
using System.Text;
using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BEPeer.Controllers
{
    [Route("rest/v1/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> Register(ReqRegisterUserDto register)
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

                var res = await _userServices.Register(register);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User registered",
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

        // Get all users method
        [HttpGet]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userServices.GetAllUsers();

                return Ok(new ResBaseDto<List<ResUserDto>>
                {
                    Success = true,
                    Message = "List of users",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<ResUserDto>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        //login method
        [HttpPost]
        public async Task<IActionResult> Login(ReqLoginDto login)
        {
            try 
            {
                var user = await _userServices.Login(login);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User Logged in!",
                    Data = user,
                });
            }

            catch (Exception ex)
            {
                if (ex.Message== "Invalid email or password")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success= false,
                        Message = ex.Message,
                        Data =null,
                    });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                });
            }
        }

        //Update users method
        [HttpPut]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUser(ReqUpdateUserDto updateUser, string Id)
        {
            try
            {
                var result = await _userServices.UpdateUser(updateUser, Id);
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
        
        //Delete users method
        [HttpDelete]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var response = await _userServices.Delete(id);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User Deleted",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResBaseDto<object>
                {
                    Success = false,
                    Message = "Unauthorized"
                });
            }
        }

        //Get users by Id
        [HttpGet]
        public async Task<IActionResult> GetUserId(string Id)
        {
            try
            {
                var users = await _userServices.GetUserId(Id);

                return Ok(new ResBaseDto<ResGetUserIdDto>
                {
                    Success = true,
                    Message = "Berhasil menampilkan data Id",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<ResGetUserIdDto>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            return Ok(new ResBaseDto<ResGetUserIdDto>());

        }


    }
}
