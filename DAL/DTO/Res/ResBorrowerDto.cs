using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResBorrowerDto
    {
        public string BorrowerId { get; set; }
        public string BorrowerName { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
    }
}
