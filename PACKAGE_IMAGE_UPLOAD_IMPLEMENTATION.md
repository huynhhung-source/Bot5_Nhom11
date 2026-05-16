# Package Image Upload Implementation

## Summary of Changes

You can now upload images directly in the Package Edit and Create pages. The previous implementation only allowed entering image URLs manually. This update adds complete file upload functionality with drag-and-drop support.

## Features Added

### 1. **File Upload with Drag-and-Drop**
   - Users can drag and drop image files directly onto the upload area
   - Click to browse and select files from the file system
   - Visual feedback with hover and drag-over states

### 2. **Image Preview**
   - Real-time preview of selected image before saving
   - Shows file name and size information
   - Displays URL-based images as well

### 3. **File Validation**
   - Validates file type (JPG, PNG, GIF, WebP only)
   - Enforces maximum file size limit of 5MB
   - User-friendly error messages

### 4. **Smart Image Handling**
   - Option to upload a new file
   - Option to enter a URL manually
   - Automatic deletion of old local images when replaced
   - Preserves existing image if no new upload or URL is provided

### 5. **Automatic Directory Management**
   - Creates `/images/packages/` directory automatically if it doesn't exist
   - Generates unique filenames using GUID to prevent collisions
   - Stores uploaded files securely on the server

## Files Modified

### 1. **doanweb/Areas/Admin/Views/Package/Edit.cshtml**
   - Added file upload input with drag-and-drop zone
   - Added client-side file validation
   - Added image preview functionality
   - Maintained URL input as fallback option

### 2. **doanweb/Areas/Admin/Views/Package/Create.cshtml**
   - Added identical file upload functionality as Edit view
   - Consistent user experience between Create and Edit pages

### 3. **doanweb/Areas/Admin/Controllers/PackageController.cs**
   - Added `IWebHostEnvironment` dependency injection
   - Updated `Create()` POST action to handle `IFormFile? ImageFile` parameter
   - Updated `Edit()` POST action to handle `IFormFile? ImageFile` parameter
   - Added `SaveUploadedFile()` helper method for secure file saving
   - Added `DeleteOldFile()` helper method for cleanup
   - Updated `Delete()` action to clean up associated images
   - Added comprehensive error handling and logging

## How to Use

### Creating a New Package with Image

1. Navigate to Admin ? Package Management ? Add New Package
2. Fill in package details (Name, Price, Duration, Description)
3. **Upload Image Option:**
   - Drag and drop an image file onto the drop zone, OR
   - Click the drop zone to browse and select an image file
   - Or enter an image URL if preferred
4. Click "Add Package" to save

### Editing a Package with New Image

1. Navigate to Admin ? Package Management ? Edit Package
2. Update package details as needed
3. **To Change Image:**
   - Drag and drop a new image file onto the drop zone, OR
   - Click to browse and select a new image file
   - The old image will be automatically deleted from the server
4. **To Keep Existing Image:**
   - Leave the upload area empty
   - Don't enter a new URL
5. Click "Update" to save changes

### Supported Image Formats
- **JPEG** (.jpg, .jpeg)
- **PNG** (.png)
- **GIF** (.gif)
- **WebP** (.webp)
- **Maximum File Size:** 5MB

## Technical Details

### File Upload Process

1. **Client-Side Validation:**
   - File type check (must be image)
   - File size check (max 5MB)
   - User-friendly error messages

2. **Server-Side Processing:**
   - Additional file type validation
   - Size verification
   - Automatic directory creation
   - Unique filename generation (GUID + original extension)
   - Secure file storage in `/wwwroot/images/packages/`

3. **Database Storage:**
   - Only the relative path is stored (e.g., `/images/packages/abc123.jpg`)
   - Old local images are deleted when replaced
   - External URLs are preserved if entered

### File Structure
```
wwwroot/
??? images/
    ??? packages/
        ??? a1b2c3d4-e5f6-4a7b-8c9d-e0f1g2h3i4j5.jpg
        ??? b2c3d4e5-f6g7-5b8c-9d0e-f1g2h3i4j5k6.png
        ??? ... (other uploaded package images)
```

