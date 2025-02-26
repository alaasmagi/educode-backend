using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using App.BLL;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using WebApp.Models;

namespace WebApp.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManagement userManagement;
        private readonly AuthBrain authService;

        public UserController(AppDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
            userManagement = new UserManagement(_context);
            authService = new AuthBrain(config);
        }
        
        
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = userManagement.AuthenticateUser(model.UniId, model.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid UNI-ID or password" });
            }

            var token = authService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
        
        // GET: api/User
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        
        // GET: api/User/UniId/<uni-id>
        [Authorize]
        [HttpGet("UniId/{uniId}")]
        public async Task<ActionResult<UserEntity>> GetUserEntityByUniId(string uniId)
        {
            var userEntity = await _context.Users.Include(x => x.UserType)
                .FirstOrDefaultAsync(x => x.UniId == uniId);

            if (userEntity == null)
            {
                return NotFound();
            }

            return userEntity;
        }
        
        // GET: api/User/Id/5
        [Authorize]
        [HttpGet("Id/{id}")]
        public async Task<ActionResult<UserEntity>> GetUserEntity(int id)
        {
            var userEntity = await _context.Users.FindAsync(id);

            if (userEntity == null)
            {
                return NotFound();
            }

            return userEntity;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserEntity(int id, UserEntity userEntity)
        {
            if (id != userEntity.Id)
            {
                return BadRequest();
            }

            _context.Entry(userEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<UserEntity>> PostUserEntity(UserEntity userEntity)
        {
            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserEntity", new { id = userEntity.Id }, userEntity);
        }

        // DELETE: api/User/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserEntity(int id)
        {
            var userEntity = await _context.Users.FindAsync(id);
            if (userEntity == null)
            {
                return NotFound();
            }

            _context.Users.Remove(userEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserEntityExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
