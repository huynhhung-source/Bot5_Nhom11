# Hướng Dẫn Sử Dụng Admin Dashboard

## Giới Thiệu

Admin Dashboard là giao diện quản lý toàn bộ hệ thống gym trực tuyến. Nó cung cấp các thống kê, quản lý người dùng, gói tập, lớp học, thanh toán, và nhiều chức năng khác.

## Cấu Trúc Dashboard

### 1. Sidebar Navigation (Thanh Điều Hướng Bên Trái)

Sidebar cố định ở bên trái màn hình với các menu chính:

- **Dashboard** - Về trang dashboard chính
- **Quản lý Người dùng** - Quản lý tất cả người dùng
- **Quản lý Gói Tập** - Quản lý các gói tập
- **Quản lý Lớp Tập** - Quản lý các lớp tập
- **Quản lý Thanh toán** - Quản lý giao dịch thanh toán
- **Báo cáo** - Xem báo cáo thống kê
- **Cài đặt** - Cài đặt hệ thống
- **Đăng xuất** - Thoát khỏi admin panel

**Tính năng:**
- Highlight menu hiện tại (active state)
- Hover effect cho tất cả menu items
- Responsive trên mobile devices
- Sidebar collapse/expand trên mobile

### 2. Top Bar (Thanh Công Cụ Trên Cùng)

Thanh bar ở trên cùng hiển thị:

- **Menu Toggle** - Nút để mở/đóng sidebar (trên mobile)
- **User Info** - Tên người dùng và avatar
- **Logout Button** - Nút đăng xuất

### 3. Main Dashboard (Trang Chính)

#### A. Thống Kê Chính (4 Cards)

1. **Tổng Người Dùng**
   - Hiển thị: Tổng số người dùng, số người dùng hoạt động
   - Icon: Người dùng (xanh lam)
   - Action: Link quản lý người dùng

2. **Gói Tập Hiệu Động**
   - Hiển thị: Tổng gói tập sẵn sàng
   - Icon: Túi mua sắm (xanh lá)
   - Action: Link quản lý gói tập

3. **Đăng Ký Hoạt Động**
   - Hiển thị: Số đăng ký hoạt động trên tổng đăng ký
   - Icon: Lửa (vàng)
   - Action: Tiến độ bar

4. **Tổng Doanh Thu**
   - Hiển thị: Tổng tiền từ thanh toán thành công
   - Icon: Tiền (cam)
   - Action: Link xem chi tiết

#### B. Thống Kê Phụ (2 Cards)

1. **Lớp Tập**
   - Số lượng lớp tập
   - Link quản lý

2. **Thanh Toán**
   - Số lượng giao dịch
   - Link quản lý

#### C. Quản Lý Nhanh (3 Cards)

1. **Quản Lý Gói Tập**
   - Hiển thị số gói tập
   - Link trực tiếp tới trang quản lý

2. **Quản Lý Người Dùng**
   - Hiển thị số người dùng
   - Link trực tiếp tới trang quản lý

3. **Thống Kê**
   - Hiển thị tổng doanh thu
   - Link trực tiếp

#### D. Thống Kê Chi Tiết

Bảng tóm tắt toàn bộ dữ liệu hệ thống:
- Tổng người dùng
- Tổng gói tập
- Tổng đăng ký
- Số đăng ký hoạt động
- Tổng giao dịch thanh toán
- Tổng lớp tập

## URLs Chính

| Chức năng | URL |
|-----------|-----|
| Dashboard | `/admin/home/index` |
| Dashboard Alternative | `/admin/home/dashboard` |
| Quản lý Người dùng | `/admin/user/index` |
| Thêm Người dùng | `/admin/user/create` |
| Chỉnh sửa Người dùng | `/admin/user/edit/{id}` |
| Chi tiết Người dùng | `/admin/user/details/{id}` |
| Quản lý Gói Tập | `/admin/package/index` |
| Thêm Gói Tập | `/admin/package/create` |
| Chỉnh sửa Gói Tập | `/admin/package/edit/{id}` |

## Thư Mục Cấu Trúc

