using Microsoft.AspNetCore.Mvc;
using UniversityLibrary.Application.DTOs.Loan;
using UniversityLibrary.Application.Interfaces;


namespace UniversityLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetAll()
        {
            var loans = await _loanService.GetAllAsync();
            return Ok(loans);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoanDto>> GetById(int id)
        {
            var loan = await _loanService.GetByIdAsync(id);
            if (loan == null)
                return NotFound(new { message = $"Loan with ID {id} not found." });

            return Ok(loan);
        }

        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetByBookId(int bookId)
        {
            var loans = await _loanService.GetByBookIdAsync(bookId);
            return Ok(loans);
        }

        [HttpGet("student/{studentName}")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetByStudentName(string studentName)
        {
            var loans = await _loanService.GetByStudentNameAsync(studentName);
            return Ok(loans);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetActiveLoans()
        {
            var loans = await _loanService.GetActiveLoansAsync();
            return Ok(loans);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetOverdueLoans()
        {
            var loans = await _loanService.GetOverdueLoansAsync();
            return Ok(loans);
        }

        [HttpGet("can-borrow/{bookId}")]
        public async Task<ActionResult<bool>> CanBorrowBook(int bookId)
        {
            var canBorrow = await _loanService.CanBorrowBookAsync(bookId);
            return Ok(canBorrow);
        }

        [HttpPost]
        public async Task<ActionResult<LoanDto>> Create([FromBody] CreateLoanDto dto)
        {
            try
            {
                var loan = await _loanService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/return")]
        public async Task<ActionResult<LoanDto>> ReturnLoan(int id)
        {
            try
            {
                var loan = await _loanService.ReturnLoanAsync(id);
                return Ok(loan);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _loanService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = $"Loan with ID {id} not found." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}