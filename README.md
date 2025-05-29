# MD5-Hash-Changer
C# Application to Change MD5 Hash of any file. It works by appending "null" characters to the end of file.

## 🎉 What's New in V1.3.0 - Enhanced Drag & Drop:

### ✨ New Features
- **🎯 Window Centering**: Application now opens in the center of your screen
- **📁 Enhanced Drag & Drop**: Drop both files AND folders with full support
- **🔄 Recursive Folder Processing**: Automatically processes all files in dropped folders and subfolders
- **🎛️ Mixed Content Support**: Drop a combination of files and folders simultaneously

### 🔧 Improvements
- **⚡ Thread Safety**: Fixed progress counters with thread-safe operations using `Interlocked.Increment`
- **🛡️ Error Handling**: Better error messages and graceful failure handling for file access issues
- **📊 UI Consistency**: Improved progress tracking and counter updates across all operations
- **🚀 Performance**: More efficient file processing and memory usage

### 🐛 Bug Fixes
- Fixed progress bar updates during file operations
- Improved bounds checking for progress values
- Better file existence and directory access checks
- Consistent file counter updates in all removal operations

## Previous Features (V1.2):
- Low Memory usage for checking large files
- Parallel Processing
- Basic Drag & Drop File support
- Select Folder functionality

## 📋 System Requirements & Download
- **.NET Framework 4.6.2** (Windows 10/11 built-in)
- **Windows 7/8/10/11** compatible
- Download from [**Release page**](https://github.com/philip47/MD5-Hash-Changer/releases/latest)

### 📥 Quick Download Links:
- **[📦 Latest Release (v1.3.0)](https://github.com/philip47/MD5-Hash-Changer/releases/tag/v1.3.0)**
- **[⬇️ Direct Download - MD5_Hash_Changer_v1.3.0.exe](https://github.com/philip47/MD5-Hash-Changer/releases/download/v1.3.0/MD5_Hash_Changer_v1.3.0.exe)**
- **[⚙️ Configuration File](https://github.com/philip47/MD5-Hash-Changer/releases/download/v1.3.0/MD5_Hash_Changer_v1.3.0.exe.config)**

## 🚀 How to Use:
1. **Download** both `MD5_Hash_Changer_v1.3.0.exe` and `MD5_Hash_Changer_v1.3.0.exe.config`
2. **Place** both files in the same folder
3. **Run** the executable
4. **Drag & Drop** files or entire folders to add them to the list
5. **Process** files to change their MD5 hashes

## 🎯 Enhanced Drag & Drop Features:
- **Single Files**: Drag individual files to add them
- **Multiple Files**: Select and drag multiple files at once
- **Folders**: Drag entire folders to recursively add all contained files
- **Mixed Content**: Drag a combination of files and folders in one operation
- **Nested Folders**: Automatically processes all subdirectories


![md5-hash-changer](https://github.com/ewwink/MD5-Hash-Changer/blob/master/screenshoot.jpg?raw=true)
