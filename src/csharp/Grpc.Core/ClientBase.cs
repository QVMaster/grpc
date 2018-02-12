#region Copyright notice and license

// Copyright 2015-2016 gRPC authors.
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
using Grpc.Core.Interceptors;
using Grpc.Core.Internal;
using Grpc.Core.Utils;

namespace Grpc.Core
{
    /// <summary>
    /// Generic base class for client-side stubs.
    /// </summary>
    public abstract class ClientBase<T> : ClientBase
        where T : ClientBase<T>
    {
        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class that
        /// throws <c>NotImplementedException</c> upon invocation of any RPC.
        /// This constructor is only provided to allow creation of test doubles
        /// for client classes (e.g. mocking requires a parameterless constructor).
        /// </summary>
        protected ClientBase() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected ClientBase(ClientBaseConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="channel">The channel to use for remote call invocation.</param>
        public ClientBase(Channel channel) : base(channel)
        {
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="callInvoker">The <c>CallInvoker</c> for remote call invocation.</param>
        public ClientBase(CallInvoker callInvoker) : base(callInvoker)
        {
        }

        /// <summary>
        /// Creates a new client that sets host field for calls explicitly.
        /// gRPC supports multiple "hosts" being served by a single server.
        /// By default (if a client was not created by calling this method),
        /// host <c>null</c> with the meaning "use default host" is used.
        /// </summary>
        public T WithHost(string host)
        {
            var newConfiguration = this.Configuration.WithHost(host);
            return NewInstance(newConfiguration);
        }

        /// <summary>
        /// Creates a new instance of client from given <c>ClientBaseConfiguration</c>.
        /// </summary>
        protected abstract T NewInstance(ClientBaseConfiguration configuration);
    }

    /// <summary>
    /// Base class for client-side stubs.
    /// </summary>
    public abstract class ClientBase
    {
        readonly ClientBaseConfiguration configuration;
        readonly CallInvoker callInvoker;

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class that
        /// throws <c>NotImplementedException</c> upon invocation of any RPC.
        /// This constructor is only provided to allow creation of test doubles
        /// for client classes (e.g. mocking requires a parameterless constructor).
        /// </summary>
        protected ClientBase() : this(new UnimplementedCallInvoker())
        {
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected ClientBase(ClientBaseConfiguration configuration)
        {
            this.configuration = GrpcPreconditions.CheckNotNull(configuration, "configuration");
            this.callInvoker = configuration.CreateDecoratedCallInvoker();
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="channel">The channel to use for remote call invocation.</param>
        public ClientBase(Channel channel) : this(new DefaultCallInvoker(channel))
        {
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="callInvoker">The <c>CallInvoker</c> for remote call invocation.</param>
        public ClientBase(CallInvoker callInvoker) : this(new ClientBaseConfiguration(callInvoker, null))
        {
        }

        /// <summary>
        /// Gets the call invoker.
        /// </summary>
        protected CallInvoker CallInvoker
        {
            get { return this.callInvoker; }
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        internal ClientBaseConfiguration Configuration
        {
            get { return this.configuration; }
        }

        /// <summary>
        /// Represents configuration of ClientBase. The class itself is visible to
        /// subclasses, but contents are marked as internal to make the instances opaque.
        /// The verbose name of this class was chosen to make name clash in generated code 
        /// less likely.
        /// </summary>
        protected internal class ClientBaseConfiguration
        {
            private class ClientHeaderInterceptor : GenericInterceptor
            {
                readonly Func<IMethod, string, CallOptions, Tuple<string, CallOptions>> interceptor;

                /// <summary>
                /// Creates a new instance of ClientHeaderInterceptor given the specified header interceptor function.
                /// </summary>
                public ClientHeaderInterceptor(Func<IMethod, string, CallOptions, Tuple<string, CallOptions>> interceptor)
                {
                    this.interceptor = GrpcPreconditions.CheckNotNull(interceptor, "interceptor");
                }

                protected override ClientCallArbitrator<TRequest, TResponse> InterceptCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, bool clientStreaming, bool serverStreaming, TRequest request)
                {
                    var newHeaders = interceptor(context.Method, context.Host, context.Options);
                    return new ClientCallArbitrator<TRequest, TResponse>
                    {
                        Context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, newHeaders.Item1, newHeaders.Item2)
                    };
                }
            }

            readonly CallInvoker undecoratedCallInvoker;
            readonly string host;

            internal ClientBaseConfiguration(CallInvoker undecoratedCallInvoker, string host)
            {
                this.undecoratedCallInvoker = GrpcPreconditions.CheckNotNull(undecoratedCallInvoker);
                this.host = host;
            }

            internal CallInvoker CreateDecoratedCallInvoker()
            {
                return undecoratedCallInvoker.Intercept(new ClientHeaderInterceptor((method, host, options) => Tuple.Create(this.host, options)));
            }

            internal ClientBaseConfiguration WithHost(string host)
            {
                GrpcPreconditions.CheckNotNull(host, "host");
                return new ClientBaseConfiguration(this.undecoratedCallInvoker, host);
            }
        }
    }
}
