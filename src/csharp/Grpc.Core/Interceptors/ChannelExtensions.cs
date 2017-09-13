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

namespace Grpc.Core.Interceptors
{
    /// <summary>
    /// Provides extension methods to make it easy to register interceptors on Channel objects.
    /// </summary>
    public static class ChannelExtensions
    {
	/// <summary>
	/// Returns a CallInvoker instance that intercepts a channel with the given interceptor.
	/// </summary>
        public static CallInvoker Intercept(this Channel channel, ClientInterceptor interceptor)
        {
            return new Internal.InterceptingCallInvoker(new DefaultCallInvoker(channel), null, null);
        }
	
	/// <summary>
	/// Returns a CallInvoker that is processed by the specified interceptor before going through the underlying channel.
	/// </summary>
        public static CallInvoker Intercept<TInterceptor>(this Channel channel)
            where TInterceptor : ClientInterceptor, new()
        {
            return channel.Intercept(new TInterceptor());
        }

        //public static CallInvoker Intercept(this Channel channel)
    }
}
