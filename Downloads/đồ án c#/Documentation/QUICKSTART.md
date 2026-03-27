# ⚡ Quick Start Guide - Bắt Đầu Nhanh

## ⏱️ 5 Bước Để Chạy API (5 phút)

### 1️⃣ Mở Project

```bash
# Terminal
cd c:\Users\WIN\Downloads\đồ án c#\MyWebApi
code .
```

### 2️⃣ Chuẩn Bị Database

**Mở Package Manager Console** trong Visual Studio:
- **Tools** → **NuGet Package Manager** → **Package Manager Console**

Chạy 2 lệnh:
```powershell
Add-Migration InitialCreate
Update-Database
```

### 3️⃣ Run Application

Nhấn <kbd>F5</kbd> hoặc:
```bash
dotnet run
```

### 4️⃣ Mở API Documentation

Browser truy cập:
```
https://localhost:5001
```

### 5️⃣ Test API

Click "Try it out" và test các endpoint

---

## 🎯 Các Lệnh Thường Dùng

```bash
# Build
dotnet build

# Run
dotnet run

# Add Package
dotnet add package PackageName

# Create Migration
Add-Migration MigrationName

# Update Database
Update-Database

# Clean
dotnet clean
```

---

## 📞 Cần Giúp?

1. **Không chạy được?**
   - Kiểm tra SQL Server: `sqllocaldb start mssqllocaldb`
   - Build lại: `dotnet build`

2. **Database error?**
   - Xóa database: SQL Server Explorer
   - Chạy lại: `Update-Database`

3. **HTTPS error?**
   - DevTools HTTP → HTTPS
   - Hoặc thêm `--no-https` flag

---

## 📚 Tài Liệu Khác

- [README.md](README.md) - Hướng dẫn đầy đủ
- [MIGRATIONS.md](MIGRATIONS.md) - Chi tiết về Database
- [API_TESTING.md](API_TESTING.md) - Cách test API
- [ARCHITECTURE.md](ARCHITECTURE.md) - Giải thích code

---

**Chúc bạn thành công! 🚀**
