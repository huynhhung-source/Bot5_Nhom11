# 📸 Hướng Dẫn: Tính Năng Hình Ảnh Gói Tập

## 🎯 Tổng Quan

Đã thêm tính năng upload và hiển thị hình ảnh cho các gói tập:

### 1. **Admin Panel - Create & Edit Package** 🖼️

#### A. Trường Upload Hình Ảnh
- **Vị trí**: Giữa **Mô Tả** và **Loại Gói Tập**
- **Loại trường**: Input text + Button upload
- **Placeholder**: `Nhập URL hình ảnh hoặc chọn từ thư mục`

#### B. Tính Năng
- ✅ Nhập đường dẫn hình ảnh trực tiếp
- ✅ Hiển thị preview hình ảnh ngay lập tức
- ✅ Button "Chọn Hình" để mở dialog nhập URL
- ✅ Hỗ trợ cả URL tuyệt đối (`https://...`) và tương đối (`/images/...`)

#### C. Ví Dụ URL Hình Ảnh
```
Đường dẫn tương đối: /images/packages/fatlose.jpg
Đường dẫn tuyệt đối: https://example.com/image.jpg
```

---

### 2. **Trang Chi Tiết Gói Tập** (Detail.cshtml) 📷

#### A. Vị Trí Hình Ảnh
```
Breadcrumb
    ↓
Header (Tên, Thời hạn, Giá)
    ↓
🖼️ HÌNH ẢNH (Tên mới - Nằm trên Mô Tả)
    ↓
📋 Mô Tả Gói
    ↓
✅ Tính Năng
    ↓
... (phần còn lại)
```

