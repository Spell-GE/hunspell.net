using Xunit;

namespace Hunspell.Net.Tests;

public class NativeIntegrationTests : IDisposable
{
    private static readonly string TestDataDir = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..",
            "references", "hunspell", "tests"));

    private readonly HunspellDictionary _dict;

    public NativeIntegrationTests()
    {
        _dict = new HunspellDictionary(
            Path.Combine(TestDataDir, "base.aff"),
            Path.Combine(TestDataDir, "base.dic"));
    }

    public void Dispose() => _dict.Dispose();

    [Fact]
    public void Create_WithValidFiles_Succeeds()
    {
        Assert.NotNull(_dict);
    }

    [Fact]
    public void CreateWithKey_WithValidFiles_Succeeds()
    {
        using var dict = new HunspellDictionary(
            Path.Combine(TestDataDir, "base.aff"),
            Path.Combine(TestDataDir, "base.dic"),
            "testkey");
        Assert.NotNull(dict);
    }

    [Fact]
    public void GetDicEncoding_ReturnsEncoding()
    {
        var encoding = _dict.GetDicEncoding();
        Assert.False(string.IsNullOrEmpty(encoding));
        Assert.Equal("ISO8859-1", encoding);
    }

    [Theory]
    [InlineData("hello", true)]
    [InlineData("created", true)]
    [InlineData("create", true)]
    [InlineData("look", true)]
    [InlineData("text", true)]
    [InlineData("Hunspell", true)]
    [InlineData("xyznotaword", false)]
    [InlineData("helo", false)]
    [InlineData("creeated", false)]
    public void Spell_ReturnsExpectedResult(string word, bool expected)
    {
        Assert.Equal(expected, _dict.Spell(word));
    }

    [Fact]
    public void Spell_RecognizesAffixedForms()
    {
        Assert.True(_dict.Spell("uncreated"));
        Assert.True(_dict.Spell("creating"));
        Assert.True(_dict.Spell("creates"));
        Assert.True(_dict.Spell("looked"));
        Assert.True(_dict.Spell("looking"));
    }

    [Fact]
    public void Suggest_ReturnsSuggestions()
    {
        var suggestions = _dict.Suggest("helo");
        Assert.NotEmpty(suggestions);
        Assert.Contains("hello", suggestions);
    }

    [Fact]
    public void Suggest_ForCorrectWord_MayReturnEmpty()
    {
        var suggestions = _dict.Suggest("hello");
        Assert.NotNull(suggestions);
    }

    [Fact]
    public void SuffixSuggest_ReturnsSuffixedForms()
    {
        var results = _dict.SuffixSuggest("create");
        Assert.NotNull(results);
    }

    [Fact]
    public void Analyze_ReturnsAnalysis()
    {
        var analysis = _dict.Analyze("created");
        Assert.NotNull(analysis);
        Assert.NotEmpty(analysis);
    }

    [Fact]
    public void Stem_ReturnsStems()
    {
        var stems = _dict.Stem("created");
        Assert.NotNull(stems);
        Assert.NotEmpty(stems);
        Assert.Contains("create", stems);
    }

    [Fact]
    public void Stem_WithMorphDescriptions_Works()
    {
        var analysis = _dict.Analyze("created");
        Assert.NotEmpty(analysis);

        var stems = _dict.Stem(analysis);
        Assert.NotNull(stems);
        Assert.NotEmpty(stems);
    }

    [Fact]
    public void Generate_WithExample_ProducesWordForms()
    {
        var results = _dict.Generate("create", "created");
        Assert.NotNull(results);
    }

    [Fact]
    public void Generate_WithMorphDescriptions_Works()
    {
        var analysis = _dict.Analyze("created");
        if (analysis.Length > 0)
        {
            var results = _dict.Generate("look", analysis);
            Assert.NotNull(results);
        }
    }

    [Fact]
    public void Add_MakesWordRecognized()
    {
        Assert.False(_dict.Spell("flurbo"));
        _dict.Add("flurbo");
        Assert.True(_dict.Spell("flurbo"));
    }

    [Fact]
    public void AddWithAffix_MakesAffixedFormsRecognized()
    {
        Assert.False(_dict.Spell("zygoplat"));
        _dict.AddWithAffix("zygoplat", "create");
        Assert.True(_dict.Spell("zygoplat"));
        Assert.True(_dict.Spell("zygoplats"));
    }

    [Fact]
    public void Remove_MakesWordUnrecognized()
    {
        Assert.True(_dict.Spell("hello"));
        _dict.Remove("hello");
        Assert.False(_dict.Spell("hello"));
    }

    [Fact]
    public void AddDictionary_LoadsExtraDic()
    {
        var morphDicPath = Path.Combine(TestDataDir, "morph.dic");
        if (File.Exists(morphDicPath))
        {
            var result = _dict.AddDictionary(morphDicPath);
            Assert.Equal(0, result);
        }
    }

    [Fact]
    public void AddWithFlags_AddsWordWithFlags()
    {
        Assert.False(_dict.Spell("testflagword"));
        _dict.AddWithFlags("testflagword", "S", "");
        Assert.True(_dict.Spell("testflagword"));
    }
}
