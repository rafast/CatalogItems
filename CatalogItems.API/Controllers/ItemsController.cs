using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogItems.API.Entities;
using CatalogItems.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CatalogItems.API.Controllers
{
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        private readonly IItemsRepository repository;
        private readonly ILogger<ItemsController> logger;

        public ItemsController(IItemsRepository repository, ILogger<ItemsController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync(string name = null)
        {
            var items = (await repository.GetItemsAsync())
                   .Select(item => item.AsDto());

            if (!string.IsNullOrWhiteSpace(name))
            {
                items = items.Where(item => item.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Retrieved {items.Count()} items.");

            return items;
        }

        // GET api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = await repository.GetItemAsync(id);

            if (item == null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        // POST api/Items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync([FromBody] CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Description = itemDto.Description,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
        }

        // PUT api/Items/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(Guid id, [FromBody] UpdateItemDto itemDto)
        {
            var existingItem = await repository.GetItemAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            existingItem.Name = itemDto.Name;
            existingItem.Price = itemDto.Price;

            await repository.UpdateItemAsync(existingItem);

            return NoContent();
        }

        // DELETE api/Items/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var existingItem = await repository.GetItemAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            await repository.DeleteItemAsync(id);
            return NoContent();
        }
    }
}

