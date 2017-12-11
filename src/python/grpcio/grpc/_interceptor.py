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

import collections

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


class _ClientCallDetails(
        collections.namedtuple('_ClientCallDetails',
                               ('method', 'timeout', 'metadata',
                                'credentials')), grpc.ClientCallDetails):
    pass


class _UnaryUnaryMultiCallable(grpc.UnaryUnaryMultiCallable):

    def __init__(self, thunk, method, interceptor):
        self._thunk = thunk
        self._method = method
        self._interceptor = interceptor

    def __call__(self, request, timeout=None, metadata=None, credentials=None):
        future = self.future(
            request,
            timeout=timeout,
            metadata=metadata,
            credentials=credentials)
        return future.result()

    def with_call(self, request, timeout=None, metadata=None, credentials=None):
        future = self.future(
            request,
            timeout=timeout,
            metadata=metadata,
            credentials=credentials)
        return future.result(), future

    def future(self, request, timeout=None, metadata=None, credentials=None):

        def continuation(client_call_details, request):
            return self._thunk(client_call_details.method).future(
                request,
                timeout=client_call_details.timeout,
                metadata=client_call_details.metadata,
                credentials=client_call_details.credentials)

        return self._interceptor.intercept_unary_unary(
            continuation,
            _ClientCallDetails(self._method, timeout, metadata, credentials),
            request)


class _UnaryStreamMultiCallable(grpc.UnaryStreamMultiCallable):

    def __init__(self, thunk, method, interceptor):
        self._thunk = thunk
        self._method = method
        self._interceptor = interceptor

    def __call__(self, request, timeout=None, metadata=None, credentials=None):

        def continuation(client_call_details, request):
            return self._thunk(client_call_details.method)(
                request,
                timeout=client_call_details.timeout,
                metadata=client_call_details.metadata,
                credentials=client_call_details.credentials)

        return self._interceptor.intercept_unary_stream(
            continuation,
            _ClientCallDetails(self._method, timeout, metadata, credentials),
            request)


class _StreamUnaryMultiCallable(grpc.StreamUnaryMultiCallable):

    def __init__(self, thunk, method, interceptor):
        self._thunk = thunk
        self._method = method
        self._interceptor = interceptor

    def __call__(self,
                 request_iterator,
                 timeout=None,
                 metadata=None,
                 credentials=None):
        future = self.future(
            request_iterator,
            timeout=timeout,
            metadata=metadata,
            credentials=credentials)
        return future.result()

    def with_call(self,
                  request_iterator,
                  timeout=None,
                  metadata=None,
                  credentials=None):
        future = self.future(
            request_iterator,
            timeout=timeout,
            metadata=metadata,
            credentials=credentials)
        return future.result(), future

    def future(self,
               request_iterator,
               timeout=None,
               metadata=None,
               credentials=None):

        def continuation(client_call_details, request_iterator):
            return self._thunk(client_call_details.method).future(
                request_iterator,
                timeout=client_call_details.timeout,
                metadata=client_call_details.metadata,
                credentials=client_call_details.credentials)

        return self._interceptor.intercept_stream_unary(
            continuation,
            _ClientCallDetails(self._method, timeout, metadata, credentials),
            request_iterator)


class _StreamStreamMultiCallable(grpc.StreamStreamMultiCallable):

    def __init__(self, thunk, method, interceptor):
        self._thunk = thunk
        self._method = method
        self._interceptor = interceptor

    def __call__(self,
                 request_iterator,
                 timeout=None,
                 metadata=None,
                 credentials=None):

        def continuation(client_call_details, request_iterator):
            return self._thunk(client_call_details.method)(
                request_iterator,
                timeout=client_call_details.timeout,
                metadata=client_call_details.metadata,
                credentials=client_call_details.credentials)

        return self._interceptor.intercept_stream_stream(
            continuation,
            _ClientCallDetails(self._method, timeout, metadata, credentials),
            request_iterator)


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
