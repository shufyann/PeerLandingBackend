using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DAL.Repositories.Services
{
    public class UserServices : IUserServices
    {
        private readonly PeerlandingContext _context;
        private readonly IConfiguration _configuration;

        public UserServices(PeerlandingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> Register(ReqRegisterUserDto register)
        {
            try
            {
                
                var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == register.Email);
                if (isAnyEmail != null)
                {
                    throw new Exception("email already used");
                }

               
                var newUser = new MstUser
                {
                    Name = register.Name,
                    Email = register.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(register.Password), // Hash the password
                    Role = register.Role,
                    Balance = register.Balance,
                };

               
                await _context.MstUsers.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return newUser.Name;
            }
            catch (Exception ex)
            {
                
                throw new Exception("An error occurred while registering the user: " + ex.Message);
            }
        }

        // Get method
        public async Task<List<ResUserDto>> GetAllUsers()
        {
            return await _context.MstUsers
                .Where(user => user.Role == "lender" || user.Role == "borrower")
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance,
                }).ToListAsync();

        }

        // Login method
        public async Task<ResLoginDto>Login(ReqLoginDto reqLogin)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e=>e.Email == reqLogin.Email);
            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(reqLogin.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid email or password");
            }

            var token = GenerateJwtToken(user);

            var loginResponse = new ResLoginDto
            {
                Id = user.Id,
                Token = token,
                Role = user.Role
            };

            return loginResponse;
        }

        private string GenerateJwtToken(MstUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                    issuer: jwtSettings["ValidIssuer"],
                    audience: jwtSettings["ValidAudience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Update Method
        public async Task<string> UpdateUser(ReqUpdateUserDto updateUser, string Id)
        {
            try
            {
                var user = await _context.MstUsers.FindAsync(Id);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                
                user.Name = updateUser.Name;
                user.Role = updateUser.Role;
                user.Balance = updateUser.Balance;

                _context.MstUsers.Update(user);
                await _context.SaveChangesAsync();

                return $"User {user.Name} has been updated successfully.";
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the user: " + ex.Message);
            }
        }

        // Delete Method
        public async Task<ResDeleteDto> Delete(string id)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Id == id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            _context.MstUsers.Remove(user);
            await _context.SaveChangesAsync();
            var deleteResponse = new ResDeleteDto
            {
                Message = "User deleted"
            };

            return deleteResponse;

        }

        //Get Id Method
        public async Task<ResGetUserIdDto> GetUserId(string Id)
        {
            return await _context.MstUsers
                .Where(user => user.Id == Id)
                .Select(user => new ResGetUserIdDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Role = user.Role,
                    Balance = user.Balance,
                }).SingleOrDefaultAsync();
        }

        
    }
}
