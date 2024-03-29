﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// 

using AutoRest.Core.Model;

namespace AutoRest.Ansible.Model
{
    public class SequenceTypePy : SequenceType, IExtendedModelTypePy
    {
        public SequenceTypePy()
        {
            Name.OnGet += v => $"list";
        }

        public string TypeDocumentation => $"list[{((IExtendedModelTypePy)ElementType).TypeDocumentation}]";
        public string ReturnTypeDocumentation => TypeDocumentation;
    }
}