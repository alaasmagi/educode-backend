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
    public class UserAuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserAuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserAuth
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAuthEntity>>> GetUserAuthData()
        {
            return await _context.UserAuthData.ToListAsync();
        }

        // GET: api/UserAuth/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAuthEntity>> GetUserAuthEntity(int id)
        {
            var userAuthEntity = await _context.UserAuthData.FindAsync(id);

            if (userAuthEntity == null)
            {
                return NotFound();
            }

            return userAuthEntity;
        }

        // PUT: api/UserAuth/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAuthEntity(int id, UserAuthEntity userAuthEntity)
        {
            if (id != userAuthEntity.Id)
            {
                return BadRequest();
            }

            _context.Entry(userAuthEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAuthEntityExists(id))
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

        // POST: api/UserAuth
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserAuthEntity>> PostUserAuthEntity(UserAuthEntity userAuthEntity)
        {
            _context.UserAuthData.Add(userAuthEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserAuthEntity", new { id = userAuthEntity.Id }, userAuthEntity);
        }

        // DELETE: api/UserAuth/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAuthEntity(int id)
        {
            var userAuthEntity = await _context.UserAuthData.FindAsync(id);
            if (userAuthEntity == null)
            {
                return NotFound();
            }

            _context.UserAuthData.Remove(userAuthEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserAuthEntityExists(int id)
        {
            return _context.UserAuthData.Any(e => e.Id == id);
        }
    }
}
