# Copyright 2017 gRPC authors.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
"""Implementation of gRPC Python interceptors."""

import grpc


class _ServicePipeline(object):

    def __init__(self, interceptors):
        self.interceptors = tuple(interceptors)

    def _continuation(self, thunk, index):
        return lambda context: self._intercept_at(thunk, index, context)

    def _intercept_at(self, thunk, index, context):
        if index < len(self.interceptors):
            interceptor = self.interceptors[index]
            thunk = self._continuation(thunk, index + 1)
            return interceptor.intercept_service(thunk, context)
        return thunk(context)

    def execute(self, thunk, context):
        return self._intercept_at(thunk, 0, context)


def service_pipeline(interceptors):
    if interceptors:
        return _ServicePipeline(interceptors)
    return None


class _UnaryUnaryMultiCallable(grpc.UnaryUnaryMultiCallable):

    def __init__(self, thunk, method, interceptor):
        self._thunk = thunk
        self._method = method
        self._interceptor = interceptor

    def __call__(self, *args, **kwargs):
        future = self.future(*args, **kwargs)
        return future.result()

    def with_call(self, *args, **kwargs):
        future = self.future(*args, **kwargs)
        return future.result(), future

    def future(self, *args, **kwargs):

        def continuation(method, *args, **kwargs):
            return self._thunk(method).future(*args, **kwargs)

        return self._interceptor.intercept_unary_unary(
            continuation, self._method, *args, **kwargs)


class _UnaryStreamMultiCallable(grpc.UnaryStreamMultiCallable):

    def __init__(self, thunk, method, interceptor):
        self._thunk = thunk
        self._method = method
        self._interceptor = interceptor

    def __call__(self, *args, **kwargs):

        def continuation(method, *args, **kwargs):
            return self._thunk(method)(*args, **kwargs)

        return self._interceptor.intercept_unary_stream(
            continuation, self._method, *args, **kwargs)


class _StreamUnaryMultiCallable(grpc.StreamUnaryMultiCallable):

    def __init__(self, thunk, method, interceptor):
        self._thunk = thunk
        self._method = method
        self._interceptor = interceptor

    def __call__(self, *args, **kwargs):
        future = self.future(*args, **kwargs)
        return future.result()

    def with_call(self, *args, **kwargs):
        future = self.future(*args, **kwargs)
        return future.result(), future

    def future(self, *args, **kwargs):

        def continuation(method, *args, **kwargs):
            return self._thunk(method).future(*args, **kwargs)

        return self._interceptor.intercept_stream_unary(
            continuation, self._method, *args, **kwargs)


class _StreamStreamMultiCallable(grpc.StreamStreamMultiCallable):

    def __init__(self, thunk, method, interceptor):
        self._thunk = thunk
        self._method = method
        self._interceptor = interceptor

    def __call__(self, *args, **kwargs):

        def continuation(method, *args, **kwargs):
            return self._thunk(method)(*args, **kwargs)

        return self._interceptor.intercept_stream_stream(
            continuation, self._method, *args, **kwargs)


class _Channel(grpc.Channel):

    def __init__(self, channel, interceptor):
        self._channel = channel
        self._interceptor = interceptor

    def subscribe(self, *args, **kwargs):
        self._channel.subscribe(*args, **kwargs)

    def unsubscribe(self, *args, **kwargs):
        self._channel.unsubscribe(*args, **kwargs)

    def unary_unary(self,
                    method,
                    request_serializer=None,
                    response_deserializer=None):
        thunk = lambda m: self._channel.unary_unary(m, request_serializer, response_deserializer)
        if isinstance(self._interceptor, grpc.UnaryUnaryClientInterceptor):
            return _UnaryUnaryMultiCallable(thunk, method, self._interceptor)
        return thunk(method)

    def unary_stream(self,
                     method,
                     request_serializer=None,
                     response_deserializer=None):
        thunk = lambda m: self._channel.unary_stream(m, request_serializer, response_deserializer)
        if isinstance(self._interceptor, grpc.UnaryStreamClientInterceptor):
            return _UnaryStreamMultiCallable(thunk, method, self._interceptor)
        return thunk(method)

    def stream_unary(self,
                     method,
                     request_serializer=None,
                     response_deserializer=None):
        thunk = lambda m: self._channel.stream_unary(m, request_serializer, response_deserializer)
        if isinstance(self._interceptor, grpc.StreamUnaryClientInterceptor):
            return _StreamUnaryMultiCallable(thunk, method, self._interceptor)
        return thunk(method)

    def stream_stream(self,
                      method,
                      request_serializer=None,
                      response_deserializer=None):
        thunk = lambda m: self._channel.stream_stream(m, request_serializer, response_deserializer)
        if isinstance(self._interceptor, grpc.StreamStreamClientInterceptor):
            return _StreamStreamMultiCallable(thunk, method, self._interceptor)
        return thunk(method)


def intercept_channel(channel, *interceptors):
    for interceptor in reversed(list(interceptors)):
        if not isinstance(interceptor, grpc.UnaryUnaryClientInterceptor) and \
           not isinstance(interceptor, grpc.UnaryStreamClientInterceptor) and \
           not isinstance(interceptor, grpc.StreamUnaryClientInterceptor) and \
           not isinstance(interceptor, grpc.StreamStreamClientInterceptor):
            raise TypeError('interceptor must be '
                            'grpc.UnaryUnaryClientInterceptor or '
                            'grpc.UnaryStreamClientInterceptor or '
                            'grpc.StreamUnaryClientInterceptor or '
                            'grpc.StreamStreamClientInterceptor or ')
        channel = _Channel(channel, interceptor)
    return channel
