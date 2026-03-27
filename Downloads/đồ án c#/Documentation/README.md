# 🌐 Hệ Thống Thuyết Minh Đa Ngôn Ngữ - ASP.NET Core Web API

## 📖 Giới Thiệu

Đây là một ASP.NET Core Web API (.NET 10.0) được thiết kế để quản lý các thuyết minh (giải thích, hướng dẫn) có hỗ trợ đa ngôn ngữ. Hệ thống cho phép:

- ✅ Tạo, đọc, cập nhật, xóa thuyết minh (CRUD)
- ✅ Quản lý đa ngôn ngữ (Tiếng Việt, English, 中文, v.v.)
- ✅ Dịch thuyết minh sang các ngôn ngữ khác
- ✅ Theo dõi trạng thái dịch (Pending, Approved, Rejected)
- ✅ API RESTful với tài liệu Swagger

---

## 🏗️ Cấu Trúc Dự Án

```
MyWebApi/
├── Models/                      # Các model dữ liệu
│   ├── Language.cs             # Model cho Ngôn ngữ
│   ├── Explanation.cs          # Model cho Thuyết minh
│   └── ExplanationContent.cs   # Model cho Nội dung dịch
├── Data/
│   └── ApplicationDbContext.cs # DbContext (kết nối Database)
├── Services/                    # Business Logic
│   ├── ExplanationService.cs   # Xử lý logic Thuyết minh
│   └── LanguageService.cs      # Xử lý logic Ngôn ngữ
├── Controllers/                 # API Endpoints
│   ├── ExplanationsController.cs
│   └── LanguagesController.cs
├── Program.cs                  # Cấu hình ứng dụng
├── appsettings.json            # Cấu hình (ConnectionString, Logging)
└── MyWebApi.csproj            # File cấu hình Project
```

---

## 🚀 Hướng Dẫn Setup (Từng Bước)

### 1️⃣ **Yêu Cầu Tiên Quyết**
- Visual Studio 2022 (hoặc VS Code)
- .NET 10.0 SDK
- SQL Server (LocalDB hoặc full version)

### 2️⃣ **Clone & Mở Project**

```bash
# Mở Terminal trong thư mục đồ án
cd c:\Users\WIN\Downloads\đồ án c#

# Mở Visual Studio hoặc VS Code
code .
```

### 3️⃣ **Cài Đặt Dependencies**

Các NuGet package đã được cấu hình trong `MyWebApi.csproj`:
- ✅ `Microsoft.EntityFrameworkCore` (10.0.5)
- ✅ `Microsoft.EntityFrameworkCore.SqlServer` (10.0.5)
- ✅ `Microsoft.EntityFrameworkCore.Tools` (10.0.5)
- ✅ `Swashbuckle.AspNetCore` (7.0.10)

Tải dependencies:
```bash
dotnet restore
```

### 4️⃣ **Tạo Database Migration**

Migrations giúp tạo bảng dữ liệu trong SQL Server.

**Mở Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console)

Chạy lệnh:
```powershell
# Tạo migration đầu tiên
Add-Migration InitialCreate

# Áp dụng migration vào database
Update-Database
```

**Lưu ý:** Nếu lỗi "The type or namespace name 'MyWebApi' could not be found", hãy:
1. Build project trước: `dotnet build`
2. Rồi chạy lại lệnh migration

### 5️⃣ **Kiểm Tra Database**

Mở **SQL Server Object Explorer** trong Visual Studio:
- Mở `(localdb)\mssqllocaldb`
- Tìm database `MultilingualExplanationDb`
- Xem các bảng: `Languages`, `Explanations`, `ExplanationContents`

### 6️⃣ **Chạy Application**

#### Cách 1: Sử dụng Visual Studio
```
Nhấn F5 hoặc Debug → Start Debugging
```

#### Cách 2: Sử dụng Terminal
```bash
dotnet run
```

**Kết quả:** API sẽ chạy tại `https://localhost:5001`

### 7️⃣ **Truy Cập Swagger (Tài Liệu API)**

Mở browser và truy cập:
```
https://localhost:5001
```

Bạn sẽ thấy giao diện Swagger với các endpoint API.

---

## 📡 API Endpoints

### **Languages (Ngôn Ngữ)**

| Phương thức | Endpoint | Mô tả |
|-----------|----------|------|
| `GET` | `/api/languages` | Lấy tất cả ngôn ngữ |
| `GET` | `/api/languages/{id}` | Lấy chi tiết ngôn ngữ |
| `GET` | `/api/languages/code/{code}` | Tìm ngôn ngữ theo code |
| `POST` | `/api/languages` | Tạo ngôn ngữ mới |
| `PUT` | `/api/languages/{id}` | Cập nhật ngôn ngữ |
| `DELETE` | `/api/languages/{id}` | Xóa ngôn ngữ |

### **Explanations (Thuyết Minh)**

