# 🗄️ Hướng Dẫn Entity Framework Core Migrations

## Migrations là gì?

**Migrations** là cách để tạo và cập nhật cấu trúc database (schema) một cách an toàn. Nó giúp:
- Tạo bảng dữ liệu từ Models C#
- Cập nhật schema khi Models thay đổi
- Quản lý phiên bản database
- Dễ dàng rollback về phiên bản trước

---

## 📋 Các Bước Chi Tiết

### 1️⃣ **Chuẩn Bị**

✅ Đảm bảo file cproject được build:
```bash
dotnet build
```

✅ Kiểm tra `Program.cs` có cấu hình DbContext:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);
```

✅ Kiểm tra `appsettings.json` có ConnectionString:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MultilingualExplanationDb;..."
```

### 2️⃣ **Mở Package Manager Console**

Trong **Visual Studio**:
- **Tools** → **NuGet Package Manager** → **Package Manager Console**

Hoặc dùng **Terminal**:
```bash
cd c:\Users\WIN\Downloads\đồ án c#\MyWebApi
```

### 3️⃣ **Tạo Migration (Migration Tạo Lần Đầu Tiên)**

```powershell
# Tạo migration có tên "InitialCreate"
Add-Migration InitialCreate
```

**Kết quả:**
- Tạo thư mục `Migrations/`
- Tạo file `[Timestamp]_InitialCreate.cs`
- Tạo file `ApplicationDbContextModelSnapshot.cs`

**File migration** chứa:
- `Up()`: Lệnh tạo bảng, cột, khóa ngoài
- `Down()`: Lệnh hoàn tác (xóa bảng)

### 4️⃣ **Áp Dụng Migration vào Database**

```powershell
# Cập nhật database với migration mới
Update-Database
```

**Kết quả:**
- ✅ Database `MultilingualExplanationDb` được tạo
- ✅ Các bảng được tạo (Languages, Explanations, ExplanationContents)
- ✅ Seed data được thêm vào

**Kiểm tra:** Mở **SQL Server Object Explorer** → (localdb)\mssqllocaldb → Databases → MultilingualExplanationDb

### 5️⃣ **Nếu Muốn Xóa Migration (Lỗi)**

```powershell
# Xóa migration cuối cùng (chưa áp dụng)
Remove-Migration
```

---

## 🔄 Khi Models Thay Đổi (Ví Dụ)

### Ví Dụ: Thêm Property Mới

**Bước 1:** Sửa Model
```csharp
public class Language
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    
    // ✅ THÊM: Property mới
    public bool IsActive { get; set; } = true;
}
```

**Bước 2:** Tạo migration mới
```powershell
Add-Migration AddLanguageIsActiveField
```

**Bước 3:** Áp dụng
```powershell
Update-Database
```

---

## 📊 Các Lệnh Thường Dùng

| Lệnh | Mô Tả |
|------|------|
| `Add-Migration MigrationName` | Tạo migration mới |
| `Update-Database` | Áp dụng migration vào database |
| `Remove-Migration` | Xóa migration cuối cùng (chưa áp dụng) |
| `Get-Migration` | Xem danh sách migrations |
| `Update-Database -Migration MigrationName` | Rollback về migration cụ thể |

---

## ⚠️ Xử Lý Lỗi Migrations

### ❌ Lỗi: "The type or namespace name 'ApplicationDbContext' could not be found"

**Nguyên nhân:** Project chưa build

**Giải pháp:**
```bash
dotnet build
```

Rồi chạy lại `Add-Migration`

### ❌ Lỗi: "A DbContext has been initialized in the application"

**Nguyên nhân:** Có 2 DbContext hoặc cấu hình không đúng

**Giải pháp:** 
1. Kiểm tra `Program.cs` chỉ có 1 `AddDbContext`
2. Build lại project

### ❌ Lỗi: "Cannot find database [DatabaseName]"

**Nguyên nhân:** SQL Server không chạy hoặc connection string sai

**Giải pháp:**
```powershell
# Kiểm tra LocalDB chạy chưa
sqllocaldb i

# Khởi động LocalDB
sqllocaldb start mssqllocaldb
```

Kiểm tra `appsettings.json` connection string

### ❌ Lỗi: "String reference not set to an instance of a String"

**Nguyên nhân:** Property không được khởi tạo

**Giải pháp:**
```csharp
// ❌ Sai
public string Name { get; set; }

// ✅ Đúng
public string Name { get; set; } = string.Empty;
```

---

## 💡 Best Practices

1. **Tạo migration cho mỗi thay đổi lớn**
   ```powershell
   Add-Migration AddExplanationTable
   Add-Migration AddLanguageValidation
   ```

2. **Luôn kiểm tra generated migration**
   ```csharp
   // Xem file Migrations/[Timestamp]_MigrationName.cs
   ```

3. **Commit migrations vào version control**
   ```bash
   git add Migrations/
   git commit -m "Add migration for language table"
   ```

4. **Không xóa migration sau khi deployed**
   - Tạo migration mới ngược lại thay vì xóa

---

## 🎯 Tóm Tắt Quy Trình

```
1. Sửa Models (Language.cs, Explanation.cs, etc.)
      ↓
2. Build project (dotnet build)
      ↓
3. Tạo migration (Add-Migration MigrationName)
      ↓
4. Review file migration
      ↓
5. Áp dụng (Update-Database)
      ↓
6. ✅ Database được cập nhật
```

---

**Tài liệu tham khảo:** [Microsoft EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
