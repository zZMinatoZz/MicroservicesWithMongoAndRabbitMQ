using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Play.Inventory.Service.Dtos.Dtos;

namespace Play.Inventory.Service.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient httpClient;

        public CatalogClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
        {
            // send GET request to catalog api to get items by uri
            var items = await httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");

            return items;
        }
    }
}