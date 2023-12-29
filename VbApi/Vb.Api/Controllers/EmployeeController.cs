// Employee sınıfı Vb.Data.Entity namespace'i altında oluşturulduğunu varsayalım
using System;
using System.ComponentModel.DataAnnotations;
using Vb.Data.Entity;

namespace Vb.Data.Entity
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 250, MinimumLength = 10, ErrorMessage = "Invalid Name")]
        public string Name { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Phone is not valid.")]
        public string Phone { get; set; }

        [Range(minimum: 50, maximum: 400, ErrorMessage = "Hourly salary does not fall within allowed range.")]
        public double HourlySalary { get; set; }
    }
}

// EmployeeController sınıfı VbApi.Controllers namespace'i altında oluşturulduğunu varsayalım
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vb.Data;
using Vb.Data.Entity;

namespace VbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly VbDbContext dbContext;

        public EmployeeController(VbDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> Get()
        {
            return await dbContext.Employees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> Get(int id)
        {
            var employee = await dbContext.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> Post([FromBody] Employee employee)
        {
            dbContext.Employees.Add(employee);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            dbContext.Entry(employee).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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
            var employee = await dbContext.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            dbContext.Employees.Remove(employee);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return dbContext.Employees.Any(e => e.Id == id);
        }
    }
}
