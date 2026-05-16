# ?? Thêm Nút "My Order" (??n Hàng C?a Tôi)

## ? Hoàn Thành

?ã thêm nút "??n Hàng" vào header c?a trang web, gi?a icon gi? hàng và tên user.

---

## ?? V? Trí Nút

**Header** ? **Khi user ?ã ??ng nh?p**

```
[Gi? Hàng] [??n Hàng] [Tên User] | [Tài Kho?n] | [??ng Xu?t]
```

---

## ?? Liên K?t

- **URL**: `/Order`
- **Action**: `MyOrders()`
- **Controller**: `OrderController`

---

## ?? Giao Di?n

### **Danh Sách ??n Hàng**

| Thông Tin | Chi Ti?t |
|-----------|---------|
| **Mã ??n Hàng** | #123 |
| **Ngày ??t** | 15/01/2024 10:30 |
| **T?ng Ti?n** | 2,000,000? |
| **Tr?ng Thái** | Ch? x? lý |
| **Thao Tác** | Xem Chi Ti?t / H?y |

### **Tr?ng Thái ??n Hàng**

- ?? **Ch? x? lý** (Pending) - Có th? h?y
- ? **?ã xác nh?n** (Confirmed) - Không th? h?y
- ?? **?ang giao** (Shipped) - Không th? h?y
- ?? **?ã giao** (Delivered) - Không th? h?y
- ? **?ã h?y** (Cancelled) - Màu ??

---

## ?? Code Thay ??i

### **1. File: `_Layout.cshtml`**

**Thêm nút "??n Hàng" gi?a Gi? Hàng và Tên User:**

```html
<!-- My Order Link -->
<a href="/Order" style="color: #ffffff; font-size: 14px; text-decoration: none; 
   font-weight: 600; transition: color 0.3s; display: flex; align-items: center;">
    <i class="fa fa-list-ul" style="margin-right: 5px;"></i> ??n Hàng
</a>
```

### **2. File: `MyOrders.cshtml`** (T?o m?i)

**Hi?n th? danh sách ??n hàng c?a user**

G?m:
- Breadcrumb navigation
- B?ng danh sách ??n hàng (Mã, Ngày, Ti?n, Tr?ng Thái)
- Nút "Xem Chi Ti?t" & "H?y" (n?u tr?ng thái là Pending)
- Responsive design

---

## ?? Lu?ng Ho?t ??ng

```
User Click "??n Hàng"
        ?
        GET /Order
        ?
OrderController.MyOrders()
        ?
L?y userId t? Session
        ?
Query: WHERE UserId = userId
        ?
OrderBy: CreatedDate DESC
        ?
Return View: MyOrders.cshtml
        ?
Hi?n th? danh sách ??n hàng
        ?
User click "Xem Chi Ti?t"
        ?
?i ??n OrderController.Details(id)
        ?
OR
        ?
User click "H?y" (n?u tr?ng thái = Pending)
        ?
POST /Order/CancelOrder/{id}
        ?
C?p nh?t Status = "Cancelled"
        ?
Quay l?i trang MyOrders
```

---

## ?? Tính N?ng

? Xem danh sách t?t c? ??n hàng c?a user  
? S?p x?p theo ngày ??t hàng (m?i nh?t tr??c)  
? Hi?n th? thông tin: Mã, Ngày, Ti?n, Tr?ng Thái  
? Nút "Xem Chi Ti?t" ? Xem chi ti?t ??n hàng  
? Nút "H?y" (ch? khi status = Pending)  
? Responsive design cho mobile  
? Xác nh?n tr??c khi h?y  
? Thông báo thành công/l?i  

---

## ?? Cách S? D?ng

### **Cho User**

1. ??ng nh?p vào tài kho?n
2. Header s? hi?n th? nút "??n Hàng"
3. Click vào nút "??n Hàng"
4. Xem danh sách t?t c? ??n hàng
5. Click "Xem Chi Ti?t" ?? xem chi ti?t
6. Click "H?y" ?? h?y ??n (n?u tr?ng thái = Pending)

### **Cho Admin**

Admin không th?y nút này (có menu riêng ? Admin Panel)

---

## ?? Responsive Design

| Device | Layout |
|--------|--------|
| **Desktop** | B?ng ??y ?? |
| **Tablet** | Font nh? h?n, padding gi?m |
| **Mobile** | Font 12px, các nút stack |

---

## ?? B?o M?t

? Ch? user ?ã ??ng nh?p m?i th?y nút  
? Ch? xem ???c ??n hàng c?a chính mình  
? Xác nh?n CSRF tr??c khi h?y  
? Ch? h?y ???c khi status = Pending  

---

## ?? Test

1. **User ch?a ??ng nh?p**: Không th?y nút "??n Hàng"
2. **User ?ã ??ng nh?p**: Th?y nút "??n Hàng" ? header
3. **Click nút "??n Hàng"**: Redirect ??n `/Order`
4. **Xem danh sách**: Hi?n th? t?t c? ??n hàng
5. **Click "Xem Chi Ti?t"**: Xem chi ti?t ??n hàng
6. **Click "H?y" (Pending)**: H?y ??n hàng
7. **User khác**: Không th?y ??n hàng c?a user khác

---

## ?? Database

**Table: Orders**
- OrderId (PK)
- UserId (FK)
- OrderDate
- TotalAmount
- Status (Pending, Confirmed, Shipped, Delivered, Cancelled)
- DeliveryAddress
- Notes
- OrderItems (FK)

---

## ? Checklist Hoàn Thành

- [x] Thêm nút "??n Hàng" vào header
- [x] Hi?n th? ch? khi user ?ã ??ng nh?p
- [x] T?o view `MyOrders.cshtml`
- [x] L?y ??n hàng t? database
- [x] Hi?n th? danh sách v?i tr?ng thái
- [x] Thêm nút "Xem Chi Ti?t"
- [x] Thêm nút "H?y" (n?u Pending)
- [x] Responsive design
- [x] Test hoàn ch?nh
- [x] Build thành công

---

**Hoàn thành! Nút "My Order" ?ã s?n sàng s? d?ng. ??**
