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
    public class UserController(AppDbContext context) : ControllerBase
    {
        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetUsers()
        {
            return await context.Users.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserEntity>> GetUserEntity(int id)
        {
            var userEntity = await context.Users.FindAsync(id);

            if (userEntity == null)
            {
                return NotFound();
            }

            return userEntity;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserEntity(int id, UserEntity userEntity)
        {
            if (id != userEntity.Id)
            {
                return BadRequest();
            }

            context.Entry(userEntity).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserEntityExists(id))
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

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserEntity>> PostUserEntity(UserEntity userEntity)
        {
            context.Users.Add(userEntity);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetUserEntity", new { id = userEntity.Id }, userEntity);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserEntity(int id)
        {
            var userEntity = await context.Users.FindAsync(id);
            if (userEntity == null)
            {
                return NotFound();
            }

            context.Users.Remove(userEntity);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserEntityExists(int id)
        {
            return context.Users.Any(e => e.Id == id);
        }
    }
}
