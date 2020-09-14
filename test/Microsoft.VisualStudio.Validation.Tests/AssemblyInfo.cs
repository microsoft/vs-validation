// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

// Suppress xunit parallelizing tests since we manipulate statics (TraceListeners)
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
