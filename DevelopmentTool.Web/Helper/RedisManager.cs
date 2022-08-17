using StackExchange.Redis;

namespace DevelopmentTool.Web.Helper;

/// <summary>
/// redis连接类
/// </summary>
public static class RedisManager
{
    /// <summary>
    /// redis连接
    /// </summary>
    private static readonly ConnectionMultiplexer _connectionMultiplexer;

    static RedisManager()
    {
        _connectionMultiplexer =
            ConnectionMultiplexer.Connect(
                "10.100.20.72:1379,password=1qaz#EDC4rfv^YHN, abortConnect=false, defaultDatabase=13");
    }

    /// <summary>
    /// redis数据库
    /// </summary>
    public static IDatabase Instance => _connectionMultiplexer.GetDatabase();
}