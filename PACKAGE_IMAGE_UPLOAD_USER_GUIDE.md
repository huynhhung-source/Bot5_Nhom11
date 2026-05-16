# Package Image Upload - User Guide

## Quick Start

### For Admin Users: How to Upload Package Images

---

## Creating a New Package with Image Upload

### Step 1: Navigate to Package Management
- Go to **Admin Dashboard** ? **Gói T?p** (Packages) ? **Thêm Gói T?p M?i** (Add New Package)

### Step 2: Fill Package Details
- **Tên Gói T?p** (Package Name): Enter the package name
- **Giá** (Price): Enter the price in Vietnamese Dong (?)
- **Th?i H?n** (Duration): Enter duration in days (e.g., 30, 90, 180)
- **Mô T?** (Description): Enter detailed description

### Step 3: Upload Package Image

#### Option A: Drag & Drop (Recommended)
1. Locate the **Hình ?nh Gói T?p** (Package Image) section
2. Drag your image file directly onto the drop zone
3. See the file appear with preview and file size
4. Image preview appears below
5. Click "Thêm Gói T?p" (Add Package) to save

#### Option B: Click to Browse
1. Click anywhere on the drop zone
2. A file browser dialog opens
3. Select your image file (JPG, PNG, GIF, or WebP)
4. File details and preview appear
5. Click "Thêm Gói T?p" (Add Package) to save

#### Option C: Enter URL
1. If you prefer to use an external image URL
2. Enter the full URL in the "Ho?c nh?p URL hình ?nh" field
3. The URL-based image will be used instead of an upload
4. Click "Thêm Gói T?p" (Add Package) to save

### Step 4: Confirm and Save
- Review all package information
- Click **Thêm Gói T?p** (Add Package) button
- Success message appears on the next screen

---

## Editing an Existing Package Image

### Step 1: Navigate to Package List
- Go to **Admin Dashboard** ? **Gói T?p** (Packages) ? **Danh sách Gói T?p** (Package List)

### Step 2: Click Edit Button
- Find the package you want to edit
- Click the **S?a** (Edit) button in the Actions column

### Step 3: Update Package Image

#### To Replace with New File
1. Scroll to the **Hình ?nh Gói T?p** (Package Image) section
2. Your current image is displayed in the preview
3. Drag and drop a NEW image file onto the drop zone
4. The old image will be automatically deleted and replaced
5. New image preview appears
6. Click **C?p Nh?t** (Update) to save

#### To Keep Existing Image
1. Leave the upload area empty
2. Don't change the URL field
3. Click **C?p Nh?t** (Update)
4. Your existing image is preserved

#### To Use External URL Instead
1. Clear the image upload if you had one selected
2. Enter a new URL in the "Ho?c nh?p URL hình ?nh" field
3. Click **C?p Nh?t** (Update)

### Step 4: Confirm Changes
- Review all changes
- Click **C?p Nh?t** (Update) button
- Success message appears on the list page

---

## Supported File Types

? **Accepted Formats:**
- JPEG (.jpg, .jpeg)
- PNG (.png)
- GIF (.gif)
- WebP (.webp)

? **Not Accepted:**
- PSD, AI, SVG (except WebP)
- Documents (PDF, Word, etc.)
- Archives (ZIP, RAR, etc.)
- Videos or Audio files

---

## File Size Requirements

- **Maximum Size:** 5 MB
- **Recommended Size:** 1-3 MB
- **Optimal Dimensions:** 1200x800 pixels or larger

### How to Reduce File Size
1. Use an online image compressor (tinypng.com, squoosh.app)
2. Use image editing software to reduce dimensions
3. Save as JPEG instead of PNG (for photos)
4. Use WebP format for smaller file sizes

---

## Common Issues & Solutions

### Issue: "Vui lòng ch?n file ?nh!" (Please select an image file)
**Cause:** Selected file is not an image file
**Solution:** 
- Make sure you selected a valid image (JPG, PNG, GIF, WebP)
- Don't select documents, videos, or other file types

### Issue: "Kích th??c file không ???c v??t quá 5MB!" (File size exceeds 5MB)
**Cause:** Image file is too large
**Solution:**
- Compress the image using an online tool
- Reduce image dimensions
- Save as a more efficient format (JPEG or WebP)

### Issue: Image doesn't appear after upload
**Cause:** Possible server or permission issue
**Solution:**
- Try uploading again
- Refresh the page and check if image appears
- Try a different image file
- Contact administrator if problem persists

### Issue: Old image not deleted after replacement
**Cause:** File system permission issue
**Solution:**
- Don't worry - it won't be visible to users
- The database only references the new image
- Contact administrator to clean up old files

---

## Best Practices

### ? DO:
- ? Compress images before uploading
- ? Use high-quality images (at least 1000px width)
- ? Use relevant images for the package
- ? Test the image display on the website
- ? Keep file size under 3MB for faster loading

### ? DON'T:
- ? Upload very large files (>5MB)
- ? Use low-resolution images (< 500px)
- ? Upload non-image files
- ? Use watermarked images from competitors
- ? Upload personal or sensitive images

---

## Image Preview & Display

### Where Images Appear
1. **Admin Dashboard:** Package list view
2. **Customer Website:** 
   - Package listing page (Gói T?p Luy?n)
   - Package detail page
   - Search results

### Preview Display
- Rectangular with rounded corners
- Aspect ratio: ~3:2 (width:height)
- Maximum dimensions shown: 200x150px on admin side
- Full-sized on customer-facing pages

---

## Troubleshooting Tips

### Step 1: Check File Type
```
JPG files: thumbnail.jpg ?
PNG files: image.png ?
GIF files: animation.gif ?
WebP files: picture.webp ?
PSD files: design.psd ?
SVG files: logo.svg ?
```

### Step 2: Check File Size
```
1.5 MB file ?
5.0 MB file ? (maximum)
6.5 MB file ? (too large)
```

### Step 3: Browser Compatibility
- Chrome ? (recommended)
- Firefox ?
- Safari ?
- Edge ?
- Internet Explorer ? (not supported)

### Step 4: Clear Browser Cache
If images aren't showing:
1. Press Ctrl+Shift+Delete (or Cmd+Shift+Delete on Mac)
2. Select "All time"
3. Check "Cached images and files"
4. Click "Clear data"
5. Refresh the page

---

## Need Help?

### Contact Information:
- **Admin Email:** admin@gymcenter.com
- **Support Phone:** 125-711-811
- **Support Hours:** 24/7

### FAQs:
**Q: Can I upload multiple images per package?**
A: Currently, only one image per package is supported.

**Q: Can I delete an image without replacing it?**
A: Yes, but the package must have either a file or a URL. You can replace with a placeholder URL.

**Q: How long are uploaded images kept?**
A: As long as the package exists. Images are deleted when the package is deleted.

**Q: Can I edit the image file name?**
A: No, the system automatically generates unique names for security.

**Q: What happens to old images when I update?**
A: Old local images are automatically deleted from the server.

---

## Video Tutorial

[Watch the upload tutorial] (https://example.com/tutorial)

---

**Last Updated:** 2025
**Version:** 1.0
