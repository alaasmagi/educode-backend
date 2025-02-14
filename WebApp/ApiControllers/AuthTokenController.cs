using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;

namespace WebApp.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthTokenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthTokenController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AuthToken
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAuthTokenEntity>>> GetUserAuthTokens()
        {
            return await _context.UserAuthTokens.Include(u=> u.User).Include(u=> u.User.UserType).ToListAsync();
        }

        // GET: api/AuthToken/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAuthTokenEntity>> GetUserAuthTokenEntity(int id)
        {
            var userAuthTokenEntity = await _context.UserAuthTokens.FindAsync(id);

            if (userAuthTokenEntity == null)
            {
                return NotFound();
            }

            return userAuthTokenEntity;
        }

        // PUT: api/AuthToken/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAuthTokenEntity(int id, UserAuthTokenEntity userAuthTokenEntity)
        {
            if (id != userAuthTokenEntity.Id)
            {
                return BadRequest();
            }

            _context.Entry(userAuthTokenEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAuthTokenEntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AuthToken
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserAuthTokenEntity>> PostUserAuthTokenEntity(UserAuthTokenEntity userAuthTokenEntity)
        {
            _context.UserAuthTokens.Add(userAuthTokenEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserAuthTokenEntity", new { id = userAuthTokenEntity.Id }, userAuthTokenEntity);
        }

        // DELETE: api/AuthToken/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAuthTokenEntity(int id)
        {
            var userAuthTokenEntity = await _context.UserAuthTokens.FindAsync(id);
            if (userAuthTokenEntity == null)
            {
                return NotFound();
            }

            _context.UserAuthTokens.Remove(userAuthTokenEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserAuthTokenEntityExists(int id)
        {
            return _context.UserAuthTokens.Any(e => e.Id == id);
        }
    }
}
