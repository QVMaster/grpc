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
        collections.namedtuple('_ClientCallDetails', (
            'method', 'request_streaming', 'response_streaming',
            'request_serializer', 'response_deserializer', 'timeout',
            'metadata', 'credentials'), grpc.ClientCallDetails)):
    pass


def _continuation(thunk):

    def continuation(client_call_details, request_iterator):
        if not client_call_details.request_streaming:
            next(request_iterator)
            return thunk(client_call_details)(
                request,
                timeout=client_call_details.timeout,
                metadata=client_call_details.metadata,
                credentials=client_call_details.credentials)


class _UnaryUnaryMultiCallable(grpc.UnaryUnaryMultiCallable):

    def __init__(self, thunk, interceptor, method, request_serializer,
                 response_deserializer):
        self._thunk = thunk
        self._interceptor = interceptor
        self._method = method
        self._request_serializer = request_serializer
        self._response_deserializer = response_deserializer

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

        response_iterator = self._interceptor.intercept_call(
            _continuation(self._thunk),
            _ClientCallDetails(
                self._method, False, False, self._request_serializer,
                self._response_deserializer, timeout, metadata, credentials),
            (request,))


class _UnaryStreamMultiCallable(grpc.UnaryStreamMultiCallable):

    def __init__(self, thunk, interceptor, method, request_serializer,
                 response_deserializer):
        self._thunk = thunk
        self._interceptor = interceptor
        self._method = method
        self._request_serializer = request_serializer
        self._response_deserializer = response_deserializer

    def __call__(self, request, timeout=None, metadata=None, credentials=None):
        return self._interceptor.intercept_call(
            continuation,
            _ClientCallDetails(self._method, timeout, metadata, credentials),
            request)

    def _continuation(self, client_call_details, request):
        return self._thunk(client_call_details.method)(
            request,
            timeout=client_call_details.timeout,
            metadata=client_call_details.metadata,
            credentials=client_call_details.credentials)


class _StreamUnaryMultiCallable(grpc.StreamUnaryMultiCallable):

    def __init__(self, thunk, interceptor, method, request_serializer,
                 response_deserializer):
        self._thunk = thunk
        self._interceptor = interceptor
        self._method = method
        self._request_serializer = request_serializer
        self._response_deserializer = response_deserializer

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

    def __init__(self, thunk, interceptor, method, request_serializer,
                 response_deserializer):
        self._thunk = thunk
        self._interceptor = interceptor
        self._method = method
        self._request_serializer = request_serializer
        self._response_deserializer = response_deserializer

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

        return self._interceptor.intercept_call(
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
        return _UnaryUnaryMultiCallable(self._thunk, self._interceptor, method,
                                        request_serializer,
                                        response_deserializer)

    def unary_stream(self,
                     method,
                     request_serializer=None,
                     response_deserializer=None):
        return _UnaryStreamMultiCallable(self._thunk, self._interceptor, method,
                                         request_serializer,
                                         response_deserializer)

    def stream_unary(self,
                     method,
                     request_serializer=None,
                     response_deserializer=None):
        return _StreamUnaryMultiCallable(self._thunk, self._interceptor, method,
                                         request_serializer,
                                         response_deserializer)

    def stream_stream(self,
                      method,
                      request_serializer=None,
                      response_deserializer=None):
        return _StreamStreamMultiCallable(self._thunk, self._interceptor,
                                          method, request_serializer,
                                          response_deserializer)

    def _thunk(self, call):
        if not call.request_streaming and not call.response_streaming:
            return self._channel.unary_unary(
                call.method,
                request_serializer=call.request_serializer,
                response_deserializer=call.response_deserializer)
        if not call.request_streaming and call.response_streaming:
            return self._channel.unary_stream(
                call.method,
                request_serializer=call.request_serializer,
                response_deserializer=call.response_deserializer)
        if call.request_streaming and not call.response_streaming:
            return self._channel.stream_unary(
                call.method,
                request_serializer=call.request_serializer,
                response_deserializer=call.response_deserializer)
        if call.request_streaming and call.response_streaming:
            return self._channel.stream_stream(
                call.method,
                request_serializer=call.request_serializer,
                response_deserializer=call.response_deserializer)
        raise ValueError('call is not a valid ClientCallDetails value')


def intercept_channel(channel, *interceptors):
    for interceptor in reversed(list(interceptors)):
        if not isinstance(interceptor, grpc.ClientInterceptor):
            raise TypeError('interceptor must be grpc.ClientInterceptor')
        channel = _Channel(channel, interceptor)
    return channel