```
doanweb/
├── Areas/
│   └── Admin/
│       ├── Controllers/
│       │   ├── HomeController.cs (Dashboard logic)
│       │   ├── UserController.cs (User management)
│       │   └── PackageController.cs (Package management)
│       └── Views/
│           ├── Home/
│           │   ├── Index.cshtml (Main dashboard)
│           │   └── Dashboard.cshtml (Alternative view)
│           ├── Shared/
│           │   ├── _Layout.cshtml (Admin layout)
│           │   ├── _AdminSidebar.cshtml (Sidebar navigation)
│           │   └── _ViewStart.cshtml
│           ├── User/
│           │   ├── Index.cshtml
│           │   ├── Create.cshtml
│           │   ├── Edit.cshtml
│           │   └── Details.cshtml
│           └── Package/
│               ├── Index.cshtml
│               ├── Create.cshtml
│               └── Edit.cshtml
├── wwwroot/
│   └── css/
│       ├── admin.css (Admin styles)
│       └── style.css (Main styles)
└── Models/
    └── DatabaseModels.cs
```

## Các Thống Kê Được Hiển Thị

### DashboardViewModel Properties

```csharp
public class DashboardViewModel
{
    public int TotalUsers { get; set; }              // Tổng người dùng
    public int ActiveUsers { get; set; }             // Người dùng hoạt động
    public int TotalPackages { get; set; }           // Tổng gói tập
    public int TotalSubscriptions { get; set; }      // Tổng đăng ký
    public int ActiveSubscriptions { get; set; }     // Đăng ký hoạt động
    public decimal TotalRevenue { get; set; }        // Tổng doanh thu
    public int TotalClasses { get; set; }            // Tổng lớp tập
    public int TotalPayments { get; set; }           // Tổng giao dịch
}
```

## Tính Năng Đặc Biệt

### 1. Responsive Design
- **Desktop**: Sidebar cố định bên trái, full layout
- **Tablet**: Sidebar collapsible
- **Mobile**: Sidebar offcanvas, top bar simplified

### 2. Color Scheme
- Primary: #007bff (Blue)
- Success: #28a745 (Green)
- Warning: #ffc107 (Yellow)
- Danger: #dc3545 (Red)
- Info: #17a2b8 (Teal)
- Brand: #f36100 (Orange)

### 3. Interactive Elements
- Hover effects trên cards
- Smooth transitions
- Badge badges cho status
- Progress bars cho subscriptions

### 4. Security
- Kiểm tra quyền Admin bắt buộc
- Session-based authentication
- CSRF token validation

## Customization

### Thêm Menu Item Mới

Edit `_AdminSidebar.cshtml`:
```razor
<li class="nav-item">
    <a class="nav-link" href="/admin/new-feature/index">
        <i class="fa fa-icon-name"></i> Tên Menu
    </a>
</li>
```

### Thay đổi Style

Edit `wwwroot/css/admin.css`:
```css
.stat-card {
    /* Custom styles */
}
```

### Thêm Thống Kê Mới

1. Thêm property vào `DashboardViewModel`
2. Tính toán giá trị trong `HomeController.Index()`
3. Thêm card/display trong view

## Best Practices

1. **Security**
   - Luôn kiểm tra quyền Admin
   - Validate input dữ liệu
   - Use CSRF tokens

2. **Performance**
   - Load dữ liệu async
   - Optimize database queries
   - Cache thường xuyên sử dụng dữ liệu

3. **UX**
   - Keep sidebar navigation simple
   - Use consistent colors
   - Provide clear feedback
   - Mobile-first approach

## Troubleshooting

### Sidebar không hiển thị
- Kiểm tra CSS file `admin.css` được load
- Verify `_AdminSidebar.cshtml` được render
- Check z-index conflicts

### Dashboard không hiển thị dữ liệu
- Verify database connection
- Check HomeController action
- Inspect browser console for errors

### Responsive issues
- Test breakpoints: 576px, 768px, 992px, 1200px
- Use browser DevTools
- Check viewport meta tag

## Liên Hệ & Hỗ Trợ

Nếu gặp vấn đề:
1. Kiểm tra logs
2. Xem browser console
3. Kiểm tra database connection
4. Verify user permissions

