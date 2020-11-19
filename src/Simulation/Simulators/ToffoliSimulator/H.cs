﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Quantum.Simulation.Core;

namespace Microsoft.Quantum.Simulation.Simulators
{
    public partial class ToffoliSimulator
    {
        /// <summary>
        /// The implementation of the operation.
        /// For the Toffoli simulator, the implementation throws a run-time error.
        /// </summary>
        public Func<Qubit, QVoid> H_Body() => (q1) =>
            throw new NotImplementedException();

        public Func<(IQArray<Qubit>, Qubit), QVoid> H_ControlledBody() => (args) =>
            throw new NotImplementedException();
    }
}
