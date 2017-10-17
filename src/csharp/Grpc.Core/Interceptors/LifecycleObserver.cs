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

        interface observer
        {
            Task OnBeginCall(ServerCallContext context);
            Task OnEndCall(ServerCallContext context);
        }

        class ob : observer {
            public Task OnBeginCall(ServerCallContext context) {
                return Task.Run(() => Console.WriteLine("ob.OnBeginCall()"));
            }
            public Task OnEndCall(ServerCallContext context) {
                return Task.Run(() => Console.WriteLine("ob.OnEndCall()"));            }
        }

        Task<observer> InterceptServerCall(ServerCallContext context)
        {
            return Task.FromResult((observer)new ob());
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

                return handler;
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

        static IAsyncStreamReader<T> InterceptStreamReader<T>(IAsyncStreamReader<T> reader, Func<T,T> hook)
        {
            return reader;
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
            return async (context, handler) => {
                var interceptor = await InterceptServerCall(context).ConfigureAwait(false);
                if (interceptor == null)
                {
                    return handler;
                }

                // interceptor.OnBeginCall
                return new DuplexStreamingServerMethod<TRequest, TResponse>(async (originalRequestStream, originalResponseStream, ctx) => { 
                    await interceptor.OnBeginCall(ctx);
                    IAsyncStreamReader<TRequest> newRequestStream = originalRequestStream;
                    IServerStreamWriter<TResponse> newResponseStream = originalResponseStream;
                    await handler(newRequestStream, newResponseStream, ctx);
                    await interceptor.OnEndCall(ctx);
                });
            };
        }
    }
}
