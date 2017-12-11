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


class _ServicePipeline(object):

    def __init__(self, interceptors):
        self.interceptors = tuple(interceptors)

    def _continuation(self, index, thunk):
        return lambda context: self._intercept_at(index, context, thunk)

    def _intercept_at(self, index, context, thunk):
        if index < len(self.interceptors):
            interceptor = self.interceptors[index]
            thunk = self._continuation(index + 1, thunk)
            return interceptor.intercept_service(context, thunk)
        return thunk(context)

    def execute(self, context, thunk):
        return self._intercept_at(0, context, thunk)


def service_pipeline(interceptors):
    if interceptors is None or len(interceptors) == 0:
        return None
    return _ServicePipeline(interceptors)
