using Grpc.Core;
using System;
using System.Threading;

namespace MagicOnion
{
    // 用于 MagicOnionEngine 程序集扫描以提高分析速度。
    public interface IServiceMarker
    {

    }

    public interface IService<TSelf> : IServiceMarker
    {
        TSelf WithOptions(CallOptions option);
        TSelf WithHeaders(Metadata headers);
        TSelf WithDeadline(DateTime deadline);
        TSelf WithCancellationToken(CancellationToken cancellationToken);
        TSelf WithHost(string host);
    }
}
