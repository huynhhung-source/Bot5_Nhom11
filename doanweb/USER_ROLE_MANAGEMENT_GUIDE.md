# Hướng Dẫn Phân Quyền Người Dùng

## Tính Năng Mới

Admin có thể phân quyền cho người dùng khi **Tạo** hoặc **Chỉnh Sửa** người dùng. Hiện có 2 loại quyền:

- **Admin** - Quản trị viên hệ thống (truy cập toàn bộ tính năng quản lý)
- **Customer** - Khách hàng thường (truy cập chức năng khách hàng)

## Cách Sử Dụng

### 1. Tạo Người Dùng Mới Với Quyền

1. Vào **Admin** → **Quản Lý Người Dùng**
2. Nhấn **Thêm Người Dùng Mới**
3. Điền thông tin người dùng
4. **Phần Quan Trọng**: Chọn quyền từ dropdown **Phân Quyền**
   - Nếu không chọn, mặc định sẽ là **Customer**
5. Nhấn **Thêm Người Dùng**

### 2. Chỉnh Sửa Quyền Người Dùng Hiện Tại

1. Vào **Admin** → **Quản Lý Người Dùng**
2. Nhấn **Sửa** bên cạnh người dùng cần thay đổi
3. Scroll xuống tìm dropdown **Phân Quyền**
4. Chọn quyền mới (Admin hoặc Customer)
5. Nhấn **Cập Nhật**

### 3. Xem Quyền Của Người Dùng

- Trong danh sách người dùng, cột **Quyền** sẽ hiển thị:
  - <img style="background: #dc3545; color: white; padding: 3px 8px; border-radius: 3px;" alt="Admin">  **Admin** (badge đỏ)
  - <img style="background: #17a2b8; color: white; padding: 3px 8px; border-radius: 3px;" alt="Customer">  **Customer** (badge xanh)

## Thông Tin Kỹ Thuật

### Thay Đổi Trong Database

- Bảng **UserRoles** được sử dụng để lưu trữ mối quan hệ giữa User và Role
- Mỗi người dùng có thể có một hoặc nhiều role (hiện tại sử dụng 1 role chính)

### Thay Đổi Trong Code

**Controller** - `Areas/Admin/Controllers/UserController.cs`:
- Thêm method `PopulateRoles()` để lấy danh sách role từ database
- Cập nhật `Create()` POST method để xử lý parameter `roleId`
- Cập nhật `Edit()` POST method để thay đổi role của người dùng

**Views**:
- **Create.cshtml**: Thêm select dropdown cho Phân Quyền
- **Edit.cshtml**: Thêm select dropdown cho Phân Quyền (với JavaScript để set giá trị mặc định)
- **Index.cshtml**: Thêm cột Quyền để hiển thị role của từng người dùng

## Ví Dụ Sử Dụng

### Tạo Admin Mới

```
Tên: Nguyễn Văn A
Email: admin@gym.com
Số Điện Thoại: 0912345678
Phân Quyền: Admin  ← Chọn Admin
```

### Chuyển Customer Thành Admin

1. Tìm người dùng "Trần Thị B" (hiện là Customer)
2. Nhấn **Sửa**
3. Thay đổi **Phân Quyền** từ "Customer" → "Admin"
4. Nhấn **Cập Nhật**

## Ghi Chú

- **Mật khẩu mặc định**: `User@123` khi tạo người dùng mới
- **Trạng thái mặc định**: `Active` (hoạt động)
- **Quyền mặc định**: `Customer` nếu không chọn quyền
- Admin có thể thay đổi quyền của bất kỳ người dùng nào
- Mỗi người dùng sẽ có **1 role chính** (có thể mở rộng cho nhiều role trong tương lai)
