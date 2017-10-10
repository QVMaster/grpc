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
    /// Carries along the context associated with intercepted invocations on the client side.
    /// This is an EXPERIMENTAL API.
    /// </summary>
    public class ClientInterceptorContext<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// Creates a new instance of <see cref="Grpc.Core.Interceptors.ClientInterceptorContext{TRequest, TResponse}" />
        /// with the specified method, host, and call options.
        /// </summary>
        /// <param name="method">A <see cref="Grpc.Core.Method{TRequest, TResponse}"/> object representing the method to be invoked.</param>
        /// <param name="host">The host to dispatch the current call to.</param>
        /// <param name="options">A <see cref="Grpc.Core.CallOptions"/> instance containing the call options of the current call.</param>

        public ClientInterceptorContext(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            Method = method;
            Host = host;
            Options = options;
        }

        /// <summary>
        /// Gets the <see cref="Grpc.Core.Method{TRequest, TResponse}"/> representing
        /// the method to be invoked.
        /// </summary>
        public Method<TRequest, TResponse> Method { get; }

        /// <summary>
        /// Gets the host that the currect invocation will be dispatched to.
        /// </summary>

        public string Host { get; }

        /// <summary>
        /// Gets the <see cref="Grpc.Core.CallOptions"/> structure representing the
        /// call options associated with the current invocation.
        /// </summary>

        public CallOptions Options { get; }
    }

    /// <summary>
    /// Serves as the base class for gRPC interceptors.
    /// This is an EXPERIMENTAL API.
    /// </summary>
    public abstract class Interceptor
    {
        /// <summary>
        /// Represents a continuation for intercepting simple blocking invocations.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this invocation.</typeparam>
        /// <typeparam name="TResponse">Response message type for this invocation.</typeparam>
        /// <param name="request">The request value to continue the invocation with.</param>
        /// <param name="context">
        /// The <see cref="Grpc.Core.Interceptors.ClientInterceptorContext{TRequest, TResponse}"/>
        /// instance to pass to the next step in the invocation process.
        /// </param>
        public delegate TResponse BlockingUnaryCallContinuation<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class;

        /// <summary>
        /// Represents a continuation for intercepting simple asynchronous invocations.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this invocation.</typeparam>
        /// <typeparam name="TResponse">Response message type for this invocation.</typeparam>
        /// <param name="request">The request value to continue the invocation with.</param>
        /// <param name="context">
        /// The <see cref="Grpc.Core.Interceptors.ClientInterceptorContext{TRequest, TResponse}"/>
        /// instance to pass to the next step in the invocation process.
        /// </param>
        public delegate AsyncUnaryCall<TResponse> AsyncUnaryCallContinuation<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class;

        /// <summary>
        /// Represents a continuation for intercepting asynchronous server-streaming invocations.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this invocation.</typeparam>
        /// <typeparam name="TResponse">Response message type for this invocation.</typeparam>
        /// <param name="request">The request value to continue the invocation with.</param>
        /// <param name="context">
        /// The <see cref="Grpc.Core.Interceptors.ClientInterceptorContext{TRequest, TResponse}"/>
        /// instance to pass to the next step in the invocation process.
        /// </param>
        public delegate AsyncServerStreamingCall<TResponse> AsyncServerStreamingCallContinuation<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class;

        /// <summary>
        /// Represents a continuation for intercepting asynchronous client-streaming invocations.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this invocation.</typeparam>
        /// <typeparam name="TResponse">Response message type for this invocation.</typeparam>
        /// <param name="context">
        /// The <see cref="Grpc.Core.Interceptors.ClientInterceptorContext{TRequest, TResponse}"/>
        /// instance to pass to the next step in the invocation process.
        /// </param>
        public delegate AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCallContinuation<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class;

        /// <summary>
        /// Represents a continuation for intercepting asynchronous duplex invocations.
        /// </summary>
        /// <param name="context">
        /// The <see cref="Grpc.Core.Interceptors.ClientInterceptorContext{TRequest, TResponse}"/>
        /// instance to pass to the next step in the invocation process.
        /// </param>
        public delegate AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCallContinuation<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class;

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
        public virtual TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
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
        public virtual AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
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
        public virtual AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
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
        public virtual AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
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
        public virtual AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            return continuation(context);
        }

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

        /// <summary>
        /// Used by <c>WrapDelegate</c> function to wire up a non-generic
        /// handler to a type-safe generic interceptor function with the
        /// correct type arguments.
        /// </summary>
        private static class WrapUtil<TRequest, TResponse>
            where TRequest : class
            where TResponse : class
        {
            public static UnaryServerMethod<TRequest, TResponse> Unary(
                UnaryServerMethod<TRequest, TResponse> handler,
                Interceptor interceptor)
            {
                return (request, context) =>
                    interceptor.UnaryServerHandler<TRequest, TResponse>(request, context, handler);
            }

            public static ClientStreamingServerMethod<TRequest, TResponse> ClientStreaming(
                ClientStreamingServerMethod<TRequest, TResponse> handler,
                Interceptor interceptor)
            {
                return (request, context) =>
                    interceptor.ClientStreamingServerHandler<TRequest, TResponse>(request, context, handler);
            }

            public static ServerStreamingServerMethod<TRequest, TResponse> ServerStreaming(
                ServerStreamingServerMethod<TRequest, TResponse> handler,
                Interceptor interceptor)
            {
                return (request, response, context) =>
                    interceptor.ServerStreamingServerHandler<TRequest, TResponse>(request, response, context, handler);
            }

            public static DuplexStreamingServerMethod<TRequest, TResponse> DuplexStreaming(
                DuplexStreamingServerMethod<TRequest, TResponse> handler,
                Interceptor interceptor)
            {
                return (request, response, context) =>
                    interceptor.DuplexStreamingServerHandler<TRequest, TResponse>(request, response, context, handler);
            }
        }

        /// <summary>
        /// Returns a wrapped delegate that intercepts the calls to the
        /// given delegate <c>d</c> in a type-safe fashion.
        /// This is necessary because the interceptor does not have
        /// a priori information about the generic type of each handler
        /// and needs to reconsturct that information by reflecting over
        /// the delegate and matching it with the appropriate generic
        /// interceptor function.
        /// </summary>
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

        /// <summary>
        /// Decorates an instance of <see cref="Grpc.Core.Internal.IServerCallHandler"/>
        /// and intercepts the execution its handlers.
        /// </summary>
        /// <returns>
        /// Returns a decorated <see cref="Grpc.Core.Internal.IServerCallHandler"/>
        /// if <c>handler</c> implements <see cref="Grpc.Core.Internal.IInterceptableCallHandler" />,
        /// unmodified <c>handler</c> otherwise.
        /// </returns>
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
