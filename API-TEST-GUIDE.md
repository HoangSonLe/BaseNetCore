# API Test Guide - Current User ID

## Tổng quan
Tôi đã tạo 2 API test endpoints để gọi tới UserService và trả về current user ID:

### 1. **GetCurrentUserId** 
- **URL**: `GET /api/Test/GetCurrentUserId`
- **Mô tả**: Trả về thông tin cơ bản của user hiện tại
- **Response**: 
```json
{
  "UserId": 123,
  "IsAuthenticated": true,
  "UserName": "admin",
  "Name": "Administrator",
  "Message": "Current user retrieved successfully"
}
```

### 2. **GetCurrentUserInfo**
- **URL**: `GET /api/Test/GetCurrentUserInfo`  
- **Mô tả**: Trả về thông tin user hiện tại với context mở rộng
- **Response**:
```json
{
  "CurrentUser": {
    "UserId": 123,
    "IsAuthenticated": true,
    "UserName": "admin", 
    "Name": "Administrator",
    "Message": "Current user retrieved successfully"
  },
  "RequestInfo": {
    "Timestamp": "2024-01-15T10:30:00Z",
    "RequestId": "guid-here",
    "UserAgent": "Mozilla/5.0...",
    "RemoteIpAddress": "127.0.0.1",
    "IsHttps": true
  },
  "ServerInfo": {
    "MachineName": "DEV-MACHINE",
    "ProcessId": 12345,
    "WorkingSet": 67890123
  }
}
```

## Cách test

### Sử dụng file test-current-user-api.http
1. Mở file `test-current-user-api.http` trong VS Code
2. Cài đặt extension "REST Client" nếu chưa có
3. Click vào "Send Request" ở trên mỗi request

### Sử dụng Postman
1. Tạo GET request tới: `https://localhost:7001/api/Test/GetCurrentUserId`
2. Thêm headers:
   - `Accept: application/json`
   - `Content-Type: application/json`
3. Nếu cần authentication, thêm: `Authorization: Bearer YOUR_TOKEN`

### Sử dụng curl
```bash
# Test GetCurrentUserId
curl -X GET "https://localhost:7001/api/Test/GetCurrentUserId" \
     -H "Accept: application/json" \
     -H "Content-Type: application/json"

# Test GetCurrentUserInfo  
curl -X GET "https://localhost:7001/api/Test/GetCurrentUserInfo" \
     -H "Accept: application/json" \
     -H "Content-Type: application/json"
```

## Các trường hợp response

### User đã authenticated
- **Status Code**: 200 OK
- **IsAuthenticated**: true
- **UserId**: ID thực của user
- **UserName**: Tên đăng nhập
- **Name**: Tên hiển thị

### User chưa authenticated  
- **Status Code**: 401 Unauthorized
- **IsAuthenticated**: false
- **UserId**: null
- **UserName**: null
- **Name**: null
- **Message**: "User is not authenticated"

### Lỗi server
- **Status Code**: 500 Internal Server Error
- **Message**: "Internal server error"
- **Error**: Chi tiết lỗi
- **Timestamp**: Thời gian xảy ra lỗi

## Lưu ý
- API sử dụng `ICurrentUserContext` để lấy thông tin user hiện tại
- Cần đảm bảo user đã đăng nhập để có thông tin đầy đủ
- Port mặc định là 7001 (HTTPS) hoặc 5000 (HTTP), có thể thay đổi tùy cấu hình
