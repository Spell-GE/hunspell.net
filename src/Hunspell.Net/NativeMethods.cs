using System.Runtime.InteropServices;

namespace Hunspell.Net;

internal static partial class NativeMethods
{
    private const string LibName = "hunspell";

    [LibraryImport(LibName, EntryPoint = "Hunspell_create", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial IntPtr Create(string affpath, string dpath);

    [LibraryImport(LibName, EntryPoint = "Hunspell_create_key", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial IntPtr CreateKey(string affpath, string dpath, string key);

    [LibraryImport(LibName, EntryPoint = "Hunspell_destroy")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial void Destroy(IntPtr handle);

    [LibraryImport(LibName, EntryPoint = "Hunspell_add_dic", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int AddDic(IntPtr handle, string dpath);

    [LibraryImport(LibName, EntryPoint = "Hunspell_spell", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int Spell(IntPtr handle, string word);

    [LibraryImport(LibName, EntryPoint = "Hunspell_get_dic_encoding")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial IntPtr GetDicEncoding(IntPtr handle);

    [LibraryImport(LibName, EntryPoint = "Hunspell_suggest", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int Suggest(IntPtr handle, out IntPtr slst, string word);

    [LibraryImport(LibName, EntryPoint = "Hunspell_suffix_suggest", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int SuffixSuggest(IntPtr handle, out IntPtr slst, string word);

    [LibraryImport(LibName, EntryPoint = "Hunspell_analyze", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int Analyze(IntPtr handle, out IntPtr slst, string word);

    [LibraryImport(LibName, EntryPoint = "Hunspell_stem", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int Stem(IntPtr handle, out IntPtr slst, string word);

    [LibraryImport(LibName, EntryPoint = "Hunspell_stem2")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int Stem2(IntPtr handle, out IntPtr slst, IntPtr desc, int n);

    [LibraryImport(LibName, EntryPoint = "Hunspell_generate", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int Generate(IntPtr handle, out IntPtr slst, string word, string word2);

    [LibraryImport(LibName, EntryPoint = "Hunspell_generate2", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int Generate2(IntPtr handle, out IntPtr slst, string word, IntPtr desc, int n);

    [LibraryImport(LibName, EntryPoint = "Hunspell_add", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int Add(IntPtr handle, string word);

    [LibraryImport(LibName, EntryPoint = "Hunspell_add_with_flags", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int AddWithFlags(IntPtr handle, string word, string flags, string desc);

    [LibraryImport(LibName, EntryPoint = "Hunspell_add_with_affix", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int AddWithAffix(IntPtr handle, string word, string example);

    [LibraryImport(LibName, EntryPoint = "Hunspell_remove", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int Remove(IntPtr handle, string word);

    [LibraryImport(LibName, EntryPoint = "Hunspell_free_list")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial void FreeList(IntPtr handle, ref IntPtr slst, int n);
}
