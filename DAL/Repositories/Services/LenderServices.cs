using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Services
{
    public class LenderServices : ILenderServices
    {
        private readonly PeerlandingContext _context;

        public LenderServices (PeerlandingContext context)
        {
            _context = context;
        }

        public async Task<ResBalanceDto> GetBalanceDto (string lenderId)
        {
            var lender = await _context.MstUsers.FindAsync(lenderId);
            if (lender == null)
            {
                throw new Exception("Lender not found");
            }

            return new ResBalanceDto
            {
                Balance = lender.Balance ?? 0
            };
        }

        public async Task<string> UpdateBalance(string lenderId, ReqUpdateBalanceDto balanceDto)
        {
            var lender = await _context.MstUsers.FindAsync(lenderId);
            if (lender == null)
            {
                throw new Exception("Lender not found");
            }

            lender.Balance = balanceDto.Balance;
            _context.MstUsers.Update(lender);
            await _context.SaveChangesAsync();

            return $"Balance for lender {lender.Name} has been updated.";
        }

        public async Task<string> WithdrawBalance(string id, ReqWithdrawBalanceDto withdrawDto)
        {
            var user = await _context.MstUsers.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user.Balance < withdrawDto.Amount)
            {
                throw new Exception("Saldo tidak cukup");
            }

            user.Balance -= withdrawDto.Amount;

            _context.MstUsers.Update(user);
            await _context.SaveChangesAsync();

            return $"Saldo telah berhasil ditarik. Sisa saldo: {user.Balance}";
        }

        public async Task<List<ResBorrowerDto>> GetBorrowersByLenderId(string lenderId)
        {
            var loans = await _context.MstLoans
                .Where(l => l.User.Id == lenderId) 
                .Select(l => new ResBorrowerDto
                {
                    BorrowerId = l.BorrowerId,
                    BorrowerName = l.User.Name,
                    Status = l.Status,
                    Amount = l.Amount,
                    InterestRate = l.InterestRate,
                    Duration = l.Duration
                })
                .ToListAsync();

            return loans;
        }
        public async Task<ResUserDto> GetUserById(string id)
        {
            
            var user = await _context.MstUsers
                .Where(u => u.Id == id)
                .Select(u => new ResUserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    Balance = u.Balance
                })
                .FirstOrDefaultAsync();

            return user;
        }
    }
}

