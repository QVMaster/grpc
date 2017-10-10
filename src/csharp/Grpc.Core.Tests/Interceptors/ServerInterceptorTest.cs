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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Core.Internal;
using Grpc.Core.Tests;
using Grpc.Core.Utils;
using NUnit.Framework;

namespace Grpc.Core.Interceptors.Tests
{
    public class ServerInterceptorTest
    {
        const string Host = "127.0.0.1";

        private class AddRequestHeaderServerInterceptor : ServerInterceptor
        {
            readonly Metadata.Entry header;

            public AddRequestHeaderServerInterceptor(string key, string value)
            {
                this.header = new Metadata.Entry(key, value);
            }
            public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
            {
                context.RequestHeaders.Add(header);
                return continuation(request, context);
            }

            public Metadata.Entry Header
            {
                get
                {
                    return header;
                }
            }
        }

        [Test]
        public void AddRequestHeaderInServerInterceptor()
        {
            var helper = new MockServiceHelper(Host);
            var interceptor = new AddRequestHeaderServerInterceptor("x-interceptor", "hello world");
            helper.UnaryHandler = new UnaryServerMethod<string, string>((request, context) =>
            {
                var interceptorHeader = context.RequestHeaders.Last(m => (m.Key == interceptor.Header.Key)).Value;
                Assert.AreEqual(interceptorHeader, interceptor.Header.Value);
                return Task.FromResult("PASS");
            });
            helper.ServiceDefinition = helper.ServiceDefinition.Intercept(interceptor);
            var server = helper.GetServer();
            server.Start();
            var channel = helper.GetChannel();
            Assert.AreEqual("PASS", Calls.BlockingUnaryCall(helper.CreateUnaryCall(), ""));
        }
    }
}
