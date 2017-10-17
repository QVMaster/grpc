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

namespace Grpc.Core.Interceptors
{
    /// <summary>
    /// Represents an interceptor that observes the lifecycle of an RPC,
    /// and raises events to observe and/or modify headers, requests,
    /// and responses, as appropriate.
    /// This is an EXPERIMENTAL API.
    /// </summary>
    public class LifecycleObserverInterceptor : Interceptor
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
            return continuation(request, context);
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
            return continuation(request, context);
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
            return continuation(request, context);
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
            return continuation(context);
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
            return continuation(context);
        }

        /// <summary>
        /// Server-side handler for intercepting unary calls.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this method.</typeparam>
        /// <typeparam name="TResponse">Response message type for this method.</typeparam>
        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            return continuation(request, context);
        }

        /// <summary>
        /// Server-side handler for intercepting client streaming call.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this method.</typeparam>
        /// <typeparam name="TResponse">Response message type for this method.</typeparam>
        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            ServerCallContext context,
            ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            return continuation(requestStream, context);
        }

        /// <summary>
        /// Server-side handler for intercepting server streaming calls.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this method.</typeparam>
        /// <typeparam name="TResponse">Response message type for this method.</typeparam>
        public override Task ServerStreamingServerHandler<TRequest, TResponse>(
            TRequest request,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            return continuation(request, responseStream, context);
        }

        /// <summary>
        /// Server-side handler for intercepting bidi streaming calls.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this method.</typeparam>
        /// <typeparam name="TResponse">Response message type for this method.</typeparam>
        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            return continuation(requestStream, responseStream, context);
        }

        Task<dynamic> InterceptServerCall(ServerCallContext context)
        {
            return null;
        }

        /// <summary>
        /// Returns a <see cref="Grpc.Core.Interceptors.ServerHandlerInterceptor{THandler}" />
        /// function that when invoked with a <see cref="Grpc.Core.ServerCallContext" /> instance and
        /// and a <see cref="Grpc.Core.UnaryServerMethod{TRequest, TResponse}" /> handler,
        /// can return a new handler that intercepts the unary calls to the given handler
        /// and passes control to it when desired.
        /// </summary>
        public override ServerHandlerInterceptor<UnaryServerMethod<TRequest, TResponse>> GetUnaryServerHandlerInterceptor<TRequest, TResponse>()
        {
            return async (context, handler) => {
                var interceptor = await InterceptServerCall(context).ConfigureAwait(false);
                if (interceptor == null)
                {
                    return handler;
                }
                if (interceptor.OnBeginCall != null)
                {
                    context = await interceptor.OnBeginCall(context);
                }
                return (request, context) => 
            };
        }

        /// <summary>
        /// Returns a <see cref="Grpc.Core.Interceptors.ServerHandlerInterceptor{THandler}" />
        /// function that when invoked with a <see cref="Grpc.Core.ServerCallContext" /> instance and
        /// and a <see cref="Grpc.Core.ServerStreamingServerMethod{TRequest, TResponse}" /> handler,
        /// can return a new handler that intercepts the server-streaming calls to the given handler
        /// and passes control to it when desired.
        /// </summary>
        public override ServerHandlerInterceptor<ServerStreamingServerMethod<TRequest, TResponse>> GetServerStreamingServerHandlerInterceptor<TRequest, TResponse>()
        {
            return (context, handler) => Task.FromResult(handler);
        }

        /// <summary>
        /// Returns a <see cref="Grpc.Core.Interceptors.ServerHandlerInterceptor{THandler}" />
        /// function that when invoked with a <see cref="Grpc.Core.ServerCallContext" /> instance and
        /// and a <see cref="Grpc.Core.ClientStreamingServerMethod{TRequest, TResponse}" /> handler,
        /// can return a new handler that intercepts the client-streaming calls to the given handler
        /// and passes control to it when desired.
        /// </summary>
        public override ServerHandlerInterceptor<ClientStreamingServerMethod<TRequest, TResponse>> GetClientStreamingServerHandlerInterceptor<TRequest, TResponse>()
        {
            return (context, handler) => Task.FromResult(handler);
        }


        /// <summary>
        /// Returns a <see cref="Grpc.Core.Interceptors.ServerHandlerInterceptor{THandler}" />
        /// function that when invoked with a <see cref="Grpc.Core.ServerCallContext" /> instance and
        /// and a <see cref="Grpc.Core.DuplexStreamingServerMethod{TRequest, TResponse}" /> handler,
        /// can return a new handler that intercepts the duplex-streaming calls to the given handler
        /// and passes control to it when desired.
        /// </summary>
        public override ServerHandlerInterceptor<DuplexStreamingServerMethod<TRequest, TResponse>> GetDuplexStreamingServerHandlerInterceptor<TRequest, TResponse>()
        {
            return (context, handler) => Task.FromResult(handler);
        }
    }
}
