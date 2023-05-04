using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DominoWebAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace DominoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DominoesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public DominoesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Dominoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Domino>>> GetDomino()
        {
          if (_context.Domino == null)
          {
              return NotFound();
          }
            return await _context.Domino.ToListAsync();
        }

        // GET: api/Dominoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Domino>> GetDomino(int id)
        {
          if (_context.Domino == null)
          {
              return NotFound();
          }
            var domino = await _context.Domino.FindAsync(id);

            if (domino == null)
            {
                return NotFound();
            }

            return domino;
        }

        // PUT: api/Dominoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDomino(int id, Domino domino)
        {
            if (id != domino.GameId)
            {
                return BadRequest();
            }

            _context.Entry(domino).State = EntityState.Modified;

            try
            {
                string input = domino.DominoGame;
                if (input.Length < 7 || input.Length > 23)
                {
                    return BadRequest(new { message = "La cadena debe tener minimo 2 y maximo 6 fichas" });
                }
                else
                {
                    var dominoes = DominosGame.ParseDominoes(input);


                    string dominoGame = DominosGame.Chain(dominoes.OrderBy(x => Guid.NewGuid()));

                    domino.DominoGame = dominoGame;

                    if (dominoGame == "El juego no es valido")
                    {
                        return BadRequest(new { message = dominoGame });
                    }
                    else
                    {
                        
                        await _context.SaveChangesAsync();
                    }
                    //await _context.SaveChangesAsync();
                }
              
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DominoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Juego de dominó actualizado correctamente"});
           
        }

        // POST: api/Dominoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Domino>> PostDomino(Domino domino)
        {
            if (_context.Domino == null)
            {
                return Problem("Entity set 'ApplicationDBContext.Domino'  is null.");
            }

            string input = domino.DominoGame;
            if (input.Length < 7 || input.Length > 23)
            {
                return BadRequest(new { message = "La cadena debe tener minimo 2 y maximo 6 fichas" });
            }
            else
            {

                var dominoes = DominosGame.ParseDominoes(input);


                string dominoGame = DominosGame.Chain(dominoes.OrderBy(x => Guid.NewGuid()));

                domino.DominoGame = dominoGame;

                if (dominoGame == "El juego no es valido")
                {
                    return BadRequest(new { message = dominoGame });
                }
                else
                {
                    _context.Domino.Add(domino);
                    await _context.SaveChangesAsync();
                }
            }
            
            

            return CreatedAtAction("GetDomino", new { id = domino.GameId }, domino);
        }

        // DELETE: api/Dominoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDomino(int id)
        {
            if (_context.Domino == null)
            {
                return NotFound();
            }
            var domino = await _context.Domino.FindAsync(id);
            if (domino == null)
            {
                return NotFound();
            }

            _context.Domino.Remove(domino);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Juego de domino eliminado correctamente" });
        }

        private bool DominoExists(int id)
        {
            return (_context.Domino?.Any(e => e.GameId == id)).GetValueOrDefault();
        }
    }

    public static class DominosGame
    {
        public static string Chain(IEnumerable<(int, int)> dominoes) => TryChain(dominoes.ToList(), (0, 0), "");

        public static string TryChain(List<(int, int)> dominoes, (int first, int last) state, string currentChain)
        {
            if (dominoes.Count == 0 && state.last == state.first)
                return currentChain;
            for (int i = 0; i < dominoes.Count; i++)
            {
                var (a, b) = dominoes[i];
                if (state.last == 0)
                {
                    state = (a, b);
                    currentChain += $"{a}-{b}";
                }
                else if (state.last == a)
                {
                    state.last = b;
                    currentChain += $",{a}-{b}";
                }
                else if (state.last == b)
                {
                    state.last = a;
                    currentChain += $",{b}-{a}";
                }
                else
                {
                    continue;
                }
                var dominoesCopy = new List<(int, int)>(dominoes);
                dominoesCopy.RemoveAt(i);
                var result = TryChain(dominoesCopy, state, currentChain);
                if (result != null)
                {
                    return result;
                }
                int lastIndex = currentChain.LastIndexOf(",");
                currentChain = currentChain.Substring(0, lastIndex);
            }
            return "El juego no es valido";
        }

        public static List<(int, int)> ParseDominoes(string input)
        {
            var dominoes = new List<(int, int)>();
            var pairs = input.Split(',');
            foreach (var pair in pairs)
            {
                var values = pair.Trim().Split('-');
                if (values.Length != 2 || !int.TryParse(values[0], out int a) || !int.TryParse(values[1], out int b))
                {
                    throw new ArgumentException("Invalid input format");
                }
                dominoes.Add((a, b));
            }
            return dominoes;
        }
    }
}
