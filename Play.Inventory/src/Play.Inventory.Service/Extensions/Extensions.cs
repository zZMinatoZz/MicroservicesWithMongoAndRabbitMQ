using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Play.Inventory.Service.Entities;
using static Play.Inventory.Service.Dtos.Dtos;

namespace Play.Inventory.Service.Extensions
{
    public static class Extensions
    {
        public static InventoryItemDto AsDto(this InventoryItem item, string name, string description)
        {
            return new InventoryItemDto(item.CatalogItemId, name, description, item.Quantity, item.AcquiredDate);
        }
    }
}