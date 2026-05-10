using System.Runtime.InteropServices;

namespace Hunspell.Net;

public sealed class HunspellDictionary : IDisposable
{
    private IntPtr _handle;
    private readonly object _syncLock = new();
    private bool _disposed;

    public HunspellDictionary(string affPath, string dicPath, string? key = null)
    {
        ArgumentNullException.ThrowIfNull(affPath);
        ArgumentNullException.ThrowIfNull(dicPath);

        _handle = key is null
            ? NativeMethods.Create(affPath, dicPath)
            : NativeMethods.CreateKey(affPath, dicPath, key);

        if (_handle == IntPtr.Zero)
            throw new InvalidOperationException(
                $"Failed to create Hunspell instance. Verify that '{affPath}' and '{dicPath}' exist and are valid.");
    }

    public int AddDictionary(string dicPath)
    {
        ArgumentNullException.ThrowIfNull(dicPath);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return NativeMethods.AddDic(_handle, dicPath);
        }
    }

    public bool Spell(string word)
    {
        ArgumentNullException.ThrowIfNull(word);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return NativeMethods.Spell(_handle, word) != 0;
        }
    }

    public string GetDicEncoding()
    {
        lock (_syncLock)
        {
            ThrowIfDisposed();
            var ptr = NativeMethods.GetDicEncoding(_handle);
            return Marshal.PtrToStringUTF8(ptr) ?? string.Empty;
        }
    }

    public string[] Suggest(string word)
    {
        ArgumentNullException.ThrowIfNull(word);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return MarshalStringList((out IntPtr slst) => NativeMethods.Suggest(_handle, out slst, word));
        }
    }

    public string[] SuffixSuggest(string word)
    {
        ArgumentNullException.ThrowIfNull(word);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return MarshalStringList((out IntPtr slst) => NativeMethods.SuffixSuggest(_handle, out slst, word));
        }
    }

    public string[] Analyze(string word)
    {
        ArgumentNullException.ThrowIfNull(word);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return MarshalStringList((out IntPtr slst) => NativeMethods.Analyze(_handle, out slst, word));
        }
    }

    public string[] Stem(string word)
    {
        ArgumentNullException.ThrowIfNull(word);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return MarshalStringList((out IntPtr slst) => NativeMethods.Stem(_handle, out slst, word));
        }
    }

    public string[] Stem(string[] morphDescriptions)
    {
        ArgumentNullException.ThrowIfNull(morphDescriptions);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return MarshalWithStringArray(morphDescriptions, (descPtr, count) =>
            {
                int n = NativeMethods.Stem2(_handle, out var slst, descPtr, count);
                return (slst, n);
            });
        }
    }

    public string[] Generate(string word, string example)
    {
        ArgumentNullException.ThrowIfNull(word);
        ArgumentNullException.ThrowIfNull(example);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return MarshalStringList((out IntPtr slst) => NativeMethods.Generate(_handle, out slst, word, example));
        }
    }

    public string[] Generate(string word, string[] morphDescriptions)
    {
        ArgumentNullException.ThrowIfNull(word);
        ArgumentNullException.ThrowIfNull(morphDescriptions);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return MarshalWithStringArray(morphDescriptions, (descPtr, count) =>
            {
                int n = NativeMethods.Generate2(_handle, out var slst, word, descPtr, count);
                return (slst, n);
            });
        }
    }

    public int Add(string word)
    {
        ArgumentNullException.ThrowIfNull(word);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return NativeMethods.Add(_handle, word);
        }
    }

    public int AddWithFlags(string word, string flags, string desc = "")
    {
        ArgumentNullException.ThrowIfNull(word);
        ArgumentNullException.ThrowIfNull(flags);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return NativeMethods.AddWithFlags(_handle, word, flags, desc ?? "");
        }
    }

    public int AddWithAffix(string word, string example)
    {
        ArgumentNullException.ThrowIfNull(word);
        ArgumentNullException.ThrowIfNull(example);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return NativeMethods.AddWithAffix(_handle, word, example);
        }
    }

    public int Remove(string word)
    {
        ArgumentNullException.ThrowIfNull(word);
        lock (_syncLock)
        {
            ThrowIfDisposed();
            return NativeMethods.Remove(_handle, word);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        lock (_syncLock)
        {
            if (_disposed) return;
            _disposed = true;
            if (_handle != IntPtr.Zero)
            {
                NativeMethods.Destroy(_handle);
                _handle = IntPtr.Zero;
            }
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private delegate int StringListFunc(out IntPtr slst);

    private string[] MarshalStringList(StringListFunc func)
    {
        int count = func(out var slst);
        try
        {
            return ReadStringArray(slst, count);
        }
        finally
        {
            if (slst != IntPtr.Zero && count > 0)
                NativeMethods.FreeList(_handle, ref slst, count);
        }
    }

    private string[] MarshalWithStringArray(string[] input, Func<IntPtr, int, (IntPtr slst, int count)> func)
    {
        var utf8Ptrs = new IntPtr[input.Length];
        try
        {
            for (int i = 0; i < input.Length; i++)
                utf8Ptrs[i] = Marshal.StringToCoTaskMemUTF8(input[i]);

            var pinnedArray = GCHandle.Alloc(utf8Ptrs, GCHandleType.Pinned);
            try
            {
                var (slst, count) = func(pinnedArray.AddrOfPinnedObject(), input.Length);
                try
                {
                    return ReadStringArray(slst, count);
                }
                finally
                {
                    if (slst != IntPtr.Zero && count > 0)
                        NativeMethods.FreeList(_handle, ref slst, count);
                }
            }
            finally
            {
                pinnedArray.Free();
            }
        }
        finally
        {
            foreach (var ptr in utf8Ptrs)
                if (ptr != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(ptr);
        }
    }

    private static string[] ReadStringArray(IntPtr arrayPtr, int count)
    {
        if (count <= 0 || arrayPtr == IntPtr.Zero)
            return [];

        var results = new string[count];
        for (int i = 0; i < count; i++)
        {
            var strPtr = Marshal.ReadIntPtr(arrayPtr, i * IntPtr.Size);
            results[i] = Marshal.PtrToStringUTF8(strPtr) ?? string.Empty;
        }
        return results;
    }
}
