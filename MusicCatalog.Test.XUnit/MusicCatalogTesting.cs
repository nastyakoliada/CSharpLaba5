namespace MusicCatalog.Test.XUnit;
using MusicCatalog.Laba5;

/// <summary>
/// ����� ��� ������������ ������������ �������� � ���� 3. 
/// ��� ������������ ������������ �������� ����������. � ������� ������������ ��� ����������
/// ��� ��������������.
/// </summary>
public class MusicCatalogTesting
{
    MusicCatalog catalog = null!;
    /// <summary>
    /// ��������������� ����� ������� ���������� �������
    /// </summary>
    private MockSerializer CreateTestCatalog()
    {
        MockSerializer ts = new();
        catalog = new MusicCatalog(ts);
        return ts;
    }
    /// <summary>
    /// ������������ ���������� ��������
    /// </summary>
    [Fact]
    public async void AddTesting()
    {
        // ������� �������, �������� �������� ������������ ��� ��������
        var ts = CreateTestCatalog();
        // ��������� ����� ����������
        await catalog.AddComposition(new Composition { Author = "Billy Joel", SongName = "Piano man" });

        //���������, ��� � ������������� ������ ����� ������������ ���� ����������
        Assert.Collection<Composition>(ts.SerializedCompositions,
            c =>
            {
                Assert.Equal("Sia", c.Author);
                Assert.Equal("Forever", c.SongName);
            },
            c => {
                Assert.Equal("Billie Eilish", c.Author);
                Assert.Equal("Lovely", c.SongName);
            },
            c => {
                Assert.Equal("Billy Joel", c.Author);
                Assert.Equal("Piano man", c.SongName);
            }
            );
        // ����������, ��� �� ��� 3
        Assert.Equal(3,ts.SerializedCompositions.Count());
    }
    // ������������ �� ��, ��� �������� ������� �� ��������� ���� ����������
    [Fact]
    public async void EnumerateTesting()
    {
        CreateTestCatalog();
        Assert.Collection<Composition>(await catalog.EnumerateAllCompositions(),
            c =>
            {
                Assert.Equal("Billie Eilish", c.Author);
                Assert.Equal("Lovely", c.SongName);
            },
            c =>
            {
                Assert.Equal("Sia", c.Author);
                Assert.Equal("Forever", c.SongName);
            }
            );
    }
    /// <summary>
    /// ������������ �������� ���������� �� ��������
    /// </summary>
    [Fact]
    public async void RemoveTesting()
    {
        // ������� �������, � �������������� �������� �������������
        var ts = CreateTestCatalog();
        // ���������, ��� ����� ��������, ������������ ������� ��� ���������� ������ ���� ����������
        await catalog.Remove("Sia");
        Assert.Collection<Composition>(ts.SerializedCompositions,
            c =>
            {
                Assert.Equal("Billie Eilish", c.Author);
                Assert.Equal("Lovely", c.SongName);
            }
            );
        Assert.Single(ts.SerializedCompositions);
    }
    /// <summary>
    /// ������������ ������ � ��������
    /// </summary>
    [Fact]
    public async void SearchTesting()
    {
        CreateTestCatalog();

        var sch = await catalog.Search("Bil");

        Assert.Collection<Composition>(sch,
            c =>
            {
                Assert.Equal("Billie Eilish", c.Author);
                Assert.Equal("Lovely", c.SongName);
            }
            );
        Assert.Single(sch);
    }
}