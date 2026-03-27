# 📦 Giải Thích Chi Tiết Cấu Trúc Code

## 🏗️ Kiến Trúc Tổng Quan

```
┌─────────────────────────────────────────┐
│        ASP.NET Core Web API             │
├─────────────────────────────────────────┤
│  Controllers (API Routes)               │
│  ├── ExplanationsController.cs          │
│  └── LanguagesController.cs             │
├─────────────────────────────────────────┤
│  Services (Business Logic)              │
│  ├── ExplanationService.cs              │
│  └── LanguageService.cs                 │
├─────────────────────────────────────────┤
│  Models (Data Structures)               │
│  ├── Language.cs                        │
│  ├── Explanation.cs                     │
│  └── ExplanationContent.cs              │
├─────────────────────────────────────────┤
│  Data (Database)                        │
│  └── ApplicationDbContext.cs            │
├─────────────────────────────────────────┤
│  SQL Server (Database)                  │
│  ├── Languages Table                    │
│  ├── Explanations Table                 │
│  └── ExplanationContents Table          │
└─────────────────────────────────────────┘
```

---

## 📖 Mỗi Thành Phần Chi Tiết

### 1️⃣ **Models** (Dữ Liệu)

#### Mục đích:
- Định nghĩa cấu trúc dữ liệu
- Ánh xạ thành các cột trong bảng database

#### Ví dụ: Language.cs

```csharp
public class Language
{
    public int Id { get; set; }              // Khóa chính
    public string Name { get; set; }         // Tên ngôn ngữ
    public string Code { get; set; }         // Mã code (vi-VN, en-US)
    public string? Description { get; set; } // Mô tả (tùy chọn - nullable)
    public DateTime CreatedAt { get; set; }  // Ngày tạo
    
    // Quan hệ (Navigation Property)
    public ICollection<ExplanationContent> ExplanationContents { get; set; } = [];
}
```

**Quy ước:**
- Tên class = Tên bảng (Language → Languages table)
- Property có thuộc tính công khai (get; set;)
- Primary key tên là `Id`
- Các field bắt buộc không nullable (string Name)
- Các field tùy chọn nullable (string? Description)

---

### 2️⃣ **DbContext** (Database Connection)

#### Mục đích:
- Kết nối với SQL Server
- Ánh xạ Models → Tables
- Cấu hình quan hệ và ràng buộc dữ liệu

#### Ví dụ: ApplicationDbContext.cs

```csharp
public class ApplicationDbContext : DbContext
{
    // Constructor - Nhận configuration
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    
    // DbSets = Collections của Models
    public DbSet<Language> Languages { get; set; }
    public DbSet<Explanation> Explanations { get; set; }
    
    // OnModelCreating = Cấu hình chi tiết ánh xạ
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Cấu hình: độ dài string max, primary key, indexes, etc.
        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.Id);                    // Primary key
            entity.Property(e => e.Name)                 // Cấu hình property
                .IsRequired()                            // Bắt buộc có
                .HasMaxLength(100);                      // Tối đa 100 ký tự
        });
    }
}
```

**Quy ước:**
- Class tên `ApplicationDbContext`
- Inherit từ `DbContext`
- Public DbSet properties cho mỗi Model
- Override `OnModelCreating` để cấu hình

---

### 3️⃣ **Services** (Business Logic)

#### Mục đích:
- Xử lý logic nghiệp vụ (CRUD, validation)
- Cách ly logic từ Controllers
- Dễ test

#### Ví dụ: LanguageService.cs

```csharp
// Interface - Định nghĩa các phương thức
public interface ILanguageService
{
    Task<List<Language>> GetAllAsync();
    Task<Language?> GetByIdAsync(int id);
    Task<Language> CreateAsync(Language language);
    Task<Language> UpdateAsync(int id, Language language);
    Task<bool> DeleteAsync(int id);
}

// Implementation - Thực hiện các phương thức
public class LanguageService : ILanguageService
{
    private readonly ApplicationDbContext _context;
    
    public LanguageService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Language>> GetAllAsync()
    {
        return await _context.Languages
            .OrderBy(l => l.Name)
            .ToListAsync();
    }
    
    public async Task<Language> CreateAsync(Language language)
    {
        // Validation
        var exists = await _context.Languages
            .AnyAsync(l => l.Code == language.Code);
        
        if (exists)
            throw new InvalidOperationException($"Mã '{language.Code}' đã tồn tại");
        
        // Add to database
        _context.Languages.Add(language);
        await _context.SaveChangesAsync();
        return language;
    }
}
```

**Quy ước:**
- Luôn tạo Interface (ILanguageService)
- Implement interface trong Service class
- Sử dụng async/await (Task<T>)
- Injection DbContext qua constructor
- All methods return Task<T> or Task

---

### 4️⃣ **Controllers** (API Routes)

#### Mục đích:
- Tiếp nhận HTTP requests
- Gọi Services xử lý
- Trả về HTTP responses

#### Ví dụ: LanguagesController.cs

