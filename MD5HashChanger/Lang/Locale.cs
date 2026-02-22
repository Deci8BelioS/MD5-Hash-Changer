using System.Globalization;
using System.Resources;
using System.Reflection;

namespace MD5_Hash_Changer.Lang;

public enum AppLanguage { ES, EN }

public static class Locale
{
    private static ResourceManager _resManager = new ResourceManager("MD5_Hash_Changer.Lang.Strings", Assembly.GetExecutingAssembly());
    private static CultureInfo _culture = CultureInfo.CurrentUICulture;

    public static AppLanguage Current { get; private set; } = AppLanguage.EN;

    public static void Set(AppLanguage lang)
    {
        Current = lang;
        _culture = new CultureInfo(lang == AppLanguage.EN ? "en" : "es");
        Thread.CurrentThread.CurrentUICulture = _culture;
        Thread.CurrentThread.CurrentCulture = _culture;
    }

    private static string Get(string key) => _resManager.GetString(key, _culture) ?? key;

    public static string BtnAddFiles => Get("BtnAddFiles");
    public static string BtnAddFolder => Get("BtnAddFolder");
    public static string BtnRemoveSelected => Get("BtnRemoveSelected");
    public static string BtnRemoveAll => Get("BtnRemoveAll");
    public static string BtnStart => Get("BtnStart");
    public static string BtnStop => Get("BtnStop");
    public static string ColFileName => Get("ColFileName");
    public static string ColOldMD5 => Get("ColOldMD5");
    public static string ColNewMD5 => Get("ColNewMD5");
    public static string ColStatus => Get("ColStatus");
    public static string CtxCopyRows => Get("CtxCopyRows");
    public static string CtxDeleteRows => Get("CtxDeleteRows");
    public static string CtxOpenFile => Get("CtxOpenFile");
    public static string CtxExportCSV => Get("CtxExportCSV");
    public static string StatusWaiting => Get("StatusWaiting");
    public static string StatusProcessing => Get("StatusProcessing");
    public static string StatusEmpty => Get("StatusEmpty");
    public static string DlgSelectFiles => Get("DlgSelectFiles");
    public static string DlgExportTitle => Get("DlgExportTitle");
    public static string DlgFilterCSV => Get("DlgFilterCSV");
    public static string DlgFolderTitle => Get("DlgFolderTitle");
    public static string DlgFolderOkBtn => Get("DlgFolderOkBtn");
    public static string DlgFolderNameLabel => Get("DlgFolderNameLabel");
    public static string LabelItem => Get("LabelItem");
    public static string LabelTotal => Get("LabelTotal");
    public static string WindowTitle => Get("WindowTitle");

    public static string ErrDirAccess(string path, string msg)
    {
        return string.Format(Get("ErrDirAccess"), path, msg);
    }
}