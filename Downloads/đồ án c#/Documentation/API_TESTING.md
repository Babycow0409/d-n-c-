# 🧪 Hướng Dẫn Test API

## Các Cách Test API

### 1️⃣ **Swagger UI** (Cách Dễ Nhất)

**Bước 1:** Run ứng dụng
```bash
dotnet run
```

**Bước 2:** Mở browser: `https://localhost:5001`

**Bước 3:** Bạn sẽ thấy Swagger UI với tất cả endpoints

**Bước 4:** Click "Try it out" để test

---

### 2️⃣ **Postman** (Công Cụ Ch전)

**Tải về:** https://www.postman.com/downloads/

**Các bước:**
1. Mở Postman
2. Tạo Request mới (New → Request)
3. Chọn method (GET, POST, PUT, DELETE)
4. Nhập URL: `https://localhost:5001/api/languages`
5. Click "Send"

---

### 3️⃣ **cURL** (Terminal)

```bash
# GET - Lấy tất cả ngôn ngữ
curl --insecure https://localhost:5001/api/languages

# POST - Tạo ngôn ngữ mới
curl --insecure -X POST https://localhost:5001/api/languages `
  -H "Content-Type: application/json" `
  -d '{"name":"Tiếng Nhật","code":"ja-JP","description":"Japanese"}'

# GET - Lấy ngôn ngữ theo ID
curl --insecure https://localhost:5001/api/languages/1

# PUT - Cập nhật
curl --insecure -X PUT https://localhost:5001/api/languages/1 `
  -H "Content-Type: application/json" `
  -d '{"name":"Tiếng Việt Updated","code":"vi-VN","description":"Updated"}'

# DELETE - Xóa
curl --insecure -X DELETE https://localhost:5001/api/languages/1
```

---

### 4️⃣ **VS Code REST Client** (Extension)

**Cài đặt:** 
- Mở VS Code Extensions
- Search: "REST Client"
- Cài bởi Huachao Mao

**Sử dụng:**

Tạo file `requests.http`:

```http
### Health Check
GET https://localhost:5001/health

### Lấy tất cả ngôn ngữ
GET https://localhost:5001/api/languages

### Lấy ngôn ngữ theo ID
GET https://localhost:5001/api/languages/1

### Tạo ngôn ngữ mới
POST https://localhost:5001/api/languages
Content-Type: application/json

{
  "name": "Tiếng Nhật",
  "code": "ja-JP",
  "description": "Japanese"
}

### Cập nhật ngôn ngữ
PUT https://localhost:5001/api/languages/1
Content-Type: application/json

{
  "name": "Tiếng Nhật",
  "code": "ja-JP",
  "description": "Updated"
}

### Xóa ngôn ngữ
DELETE https://localhost:5001/api/languages/1

### Lấy ngôn ngữ theo code
GET https://localhost:5001/api/languages/code/vi-VN
```

Click "Send Request" trên mỗi request để test.

---

## 📋 Test Endpoints

### Ngôn Ngữ (Languages)

#### 1. Lấy tất cả ngôn ngữ
```
GET /api/languages
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Tiếng Việt",
    "code": "vi-VN",
    "description": "Vietnamese",
    "createdAt": "2024-03-27T09:00:00Z"
  },
  {
    "id": 2,
    "name": "English",
    "code": "en-US",
    "description": "English - United States",
    "createdAt": "2024-03-27T09:00:00Z"
  }
]
```

#### 2. Tạo ngôn ngữ mới
```
POST /api/languages
Content-Type: application/json

{
  "name": "Tiếng Nhật",
  "code": "ja-JP",
  "description": "Japanese"
}
```

**Response (201 Created):**
```json
{
  "id": 4,
  "name": "Tiếng Nhật",
  "code": "ja-JP",
  "description": "Japanese",
  "createdAt": "2024-03-27T10:30:00Z"
}
```

