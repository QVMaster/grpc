# Copyright 2016 gRPC authors.
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
"""Test of gRPC Python's application-layer API."""

import unittest

import six

import grpc

from tests.unit import _from_grpc_import_star


class AllTest(unittest.TestCase):

    def testAll(self):
        expected_grpc_code_elements = (
            'AuthMetadataContext', 'AuthMetadataPlugin',
            'AuthMetadataPluginCallback', 'Call', 'CallCredentials', 'Channel',
            'ChannelConnectivity', 'ChannelCredentials', 'Future',
            'FutureCancelledError', 'FutureTimeoutError', 'GenericRpcHandler',
            'HandlerCallDetails', 'RpcContext', 'RpcError', 'RpcMethodHandler',
            'Server', 'ServerCertificateConfiguration', 'ServerCredentials',
            'ServiceRpcHandler', 'ServicerContext', 'StatusCode',
            'StreamStreamClientInterceptor', 'StreamStreamMultiCallable',
            'StreamStreamServerInterceptor', 'StreamUnaryClientInterceptor',
            'StreamUnaryMultiCallable', 'StreamUnaryServerInterceptor',
            'UnaryStreamClientInterceptor', 'UnaryStreamMultiCallable',
            'UnaryStreamServerInterceptor', 'UnaryUnaryClientInterceptor',
            'UnaryUnaryMultiCallable', 'UnaryUnaryServerInterceptor',
            'access_token_call_credentials', 'channel_ready_future',
            'composite_call_credentials', 'composite_channel_credentials',
            'dynamic_ssl_server_credentials', 'insecure_channel',
            'intercept_channel', 'intercept_server',
            'metadata_call_credentials', 'method_handlers_generic_handler',
            'secure_channel', 'server', 'ssl_channel_credentials',
            'ssl_server_certificate_configuration', 'ssl_server_credentials',
            'stream_stream_rpc_method_handler',
            'stream_unary_rpc_method_handler',
            'unary_stream_rpc_method_handler',
            'unary_unary_rpc_method_handler',)

        six.assertCountEqual(self, expected_grpc_code_elements,
                             _from_grpc_import_star.GRPC_ELEMENTS)


class ChannelConnectivityTest(unittest.TestCase):

    def testChannelConnectivity(self):
        self.assertSequenceEqual(
            (grpc.ChannelConnectivity.IDLE, grpc.ChannelConnectivity.CONNECTING,
             grpc.ChannelConnectivity.READY,
             grpc.ChannelConnectivity.TRANSIENT_FAILURE,
             grpc.ChannelConnectivity.SHUTDOWN,),
            tuple(grpc.ChannelConnectivity))


class ChannelTest(unittest.TestCase):

    def test_secure_channel(self):
        channel_credentials = grpc.ssl_channel_credentials()
        channel = grpc.secure_channel('google.com:443', channel_credentials)


if __name__ == '__main__':
    unittest.main(verbosity=2)
