#region Copyright notice and license

// Copyright 2017 gRPC authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;

namespace Grpc.Core.Interceptors
{

    public class ClientInterceptorContext<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        public ClientInterceptorContext(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            Method = method;
            Host = host;
            Options = options;
        }
        public Method<TRequest, TResponse> Method { get; }
        public string Host { get; }
        public CallOptions Options { get; }
    }


    /// <summary>
    /// Serves as the base class for gRPC client interceptors.
    /// This is an EXPERIMENTAL API.
    /// </summary>
    public abstract class ClientInterceptor
    {
        public delegate TResponse BlockingUnaryCallContinuation<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, TRequest request)
            where TRequest : class
            where TResponse : class;
        public delegate AsyncUnaryCall<TResponse> AsyncUnaryCallContinuation<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, TRequest request)
            where TRequest : class
            where TResponse : class;
        public delegate AsyncServerStreamingCall<TResponse> AsyncServerStreamingCallContinuation<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, TRequest request)
            where TRequest : class
            where TResponse : class;
        public delegate AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCallContinuation<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class;
        public delegate AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCallContinuation<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class;

        /// <summary>
        /// Intercepts a blocking invocation of a simple remote call.
        /// </summary>
        public virtual TResponse BlockingUnaryCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, TRequest request, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            return continuation(context, request);
        }

        /// <summary>
        /// Intercepts an asynchronous invocation of a simple remote call.
        /// </summary>
        public virtual AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, TRequest request, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            return continuation(context, request);
        }

        /// <summary>
        /// Intercepts an asynchronous invocation of a streaming remote call.
        /// </summary>
        public virtual AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, TRequest request, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            return continuation(context, request);
        }

        /// <summary>
        /// Intercepts an asynchronous invocation of a client streaming call.
        /// </summary>
        public virtual AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            return continuation(context);
        }

        /// <summary>
        /// Intercepts an asynchronous invocation of a duplex streaming call.
        /// </summary>
        public virtual AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            return continuation(context);
        }
    }
}