#### 3. Lấy chi tiết ngôn ngữ
```
GET /api/languages/1
```

#### 4. Cập nhật ngôn ngữ
```
PUT /api/languages/1
Content-Type: application/json

{
  "name": "Tiếng Việt (Updated)",
  "code": "vi-VN",
  "description": "Vietnamese language"
}
```

#### 5. Xóa ngôn ngữ
```
DELETE /api/languages/1
```

---

### Thuyết Minh (Explanations)

#### 1. Tạo thuyết minh mới
```
POST /api/explanations
Content-Type: application/json

{
  "title": "Giới Thiệu C# Fundamentals",
  "description": "Hướng dẫn cơ bản về C#",
  "content": "C# là ngôn ngữ lập trình hướng đối tượng...",
  "defaultLanguageId": 1,
  "createdBy": "Admin"
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "title": "Giới Thiệu C# Fundamentals",
  "description": "Hướng dẫn cơ bản về C#",
  "content": "C# là ngôn ngữ lập trình hướng đối tượng...",
  "defaultLanguageId": 1,
  "defaultLanguage": {
    "id": 1,
    "name": "Tiếng Việt",
    "code": "vi-VN"
  },
  "isActive": true,
  "createdAt": "2024-03-27T10:35:00Z",
  "updatedAt": null
}
```

#### 2. Lấy tất cả thuyết minh
```
GET /api/explanations
```

#### 3. Lấy chi tiết thuyết minh
```
GET /api/explanations/1
```

#### 4. Cập nhật thuyết minh
```
PUT /api/explanations/1
Content-Type: application/json

{
  "title": "Giới Thiệu C# Fundamentals (Updated)",
  "description": "Hướng dẫn cơ bản về C#",
  "content": "C# là ngôn ngữ lập trình hướng đối tượng...",
  "isActive": true
}
```

#### 5. Lấy bản dịch của thuyết minh
```
GET /api/explanations/1/translations
```

#### 6. Xóa thuyết minh
```
DELETE /api/explanations/1
```

---

## ✅ Test Cases (Tất Cả Trường Hợp)

### Tạo Ngôn Ngữ

| Test | Data | Kỳ Vọng |
|------|------|--------|
| Valid | Name + Code + Description | 201 Created ✅ |
| Missing Name | Code + Description | 400 Bad Request |
| Duplicate Code | Code existed | 400 Bad Request |
| Empty Code | Name + empty Code | 400 Bad Request |

### Tạo Thuyết Minh

| Test | Data | Kỳ Vọng |
|------|------|--------|
| Valid | Toàn bộ dữ liệu | 201 Created ✅ |
| Missing Title | Content + LanguageId | 400 Bad Request |
| Invalid LanguageId | Title + Code không tồn tại | 400 Bad Request |
| Missing Content | Title + LanguageId | 400 Bad Request |

---

## 🔍 Kiểm Tra Response Headers

Mỗi response sẽ có:

```
Status: 200 OK (hoặc 201 Created, 404 Not Found, etc)

Headers:
- Content-Type: application/json
- Content-Length: 1234
- Date: Thu, 27 Mar 2024 10:35:00 GMT
```

---

## 💡 Tips

1. **HTTPS:** Sử dụng `--insecure` với cURL hoặc disable SSL verify trong Postman
2. **JSON:** Always set header `Content-Type: application/json` khi POST/PUT
3. **ID:** Thay thế `{id}` bằng số thực (ví dụ: 1, 2, 3)
4. **Logs:** Kiểm tra VS Code console để xem request logs

---

## 🐛 Lỗi Thường Gặp

| Lỗi | Giải Pháp |
|-----|----------|
| 404 Not Found | Kiểm tra endpoint URL |
| 400 Bad Request | Kiểm tra JSON format |
| 500 Internal Server | Xem console log |
| SSL Error | Sử dụng `--insecure` hoặc setup HTTPS properly |

---

**Happy Testing! 🎉**
