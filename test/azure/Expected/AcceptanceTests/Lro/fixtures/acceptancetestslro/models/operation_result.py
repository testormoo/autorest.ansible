# coding=utf-8
# --------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for
# license information.
#
# Code generated by Microsoft (R) AutoRest Code Generator.
# Changes may cause incorrect behavior and will be lost if the code is
# regenerated.
# --------------------------------------------------------------------------

from msrest.serialization import Model


class OperationResult(Model):
    """OperationResult.

    :param status: The status of the request. Possible values include:
     'Succeeded', 'Failed', 'canceled', 'Accepted', 'Creating', 'Created',
     'Updating', 'Updated', 'Deleting', 'Deleted', 'OK'
    :type status: str or ~fixtures.acceptancetestslro.models.enum
    :param error:
    :type error: ~fixtures.acceptancetestslro.models.OperationResultError
    """

    _attribute_map = {
        'status': {'key': 'status', 'type': 'str'},
        'error': {'key': 'error', 'type': 'OperationResultError'},
    }

    def __init__(self, status=None, error=None):
        self.status = status
        self.error = error
