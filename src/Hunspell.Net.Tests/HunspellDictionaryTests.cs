using Xunit;

namespace Hunspell.Net.Tests;

public class HunspellDictionaryTests
{
    private static readonly string TestDataDir = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..",
            "references", "hunspell", "tests"));

    private static string AffPath => Path.Combine(TestDataDir, "base.aff");
    private static string DicPath => Path.Combine(TestDataDir, "base.dic");

    [Fact]
    public void Constructor_NullAffPath_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new HunspellDictionary(null!, "test.dic"));
    }

    [Fact]
    public void Constructor_NullDicPath_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new HunspellDictionary("test.aff", null!));
    }

    [Fact]
    public void Constructor_InvalidPaths_CreatesEmptyDictionary()
    {
        using var dict = new HunspellDictionary("/nonexistent/path.aff", "/nonexistent/path.dic");
        Assert.False(dict.Spell("hello"));
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        using var dict = new HunspellDictionary(AffPath, DicPath);
        dict.Dispose();
        dict.Dispose();
    }

    [Fact]
    public void Spell_AfterDispose_ThrowsObjectDisposedException()
    {
        var dict = new HunspellDictionary(AffPath, DicPath);
        dict.Dispose();
        Assert.Throws<ObjectDisposedException>(() => dict.Spell("hello"));
    }

    [Fact]
    public void Suggest_AfterDispose_ThrowsObjectDisposedException()
    {
        var dict = new HunspellDictionary(AffPath, DicPath);
        dict.Dispose();
        Assert.Throws<ObjectDisposedException>(() => dict.Suggest("hello"));
    }

    [Fact]
    public void Analyze_AfterDispose_ThrowsObjectDisposedException()
    {
        var dict = new HunspellDictionary(AffPath, DicPath);
        dict.Dispose();
        Assert.Throws<ObjectDisposedException>(() => dict.Analyze("hello"));
    }

    [Fact]
    public void Stem_AfterDispose_ThrowsObjectDisposedException()
    {
        var dict = new HunspellDictionary(AffPath, DicPath);
        dict.Dispose();
        Assert.Throws<ObjectDisposedException>(() => dict.Stem("hello"));
    }

    [Fact]
    public void Add_AfterDispose_ThrowsObjectDisposedException()
    {
        var dict = new HunspellDictionary(AffPath, DicPath);
        dict.Dispose();
        Assert.Throws<ObjectDisposedException>(() => dict.Add("newword"));
    }

    [Fact]
    public void Remove_AfterDispose_ThrowsObjectDisposedException()
    {
        var dict = new HunspellDictionary(AffPath, DicPath);
        dict.Dispose();
        Assert.Throws<ObjectDisposedException>(() => dict.Remove("hello"));
    }

    [Fact]
    public void Spell_NullWord_Throws()
    {
        using var dict = new HunspellDictionary(AffPath, DicPath);
        Assert.Throws<ArgumentNullException>(() => dict.Spell(null!));
    }

    [Fact]
    public void Suggest_NullWord_Throws()
    {
        using var dict = new HunspellDictionary(AffPath, DicPath);
        Assert.Throws<ArgumentNullException>(() => dict.Suggest(null!));
    }

    [Fact]
    public void Add_NullWord_Throws()
    {
        using var dict = new HunspellDictionary(AffPath, DicPath);
        Assert.Throws<ArgumentNullException>(() => dict.Add(null!));
    }

    [Fact]
    public void Remove_NullWord_Throws()
    {
        using var dict = new HunspellDictionary(AffPath, DicPath);
        Assert.Throws<ArgumentNullException>(() => dict.Remove(null!));
    }

    [Fact]
    public async Task ConcurrentSpell_DoesNotCorruptOrDeadlock()
    {
        using var dict = new HunspellDictionary(AffPath, DicPath);
        var words = new[] { "hello", "world", "create", "look", "text", "said", "seven" };
        var exceptions = new List<Exception>();

        var tasks = Enumerable.Range(0, 20).Select(_ => Task.Run(() =>
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    foreach (var word in words)
                        dict.Spell(word);
                }
            }
            catch (Exception ex)
            {
                lock (exceptions)
                    exceptions.Add(ex);
            }
        })).ToArray();

        await Task.WhenAll(tasks);
        Assert.Empty(exceptions);
    }

    [Fact]
    public async Task ConcurrentMixedOperations_DoesNotCorruptOrDeadlock()
    {
        using var dict = new HunspellDictionary(AffPath, DicPath);
        var exceptions = new List<Exception>();

        var tasks = Enumerable.Range(0, 10).Select(i => Task.Run(() =>
        {
            try
            {
                for (int j = 0; j < 50; j++)
                {
                    dict.Spell("hello");
                    dict.Suggest("helo");
                    dict.Analyze("created");
                    dict.Stem("created");
                }
            }
            catch (Exception ex)
            {
                lock (exceptions)
                    exceptions.Add(ex);
            }
        })).ToArray();

        await Task.WhenAll(tasks);
        Assert.Empty(exceptions);
    }
}
