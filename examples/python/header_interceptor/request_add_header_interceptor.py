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

"""Interceptor that adds headers to outgoing requests."""

import collections

import grpc

class _ClientCallDetails(
        collections.namedtuple('_ClientCallDetails',
                               ('method', 'timeout', 'metadata',
                                'credentials')), grpc.ClientCallDetails):
    pass


class RequestAddHeaderInterceptor(grpc.UnaryUnaryClientInterceptor):

    def __init__(self, header, value):
        self._header = header
        self._value = value

    def intercept_unary_unary(self, continuation, client_call_details, request):
        metadata = []
        if client_call_details.metadata is not None:
            metadata = list(client_call_details.metadata)
        metadata.append((self._header, self._value))
        client_call_details = _ClientCallDetails(client_call_details.method,
                                                 client_call_details.timeout,
                                                 metadata,
                                                 client_call_details.credentials)
        return continuation(client_call_details, request)
