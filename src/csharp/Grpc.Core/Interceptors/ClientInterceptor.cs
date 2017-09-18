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
using System.Threading.Tasks;
using Grpc.Core.Utils;

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

        /// <summary>
        /// Returns a new instance of <c>AsyncUnaryCall</c> that has hooks to intercept on an underlying object of the same type.
        /// </summary>
        protected static AsyncUnaryCall<TResponse> Intercept<TRequest, TResponse>(AsyncUnaryCall<TResponse> call,
            Func<Task<TResponse>, TResponse> response = null,
            Func<Task<Metadata>, Metadata> responseHeaders = null,
            Func<Func<Status>, Func<Status>> getStatus = null,
            Func<Func<Metadata>, Func<Metadata>> getTrailers = null,
            Func<Action, Action> dispose = null)
        {
            GrpcPreconditions.CheckNotNull(call, "call");
            var callResponse = call.ResponseAsync;
            Task<Metadata> callResponseHeaders = call.ResponseHeadersAsync;
            Func<Status> callStatus = call.GetStatus;
            Func<Metadata> callTrailers = call.GetTrailers;
            Action callDispose = call.Dispose;

            if (callResponse != null)
            {
                var responseOriginal = callResponse;
                callResponse = new Task<TResponse>(() => response(responseOriginal));
            }

            if (responseHeaders != null)
            {
                var callResponseHeadersOriginal = callResponseHeaders; // Ensure it will not be captured by the closure
                callResponseHeaders = new Task<Metadata>(() => responseHeaders(callResponseHeadersOriginal));
            }

            if (getStatus != null)
            {
                callStatus = getStatus(callStatus);
            }

            if (getTrailers != null)
            {
                callTrailers = getTrailers(callTrailers);
            }

            if (dispose != null)
            {
                callDispose = dispose(callDispose);
            }

            return new AsyncUnaryCall<TResponse>(callResponse, callResponseHeaders, callStatus, callTrailers, callDispose);
        }

        /// <summary>
        /// Returns a new instance of <c>AsyncServerStreamingCall</c> that has hooks to intercept on an underlying object of the same type.
        /// </summary>
        protected static AsyncServerStreamingCall<TResponse> Intercept<TRequest, TResponse>(AsyncServerStreamingCall<TResponse> call,
            Func<IAsyncStreamReader<TResponse>, IAsyncStreamReader<TResponse>> responseStream = null,
            Func<Task<Metadata>, Metadata> responseHeaders = null,
            Func<Func<Status>, Func<Status>> getStatus = null,
            Func<Func<Metadata>, Func<Metadata>> getTrailers = null,
            Func<Action, Action> dispose = null)
        {
            GrpcPreconditions.CheckNotNull(call, "call");
            var callResponseStream = call.ResponseStream;
            Task<Metadata> callResponseHeaders = call.ResponseHeadersAsync;
            Func<Status> callStatus = call.GetStatus;
            Func<Metadata> callTrailers = call.GetTrailers;
            Action callDispose = call.Dispose;

            if (responseStream != null)
            {
                callResponseStream = responseStream(callResponseStream);
            }

            if (responseHeaders != null)
            {
                var callResponseHeadersOriginal = callResponseHeaders; // Ensure it will not be captured by the closure
                callResponseHeaders = new Task<Metadata>(() => responseHeaders(callResponseHeadersOriginal));
            }

            if (getStatus != null)
            {
                callStatus = getStatus(callStatus);
            }

            if (getTrailers != null)
            {
                callTrailers = getTrailers(callTrailers);
            }

            if (dispose != null)
            {
                callDispose = dispose(callDispose);
            }

            return new AsyncServerStreamingCall<TResponse>(callResponseStream, callResponseHeaders, callStatus, callTrailers, callDispose);
        }

        /// <summary>
        /// Returns a new instance of <c>AsyncClientStreamingCall</c> that has hooks to intercept on an underlying object of the same type.
        /// </summary>
        protected static AsyncClientStreamingCall<TRequest, TResponse> Intercept<TRequest, TResponse>(AsyncClientStreamingCall<TRequest, TResponse> call,
            Func<IClientStreamWriter<TRequest>, IClientStreamWriter<TRequest>> requestStream = null,
            Func<Task<TResponse>, TResponse> response = null,
            Func<Task<Metadata>, Metadata> responseHeaders = null,
            Func<Func<Status>, Func<Status>> getStatus = null,
            Func<Func<Metadata>, Func<Metadata>> getTrailers = null,
            Func<Action, Action> dispose = null)
        {
            GrpcPreconditions.CheckNotNull(call, "call");
            var callRequestStream = call.RequestStream;
            var callResponse = call.ResponseAsync;
            Task<Metadata> callResponseHeaders = call.ResponseHeadersAsync;
            Func<Status> callStatus = call.GetStatus;
            Func<Metadata> callTrailers = call.GetTrailers;
            Action callDispose = call.Dispose;

            if (requestStream != null)
            {
                callRequestStream = requestStream(callRequestStream);
            }

            if (callResponse != null)
            {
                var responseOriginal = callResponse;
                callResponse = new Task<TResponse>(() => response(responseOriginal));
            }

            if (responseHeaders != null)
            {
                var callResponseHeadersOriginal = callResponseHeaders; // Ensure it will not be captured by the closure
                callResponseHeaders = new Task<Metadata>(() => responseHeaders(callResponseHeadersOriginal));
            }

            if (getStatus != null)
            {
                callStatus = getStatus(callStatus);
            }

            if (getTrailers != null)
            {
                callTrailers = getTrailers(callTrailers);
            }

            if (dispose != null)
            {
                callDispose = dispose(callDispose);
            }

            return new AsyncClientStreamingCall<TRequest, TResponse>(callRequestStream, callResponse, callResponseHeaders, callStatus, callTrailers, callDispose);
        }

        /// <summary>
        /// Returns a new instance of <c>AsyncDuplexStreamingCall</c> that has hooks to intercept on an underlying object of the same type.
        /// </summary>
        protected static AsyncDuplexStreamingCall<TRequest, TResponse> Intercept<TRequest, TResponse>(AsyncDuplexStreamingCall<TRequest, TResponse> call,
            Func<IClientStreamWriter<TRequest>, IClientStreamWriter<TRequest>> requestStream = null,
            Func<IAsyncStreamReader<TResponse>, IAsyncStreamReader<TResponse>> responseStream = null,
            Func<Task<Metadata>, Metadata> responseHeaders = null,
            Func<Func<Status>, Func<Status>> getStatus = null,
            Func<Func<Metadata>, Func<Metadata>> getTrailers = null,
            Func<Action, Action> dispose = null)
        {
            GrpcPreconditions.CheckNotNull(call, "call");
            var callRequestStream = call.RequestStream;
            var callResponseStream = call.ResponseStream;
            Task<Metadata> callResponseHeaders = call.ResponseHeadersAsync;
            Func<Status> callStatus = call.GetStatus;
            Func<Metadata> callTrailers = call.GetTrailers;
            Action callDispose = call.Dispose;

            if (requestStream != null)
            {
                callRequestStream = requestStream(callRequestStream);
            }

            if (responseStream != null)
            {
                callResponseStream = responseStream(callResponseStream);
            }

            if (responseHeaders != null)
            {
                var callResponseHeadersOriginal = callResponseHeaders; // Ensure it will not be captured by the closure
                callResponseHeaders = new Task<Metadata>(() => responseHeaders(callResponseHeadersOriginal));
            }

            if (getStatus != null)
            {
                callStatus = getStatus(callStatus);
            }

            if (getTrailers != null)
            {
                callTrailers = getTrailers(callTrailers);
            }

            if (dispose != null)
            {
                callDispose = dispose(callDispose);
            }

            return new AsyncDuplexStreamingCall<TRequest, TResponse>(callRequestStream, callResponseStream, callResponseHeaders, callStatus, callTrailers, callDispose);
        }
    }
}
