using StackExchange.Redis;

namespace DevelopmentTool.Helper;

public static class RedisManager
{
    private static readonly ConnectionMultiplexer _connectionMultiplexer;

    static RedisManager()
    {
        _connectionMultiplexer =
            ConnectionMultiplexer.Connect(
                "10.100.20.72:1379,password=1qaz#EDC4rfv^YHN, abortConnect=false, defaultDatabase=13");
    }

    public static IDatabase Instance => _connectionMultiplexer.GetDatabase();
}