using NewLife.Log;
using XCode;
using XCode.DataAccessLayer;

namespace GitCandyXUnitTest;

/// <summary>GitCandy测试集合定义</summary>
[CollectionDefinition("GitCandyTests")]
public class GitCandyTestCollection : ICollectionFixture<DatabaseFixture> { }

/// <summary>数据库测试夹具。所有测试类共享同一个SQLite数据库</summary>
public class DatabaseFixture : IDisposable
{
    /// <summary>数据库文件路径</summary>
    public static String TempDbPath { get; private set; } = "";

    /// <summary>是否已初始化</summary>
    private static Boolean _initialized;

    /// <summary>静态锁</summary>
    private static readonly Object _lock = new();

    public DatabaseFixture()
    {
        if (_initialized) return;
        lock (_lock)
        {
            if (_initialized) return;
            _initialized = true;

            XTrace.UseConsole();

            // 只创建一次数据库，所有测试共享
            TempDbPath = Path.Combine(Path.GetTempPath(), $"GitCandyTest_Global.db");
            if (File.Exists(TempDbPath)) File.Delete(TempDbPath);

            var connStr = $"Data Source={TempDbPath};";
            DAL.AddConnStr("GitCandy", connStr, null, "SQLite");
            EntityFactory.InitConnection("GitCandy");
        }
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(TempDbPath)) File.Delete(TempDbPath);
        }
        catch { }
    }
}
