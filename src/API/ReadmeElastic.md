Elastic:
	NEST: Supports Elasticsearch 7.x and below
	Elastic.Clients.Elasticsearch: Built for Elasticsearch 8.x and above


Mình sẽ làm ví dụ chi tiết với từng kiểu mapping trong Elasticsearch nhé! 🎯  

Mục tiêu là so sánh 3 cách mapping phổ biến:  
- **Text**: Dùng để phân tích nội dung, phù hợp với tìm kiếm full-text (từng từ).  
- **Keyword**: Dùng để lưu chuỗi nguyên vẹn, phù hợp với tìm kiếm chính xác hoặc wildcard.  
- **Text + Keyword**: Kết hợp cả hai để linh hoạt hơn trong tìm kiếm.  

Hãy cùng thử từng trường hợp! 🚀  

---

## 🔧 **Tạo index với từng kiểu mapping**

```json
PUT flights
{
  "mappings": {
    "properties": {
      "FlightNum_text": {
        "type": "text"
      },
      "FlightNum_keyword": {
        "type": "keyword"
      },
      "FlightNum_combined": {
        "type": "text",
        "fields": {
          "keyword": {
            "type": "keyword"
          }
        }
      }
    }
  }
}
```

➡️ Ở đây mình tạo ra 3 trường:  
- `FlightNum_text`: Mapping dạng `text`.  
- `FlightNum_keyword`: Mapping dạng `keyword`.  
- `FlightNum_combined`: Kết hợp cả `text` và `keyword`.  

---

## 🛠 **Chèn dữ liệu mẫu**  

```json
POST flights/_bulk
{ "index": { "_id": 1 } }
{ "FlightNum_text": "AA123", "FlightNum_keyword": "AA123", "FlightNum_combined": "AA123" }
{ "index": { "_id": 2 } }
{ "FlightNum_text": "AA123456", "FlightNum_keyword": "AA123456", "FlightNum_combined": "AA123456" }
{ "index": { "_id": 3 } }
{ "FlightNum_text": "BB789", "FlightNum_keyword": "BB789", "FlightNum_combined": "BB789" }
```

---

## 🔍 **1. Tìm kiếm với `match` (dùng `text`)**  

```json
GET flights/_search
{
  "query": {
    "match": {
      "FlightNum_text": "AA123"
    }
  }
}
```

➡️ **Kết quả:**  
- ✅ **AA123**  
- ❌ Không tìm thấy **AA123456** (vì `match` tìm theo từ).  

---

## 🔍 **2. Tìm kiếm với `term` (dùng `keyword`)**  

```json
GET flights/_search
{
  "query": {
    "term": {
      "FlightNum_keyword": "AA123"
    }
  }
}
```

➡️ **Kết quả:**  
- ✅ **AA123**  
- ❌ Không tìm thấy **AA123456** (vì `term` cần khớp chính xác).  

---

## 🔍 **3. Tìm kiếm với `wildcard` (dùng `keyword`)**  

```json
GET flights/_search
{
  "query": {
    "wildcard": {
      "FlightNum_keyword": "*123*"
    }
  }
}
```

➡️ **Kết quả:**  
- ✅ **AA123**  
- ✅ **AA123456**  

---

## 🔍 **4. Tìm kiếm với `query_string` (dùng `text`)**  

```json
GET flights/_search
{
  "query": {
    "query_string": {
      "query": "*123*",
      "fields": ["FlightNum_text"]
    }
  }
}
```

➡️ **Kết quả:**  
- ❌ Không tìm thấy gì (vì `text` bị phân tích thành token, không dùng trực tiếp `*`).  

---

## 🔍 **5. Tìm kiếm với `match_phrase` (dùng `text`)**  

```json
GET flights/_search
{
  "query": {
    "match_phrase": {
      "FlightNum_text": "AA123"
    }
  }
}
```

➡️ **Kết quả:**  
- ✅ **AA123**  
- ❌ Không tìm thấy **AA123456**.  

---

## 🔍 **6. Tìm kiếm với `combined` (text + keyword)**  

```json
GET flights/_search
{
  "query": {
    "wildcard": {
      "FlightNum_combined.keyword": "*123*"
    }
  }
}
```

➡️ **Kết quả:**  
- ✅ **AA123**  
- ✅ **AA123456**  

---

## 🏁 **Tổng kết**  

| **Query**                 | **Text**        | **Keyword**     | **Text + Keyword** |
|---------------------------|-----------------|-----------------|--------------------|
| **match**                 | AA123           | Không tìm thấy | AA123              |
| **term**                  | Không tìm thấy | AA123           | AA123              |
| **wildcard**              | Không tìm thấy | AA123, AA123456 | AA123, AA123456    |
| **query_string**          | Không tìm thấy | AA123, AA123456 | AA123, AA123456    |
| **match_phrase**          | AA123           | Không tìm thấy | AA123              |

---

## 🚀 **Kết luận**  
- **Text** phù hợp với tìm kiếm toàn văn bản hoặc từng từ.  
- **Keyword** phù hợp với tìm kiếm chính xác hoặc chứa chuỗi con.  
- **Text + Keyword** linh hoạt nhất — bạn vừa có thể tìm full-text, vừa tìm chính xác bằng `keyword`.  

Nếu bạn muốn tìm kiếm **contain**, nên dùng `keyword` hoặc `text + keyword` và kết hợp với **`wildcard`** hoặc **`query_string`**.  

Mình khuyên dùng kiểu kết hợp để linh hoạt trong mọi tình huống! ✌️✨  

Bạn thấy phần này thế nào? Có cần mình làm thêm ví dụ không? 🚀