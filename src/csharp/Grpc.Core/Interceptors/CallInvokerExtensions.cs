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
using Grpc.Core.Utils;

namespace Grpc.Core.Interceptors
{
    /// <summary>
    /// Extends the CallInvoker class to provide the interceptor facility on the client side.
    /// </summary>
    public static class CallInvokerExtensions
    {
        /// <summary>
        /// Decorates an underlying CallInvoker to support intercept calls through a given interceptor.
        /// </summary>
        private class InterceptingCallInvoker : CallInvoker
        {
            readonly CallInvoker invoker;
            readonly ClientInterceptor interceptor;

            /// <summary>
            /// Creates a new instance of InterceptingCallInvoker with the given underlying CallInvoker and interceptor.
            /// </summary>
            public InterceptingCallInvoker(CallInvoker invoker, ClientInterceptor interceptor)
            {
                this.invoker = GrpcPreconditions.CheckNotNull(invoker, "invoker");
                this.interceptor = GrpcPreconditions.CheckNotNull(interceptor, "interceptor");
            }

            /// <summary>
            /// Intercepts a simple blocking call with the registered interceptor.
            /// </summary>
            public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
            {
                return interceptor.BlockingUnaryCall(method, host, options, request, invoker.BlockingUnaryCall);
            }

            /// <summary>
            /// Intercepts a simple asynchronous call with the registered interceptor.
            /// </summary>
            public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
            {
                return interceptor.AsyncUnaryCall(method, host, options, request, invoker.AsyncUnaryCall);
            }

            /// <summary>
            /// Intercepts an asynchronous server streaming call with the registered interceptor.
            /// </summary>
            public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
            {
                return interceptor.AsyncServerStreamingCall(method, host, options, request, invoker.AsyncServerStreamingCall);
            }

            /// <summary>
            /// Intercepts an asynchronous client streaming call with the registered interceptor.
            /// </summary>
            public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
            {
                return interceptor.AsyncClientStreamingCall(method, host, options, invoker.AsyncClientStreamingCall);
            }

            /// <summary>
            /// Intercepts an asynchronous duplex streaming call with the registered interceptor.
            /// </summary>
            public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
            {
                return interceptor.AsyncDuplexStreamingCall(method, host, options, invoker.AsyncDuplexStreamingCall);
            }
        }

        /// <summary>
        /// Returns a <c>CallInvoker</c> object that intercepts calls to the specified invoker via the given interceptor.
        /// </summary>
        public static CallInvoker Intercept(this CallInvoker invoker, ClientInterceptor interceptor)
        {
            return new InterceptingCallInvoker(invoker, interceptor);
        }
    }
}
