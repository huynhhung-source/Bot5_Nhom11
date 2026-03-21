# Admin Dashboard - Thông Tin Chi Tiết UI/UX

## 🎨 Thiết Kế Tổng Quát

### Layout Grid
```
┌─────────────────────────────────────────────────────────────┐
│                    TOP BAR (Admin Info)                     │
├──────────────┬──────────────────────────────────────────────┤
│              │                                              │
│  SIDEBAR     │              MAIN CONTENT                    │
│              │                                              │
│  250px       │           Responsive Content                 │
│              │                                              │
│  Fixed       │     - Dashboard Cards                        │
│  Navigation  │     - Statistics                             │
│              │     - Tables & Forms                         │
│              │     - Quick Actions                          │
│              │                                              │
└──────────────┴──────────────────────────────────────────────┘
```

## 📊 Dashboard Section Breakdown

### 1. Sidebar Navigation (250px)
```
┌─────────────────────┐
│ 🛡️ ADMIN PANEL      │ (Header)
├─────────────────────┤
│ 📊 Dashboard        │
│ 👥 Quản lý Người    │
│ 🛍️  Quản lý Gói     │
│ 📅 Quản lý Lớp      │
│ 💳 Quản lý Thanh    │
├─────────────────────┤
│ 📈 Báo cáo          │
│ ⚙️  Cài đặt          │
│ 🚪 Đăng xuất        │
└─────────────────────┘
```

**Styling:**
- Background: Gradient (Dark Navy Blue)
- Text: White with hover highlight
- Active: Orange left border + background
- Icons: 20px width for alignment
- Hover: Orange text + light background

### 2. Top Bar

```
┌──────────────────────────────────────────────────────────────┐
│ ☰               User Name        🖼️  [👤 Admin Logout btn]  │
│ Menu            "Tên Admin"           [Logout]               │
│ Toggle          (On Desktop: hidden)                         │
└──────────────────────────────────────────────────────────────┘
```

**Features:**
- Position: Sticky/Fixed
- Height: ~60px
- Background: White
- Shadow: Light drop shadow
- Responsive: Menu toggle shows on mobile

### 3. Statistics Cards (4 Cards Layout)

```
Desktop (4 columns):
┌────────┬────────┬────────┬────────┐
│ Users  │Package │Subscr. │Revenue │
│ 🧑    │ 🛍️    │ ✅    │ 💰    │
└────────┴────────┴────────┴────────┘

Tablet (2 columns):
┌────────┬────────┐
│ Users  │Package │
│ 🧑    │ 🛍️    │
├────────┼────────┤
│ Subscr.│Revenue │
│ ✅    │ 💰    │
└────────┴────────┘

Mobile (1 column):
┌────────┐
│ Users  │
│ 🧑    │
├────────┤
│Package │
│ 🛍️    │
├────────┤
│ Subscr.│
│ ✅    │
├────────┤
│Revenue │
│ 💰    │
└────────┘
```

**Card Elements:**
```
┌──────────────────────────────────┐
│ 📊 Title (Uppercase)             │
│                                  │
│ Large Number    Icon             │
│ (e.g., 150)     (Faded 50%)      │
│                                  │
│ ─────────────────────────────   │
│ Small badges/info                │
│ [Link Button]                    │
└──────────────────────────────────┘
```

