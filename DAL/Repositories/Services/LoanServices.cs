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
    public class LoanServices : ILoanServices
    {
        private readonly PeerlandingContext _peerlandingContext;
        public LoanServices(PeerlandingContext peerlandingContext)
        {
            _peerlandingContext = peerlandingContext;
        }
        public async Task<string> CreateLoan(ReqLoanDto loan)
        {
            var newLoan = new MstLoans
            {
                BorrowerId = loan.BorrowerId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
            };

            await _peerlandingContext.AddAsync(newLoan);
            await _peerlandingContext.SaveChangesAsync();

            return newLoan.BorrowerId;
        }
            
        public async Task<string> UpdateLoan(ReqUpdateLoanDto updateLoan, string Id)
        {
            try
            {
                // Cari user berdasarkan ID
                var loan = await _peerlandingContext.MstLoans.FindAsync(Id);
                if (loan == null)
                {
                    throw new Exception("User not found");
                }

                // Update properti user
                
                loan.Status = updateLoan.Status;

                // Simpan perubahan ke database
                _peerlandingContext.MstLoans.Update(loan);
                await _peerlandingContext.SaveChangesAsync();

                return $"Loan {loan.Status} has been updated successfully.";
            }
            catch (Exception ex)
            {
                // Tangani kesalahan
                throw new Exception("An error occurred while updating the user: " + ex.Message);
            }
        }

        public async Task<List<ResListLoanDto>> LoanList(string status = "")
        {
            var query = _peerlandingContext.MstLoans
                .Include(l => l.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(loan => loan.Status.ToLower() == status.ToLower());
            }

            query = query.OrderByDescending(loan => loan.CreatedAt);

            var loans = await query.Select(loan => new ResListLoanDto
            {
                LoanId = loan.Id,
                BorrowerName = loan.User.Name,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
                Status = loan.Status,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt,
            }).ToListAsync();

            return loans;
        }

    }
}