## Error Handling

### Common Error Messages

| Error | Cause | Solution |
|-------|-------|----------|
| "Vui lòng ch?n file ?nh!" | Selected file is not an image | Choose a valid image file (JPG, PNG, GIF, WebP) |
| "Kích th??c file không ???c v??t quá 5MB!" | File is too large | Compress the image or choose a smaller file |
| "Lo?i file không ???c phép..." | Unsupported file type | Use JPG, PNG, GIF, or WebP format |
| "?ã x?y ra l?i khi t?o/c?p nh?t gói t?p" | Server error | Check server logs and try again |

## Browser Compatibility

- Modern browsers (Chrome, Firefox, Safari, Edge)
- Requires HTML5 File API and FormData support
- Drag-and-drop supported on all modern browsers

## Security Considerations

1. **File Type Validation:**
   - Client-side and server-side validation
   - MIME type checking
   - Extension verification

2. **File Size Limits:**
   - 5MB maximum size enforced
   - Prevents server storage issues

3. **Filename Handling:**
   - GUID-based naming prevents directory traversal
   - Original extension preserved for compatibility
   - No user-provided filename used

4. **Cleanup:**
   - Old images automatically deleted when replaced
   - Orphaned images removed when packages are deleted

## Performance Optimization

1. **Async File Operations:**
   - Non-blocking file I/O operations
   - Improved server responsiveness

2. **Unique Filenames:**
   - GUID-based names prevent conflicts
   - Multiple uploads of same file create separate entries

3. **Lazy Directory Creation:**
   - Directory created only when first file is uploaded
   - No unnecessary disk operations

## Maintenance

### To Add Support for Additional File Types:

Edit `PackageController.cs` in the `SaveUploadedFile()` method:

```csharp
var allowedTypes = new[] { 
    "image/jpeg", 
    "image/png", 
    "image/gif", 
    "image/webp",
    "image/svg+xml"  // Add this line for SVG
};
```

### To Change Maximum File Size:

Edit the size check in `SaveUploadedFile()`:

```csharp
// Change from 5MB to 10MB
if (file.Length > 10 * 1024 * 1024)
{
    throw new Exception("Kích th??c file không ???c v??t quá 10MB");
}
```

### To Change Upload Directory:

Edit the path in `SaveUploadedFile()`:

```csharp
// Change from /images/packages to /uploads/packages
var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "packages");
```

## Testing Checklist

- [ ] Upload a new image when creating a package
- [ ] Upload a new image when editing a package
- [ ] Verify old image is deleted when replaced
- [ ] Test with files > 5MB (should fail)
- [ ] Test with non-image files (should fail)
- [ ] Verify image preview displays correctly
- [ ] Test drag-and-drop functionality
- [ ] Verify images are accessible on the website
- [ ] Check server logs for any errors

## Troubleshooting

### Images Not Appearing

1. Check if `/images/packages/` directory exists
2. Verify file permissions on the directory
3. Check browser developer console for 404 errors
4. Verify the image path in the database

### Upload Button Not Working

1. Verify JavaScript is enabled in browser
2. Check browser console for JavaScript errors
3. Verify `IWebHostEnvironment` is injected in controller
4. Check server logs for exceptions

### Files Not Being Deleted

1. Verify the old file path format starts with `/images/`
2. Check file system permissions
3. Ensure the file isn't locked by another process
4. Check application logs for deletion errors

## Future Enhancements

- [ ] Image cropping/resizing before upload
- [ ] Multiple image uploads per package
- [ ] Image gallery for each package
- [ ] Thumbnail generation for performance
- [ ] Image optimization/compression
- [ ] CDN integration for image delivery
- [ ] Progress bar for large uploads
- [ ] Image compression before storage

---

## Contact & Support

If you encounter any issues with the image upload functionality, check the application logs at:
`/logs/` (if logging is configured)

For detailed troubleshooting, review the server logs and browser developer console.
