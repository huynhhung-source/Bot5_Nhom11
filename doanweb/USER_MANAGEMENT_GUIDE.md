# Hướng Dẫn Sử Dụng Trang Quản Lý Người Dùng

## Giới thiệu
Trang quản lý người dùng cho phép Admin quản lý toàn bộ người dùng trong hệ thống gym, bao gồm: tạo, chỉnh sửa, xem chi tiết và xóa người dùng.

## Các Tính Năng Chính

### 1. Danh Sách Người Dùng (Index)
**URL:** `https://localhost:7237/admin/user/index`

**Tính năng:**
- Hiển thị danh sách tất cả người dùng trong hệ thống
- Hiển thị các thông tin: ID, Tên, Email, Số điện thoại, Giới tính, Ngày tạo, Trạng thái
- Nút hành động: Xem chi tiết, Chỉnh sửa, Xóa
- Nút tạo người dùng mới
- Các badge thể hiện trạng thái: Hoạt động, Không hoạt động, Bị khóa

### 2. Tạo Người Dùng Mới (Create)
**URL:** `https://localhost:7237/admin/user/create`

**Các trường dữ liệu:**
- **Tên Người Dùng** (required): Tên đầy đủ của người dùng
- **Email** (required): Email duy nhất trong hệ thống
- **Số Điện Thoại** (required): Số điện thoại liên hệ
- **Ngày Sinh** (optional): Ngày sinh của người dùng
- **Địa Chỉ** (optional): Địa chỉ liên hệ
- **Giới Tính** (optional): Nam, Nữ hoặc Khác
- **Trạng Thái** (required): Hoạt động, Không hoạt động hoặc Bị khóa

**Lưu ý:** 
- Mật khẩu mặc định: `User@123`
- Vai trò mặc định: Customer
- Email phải là duy nhất

### 3. Chỉnh Sửa Người Dùng (Edit)
**URL:** `https://localhost:7237/admin/user/edit/{id}`

**Tính năng:**
- Chỉnh sửa tất cả thông tin người dùng ngoại trừ mật khẩu
- Hiển thị ngày tạo và ngày cập nhật lần cuối
- Kiểm tra email không trùng với người dùng khác
- Cập nhật trạng thái hoạt động

### 4. Xem Chi Tiết Người Dùng (Details)
**URL:** `https://localhost:7237/admin/user/details/{id}`

**Thông tin hiển thị:**
- **Thông tin cá nhân:** Tên, Email, Số điện thoại, Địa chỉ, Ngày sinh, Giới tính
- **Trạng thái & Lịch sử:** Trạng thái, Vai trò, Ngày tạo, Cập nhật lần cuối
- **Gói Tập Đã Mua:** Danh sách các gói tập đã mua kèm theo thông tin
  - Tên gói tập
  - Giá tiền
  - Ngày bắt đầu và kết thúc
  - Trạng thái gói tập
  - Số ngày còn lại
- **Lịch Sử Thanh Toán:** Danh sách các giao dịch thanh toán
  - Số tiền
  - Ngày thanh toán
  - Phương thức thanh toán
  - Trạng thái thanh toán
  - Mô tả

### 5. Xóa Người Dùng
**Cách thực hiện:**
1. Trong danh sách người dùng, nhấn nút "Xóa" trên hàng người dùng cần xóa
2. Xác nhận xóa trong hộp thoại pop-up
3. Hệ thống sẽ xóa:
   - Thông tin người dùng
   - Tất cả gói tập (subscriptions)
   - Đăng ký lớp tập (class enrollments)
   - Thanh toán (payments)
   - Vai trò (user roles)

## Cấu Trúc Thư Mục

```
doanweb/
├── Areas/
│   └── Admin/
│       ├── Controllers/
│       │   └── UserController.cs (Controller chính)
│       └── Views/
│           └── User/
│               ├── Index.cshtml (Danh sách)
│               ├── Create.cshtml (Thêm người dùng)
│               ├── Edit.cshtml (Chỉnh sửa)
│               └── Details.cshtml (Chi tiết)
```

## Mã Controller

**File:** `doanweb/Areas/Admin/Controllers/UserController.cs`

**Các Action:**
- `Index()` - GET: Hiển thị danh sách người dùng
- `Create()` - GET/POST: Tạo người dùng mới
- `Edit()` - GET/POST: Chỉnh sửa người dùng
- `Details()` - GET: Xem chi tiết người dùng
- `Delete()` - POST: Xóa người dùng (AJAX)

## Các Quy Tắc Xác Thực (Validation)

### Email
- Bắt buộc (required)
- Phải là định dạng email hợp lệ
- Phải duy nhất trong hệ thống (không trùng lặp)

### Tên Người Dùng
- Bắt buộc (required)
- Tối đa 100 ký tự

### Số Điện Thoại
- Bắt buộc (required)
- Tối đa 20 ký tự

### Trạng Thái
- Bắt buộc (required)
- Các giá trị: Active, Inactive, Suspended

## Bảo Mật

### Kiểm Tra Quyền Admin
Tất cả các action trong UserController đều kiểm tra quyền Admin trước khi thực hiện:
```csharp
private bool IsAdmin()
{
    var userRole = HttpContext.Session.GetString("UserRole");
    return !string.IsNullOrEmpty(userRole) && userRole == "Admin";
}
```

Nếu không phải Admin, hệ thống sẽ redirect tới trang login.

### Token CSRF
Tất cả các form POST đều có CSRF token để bảo vệ khỏi tấn công CSRF.

## Mật Khẩu Mặc Định

- **Mật khẩu mặc định:** `User@123`
- **Cách sử dụng:** Khi tạo người dùng mới, hệ thống tự động tạo mật khẩu này
- **Lưu ý:** Người dùng nên thay đổi mật khẩu sau khi đăng nhập lần đầu

## Xử Lý Lỗi

Hệ thống có các thông báo lỗi rõ ràng cho:
- Email đã tồn tại
- Thông tin không hợp lệ
- Người dùng không tồn tại
- Lỗi cơ sở dữ liệu
- Lỗi xóa do có dữ liệu liên quan

## Ghi Chú

- Tất cả các thay đổi được ghi lại với timestamp
- Khi cập nhật, trường `UpdatedDate` được tự động cập nhật
- Khi xóa, tất cả dữ liệu liên quan (subscriptions, payments, enrollments) cũng được xóa
- Các badge màu sắc giúp dễ dàng nhận biết trạng thái người dùng

