using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Extensions;
using Play.Common.Interfaces;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly ICacheService cacheService;
        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint, ICacheService cacheService)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
            this.cacheService = cacheService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            string key = "items-catalog";
            var itemsCached = cacheService.GetData<IEnumerable<ItemDto>>(key);

            if (itemsCached != null) return Ok(itemsCached);

            var items = (await itemsRepository.GetAllAsync())
                            .Select(item => item.AsDto());

            cacheService.SetData<IEnumerable<ItemDto>>(key, items, DateTimeOffset.Now.AddMinutes(5));

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            string key = $"item-{id}";
            var cachedItem = cacheService.GetData<Item>(key);
            if (cachedItem is null)
            {
                var item = await itemsRepository.GetAsync(id);

                if (item == null) return NotFound();

                cacheService.SetData<Item>(key, item, DateTimeOffset.Now.AddMinutes(5));

                return item.AsDto();
            }
            return cachedItem.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await itemsRepository.CreateAsync(item);
            // publish a message when new item is created
            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));
            // return 201 and the link reference to new item by item id
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null) return BadRequest("Can't find this item");

            item.Name = updateItemDto.Name;
            item.Description = updateItemDto.Description;
            item.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(item);

            await publishEndpoint.Publish(new CatalogItemUpdated(item.Id, item.Name, item.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);
            if (item == null) return BadRequest("Can't find this item");

            await itemsRepository.DeleteAsync(item.Id);

            await publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }
    }
}