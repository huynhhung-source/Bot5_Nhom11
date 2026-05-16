# H??ng D?n Debug L?i Thanh Toán

## ?? Các b??c ?? tìm nguyên nhân l?i thanh toán

### 1. **M? Browser Console (F12)**
- Khi b?m "Thanh Toán Ngay", m? **DevTools ? Console tab**
- Xem có l?i JavaScript nào không
- Ki?m tra logs t? `submitPaymentForm()` function

### 2. **Ki?m tra Form Submission**
- Trong **DevTools ? Network tab**, b?m "Thanh Toán Ngay"
- Tìm request POST ??n `/payment/checkout`
- Click vào request ?ó xem:
  - **Status code**: 200 (OK) hay 400/500 (Error)?
  - **Response**: Có l?i gì trong response không?
  - **Request Payload**: Các field có ???c send ?úng không?

### 3. **Ki?m tra Application Logs**
- B?t debug mode ho?c check log file
- Tìm logs t? `[Checkout POST]`
- Xem có l?i `PaymentMethod not selected` hay l?i database không?

### 4. **Ki?m tra ModelState Errors**
Trong logs, n?u th?y:
```
[Checkout POST] Invalid model state: ...
```
Thì xem chi ti?t l?i nào trong model

### 5. **Ki?m tra Session**
??m b?o user ?ã **??ng nh?p** khi thanh toán:
- Session `UserId` ph?i t?n t?i
- N?u b? redirect ??n login, thì user ch?a ??ng nh?p

## ?? Các L?i Th??ng G?p & Gi?i Pháp

| L?i | Nguyên Nhân | Gi?i Pháp |
|-----|-----------|---------|
| `PaymentMethod not selected` | Form submit nh?ng PaymentMethod tr?ng | Check xem radio button có ???c ch?n không |
| `Package not found` | Gói t?p không t?n t?i ho?c inactive | Ki?m tra gói t?p trong DB status = 'Active' |
| `User not logged in` | Session h?t h?n ho?c ch?a ??ng nh?p | ??ng nh?p l?i |
| `Invalid model state` | D? li?u form không h?p l? | Xem chi ti?t l?i trong logs |
| Redirects to Packages page | Có l?i x?y ra | Ki?m tra ErrorMessage trong TempData |

## ?? Các ?i?m C?n Ki?m Tra

### ? Ki?m tra Checkout.cshtml
```javascript
// F12 ? Console, gõ:
document.getElementById('paymentMethod').value  // Should be 'BankTransfer' or 'Cash'
document.querySelector('input[name="paymentMethodRadio"]:checked')?.value  // Should not be null
```

### ? Ki?m tra Form Data
```javascript
// F12 ? Console, gõ:
const form = new FormData(document.getElementById('paymentForm'));
for (let [key, value] of form.entries()) {
    console.log(key, value);
}
```

### ? Ki?m tra Network Request
1. F12 ? Network tab
2. B?m "Thanh Toán Ngay"
3. Tìm POST request `/payment/checkout`
4. Tab "Request" ? Xem Payload có PaymentMethod không

## ?? Debugging Checklist

- [ ] Radio button "Chuy?n Kho?n Ngân Hàng" ho?c "Ti?n M?t" có ???c ch?n?
- [ ] Khi b?m "Thanh Toán Ngay", có modal QR code hi?n ra?
- [ ] B?m "?ã Thanh Toán" trong modal, form có submit?
- [ ] Network tab có th?y POST request?
- [ ] POST request status code là 200 hay 4xx/5xx?
- [ ] Response có JSON `{success: true}` hay error message?
- [ ] User ?ã ??ng nh?p tr??c khi thanh toán?
- [ ] Gói t?p có status = 'Active' trong database?

## ?? N?u v?n l?i

1. **C?p nh?t logs**:
   - Xem `Program.cs` xem logging level
   - Thêm chi ti?t logs n?u c?n

2. **Ki?m tra Database**:
   ```sql
   -- Check n?u gói t?p t?n t?i
   SELECT * FROM Packages WHERE PackageId = [ID] AND Status = 'Active'
   
   -- Check subscription sau khi thanh toán
   SELECT * FROM Subscriptions WHERE UserId = [ID] ORDER BY SubscriptionId DESC LIMIT 1
   ```

3. **Th? l?i test payment**:
   - ??ng nh?p account khác
   - Th? gói t?p khác
   - Xem có pattern l?i nào không
