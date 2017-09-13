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
    /// <summary>
    /// Serves as the base class for gRPC client interceptors.
    /// This is an EXPERIMENTAL API.
    /// </summary>
    public abstract class ClientInterceptor
    {
        /// <summary>
        /// Intercepts a blocking invocation of a simple remote call.
        /// </summary>
        public virtual TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request, Func<Method<TRequest, TResponse>, string, CallOptions, TRequest, TResponse> next)
            where TRequest : class
            where TResponse : class
        {
            return next(method, host, options, request);
        }

        /// <summary>
        /// Intercepts an asynchronous invocation of a simple remote call.
        /// </summary>
        public virtual AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request, Func<Method<TRequest, TResponse>, string, CallOptions, TRequest, AsyncUnaryCall<TResponse>> next)
            where TRequest : class
            where TResponse : class
        {
            return next(method, host, options, request);
        }

        /// <summary>
        /// Intercepts an asynchronous invocation of a streaming remote call.
        /// </summary>
        public virtual AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request, Func<Method<TRequest, TResponse>, string, CallOptions, TRequest, AsyncServerStreamingCall<TResponse>> next)
            where TRequest : class
            where TResponse : class
        {
            return next(method, host, options, request);
        }

        /// <summary>
        /// Intercepts an asynchronous invocation of a client streaming call.
        /// </summary>
        public virtual AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, Func<Method<TRequest, TResponse>, string, CallOptions, AsyncClientStreamingCall<TRequest, TResponse>> next)
            where TRequest : class
            where TResponse : class
        {
            return next(method, host, options);
        }

        /// <summary>
        /// Intercepts an asynchronous invocation of a duplex streaming call.
        /// </summary>
        public virtual AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, Func<Method<TRequest, TResponse>, string, CallOptions, AsyncDuplexStreamingCall<TRequest, TResponse>> next)
            where TRequest : class
            where TResponse : class
        {
            return next(method, host, options);
        }
    }
}