| Phương thức | Endpoint | Mô tả |
|-----------|----------|------|
| `GET` | `/api/explanations` | Lấy tất cả thuyết minh |
| `GET` | `/api/explanations/{id}` | Lấy chi tiết thuyết minh |
| `GET` | `/api/explanations/{id}/translations` | Lấy tất cả bản dịch |
| `POST` | `/api/explanations` | Tạo thuyết minh mới |
| `PUT` | `/api/explanations/{id}` | Cập nhật thuyết minh |
| `DELETE` | `/api/explanations/{id}` | Xóa thuyết minh |

---

## 💡 Ví Dụ Sử Dụng API

### ✨ 1. Tạo Ngôn Ngữ Mới

**Request:**
```
POST /api/languages
Content-Type: application/json

{
  "name": "Tiếng Nhật",
  "code": "ja-JP",
  "description": "Japanese"
}
```

**Response:**
```json
{
  "id": 4,
  "name": "Tiếng Nhật",
  "code": "ja-JP",
  "description": "Japanese",
  "createdAt": "2024-03-27T10:30:00Z"
}
```

### ✨ 2. Tạo Thuyết Minh Mới

**Request:**
```
POST /api/explanations
Content-Type: application/json

{
  "title": "Cách Sử Dụng Entity Framework",
  "description": "Hướng dẫn cơ bản",
  "content": "Entity Framework là ORM cho .NET...",
  "defaultLanguageId": 1,
  "createdBy": "Admin"
}
```

**Response:**
```json
{
  "id": 1,
  "title": "Cách Sử Dụng Entity Framework",
  "description": "Hướng dẫn cơ bản",
  "content": "Entity Framework là ORM cho .NET...",
  "defaultLanguageId": 1,
  "defaultLanguage": {
    "id": 1,
    "name": "Tiếng Việt",
    "code": "vi-VN",
    "createdAt": "2024-03-27T09:00:00Z"
  },
  "isActive": true,
  "createdAt": "2024-03-27T10:35:00Z"
}
```

### ✨ 3. Lấy Tất Cả Thuyết Minh

**Request:**
```
GET /api/explanations
```

**Response:**
```json
[
  {
    "id": 1,
    "title": "Cách Sử Dụng Entity Framework",
    "content": "Entity Framework là ORM cho .NET...",
    "isActive": true,
    "contents": [
      {
        "id": 1,
        "explanationId": 1,
        "languageId": 2,
        "language": {
          "id": 2,
          "name": "English",
          "code": "en-US"
        },
        "translatedContent": "Entity Framework is an ORM for .NET...",
        "status": "Approved"
      }
    ]
  }
]
```

---

## 🔧 Cấu Hình Connection String

File: `appsettings.json`

### SQL Server LocalDB (Mặc định)
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MultilingualExplanationDb;Trusted_Connection=true;Encrypt=false;"
```

### SQL Server Network
```json
"DefaultConnection": "Server=YOUR_SERVER_NAME;Database=MultilingualExplanationDb;User Id=sa;Password=YourPassword;Encrypt=false;"
```

---

## 📚 Giải Thích Kiến Trúc

### **Models** (Dữ liệu)
- `Language`: Lưu thông tin ngôn ngữ
- `Explanation`: Thuyết minh gốc
- `ExplanationContent`: Bản dịch của thuyết minh

### **Services** (Xử lý Logic)
- Thực hiện CRUD operations
- Xác thực dữ liệu
- Xử lý quan hệ dữ liệu

### **Controllers** (API Routes)
- Tiếp nhận request từ client
- Gọi Services xử lý
- Trả về response

### **DbContext** (Database)
- Ánh xạ Objects ↔ Database Tables
- Quản lý quan hệ
- Hỗ trợ migrations

---

## 🐛 Xử Lý Lỗi Thường Gặp

### ❌ Lỗi: "Cannot connect to SQL Server"
**Giải pháp:**
1. Kiểm tra SQL Server chạy: `sqllocaldb i` (PowerShell)
2. Khởi động: `sqllocaldb start mssqllocaldb`

### ❌ Lỗi: "The Entity type does not have a primary key"
**Giải pháp:** Đảm bảo model có `Id` property

### ❌ Lỗi: "DbContext not found"
**Giải pháp:** Check `Program.cs` có đăng ký `AddDbContext` chưa

---

## 📦 Tính Năng Mở Rộng (Tương Lai)

- [ ] Authentication & Authorization (JWT)
- [ ] Pagination & Filtering
- [ ] Full-text search
- [ ] Soft delete
- [ ] Audit logging
- [ ] Unit tests
- [ ] Docker support

---

## 👨‍💻 Hỗ Trợ & Câu Hỏi

Nếu gặp vấn đề:
1. Kiểm tra console output
2. Xem file log
3. Kiểm tra connection string

---

**Happy Coding! 🚀**
