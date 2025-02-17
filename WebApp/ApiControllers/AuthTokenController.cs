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
    public class AuthTokenController(AppDbContext context) : ControllerBase
    {
        // GET: api/AuthToken
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAuthTokenEntity>>> GetUserAuthTokens()
        {
            return await context.UserAuthTokens.Include(u=> u.User).Include(u=> u.User.UserType).ToListAsync();
        }

        // GET: api/AuthToken/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAuthTokenEntity>> GetUserAuthTokenEntity(int id)
        {
            var userAuthTokenEntity = await context.UserAuthTokens.FindAsync(id);

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

            context.Entry(userAuthTokenEntity).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
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
            context.UserAuthTokens.Add(userAuthTokenEntity);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetUserAuthTokenEntity", new { id = userAuthTokenEntity.Id }, userAuthTokenEntity);
        }

        // DELETE: api/AuthToken/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAuthTokenEntity(int id)
        {
            var userAuthTokenEntity = await context.UserAuthTokens.FindAsync(id);
            if (userAuthTokenEntity == null)
            {
                return NotFound();
            }

            context.UserAuthTokens.Remove(userAuthTokenEntity);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserAuthTokenEntityExists(int id)
        {
            return context.UserAuthTokens.Any(e => e.Id == id);
        }
    }
}
