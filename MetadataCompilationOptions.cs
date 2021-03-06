﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System;

namespace CompilationOptionsReader
{
    internal class MetadataCompilationOptions
    {
        public ImmutableArray<(string optionName, string value)> Options { get; }

        public MetadataCompilationOptions(ImmutableArray<(string optionName, string value)> options)
        {
            Options = options;
        }

        public int Length => Options.Length;

        /// <summary>
        /// Attempts to get an option value. Returns false if the option value does not 
        /// exist OR if it exists more than once
        /// </summary>
        public bool TryGetUniqueOption(string optionName, out string? value)
        {
            value = null;

            var optionValues = Options.Where(pair => pair.optionName == optionName).ToArray();
            if (optionValues.Length != 1)
            {
                return false;
            }

            value = optionValues[0].value;
            return true;
        }

        public string GetUniqueOption(string optionName)
        {
            var optionValues = Options.Where(pair => pair.optionName == optionName).ToArray();
            if (optionValues.Length != 1)
            {
                throw new InvalidOperationException($"{optionName} exists {optionValues.Length} times in compilation options");
            }

            return optionValues[0].value;
        }
    }
}
