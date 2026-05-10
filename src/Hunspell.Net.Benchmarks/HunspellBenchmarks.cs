using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Json;

namespace Hunspell.Net.Benchmarks;

[MemoryDiagnoser]
[JsonExporterAttribute.Full]
public class HunspellBenchmarks
{
    private static readonly string TestDataDir = FindTestDataDir();

    private static string FindTestDataDir()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "references", "hunspell", "tests");
            if (Directory.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException(
            "Could not find 'references/hunspell/tests' in any parent of " + AppContext.BaseDirectory);
    }

    private static string AffPath => Path.Combine(TestDataDir, "base.aff");
    private static string DicPath => Path.Combine(TestDataDir, "base.dic");
    private static string MorphDicPath => Path.Combine(TestDataDir, "morph.dic");

    private HunspellDictionary _dict = null!;
    private string[] _morphDescriptions = [];

    [GlobalSetup]
    public void Setup()
    {
        _dict = new HunspellDictionary(AffPath, DicPath);
        _morphDescriptions = _dict.Analyze("created");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _dict.Dispose();
    }

    [Benchmark(Description = "Constructor")]
    public HunspellDictionary CreateDictionary()
    {
        var dict = new HunspellDictionary(AffPath, DicPath);
        dict.Dispose();
        return dict;
    }

    [Benchmark(Description = "Spell (correct word)")]
    public bool SpellCorrect() => _dict.Spell("created");

    [Benchmark(Description = "Spell (misspelled word)")]
    public bool SpellMisspelled() => _dict.Spell("creeated");

    [Benchmark(Description = "Suggest")]
    public string[] Suggest() => _dict.Suggest("helo");

    [Benchmark(Description = "SuffixSuggest")]
    public string[] SuffixSuggest() => _dict.SuffixSuggest("create");

    [Benchmark(Description = "Analyze")]
    public string[] Analyze() => _dict.Analyze("created");

    [Benchmark(Description = "Stem (word)")]
    public string[] StemWord() => _dict.Stem("created");

    [Benchmark(Description = "Stem (morph descriptions)")]
    public string[] StemMorph() => _dict.Stem(_morphDescriptions);

    [Benchmark(Description = "Generate (example)")]
    public string[] GenerateByExample() => _dict.Generate("create", "created");

    [Benchmark(Description = "Generate (morph descriptions)")]
    public string[] GenerateByMorph() => _dict.Generate("look", _morphDescriptions);

    [Benchmark(Description = "GetDicEncoding")]
    public string GetDicEncoding() => _dict.GetDicEncoding();

    [Benchmark(Description = "Add + Remove")]
    public void AddAndRemove()
    {
        _dict.Add("benchmarkword");
        _dict.Remove("benchmarkword");
    }

    [Benchmark(Description = "AddWithAffix + Remove")]
    public void AddWithAffixAndRemove()
    {
        _dict.AddWithAffix("benchaffix", "create");
        _dict.Remove("benchaffix");
    }

    [Benchmark(Description = "AddWithFlags + Remove")]
    public void AddWithFlagsAndRemove()
    {
        _dict.AddWithFlags("benchflags", "S", "");
        _dict.Remove("benchflags");
    }

    [Benchmark(Description = "AddDictionary")]
    public int AddDictionary()
    {
        return _dict.AddDictionary(MorphDicPath);
    }
}
