using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BranchCrudAPI_JWT.Data;
using BranchCrudAPI_JWT.Models;
using Microsoft.AspNetCore.Authorization;

namespace BranchCrudAPI_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly BranchCrudAPI_JWTContext _context;

        public BranchesController(BranchCrudAPI_JWTContext context)
        {
            _context = context;
        }

        // GET: api/Branches
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranch()
        {
          if (_context.Branch == null)
          {
              return NotFound();
          }
            return await _context.Branch.ToListAsync();
        }

        // GET: api/Branches/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetBranch(int id)
        {
          if (_context.Branch == null)
          {
              return NotFound();
          }
            var branch = await _context.Branch.FindAsync(id);

            if (branch == null)
            {
                return NotFound();
            }

            return branch;
        }

        // PUT: api/Branches/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranch(int id, Branch branch)
        {
            if (id != branch.Id)
            {
                return BadRequest();
            }

            _context.Entry(branch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
            return Ok(new { Message = "Branch Updated Successfully" });
        }

        // POST: api/Branches
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Branch>> PostBranch(Branch branch)
        {
          if (_context.Branch == null)
          {
              return Problem("Entity set 'BranchCrudAPI_JWTContext.Branch'  is null.");
          }
            _context.Branch.Add(branch);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBranch", new { id = branch.Id }, branch);
        }

        // DELETE: api/Branches/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            if (_context.Branch == null)
            {
                return NotFound();
            }
            var branch = await _context.Branch.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            _context.Branch.Remove(branch);
            await _context.SaveChangesAsync();

            //return NoContent();
            return Ok(new { Message = "Branch Deleted Successfully" });
        }

        private bool BranchExists(int id)
        {
            return (_context.Branch?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
