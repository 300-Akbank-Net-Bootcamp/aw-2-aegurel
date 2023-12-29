// StaffController sınıfı VbApi.Controllers namespace'i altında oluşturulduğunu varsayalım
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vb.Data;
using Vb.Data.Entity;

namespace VbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly VbDbContext dbContext;

        public StaffController(VbDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Staff>>> Get()
        {
            return await dbContext.Staff.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Staff>> Get(int id)
        {
            var staff = await dbContext.Staff.FindAsync(id);

            if (staff == null)
            {
                return NotFound();
            }

            return staff;
        }

        [HttpPost]
        public async Task<ActionResult<Staff>> Post([FromBody] Staff staff)
        {
            dbContext.Staff.Add(staff);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = staff.Id }, staff);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Staff staff)
        {
            if (id != staff.Id)
            {
                return BadRequest();
            }

            dbContext.Entry(staff).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await dbContext.Staff.FindAsync(id);

            if (staff == null)
            {
                return NotFound();
            }

            dbContext.Staff.Remove(staff);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool StaffExists(int id)
        {
            return dbContext.Staff.Any(e => e.Id == id);
        }
    }
}
