using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoTasksController : ControllerBase
    {
        private readonly ToDoListContext _context;

        public ToDoTasksController(ToDoListContext context)
        {
            _context = context;
        }

        // GET: api/ToDoTasks
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ToDoTask>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ToDoTask>>> GetToDoTasks()
        {
            try
            {
                return Ok(await _context.ToDoTasks.ToListAsync());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        // GET: api/ToDoTasks/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ToDoTask))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ToDoTask>> GetToDoTask(int id)
        {
            try
            {
                var toDoTask = await _context.ToDoTasks.FindAsync(id);

                if (toDoTask == null)
                {
                    return NotFound("This task does not exist");
                }

                return Ok(toDoTask);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        // PUT: api/ToDoTasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutToDoTask(int id, ToDoTask toDoTask)
        {
            if (id != toDoTask.Id)
            {
                return BadRequest();
            }

            _context.Entry(toDoTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoTaskExists(id))
                {
                    return NotFound("This task does not exist");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error updating the record");
                }
            }

            return NoContent();
        }

        // POST: api/ToDoTasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ToDoTask))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ToDoTask>> PostToDoTask(ToDoTask toDoTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.ToDoTasks.Add(toDoTask);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetToDoTask", new { id = toDoTask.Id }, toDoTask);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the record");
            }
        }

        // DELETE: api/ToDoTasks/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteToDoTask(int id)
        {
            try
            {
                var toDoTask = await _context.ToDoTasks.FindAsync(id);
                if (toDoTask == null)
                {
                    return NotFound("This task does not exist");
                }

                _context.ToDoTasks.Remove(toDoTask);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting the record");
            }
        }

        // PATCH: api/ToDoTasks/5
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchToDoTask(int id, [FromBody] JsonPatchDocument<ToDoTask> patchDoc)
        {
            if (patchDoc == null || id <= 0)
            {
                return BadRequest("Invalid patch document or ID.");
            }

            var existingTask = await _context.ToDoTasks.FindAsync(id);
            if (existingTask == null)
            {
                return NotFound("This task does not exist.");
            }

            patchDoc.ApplyTo(existingTask, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoTaskExists(id))
                {
                    return NotFound("This task does not exist.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool ToDoTaskExists(int id)
        {
            return _context.ToDoTasks.Any(e => e.Id == id);
        }
    }
}
