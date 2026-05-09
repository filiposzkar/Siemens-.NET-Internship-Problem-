### SOLID Principles Violations & Fixes

## 1. Dependency Inversion Principle
* **The violation:** In `Program.cs`,the Dependency Injection Principle was broken, because the application was not told how to build the dependencies for the controller. Even though the `ItemController` correctly depended on the `IItemReader` interface, the link between that interface and the `ItemRepository` class was missing. Therefore, the system did not know which class to use, it could not start the controller and the app failed.
* **The fix:** I registered the `ItemRepository` as a Scoped service in the `Program.cs` file. This provides an `ItemRepository` whenever a class requests an `IItemReader`.

**Applied fix:**
```csharp
// Program.cs - line 7
buikder.Services.AddScoped<IItemReader, ItemRepository>();
```



## 2. Single Responsibility Principle
* **The violation:** In `ItemController.cs`, the Single Responsibility Principle was violated because the controller was responsible for handling multiple aspects: HTTP requests, business logic calculations, and printing on the console. This means that the controller has multiple reasons to change, which is against the principle.
* **The lines:** 20, 25, 26, 28, 45, 49, 56
* **The fix:** I created `GradeService.cs` which implements the `IGradeService` interface. This new service is responsible for all the bussiness logic that was previously in the controller. Morevover, I removed the Console.WriteLine statements, so that the controller is only responsible for handling HTTP requests and responses.

**Applied fix:**
```csharp
// IGradeService.cs

    public interface IGradeService
    {
        double CalculateAverage(IEnumerable<Item> items);
        int GetTotalCount(IEnumerable<Item> items);
    }


 // GradeService.cs

    public class GradeService : IGradeService
    {
        public double CalculateAverage(IEnumerable<Item> items)
        {
            return items.Any() ? (double)items.Average(i => i.Value) : 0;
        }

        public int GetTotalCount(IEnumerable<Item> items)
        {
            return items.Count();
        }
    }


  // ItemController.cs
     
    public async Task<IActionResult> GetAll()
    {
        var items = await _reader.GetAllAsync();
        var itemList = items.ToList();

        var totalCount = _gradeService.GetTotalCount(itemList);
        var averageValue = _gradeService.CalculateAverage(itemList);

        return Ok(new
        {
            Data = itemList,
            Statistics = new
            {
                TotalCount = totalCount,
                AverageValue = averageValue,
                RetrievedAt = DateTime.UtcNow
            }
        });
    }


  // Program.cs

  builder.Services.AddScoped<IGradeService, GradeService>();

```



## 3. Open/Closed Principle
* **The violation:** In `ItemRepository.cs`, the Open/Closed Principle was broken, because the repository was using a hardcoded list of items, initialized in the constructor. If the data source needs to change, the `ItemRepository.cs` also needs to change, which is against the "open for extension, closed for modification" rule.
* **The line:** 8
* **The fix:** It will be resolved in Requirement IV by refactoring the repository to fetch data from an external endpoint.

