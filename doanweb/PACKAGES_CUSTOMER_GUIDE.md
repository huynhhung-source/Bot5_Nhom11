# Hướng Dẫn: Tính Năng Gói Tập Luyện Cho Khách Hàng

## 📋 Tổng Quan

Đã cập nhật hệ thống gói tập luyện với các tính năng sau:

### 1. ✅ Kiểm Tra Đăng Nhập Tự Động
- **Chưa đăng nhập**: Khi nhấn "Tham gia ngay" → Chuyển hướng đến trang **Login**
- **Đã đăng nhập**: Khi nhấn "Tham gia ngay" → Chuyển hướng đến trang **Chi tiết gói**

### 2. 📄 Trang Chi Tiết Gói Tập (Detail.cshtml)
Trang này hiển thị thông tin chi tiết gói tập với các phần:

#### A. **Header Gói Tập**
- Tên gói tập
- Thời hạn (tháng)
- Giá tiền

#### B. **Mô Tả Gói** 
- Hiển thị mô tả chi tiết từ database
- Giải thích về chương trình tập luyện

#### C. **Tính Năng Bao Gồm** ✅
- Danh sách các tính năng của gói
- Lấy từ trường `Features` trong database (tách bằng dấu phẩy)
- Mỗi tính năng hiển thị với icon ✓

#### D. **Nội Quy** 📌
Quy định chung của phòng tập:
- Tuân thủ quy định phòng tập
- Không quay phim/chụp ảnh không phép
- Giữ vệ sinh chung
- Không mang đồ ăn bẩn vào
- Hoàn trả thẻ khi hết hạn
- Phí cấp lại thẻ: 50.000₫

#### E. **Những Điều Cần Lưu Ý** ⚠️
Những thông tin quan trọng:
- Kiểm tra sức khỏe trước khi bắt đầu
- Không chuyển nhượng/cho người khác sử dụng
- Quy trình hủy gói
- Chính sách không hoàn tiền
- Tuân thủ hướng dẫn huấn luyện viên

#### F. **Sidebar - Tóm Tắt & Thanh Toán**
- Tóm tắt thông tin gói
- **Button "💳 Tiến Hành Thanh Toán"** (Chính yếu)
- Button quay lại danh sách
- Thông tin liên hệ hỗ trợ

---

## 🔧 Cơ Chế Hoạt Động

### Flow 1: Chưa Đăng Nhập
```
Trang Danh Sách Gói (Online/Offline)
         ↓
    Nhấn "Tham gia ngay"
         ↓
    Kiểm tra Session["UserId"]
         ↓
    Không tìm thấy → Lưu PackageId vào Session
         ↓
    Redirect → /Customer/Account/Login
```

### Flow 2: Đã Đăng Nhập
```
Trang Danh Sách Gói (Online/Offline)
         ↓
    Nhấn "Tham gia ngay"
         ↓
    Kiểm tra Session["UserId"]
         ↓
    Tìm thấy → Redirect → /Packages/Detail/[PackageId]
         ↓
    Hiển thị trang chi tiết
```

### Flow 3: Thanh Toán
```
Trang Chi Tiết Gói
         ↓
    Nhấn "💳 Tiến Hành Thanh Toán"
         ↓
    Kiểm tra đăng nhập lần nữa
         ↓
    Redirect → /Payment/Checkout?packageId=[PackageId]
```

---

## 📁 File Đã Cập Nhật

### 1. **PackagesController.cs**
```csharp
- Online() - Lấy danh sách gói từ database
- Offline() - Lấy danh sách gói từ database
- Detail(id) - Hiển thị chi tiết gói
- Register(packageId) - Kiểm tra đăng nhập, redirect
```

### 2. **Views/Packages/Detail.cshtml** ⭐ NEW
- Trang chi tiết gói tập hoàn chỉnh
- Hiển thị tất cả thông tin từ database
- Button thanh toán
- Responsive design

