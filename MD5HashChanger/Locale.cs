namespace MD5_Hash_Changer;

public enum AppLanguage { ES, EN }

public static class Locale
{
    public static AppLanguage Current {get; private set; } = AppLanguage.ES;
    public static void Set(AppLanguage lang) => Current = lang;
    private static string T(string es, string en) => Current == AppLanguage.ES ? es : en;
    // Botones
    public static string BtnAddFiles => T("Añadir\nArchivos", "Add\nFiles");
    public static string BtnAddFolder => T("Añadir\nCarpeta", "Add\nFolder");
    public static string BtnRemoveSelected => T("Quitar\nSelección", "Remove\nSelected");
    public static string BtnRemoveAll => T("Limpiar\nLista", "Clear\nList");
    public static string BtnStart => T("Iniciar\nCambio MD5", "Start\nMD5 Change");
    public static string BtnStop => T("Detener", "Stop");
    // Columnas DataGrid
    public static string ColFileName => T("Nombre de archivo", "File Name");
    public static string ColOldMD5 => T("MD5 anterior", "Old MD5");
    public static string ColNewMD5 => T("MD5 nuevo", "New MD5");
    public static string ColStatus => T("Estado", "Status");
    // Menú contextual
    public static string CtxCopyRows => T("Copiar filas", "Copy rows");
    public static string CtxDeleteRows => T("Eliminar filas", "Delete rows");
    public static string CtxOpenFile => T("Abrir archivo", "Open file");
    public static string CtxExportCSV => T("Exportar a CSV", "Export to CSV");
    // Estados de celda
    public static string StatusWaiting => T("en espera", "waiting");
    public static string StatusProcessing => T("procesando…", "processing…");
    public static string StatusEmpty => T("vacío", "empty");
    // Diálogos
    public static string DlgSelectFiles => T("Seleccionar archivos", "Select files");
    public static string DlgExportTitle => T("Exportar a CSV", "Export to CSV");
    public static string DlgFilterCSV => T("Archivos CSV (*.csv)|*.csv|Todos los archivos (*.*)|*.*", "CSV files (*.csv)|*.csv|All files (*.*)|*.*");
    // FolderPicker
    public static string DlgFolderTitle => T("Seleccionar Carpeta", "Select Folder");
    public static string DlgFolderOkBtn => T("Seleccionar", "Select");
    public static string DlgFolderNameLabel => T("Carpeta:", "Folder:");
    // Mensajes de error
    public static string ErrDirAccess(string path, string msg) => T($"Error al acceder al directorio {path}: {msg}", $"Error accessing directory {path}: {msg}");
    // Labels
    public static string LabelItem => T("ítem:", "item:");
    public static string LabelTotal => T("total:", "total:");
    // Título ventana
    public static string WindowTitle => "MD5 Hash Changer V1.4.1";
}
