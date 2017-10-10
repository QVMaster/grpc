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
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core.Internal;

namespace Grpc.Core.Interceptors
{

    class LifecycleObserver
    {
    }

    interface ILifecycleObserver
    {
        void BeginCall();
        void EndCall();
        void OnRequest();
        void OnResponse();
    }

    /// <summary>
    /// Implements an interceptor that intercepts invocations and raises
    /// events as they happen.
    /// This is an EXPERIMENTAL API.
    /// </summary>
    class InvocationLifecycleObserverInterceptor<TLifecycleObserver> : Interceptor
        where TLifecycleObserver : ILifecycleObserver, new()
    {
        /// <summary>
        /// Intercepts a blocking invocation of a simple remote call.
        /// </summary>
        /// <param name="request">The request message of the invocation.</param>
        /// <param name="context">
        /// The <see cref="Grpc.Core.Interceptors.ClientInterceptorContext{TRequest, TResponse}"/>
        /// associated with the current invocation.
        /// </param>
        /// <param name="continuation">
        /// The callback that continues the invocation process.
        /// This can be invoked zero or more times by the interceptor.
        /// </param>
        /// <returns>The response messaage of the current invocation.</returns>
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            BeginCall();
            var returnValue = continuation(request, context);
            EndCall();
            return returnValue;
        }

        /// <summary>
        /// Intercepts an asynchronous invocation of a simple remote call.
        /// </summary>
        /// <param name="request">The request message of the invocation.</param>
        /// <param name="context">
        /// The <see cref="Grpc.Core.Interceptors.ClientInterceptorContext{TRequest, TResponse}"/>
        /// associated with the current invocation.
        /// </param>
        /// <param name="continuation">
        /// The callback that continues the invocation process.
        /// This can be invoked zero or more times by the interceptor.
        /// </param>
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            BeginCall();
            var returnValue = continuation(request, context);
            EndCall();
            return returnValue;
        }

        /// <summary>
        /// Intercepts an asynchronous invocation of a streaming remote call.
        /// </summary>
        /// <param name="request">The request message of the invocation.</param>
        /// <param name="context">
        /// The <see cref="Grpc.Core.Interceptors.ClientInterceptorContext{TRequest, TResponse}"/>
        /// associated with the current invocation.
        /// </param>
        /// <param name="continuation">
        /// The callback that continues the invocation process.
        /// This can be invoked zero or more times by the interceptor.
        /// </param>
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            BeginCall();
            var returnValue = continuation(request, context);
            EndCall();
            return returnValue;
        }

        /// <summary>
        /// Intercepts an asynchronous invocation of a client streaming call.
        /// </summary>
        /// <param name="context">
        /// The <see cref="Grpc.Core.Interceptors.ClientInterceptorContext{TRequest, TResponse}"/>
        /// associated with the current invocation.
        /// </param>
        /// <param name="continuation">
        /// The callback that continues the invocation process.
        /// This can be invoked zero or more times by the interceptor.
        /// </param>
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            BeginCall();
            var returnValue = continuation(context);
            EndCall();
            return returnValue;
        }

        /// <summary>
        /// Intercepts an asynchronous invocation of a duplex streaming call.
        /// </summary>
        /// <param name="context">
        /// The <see cref="Grpc.Core.Interceptors.ClientInterceptorContext{TRequest, TResponse}"/>
        /// associated with the current invocation.
        /// </param>
        /// <param name="continuation">
        /// The callback that continues the invocation process.
        /// This can be invoked zero or more times by the interceptor.
        /// </param>
        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            BeginCall();
            var returnValue = continuation(context);
            EndCall();
            return returnValue;
        }

        protected virtual void BeginCall(){}
        protected virtual void EndCall(){}
    }
}