### 3. **Views/Packages/Online.cshtml** (Cập Nhật)
- Lấy dữ liệu từ `@Model` (List<Package>)
- Hiển thị các gói từ database
- Nút "Tham gia ngay" → `/packages/detail/[id]`

### 4. **Views/Packages/Offline.cshtml** (Cập Nhật)
- Tương tự Online.cshtml
- Lấy dữ liệu từ `@Model`
- Nút "Tham gia ngay" → `/packages/detail/[id]`

---

## 🚀 Cách Sử Dụng

### Bước 1: Thêm Gói Tập Vào Database
Admin → Quản lý Gói Tập → Thêm gói tập mới
```
Tên Gói: Fat Loss Program
Giá: 2490000
Thời Hạn: 90 (ngày)
Mô Tả: Chương trình giảm cân toàn diện...
Tính Năng: Chương trình cardio + tập tạ, Kế hoạch ăn uống, Video hướng dẫn,...
```

### Bước 2: Khách Hàng Xem Danh Sách
```
1. Truy cập /packages/online hoặc /packages/offline
2. Xem danh sách các gói tập
3. Nhấn "Tham gia ngay"
```

### Bước 3: Hệ Thống Kiểm Tra Đăng Nhập
- **Chưa đăng nhập** → Chuyển đến login
- **Đã đăng nhập** → Chuyển đến trang chi tiết

### Bước 4: Xem Chi Tiết & Thanh Toán
```
1. Xem thông tin chi tiết gói
2. Đọc nội quy và lưu ý
3. Nhấn "💳 Tiến Hành Thanh Toán"
4. Chuyển đến trang thanh toán
```

---

## 🎨 Tùy Chỉnh Chi Tiết

### Thêm Hình Ảnh Cho Gói
Cập nhật `DatabaseModels.cs`:
```csharp
public class Package
{
    // ...existing code...
    public string? ImageUrl { get; set; } // Thêm dòng này
}
```

Sau đó trong `Detail.cshtml`:
```html
@if (!string.IsNullOrEmpty(Model?.ImageUrl))
{
    <img src="@Model.ImageUrl" alt="@Model.PackageName" style="width: 100%; border-radius: 10px;" />
}
```

### Chỉnh Sửa Nội Quy & Lưu Ý
Mở `Detail.cshtml` tìm phần "Nội Quy" và "Những Điều Cần Lưu Ý", sửa thay thế nội dung theo ý muốn.

### Thay Đổi Màu Sắc
- Màu chính: `#f36100` (Cam)
- Màu nền: `#151515` (Đen)
- Màu text: `#c4c4c4` (Xám)
- Màu success: `#f36100`

---

## ⚙️ Cấu Hình Session

Đảm bảo file `Program.cs` có:
```csharp
builder.Services.AddSession();

// ...

app.UseSession();
```

---

## 🔗 URL Routes

| URL | Mô Tả |
|-----|-------|
| `/packages/online` | Danh sách gói online |
| `/packages/offline` | Danh sách gói offline |
| `/packages/detail/1` | Chi tiết gói ID=1 |
| `/packages/register?packageId=1` | Đăng ký gói |

---

## ✨ Tính Năng Sắp Tới

- [ ] Tích hợp thanh toán (VNPay, Stripe)
- [ ] Hệ thống đăng ký & quản lý gói cho khách hàng
- [ ] Email xác nhận đăng ký
- [ ] Đánh giá và bình luận gói tập
- [ ] So sánh các gói tập

---

## 📞 Hỗ Trợ

Nếu gặp vấn đề, vui lòng kiểm tra:
1. Database có chứa dữ liệu Package không?
2. Session đã được kích hoạt trong Program.cs?
3. URL route có đúng không?
4. CSS/JS có load đúng không?

---

**Cập nhật lần cuối:** $(DateTime.Now:dd/MM/yyyy)