**Colors by Type:**
- Blue (#007bff) - Users/Primary
- Green (#28a745) - Packages/Success
- Yellow (#ffc107) - Subscriptions/Warning
- Orange (#f36100) - Revenue/Brand

### 4. Secondary Statistics (3 Cards)

```
┌─────────────────┬─────────────────┬─────────────────┐
│ 🛍️ Packages     │ 👥 Users        │ 📊 Statistics   │
│                 │                 │                 │
│ 25 Packages     │ 150 Users       │ 50M Doanh Thu   │
│ Ready           │ Active          │                 │
│                 │                 │                 │
│ [Manage Btn]    │ [Manage Btn]    │ [Details Btn]   │
└─────────────────┴─────────────────┴─────────────────┘
```

### 5. Quick Access Section

```
┌──────────────────────────────┬──────────────────────────────┐
│ ➕ ADD NEW                   │ 📋 LISTS                     │
├──────────────────────────────┼──────────────────────────────┤
│ [+ Add User]  [+ Add Package]│ [👥 Users] [🛍️ Packages]    │
│                              │                              │
│ (2-column layout on desktop) │ (2-column layout on desktop) │
└──────────────────────────────┴──────────────────────────────┘
```

### 6. Detailed Statistics Table

```
┌────────┬────────┬────────┬────────┬────────┬────────┐
│ 150    │ 25     │ 480    │ 350    │ 1250   │ 18     │
│ Users  │Package │Subscr. │ Active │Payments│Classes │
│ 🧑    │ 🛍️    │ ✅    │ 🔥    │ 💳    │ 💪    │
└────────┴────────┴────────┴────────┴────────┴────────┘
```

## 🎨 Color Palette

| Use Case | Color | Hex | RGB |
|----------|-------|-----|-----|
| Primary | Blue | #007bff | (0, 123, 255) |
| Success | Green | #28a745 | (40, 167, 69) |
| Warning | Yellow | #ffc107 | (255, 193, 7) |
| Danger | Red | #dc3545 | (220, 53, 69) |
| Info | Teal | #17a2b8 | (23, 162, 184) |
| Brand | Orange | #f36100 | (243, 97, 0) |
| Background | Light Gray | #f8f9fa | (248, 249, 250) |
| Text | Dark | #333 | (51, 51, 51) |
| Border | Gray | #ddd | (221, 221, 221) |

## 🔤 Typography

```
Headings:
- h1: 36px, bold (Dashboard title)
- h2: 32px, bold (Card titles large)
- h3: 24px, bold (Card titles)
- h4: 20px, semi-bold (Subtitles)
- h5: 16px, semi-bold (Section titles)
- h6: 14px, semi-bold (Labels)

Body Text:
- Regular: 14px, normal
- Small: 12px, normal
- Muted: 12px, gray color

Fonts:
- Main: 'Muli', sans-serif
- Headings: 'Oswald', sans-serif
```

## 🎯 Interactive States

### Buttons
```
Normal:    [Button Text]     (Background color)
Hover:     [Button Text]     (Darker color + scale 1.02)
Active:    [Button Text]     (Color + underline)
Disabled:  [Button Text]     (Gray + opacity 0.5)
```

### Cards
```
Normal:    ┌─────────────┐   (Light shadow)
           │   Content   │
           └─────────────┘

Hover:     ┌─────────────┐   (Translate up -5px)
           │   Content   │   (Darker shadow)
           └─────────────┘
```

### Menu Items (Sidebar)
```
Normal:    [Icon] Label           (White text)
Hover:     [Icon] Label           (Orange text + bg + left border)
Active:    [Icon] Label           (Orange text + bg + left border)
```

## 📱 Responsive Breakpoints

```
Desktop (>992px):
- Sidebar: Fixed left (250px)
- Content: 4-column grid
- Top bar: Full width

Tablet (768px - 992px):
- Sidebar: Collapsible
- Content: 2-column grid
- Top bar: Adjusted padding

Mobile (<768px):
- Sidebar: Offcanvas/Slide-in
- Content: 1-column stack
- Top bar: Minimal with menu toggle
```

## 🔄 Animation & Transitions

```css
Default Transition: all 0.3s ease

Cards:      Transform (translateY -5px) on hover
Buttons:    Scale (1.02) on hover
Links:      Color change (0.3s)
Sidebar:    Slide in/out (0.3s) on mobile
Menu:       Highlight (0.3s)
```

## 📐 Spacing & Layout

```
Container Padding:  30px (Desktop) / 20px (Tablet) / 15px (Mobile)
Card Margin:        15px
Card Padding:       20px (body)
Button Padding:     10px 16px
Input Padding:      10px 12px
Border Radius:      4px (inputs/buttons), 8px (cards)
Box Shadow:         0 2px 8px rgba(0,0,0,0.1)
Hover Shadow:       0 5px 20px rgba(0,0,0,0.15)
```

## 📊 Data Display Patterns

### Statistics Card
```
┌──────────────────────────────┐
│ UPPERCASE LABEL              │
│ ────────────────────────────│
│ 1,234        [Icon 50%]      │
│ Large Value                  │
│ ────────────────────────────│
│ [Badge] [Badge]              │
│ [Link Button]                │
└──────────────────────────────┘
```

### Status Badges
```
Success:  ✅ Active       (Green background, dark text)
Danger:   ❌ Inactive     (Red background, dark text)
Warning:  ⚠️  Pending     (Yellow background, dark text)
Info:     ℹ️  Processing  (Teal background, dark text)
```

## ✅ Accessibility Features

- High contrast colors for readability
- Semantic HTML structure
- ARIA labels where appropriate
- Keyboard navigation support
- Focus states visible
- Screen reader friendly

## 📱 Mobile-First Features

1. **Top menu toggle** for sidebar
2. **Collapsible cards** on small screens
3. **Single column layout** on mobile
4. **Optimized spacing** for touch
5. **Larger touch targets** (48px minimum)

