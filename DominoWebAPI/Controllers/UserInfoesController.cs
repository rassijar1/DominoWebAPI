using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DominoWebAPI.Models;

namespace DominoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public UserInfoesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/UserInfoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetUserinfo()
        {
          if (_context.Userinfo == null)
          {
              return NotFound();
          }
            return await _context.Userinfo.ToListAsync();
        }

        // GET: api/UserInfoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserInfo>> GetUserInfo(int id)
        {
          if (_context.Userinfo == null)
          {
              return NotFound();
          }
            var userInfo = await _context.Userinfo.FindAsync(id);

            if (userInfo == null)
            {
                return NotFound();
            }

            return userInfo;
        }

        // PUT: api/UserInfoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserInfo(int id, UserInfo userInfo)
        {
            if (id != userInfo.UserId)
            {
                return BadRequest();
            }

            _context.Entry(userInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserInfoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Usuario actualizado correctamente" });
        }

        // POST: api/UserInfoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserInfo>> PostUserInfo(UserInfo userInfo)
        {
          if (_context.Userinfo == null)
          {
              return Problem("Entity set 'ApplicationDBContext.Userinfo'  is null.");
          }
            _context.Userinfo.Add(userInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserInfo", new { id = userInfo.UserId }, userInfo);
        }

        // DELETE: api/UserInfoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserInfo(int id)
        {
            if (_context.Userinfo == null)
            {
                return NotFound();
            }
            var userInfo = await _context.Userinfo.FindAsync(id);
            if (userInfo == null)
            {
                return NotFound();
            }

            _context.Userinfo.Remove(userInfo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario eliminado correctamente" });
        }

        private bool UserInfoExists(int id)
        {
            return (_context.Userinfo?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
