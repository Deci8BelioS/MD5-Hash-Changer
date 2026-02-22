using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace MD5_Hash_Changer.UI;

public class FolderPicker
{
    private readonly List<string> _resultPaths = [];
    private readonly List<string> _resultNames = [];

    public IReadOnlyList<string> ResultPaths => _resultPaths;
    public IReadOnlyList<string> ResultNames => _resultNames;
    public string? ResultPath => ResultPaths.Count > 0 ? ResultPaths[0] : null;
    public string? ResultName => ResultNames.Count > 0 ? ResultNames[0] : null;

    public string? InputPath { get; set; }
    public bool ForceFileSystem { get; set; }
    public bool Multiselect { get; set; }
    public string? Title { get; set; }
    public string? OkButtonLabel { get; set; }
    public string? FileNameLabel { get; set; }

    public bool? ShowDialog(IntPtr owner)
    {
        var dialog = (IFileOpenDialog)new FileOpenDialog();
        try
        {
            if (!string.IsNullOrEmpty(InputPath) && Directory.Exists(InputPath))
            {
                if (SHCreateItemFromParsingName(InputPath, null!, typeof(IShellItem).GUID, out var item) == 0)
                {
                    dialog.SetFolder(item);
                }
            }
            var options = FOS.FOS_PICKFOLDERS;
            if (ForceFileSystem) options |= FOS.FOS_FORCEFILESYSTEM;
            if (Multiselect) options |= FOS.FOS_ALLOWMULTISELECT;
            dialog.SetOptions(options);
            if (Title != null) dialog.SetTitle(Title);
            if (OkButtonLabel != null) dialog.SetOkButtonLabel(OkButtonLabel);
            if (FileNameLabel != null) dialog.SetFileName(FileNameLabel);
            if (owner == IntPtr.Zero)
            {
                owner = Process.GetCurrentProcess().MainWindowHandle;
                if (owner == IntPtr.Zero) owner = GetDesktopWindow();
            }
            var hr = dialog.Show(owner);
            if (hr == ERROR_CANCELLED) return null;
            if (hr != 0) return false;
            if (dialog.GetResults(out var items) != 0) return false;
            items.GetCount(out var count);
            _resultPaths.Clear();
            _resultNames.Clear();
            for (var i = 0; i < count; i++)
            {
                items.GetItemAt(i, out var item);
                if (item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out var path) == 0)
                    _resultPaths.Add(path);
                if (item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEEDITING, out var name) == 0)
                    _resultNames.Add(name);
                Marshal.ReleaseComObject(item);
            }
            Marshal.ReleaseComObject(items);
            return true;
        }
        finally
        {
            Marshal.ReleaseComObject(dialog);
        }
    }

    [DllImport("shell32")]
    private static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IShellItem ppv);

    [DllImport("user32")]
    private static extern IntPtr GetDesktopWindow();

    private const int ERROR_CANCELLED = unchecked((int)0x800704C7);

    [ComImport, Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")]
    private class FileOpenDialog { }

    [ComImport, Guid("d57c7288-d4ad-4768-be02-9d969532d960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IFileOpenDialog
    {
        [PreserveSig] int Show(IntPtr parent);
        [PreserveSig] int SetFileTypes();
        [PreserveSig] int SetFileTypeIndex(int iFileType);
        [PreserveSig] int GetFileTypeIndex(out int piFileType);
        [PreserveSig] int Advise();
        [PreserveSig] int Unadvise();
        [PreserveSig] int SetOptions(FOS fos);
        [PreserveSig] int GetOptions(out FOS pfos);
        [PreserveSig] int SetDefaultFolder(IShellItem psi);
        [PreserveSig] int SetFolder(IShellItem psi);
        [PreserveSig] int GetFolder(out IShellItem ppsi);
        [PreserveSig] int GetCurrentSelection(out IShellItem ppsi);
        [PreserveSig] int SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        [PreserveSig] int GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
        [PreserveSig] int SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
        [PreserveSig] int SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);
        [PreserveSig] int SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
        [PreserveSig] int GetResult(out IShellItem ppsi);
        [PreserveSig] int AddPlace(IShellItem psi, int alignment);
        [PreserveSig] int SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
        [PreserveSig] int Close(int hr);
        [PreserveSig] int SetClientGuid();
        [PreserveSig] int ClearClientData();
        [PreserveSig] int SetFilter([MarshalAs(UnmanagedType.IUnknown)] object pFilter);
        [PreserveSig] int GetResults(out IShellItemArray ppenum);
        [PreserveSig] int GetSelectedItems([MarshalAs(UnmanagedType.IUnknown)] out object ppsai);
    }

    [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellItem
    {
        [PreserveSig] int BindToHandler();
        [PreserveSig] int GetParent();
        [PreserveSig] int GetDisplayName(SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
        [PreserveSig] int GetAttributes();
        [PreserveSig] int Compare();
    }

    [ComImport, Guid("b63ea76d-1f85-456f-a19c-48159efa858b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellItemArray
    {
        [PreserveSig] int BindToHandler();
        [PreserveSig] int GetPropertyStore();
        [PreserveSig] int GetPropertyDescriptionList();
        [PreserveSig] int GetAttributes();
        [PreserveSig] int GetCount(out int pdwNumItems);
        [PreserveSig] int GetItemAt(int dwIndex, out IShellItem ppsi);
        [PreserveSig] int EnumItems();
    }

    private enum SIGDN : uint
    {
        SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
        SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000
    }

    [Flags]
    private enum FOS : uint
    {
        FOS_PICKFOLDERS = 0x20,
        FOS_FORCEFILESYSTEM = 0x40,
        FOS_ALLOWMULTISELECT = 0x200,
        FOS_PATHMUSTEXIST = 0x800
    }
}