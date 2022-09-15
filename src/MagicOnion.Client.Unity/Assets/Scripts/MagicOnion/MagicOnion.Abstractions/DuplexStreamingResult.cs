using Grpc.Core;
using MessagePack;
using System;
using System.Threading.Tasks;

namespace MagicOnion
{
    /// <summary>
    /// Wrapped AsyncDuplexStreamingCall.
    /// </summary>
    public struct DuplexStreamingResult<TRequest, TResponse> : IDisposable
    {
        readonly AsyncDuplexStreamingCall<byte[], byte[]> inner;
        readonly IClientStreamWriter<TRequest> requestStream;
        readonly IAsyncStreamReader<TResponse> responseStream;

        public DuplexStreamingResult(AsyncDuplexStreamingCall<byte[], byte[]> inner, IClientStreamWriter<TRequest> requestStream, IAsyncStreamReader<TResponse> responseStream, MessagePackSerializerOptions serializerOptions)
        {
            this.inner = inner;
            this.requestStream = requestStream;
            this.responseStream = responseStream;
        }

        public AsyncDuplexStreamingCall<byte[], byte[]> RawStreamingCall => inner;

        /// <summary>
        /// Async stream to read streaming responses.
        /// </summary>
        public IAsyncStreamReader<TResponse> ResponseStream
        {
            get
            {
                return responseStream;
            }
        }

        /// <summary>
        /// Async stream to send streaming requests.
        /// </summary>
        public IClientStreamWriter<TRequest> RequestStream
        {
            get
            {
                return requestStream;
            }
        }

        /// <summary>
        /// Asynchronous access to response headers.
        /// </summary>
        public Task<Metadata> ResponseHeadersAsync
        {
            get
            {
                return this.inner.ResponseHeadersAsync;
            }
        }

        /// <summary>
        /// Gets the call status if the call has already finished.
        /// Throws InvalidOperationException otherwise.
        /// </summary>
        public Status GetStatus()
        {
            return this.inner.GetStatus();
        }

        /// <summary>
        /// Gets the call trailing metadata if the call has already finished.
        /// Throws InvalidOperationException otherwise.
        /// </summary>
        public Metadata GetTrailers()
        {
            return this.inner.GetTrailers();
        }

        /// <summary>
        /// 提供调用后清理的方法
        /// 如果调用已经正常结束（请求流已经完成并且响应流已经被完全读取），不做任何事情.
        /// 否则，请求取消调用，这应该终止与调用关联的所有挂起的异步操作.
        /// 结果，调用使用的所有资源最终都应该被释放.
        /// </summary>
        /// <remarks>
        /// 通常，除非您想利用调用 <c>Dispose</c> 的“取消”语义，否则您无需释放调用
        /// </remarks>
        public void Dispose()
        {
            if (this.inner != null)
            {
                this.inner.Dispose();
            }
        }
    }
}