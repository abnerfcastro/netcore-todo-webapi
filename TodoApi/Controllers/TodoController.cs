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
            if (item == null || item.Id != id)
                return BadRequest();


        }

    }
}
