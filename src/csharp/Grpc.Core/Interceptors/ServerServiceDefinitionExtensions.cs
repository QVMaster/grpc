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
    /// Provides extension methods for <c>ServerServiceDefinition</c> to support interceptors on the server side.
    /// This is an EXPERIMENTAL API.
    /// </summary>
    public static class ServerServiceDefinitionExtensions
    {
        /// <summary>
        /// Returns a <c>ServerServiceDefinition</c> object that intercepts calls to the underlying service through the given interceptor.
        /// </summary>
        public static ServerServiceDefinition Intercept(this ServerServiceDefinition service, ServerInterceptor interceptor)
        {
            GrpcPreconditions.CheckNotNull(service, "service");
            GrpcPreconditions.CheckNotNull(interceptor, "interceptor");
            return service.SubstituteHandlers(interceptor.WrapServerCallHandler);
        }
    }
}
