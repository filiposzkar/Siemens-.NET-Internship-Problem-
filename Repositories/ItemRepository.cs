using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Repositories;

public class ItemRepository : IItemReader
{

    protected readonly List<Item> _items;
    protected int _nextId = 1;

    public ItemRepository  ()
    {
        _items = new List<Item>
        {
            new Item { Id = _nextId++, Value = 85, IsActive = true },
            new Item { Id = _nextId++, Value = 90, IsActive = true },
            new Item { Id = _nextId++, Value = 78, IsActive = true }
        };
    }

    public virtual Task<Item?> GetByIdAsync(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id && i.IsActive);
        return Task.FromResult(item);
    }

    public virtual Task<IEnumerable<Item>> GetAllAsync()
    {
        var items = _items.Where(i => i.IsActive).AsEnumerable();
        return Task.FromResult(items);
    }
}
