# H??ng D?n Phân Lo?i Gói T?p Online & Offline

## ?? T?ng Quan
H? th?ng qu?n lý gói t?p hi?n t?i ?ã ???c c?p nh?t ?? **t? ??ng phân lo?i** gói t?p d?a trên `PackageType`:
- **Gói Online** (`PackageType = "Online"`) ? Hi?n th? ? `/Packages/Online`
- **Gói Offline** (`PackageType = "Offline"`) ? Hi?n th? ? `/Packages/Offline`

---

## ?? Các Thay ??i Chính

### 1. **PackagesController.cs** (Controllers)
File controller m?i có các action:

```csharp
// Trang Gói T?p Online - ch? hi?n th? gói có PackageType = "Online"
public async Task<IActionResult> Online()
{
    var packages = await _dbContext.Packages
        .Where(p => p.Status == "Active" && 
               (p.PackageType == "Online" || p.PackageType == null))
        .OrderByDescending(p => p.CreatedDate)
        .ToListAsync();
    return View(packages);
}

// Trang Gói T?p Offline - ch? hi?n th? gói có PackageType = "Offline"
public async Task<IActionResult> Offline()
{
    var packages = await _dbContext.Packages
        .Where(p => p.Status == "Active" && p.PackageType == "Offline")
        .OrderByDescending(p => p.CreatedDate)
        .ToListAsync();
    return View(packages);
}

// Chi ti?t gói t?p
public async Task<IActionResult> Detail(int id)
{
    var package = await _dbContext.Packages
        .FirstOrDefaultAsync(p => p.PackageId == id && p.Status == "Active");
    return View(package);
}
```

### 2. **Detail.cshtml** (Views/Packages)
Trang chi ti?t gói t?p m?i hi?n th?:
- Hình ?nh gói t?p
- Giá và th?i h?n
- Thông tin gói (lo?i, danh m?c, s? bu?i)
- Mô t? chi ti?t
- Tính n?ng & l?i ích
- Nút thanh toán
- Nút quay l?i t??ng ?ng (Online/Offline)

---

## ?? Cách S? D?ng

### **B??c 1: T?o Gói T?p M?i**
1. Vào Admin Panel ? Qu?n Lý Gói T?p ? Thêm Gói T?p
2. ?i?n thông tin:
   - Tên Gói: `"Fitness Pro Online"` ho?c `"Fitness Pro Offline"`
   - Lo?i Gói: Ch?n `"Online"` ho?c `"Offline"`
   - Giá, Th?i H?n, Tính N?ng, v.v.
3. B?m "T?o"

### **B??c 2: Xem Gói T?p**
- **Gói Online**: Vào `/Packages/Online`
- **Gói Offline**: Vào `/Packages/Offline`

### **B??c 3: Xem Chi Ti?t & Thanh Toán**
- B?m "Tham gia ngay" trên b?t k? gói nào
- Xem trang chi ti?t (`/Packages/Detail/{id}`)
- B?m "Thanh Toán Ngay" ?? chuy?n ??n trang thanh toán

---

## ?? Lu?ng D? Li?u

```
Admin t?o gói v?i PackageType
        ?
Package ???c l?u vào DB
        ?
PackagesController.Online() ? L?c PackageType = "Online"
PackagesController.Offline() ? L?c PackageType = "Offline"
        ?
Online.cshtml / Offline.cshtml hi?n th? danh sách
        ?
User b?m "Tham gia ngay"
        ?
PackagesController.Detail() ? Hi?n th? chi ti?t
        ?
User b?m "Thanh Toán Ngay"
        ?
Chuy?n ??n PaymentController.Checkout()
```

---

## ?? Ví D? C? Th?

### Gói T?p Online:
| Tr??ng | Giá Tr? |
|--------|--------|
| PackageName | Yoga Online 30 Ngày |
| PackageType | **Online** |
| Price | 500,000? |
| DurationDays | 30 |
| Status | Active |

? **K?t qu?**: Gói này s? **ch? hi?n th?** ? `/Packages/Online`

### Gói T?p Offline:
| Tr??ng | Giá Tr? |
|--------|--------|
| PackageName | Gym Membership 3 Tháng |
| PackageType | **Offline** |
| Price | 1,500,000? |
| DurationDays | 90 |
| Status | Active |

? **K?t qu?**: Gói này s? **ch? hi?n th?** ? `/Packages/Offline`

---

## ? L?i Ích

? **T? ??ng phân lo?i** - Không c?n qu?n lý th? công  
? **N?m g?n** - M?i trang ch? hi?n th? gói ?úng lo?i  
? **D? qu?n lý** - Thêm/s?a/xóa gói r?t ??n gi?n  
? **Tr?i nghi?m t?t** - Ng??i dùng không b? l?n l?n  
? **D? m? r?ng** - Có th? thêm lo?i gói m?i d? dàng  

---

## ?? Các Tính N?ng B? Sung

### API Endpoints (Tùy Ch?n):
```csharp
GET /Packages/GetPackagesByType?type=Online
GET /Packages/GetPackagesByType?type=Offline
GET /Packages/GetPackageDetail?id=1
```

---

## ?? Ghi Chú

- Gói v?i `PackageType = null` ho?c tr?ng s? ???c coi là gói **Online**
- Ch? gói có `Status = "Active"` m?i ???c hi?n th?
- Trang chi ti?t s? t? ??ng chuy?n h??ng v? trang danh sách t??ng ?ng khi quay l?i

---

## ? Checklist Hoàn Thành

- [x] T?o `PackagesController.cs` v?i logic l?c
- [x] C?p nh?t `Online.cshtml` ?? ch? l?y gói Online
- [x] C?p nh?t `Offline.cshtml` ?? ch? l?y gói Offline
- [x] T?o `Detail.cshtml` ?? hi?n th? chi ti?t gói
- [x] Build thành công

---

**Hoàn thành! H? th?ng qu?n lý gói t?p Online/Offline ?ã s?n sàng s? d?ng. ??**