#### B. Đặc Điểm Hình Ảnh
- **Chiều rộng**: 100% (full width)
- **Chiều cao tối đa**: 400px
- **Object-fit**: cover (phủ toàn bộ diện tích)
- **Border-radius**: 10px (bo tròn góc)
- **Box-shadow**: Đổ bóng cam (#f36100)
- **Responsive**: Tự động thu phóng trên các thiết bị

#### C. Fallback
- Nếu URL không tồn tại → Ẩn hình ảnh
- Nếu không có ImageUrl → Không hiển thị gì

---

## 🚀 Cách Sử Dụng

### Bước 1: Thêm Gói Tập Mới (Admin)
```
1. Truy cập /admin/package/create
2. Điền thông tin gói:
   - Tên gói tập: "Fat Loss Program"
   - Giá: 2490000
   - Thời hạn: 90
   - Mô tả: "Chương trình giảm cân toàn diện..."
3. ⬇️ TRƯỜNG MỚI: Hình Ảnh Gói Tập
   - Nhập URL: https://example.com/fatlose.jpg
   - Xem preview ngay
4. Loại gói tập: Online/Offline/Combo
5. Click "Thêm Gói Tập"
```

### Bước 2: Chỉnh Sửa Gói Tập (Admin)
```
1. Truy cập /admin/package/edit/1
2. Cập nhật hình ảnh:
   - Thay đổi URL → Preview cập nhật ngay
   - Click "Chọn Hình" để nhập URL mới
3. Click "Cập Nhật"
```

### Bước 3: Khách Hàng Xem Chi Tiết
```
1. Truy cập /packages/detail/1
2. Xem hình ảnh gói tập (nếu có)
3. Xem mô tả, tính năng, nội quy
```

---

## 📁 Thay Đổi Chi Tiết

### 1. **DatabaseModels.cs**
```csharp
public class Package
{
    // ...existing code...
    
    // ✅ THÊM DÒNG NÀY:
    [StringLength(255)]
    public string? ImageUrl { get; set; }  // Lưu URL hình ảnh
}
```

### 2. **Areas/Admin/Views/Package/Create.cshtml**
```html
<!-- Thêm phần này giữa Mô Tả và Loại Gói Tập -->
<div class="mb-3">
    <label class="form-label">Hình Ảnh Gói Tập</label>
    <div class="input-group">
        <input type="text" class="form-control" id="ImageUrl" name="ImageUrl" />
        <button class="btn btn-outline-secondary" id="uploadImageBtn">
            Chọn Hình
        </button>
    </div>
    <div id="imagePreview"></div>
</div>
```

### 3. **Areas/Admin/Views/Package/Edit.cshtml**
Giống như Create.cshtml

### 4. **Views/Packages/Detail.cshtml**
```html
<!-- Thêm trước Mô Tả Gói Tập -->
@if (!string.IsNullOrEmpty(Model?.ImageUrl))
{
    <div style="margin-bottom: 40px; border-radius: 10px; box-shadow: 0 10px 30px rgba(243, 97, 0, 0.3);">
        <img src="@Model.ImageUrl" alt="@Model?.PackageName" 
             style="width: 100%; height: auto; max-height: 400px; object-fit: cover;" />
    </div>
}
```

---

## 🎨 Tùy Chỉnh Nâng Cao

### Thay Đổi Kích Thước Hình Ảnh
```html
<!-- Mở Detail.cshtml, tìm: max-height: 400px -->
<!-- Thay đổi giá trị này (ví dụ: 500px, 300px) -->
<img src="..." style="max-height: 500px;" />
```

### Thêm Bộ Lọc Hình Ảnh
```html
<img src="..." style="filter: brightness(0.9) contrast(1.1);" />
```

### Tăng Hiệu Ứng Hover
```css
.package-image {
    transition: transform 0.3s ease;
}

.package-image:hover {
    transform: scale(1.05);
}
```

---

## 💾 Database Migration

Nếu bạn chưa cập nhật database, chạy lệnh:

```powershell
# Package Manager Console
Add-Migration AddImageUrlToPackage
Update-Database
```

Hoặc thêm cột SQL trực tiếp:
```sql
ALTER TABLE Packages 
ADD ImageUrl NVARCHAR(255) NULL;
```

---

## 🔗 URL Hình Ảnh Đề Xuất

### Thư Mục Local
```
/images/packages/fatlose.jpg
/images/packages/musclebuild.jpg
/images/packages/strength.jpg
```

### Online
```
https://unsplash.com/random/400x300?gym
https://via.placeholder.com/400x300
```

---

## ✅ Checklist Triển Khai

- [ ] Cập nhật database thêm cột `ImageUrl`
- [ ] Thêm gói tập mới từ Admin với hình ảnh
- [ ] Kiểm tra preview hình ảnh trong form
- [ ] Xem chi tiết gói tập từ khách hàng
- [ ] Kiểm tra hình ảnh hiển thị đúng
- [ ] Test responsive trên mobile

---

## 🐛 Xử Lý Lỗi

### Hình ảnh không hiển thị
✅ **Giải pháp**: 
- Kiểm tra URL có đúng không
- Nhấn "Chọn Hình" để nhập lại
- Xóa URL và save lại (hình ảnh là optional)

### Preview không cập nhật
✅ **Giải pháp**:
- Refresh trang F5
- Kiểm tra console (F12) xem có lỗi JS không
- Xóa browser cache

---

## 📸 Ví Dụ Kết Quả

**Create Form:**
```
┌─────────────────────────────────────┐
│ Tên Gói Tập *                       │
├─────────────────────────────────────┤
│ Giá (₫) *  │  Thời Hạn (ngày) *    │
├─────────────────────────────────────┤
│ Mô Tả *                             │
├─────────────────────────────────────┤
│ Hình Ảnh Gói Tập                    │  ← MỚI
│ [URL input] [Chọn Hình]             │
│     [Preview]                       │
├─────────────────────────────────────┤
│ Loại Gói Tập                        │
└─────────────────────────────────────┘
```

**Detail Page:**
```
Breadcrumb > Trang chủ > Gói tập > Chi tiết gói
────────────────────────────────────────────────

     Header (Tên, Thời hạn, Giá)

┌──────────────────────────────────────────┐
│                                          │
│      🖼️ HÌNH ÁNH GÓI TẬP (Mới)         │
│         (400px cao, responsive)         │
│                                          │
└──────────────────────────────────────────┘

📋 Mô Tả Gói Tập
...

✅ Tính Năng Bao Gồm
...
```

---

**Cập nhật lần cuối:** $(DateTime.Now.ToString("dd/MM/yyyy"))

Bây giờ bạn có thể sử dụng tính năng hình ảnh cho các gói tập! 🎉
