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
using System.Linq;
using Grpc.Core.Utils;

namespace Grpc.Core.Interceptors
{
    /// <summary>
    /// Provides extension methods for <c>ServerServiceDefinition</c> to support interceptors on the server side.
    /// This is an EXPERIMENTAL API.
    /// </summary>
    public static class ServerServiceDefinitionExtensions
    {
        /// <summary>
        /// Returns a <see cref="Grpc.Core.ServerServiceDefinition" /> instance that intercepts
        /// calls to the underlying service handler via the given interceptor.
        /// </summary>
        /// <param name="service">The service to intercept.</param>
        /// <param name="interceptor">The interceptor to register on service.</param>
        public static ServerServiceDefinition Intercept(this ServerServiceDefinition service, Interceptor interceptor)
        {
            GrpcPreconditions.CheckNotNull(service, "service");
            GrpcPreconditions.CheckNotNull(interceptor, "interceptor");
            return service.SubstituteHandlers(interceptor.WrapServerCallHandler);
        }

        /// <summary>
        /// Returns a <see cref="Grpc.Core.ServerServiceDefinition" /> instance that intercepts
        /// calls to the underlying service handler via the given interceptors.
        /// </summary>
        /// <param name="service">The service to intercept.</param>
        /// <param name="interceptors">
        /// The interceptors to register on service.
        /// Control is passed to the interceptors in the order they are specified.
        /// </param>
        public static ServerServiceDefinition Intercept(this ServerServiceDefinition service, params Interceptor[] interceptors)
        {
            if (interceptors == null)
            {
                return service;
            }

            foreach (var interceptor in interceptors.Reverse())
            {
                service = Intercept(service, interceptor);
            }

            return service;
        }
    }
}
