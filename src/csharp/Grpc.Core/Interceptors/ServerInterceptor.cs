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
    /// <summary>
    /// Serves as the base class for gRPC server interceptors.
    /// This is an EXPERIMENTAL API.
    /// </summary>
    public abstract class ServerInterceptor
    {
        /// <summary>
        /// Server-side handler for intercepting unary calls.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this method.</typeparam>
        /// <typeparam name="TResponse">Response message type for this method.</typeparam>
        public virtual Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            return continuation(request, context);
        }

        /// <summary>
        /// Server-side handler for intercepting client streaming call.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this method.</typeparam>
        /// <typeparam name="TResponse">Response message type for this method.</typeparam>
        public virtual Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            ServerCallContext context,
            ClientStreamingServerMethod<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            return continuation(requestStream, context);
        }

        /// <summary>
        /// Server-side handler for intercepting server streaming calls.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this method.</typeparam>
        /// <typeparam name="TResponse">Response message type for this method.</typeparam>
        public virtual Task ServerStreamingServerHandler<TRequest, TResponse>(
            TRequest request,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            ServerStreamingServerMethod<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            return continuation(request, responseStream, context);
        }

        /// <summary>
        /// Server-side handler for intercepting bidi streaming calls.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this method.</typeparam>
        /// <typeparam name="TResponse">Response message type for this method.</typeparam>
        public virtual Task DuplexStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            return continuation(requestStream, responseStream, context);
        }

        private static class WrapUtil<TRequest, TResponse>
            where TRequest : class
            where TResponse : class
        {
            public static UnaryServerMethod<TRequest, TResponse> Unary(
                UnaryServerMethod<TRequest, TResponse> handler,
                ServerInterceptor interceptor)
            {
                return (request, context) =>
                    interceptor.UnaryServerHandler<TRequest, TResponse>(request, context, handler);
            }

            public static ClientStreamingServerMethod<TRequest, TResponse> ClientStreaming(
                ClientStreamingServerMethod<TRequest, TResponse> handler,
                ServerInterceptor interceptor)
            {
                return (request, context) =>
                    interceptor.ClientStreamingServerHandler<TRequest, TResponse>(request, context, handler);
            }

            public static ServerStreamingServerMethod<TRequest, TResponse> ServerStreaming(
                ServerStreamingServerMethod<TRequest, TResponse> handler,
                ServerInterceptor interceptor)
            {
                return (request, response, context) =>
                    interceptor.ServerStreamingServerHandler<TRequest, TResponse>(request, response, context, handler);
            }

            public static DuplexStreamingServerMethod<TRequest, TResponse> DuplexStreaming(
                DuplexStreamingServerMethod<TRequest, TResponse> handler,
                ServerInterceptor interceptor)
            {
                return (request, response, context) =>
                    interceptor.DuplexStreamingServerHandler<TRequest, TResponse>(request, response, context, handler);
            }
        }

        private Delegate WrapDelegate(Delegate d)
        {
            if (d == null)
            {
                return d;
            }

            var dType = d.GetType().GetTypeInfo();
            if (!dType.IsGenericType)
            {
                return d;
            }

            var genericType = dType.GetGenericTypeDefinition();
            if (genericType == typeof(UnaryServerMethod<,>))
            {
                return (Delegate)typeof(WrapUtil<,>).GetTypeInfo()
                    .MakeGenericType(dType.GetGenericArguments()).GetTypeInfo()
                    .GetMethod("Unary").Invoke(null, new object[] { d, this });
            }
            else if (genericType == typeof(ClientStreamingServerMethod<,>))
            {
                return (Delegate)typeof(WrapUtil<,>).GetTypeInfo()
                    .MakeGenericType(dType.GetGenericArguments()).GetTypeInfo()
                    .GetMethod("ClientStreaming").Invoke(null, new object[] { d, this });
            }
            else if (genericType == typeof(ServerStreamingServerMethod<,>))
            {
                return (Delegate)typeof(WrapUtil<,>).GetTypeInfo()
                    .MakeGenericType(dType.GetGenericArguments()).GetTypeInfo()
                    .GetMethod("ServerStreaming").Invoke(null, new object[] { d, this });
            }
            else if (genericType == typeof(DuplexStreamingServerMethod<,>))
            {
                return (Delegate)typeof(WrapUtil<,>).GetTypeInfo()
                    .MakeGenericType(dType.GetGenericArguments()).GetTypeInfo()
                    .GetMethod("DuplexStreaming").Invoke(null, new object[] { d, this });
            }

            return d;
        }

        internal IServerCallHandler WrapServerCallHandler(IServerCallHandler handler)
        {
            var interceptable = handler as IInterceptableCallHandler;
            if (interceptable == null)
            {
                return handler;
            }

            return interceptable.Intercept(WrapDelegate);
        }
    }
}