```csharp
[ApiController]                           // Đánh dấu là API controller
[Route("api/[controller]")]               // Route: /api/languages
public class LanguagesController : ControllerBase
{
    private readonly ILanguageService _service;
    
    public LanguagesController(ILanguageService service)
    {
        _service = service;
    }
    
    [HttpGet]                             // GET /api/languages
    [ProducesResponseType(StatusCodes.Status200OK)]  // Swagger docs
    public async Task<ActionResult<IEnumerable<Language>>> GetAll()
    {
        var languages = await _service.GetAllAsync();
        return Ok(languages);              // 200 OK response
    }
    
    [HttpPost]                            // POST /api/languages
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Language>> Create([FromBody] Language language)
    {
        var created = await _service.CreateAsync(language);
        
        // 201 Created + Location header
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    
    [HttpPut("{id}")]                     // PUT /api/languages/{id}
    public async Task<ActionResult<Language>> Update(int id, [FromBody] Language language)
    {
        var updated = await _service.UpdateAsync(id, language);
        return Ok(updated);
    }
    
    [HttpDelete("{id}")]                  // DELETE /api/languages/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();  // 204 or 404
    }
}
```

**Quy ước:**
- Class tên = Model + "Controller" (Language → LanguagesController)
- `[Route("api/[controller]")]` → `/api/languages`
- Mỗi method là một action (endpoint)
- HTTP verbs: `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`
- Return `ActionResult<T>` hoặc `IActionResult`

---

## 🔄 Dependency Injection - Cách Hoạt Động

### Program.cs
```csharp
// Đăng ký services
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(connectionString));
```

### Cách injection hoạt động:
```
1. Controller request ILanguageService
2. DI Container kiểm tra: Ai implements ILanguageService?
3. Thấy LanguageService được đăng ký
4. DI Container tạo LanguageService instance
5. DI Container tạo ApplicationDbContext cho LanguageService
6. Return LanguageService instance cho Controller
```

**Ưu điểm:**
- Loosely coupled (không phụ thuộc vào implementation cụ thể)
- Dễ test (thay interface với mock object)
- Dễ thay đổi implementation sau

---

## 📊 Quan Hệ Dữ Liệu

### One-to-Many (Một-Nhiều)

```csharp
// Language có nhiều ExplanationContents
public class Language
{
    public int Id { get; set; }
    public ICollection<ExplanationContent> ExplanationContents { get; set; } = [];
}

public class ExplanationContent
{
    public int LanguageId { get; set; }        // Foreign Key
    public Language? Language { get; set; }    // Navigation Property
}
```

**Database:**
```
Languages Table          ExplanationContents Table
├── Id (PK)             ├── Id (PK)
├── Name                ├── LanguageId (FK) → Languages.Id
└── ...                 └── ...
```

---

## 🎯 Quy Ước Đặt Tên (Naming Conventions)

| Thành Phần | Quy Ước | Ví Dụ |
|-----------|--------|------|
| Class | PascalCase | `LanguageService` |
| Method | PascalCase | `GetAllAsync()` |
| Field private | _camelCase | `_context` |
| Property | PascalCase | `public string Name` |
| Local variable | camelCase | `var languages = ...` |
| Constant | UPPER_CASE | `const int MAX_LENGTH = 100;` |
| Interface | IPascalCase | `ILanguageService` |
| Table | Plural | `Languages` |

---

## 🔐 Entity Framework Core Methods

| Phương Thức | Mục Đích | Ví Dụ |
|-----------|---------|------|
| `.ToListAsync()` | Convert IQueryable → List | `await context.Languages.ToListAsync()` |
| `.FirstOrDefaultAsync()` | Lấy phần tử đầu hoặc null | `await context.Languages.FirstOrDefaultAsync()` |
| `.AnyAsync()` | Kiểm tra tồn tại | `await context.Languages.AnyAsync(l => l.Id == 1)` |
| `.Add()` | Thêm record mới | `context.Languages.Add(language)` |
| `.Update()` | Cập nhật record | `context.Languages.Update(language)` |
| `.Remove()` | Xóa record | `context.Languages.Remove(language)` |
| `.SaveChangesAsync()` | Lưu thay đổi vào database | `await context.SaveChangesAsync()` |

---

## 💡 Best Practices

✅ **Làm Tốt**
```csharp
// Interface + Implementation
public interface ILanguageService { }
public class LanguageService : ILanguageService { }

// Async all the way
public async Task<Language> GetAsync(int id)

// Validation in service
if (exists) throw new InvalidOperationException("...");

// DbSet as IQueryable
var query = _context.Languages
    .Where(l => l.IsActive)
    .OrderBy(l => l.Name);
```

❌ **Tránh**
```csharp
// Không dùng DbContext directly trong Controller
public ActionResult Get() {
    var lang = new ApplicationDbContext().Languages.First();
}

// Không bỏ async/await
public Language GetSync() {
    return _context.Languages.ToList().First();
}

// Hard-code connection strings
var context = new DbContext("Server=...");
```

---

**Đó là kiến trúc! Bây giờ bạn có thể mở rộng dự án với tự tin 🚀**
