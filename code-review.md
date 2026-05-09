# Code Review Documentation

## Task 1: SOLID Principles Violations and Fixes

### 1. Dependency Inversion Principle
* **The violation:** In `Program.cs`,the Dependency Injection Principle was broken, because the application was not told how to build the dependencies for the controller. Even though the `ItemController` correctly depended on the `IItemReader` interface, the link between that interface and the `ItemRepository` class was missing. Therefore, the system did not know which class to use, it could not start the controller and the app failed.
* **The fix:** I registered the `ItemRepository` as a Scoped service in the `Program.cs` file. This provides an `ItemRepository` whenever a class requests an `IItemReader`.

**Applied fix:**
```csharp
// Program.cs - line 7
buikder.Services.AddScoped<IItemReader, ItemRepository>();
```



### 2. Single Responsibility Principle
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



### 3. Open/Closed Principle
* **The violation:** In `ItemRepository.cs`, the Open/Closed Principle was broken, because the repository was using a hardcoded list of items, initialized in the constructor. If the data source needs to change, the `ItemRepository.cs` also needs to change, which is against the "open for extension, closed for modification" rule.
* **The line:** 8
* **The fix:** It will be resolved in Task 4 by refactoring the repository to fetch data from an external endpoint.



## Task 2: Framework Update
* **The fix:** I updated the target framework in the project file from `net8.0` to `net10.0`.
* **Aplied fix:**
```xml
    <TargetFramework>net10.0</TargetFramework>
```



## Task 3: Service Layer
* **The fix:** I implemented a new function in the `GradeService.cs` called `GetFirstNPassingGrades`, which takes a list of items and an integer `n` as parameters. This function returns the first `n` items that have a value greater than or equal to 5 and that are active. I also added a new endpoint in the `ItemController.cs` called `GetTopGrades` to expose this functionality via an HTTP GET request.
* **Applied fix:**
```csharp
    // GradeService.cs

    public IEnumerable<Item> GetFirstNPassingGrades(IEnumerable<Item> items, int n)
    {
        return items.Where(i => i.IsActive && i.Value >= 5).Take(n).ToList();
    }


    // ItemController.cs

    [HttpGet("top/{n}")]
    public async Task<IActionResult> GetTopGrades(int n)
    {
        var items = await _reader.GetAllAsync();
        var topGrades = _gradeService.GetFirstNPassingGrades(items, n);
        return Ok(topGrades);
    }

```



## Task 4: Repository Refactoring
* **The fix:** I created a new file called `ExternalItemRepository.cs` to fetch data from an external endpoint instead of using a hardcoded list. I used `HttpClient` to make an asynchronous GET request to the endpoint. This way, the repository is now open for extension without modification, adhering to the Open/Closed Principle. Moreover, I modified the `Program.cs` file, to include the new repository. 
* **Aplied fix:**
```csharp
    // ExternalItemRepository.cs

    public class ExternalItemRepository : IItemReader
    {
        private readonly HttpClient _httpClient;
        private const string Url = "https://gist.githubusercontent.com/ArdeleanTudor/8ea407832cd9794960e0e6bbd1319f6e/raw/145b121103dd1%E2%80%A6";


        public ExternalItemRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<Item>>(Url);
                return items ?? new List<Item>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return new List<Item>();
            }
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            var items = await GetAllAsync();
            return items.FirstOrDefault(i => i.Id == id);
        }
    }


    // Program.cs

    builder.Services.AddHttpClient();
    builder.Services.AddScoped<IItemReader, ExternalItemRepository>();
    

    // Item.cs

    public class Item
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;
    }

```
