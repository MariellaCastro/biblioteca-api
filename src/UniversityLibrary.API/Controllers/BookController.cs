using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityLibrary.Application.DTOs.Book;
using UniversityLibrary.Application.Interfaces;
using UniversityLibrary.Domain.Ports.Out;

namespace UniversityLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IUnitOfWork _unitOfWork;

        public BooksController(IBookService bookService, IUnitOfWork unitOfWork)
        {
            _bookService = bookService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            var books = await _bookService.GetAllAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
                return NotFound(new { message = $"Book with ID {id} not found." });

            return Ok(book);
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<BookDto>> GetByISBN(string isbn)
        {
            var book = await _bookService.GetByISBNAsync(isbn);
            if (book == null)
                return NotFound(new { message = $"Book with ISBN {isbn} not found." });

            return Ok(book);
        }

        [HttpGet("author/{author}")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetByAuthor(string author)
        {
            var books = await _bookService.GetByAuthorAsync(author);
            return Ok(books);
        }

        [HttpGet("title/{title}")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetByTitle(string title)
        {
            var books = await _bookService.GetByTitleAsync(title);
            return Ok(books);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAvailableBooks()
        {
            var books = await _bookService.GetAvailableBooksAsync();
            return Ok(books);
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
        {
            try
            {
                var book = await _bookService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _bookService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = $"Book with ID {id} not found." });

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

        [HttpPost("{id}/dar-baja")]
        public async Task<IActionResult> DarBaja(int id, [FromBody] DarBajaRequestDto request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var book = await _unitOfWork.Books.GetWithLoansAsync(id);
                if (book == null)
                {
                    return NotFound(new { message = $"Libro con ID {id} no encontrado." });
                }

                bool hasActiveLoans = false;
                foreach (var loan in book.Loans)
                {
                    if (loan.Status == "Active")
                    {
                        hasActiveLoans = true;
                        break;
                    }
                }

                if (hasActiveLoans)
                {
                    return BadRequest(new { message = $"No se puede dar de baja el libro '{book.Title}' porque tiene préstamos activos." });
                }

                string mensaje = "";
                bool tieneStockParaLiquidar = book.Stock > 0;

                if (tieneStockParaLiquidar)
                {
                    mensaje = $"Libro '{book.Title}' dado de baja exitosamente. Se liquidaron {book.Stock} unidades del stock disponible.";
                    
                }
                else
                {
                    mensaje = $"Libro '{book.Title}' dado de baja exitosamente. No había stock para liquidar.";
                }

                await _unitOfWork.Books.DeleteAsync(id);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return Ok(new
                {
                    success = true,
                    message = mensaje,
                    bookId = id,
                    bookTitle = book.Title,
                    stockLiquidado = tieneStockParaLiquidar ? book.Stock : 0,
                    fechaBaja = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return BadRequest(new { message = $"Error al dar de baja el libro: {ex.Message}" });
            }
        }

        [HttpGet("{id}/previsualizar-baja")]
        public async Task<IActionResult> PrevisualizarBaja(int id)
        {
            try
            {
                var book = await _unitOfWork.Books.GetWithLoansAsync(id);
                if (book == null)
                {
                    return NotFound(new { message = $"Libro con ID {id} no encontrado." });
                }

                bool tienePrestamosActivos = false;
                int prestamosActivosCount = 0;
                
                foreach (var loan in book.Loans)
                {
                    if (loan.Status == "Active")
                    {
                        tienePrestamosActivos = true;
                        prestamosActivosCount++;
                    }
                }

                bool sePuedeDarDeBaja = !tienePrestamosActivos;
                string mensaje = sePuedeDarDeBaja 
                    ? "El libro puede ser dado de baja." 
                    : $"El libro NO puede ser dado de baja porque tiene {prestamosActivosCount} préstamo(s) activo(s).";

                return Ok(new
                {
                    libro = new
                    {
                        id = book.Id,
                        titulo = book.Title,
                        autor = book.Author,
                        isbn = book.ISBN,
                        stock = book.Stock,
                        tieneStock = book.Stock > 0
                    },
                    sePuedeDarDeBaja = sePuedeDarDeBaja,
                    mensaje = mensaje,
                    tienePrestamosActivos = tienePrestamosActivos,
                    prestamosActivosCount = prestamosActivosCount,
                    fechaConsulta = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error al previsualizar la baja: {ex.Message}" });
            }
        }
    }

    public class DarBajaRequestDto
    {
        public string Motivo { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
    }
}