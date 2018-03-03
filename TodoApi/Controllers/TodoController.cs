using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly TodoContext context;

        public TodoController(TodoContext context)
        {
            this.context = context;

            // ReSharper disable once InvertIf
            if (!this.context.TodoItems.Any())
            {
                // Persist sample value
                context.TodoItems.Add(new TodoItem {Title = "Study .NET Core"});
                context.SaveChanges();
            }
        }

        [HttpGet]
        public IEnumerable<TodoItem> Get()
        {
            return context.TodoItems.ToList();
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(long id)
        {
            var item = context.TodoItems.FirstOrDefault(t => t.Id == id);

            if (item == null)
                return NotFound();            

            return Ok(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] TodoItem item)
        {
            if (item == null)
                return BadRequest();
            

            // TODO: Ideally, should be placed inside a TodoService
            context.TodoItems.Add(item);
            context.SaveChanges();

            // Status Code: 201 Created, but responds with the created object as if using GetById route
            return CreatedAtRoute("GetTodo", new {id = item.Id}, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TodoItem item)
        {
            // According to HTTP spec, a HTTP PUT request requires the client to send the entire updated entity, not just the deltas
            // To support partial updates, use HTTP PATCH

            if (item == null || item.Id != id)
                return BadRequest();

            var todo = context.TodoItems.FirstOrDefault(t => t.Id == id);

            if (todo == null)
                return NotFound();

            todo.IsComplete = item.IsComplete;
            todo.Title = item.Title;

            context.TodoItems.Update(todo);
            context.SaveChanges();

            // Status Code: 200 No Content - standard for HTTP PUT requests
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = context.TodoItems.FirstOrDefault(t => t.Id == id);

            if (todo == null)
                return NotFound();

            context.TodoItems.Remove(todo);
            context.SaveChanges();

            return NoContent();
        }

    }
}
