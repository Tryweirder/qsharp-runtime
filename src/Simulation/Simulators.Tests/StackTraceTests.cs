﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xunit;

using System;
using System.Threading.Tasks;

using Microsoft.Quantum.Simulation.Core;
using Microsoft.Quantum.Simulation.Common;
using Microsoft.Quantum.Simulation.Simulators.Tests.Circuits;
using Microsoft.Quantum.Simulation.Simulators.Exceptions;
using Xunit.Abstractions;

namespace Microsoft.Quantum.Simulation.Simulators.Tests
{
    public class StackTraceTests
    {
        const string namespacePrefix = "Microsoft.Quantum.Simulation.Simulators.Tests.Circuits.";

        private readonly ITestOutputHelper output;
        public StackTraceTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void AlwaysFail4Test()
        {
            ToffoliSimulator sim = new ToffoliSimulator();
            StackTraceCollector sc = new StackTraceCollector(sim);
            ICallable op = sim.Get<ICallable, AlwaysFail4>();
            try
            {
                QVoid res = op.Apply<QVoid>(QVoid.Instance);
            }
            catch (ExecutionFailException)
            {
                StackFrame[] stackFrames = sc.CallStack.ToArray();

                Assert.Equal(5, stackFrames.Length);

                Assert.Equal(namespacePrefix + "AlwaysFail", stackFrames[0].operation.FullName);
                Assert.Equal(namespacePrefix + "AlwaysFail1", stackFrames[1].operation.FullName);
                Assert.Equal(namespacePrefix + "AlwaysFail2", stackFrames[2].operation.FullName);
                Assert.Equal(namespacePrefix + "AlwaysFail3", stackFrames[3].operation.FullName);
                Assert.Equal(namespacePrefix + "AlwaysFail4", stackFrames[4].operation.FullName);

                Assert.Equal(OperationFunctor.Controlled, stackFrames[0].operation.Variant);
                Assert.Equal(OperationFunctor.Controlled, stackFrames[1].operation.Variant);
                Assert.Equal(OperationFunctor.Body, stackFrames[2].operation.Variant);
                Assert.Equal(OperationFunctor.Adjoint, stackFrames[3].operation.Variant);
                Assert.Equal(OperationFunctor.Body, stackFrames[4].operation.Variant);

                Assert.Equal(14, stackFrames[2].failedLineNumber);
                Assert.Equal(21, stackFrames[4].failedLineNumber);

                // For Adjoint and Controlled we expect failedLineNumber to be equal to declarationStartLineNumber
                Assert.Equal(stackFrames[0].declarationStartLineNumber, stackFrames[0].failedLineNumber);
                Assert.Equal(stackFrames[1].declarationStartLineNumber, stackFrames[1].failedLineNumber);
                Assert.Equal(stackFrames[3].declarationStartLineNumber, stackFrames[3].failedLineNumber);
            }
        }

        [Fact]
        public void GenericFail1Test()
        {
            ToffoliSimulator sim = new ToffoliSimulator();

            {
                StackTraceCollector sc = new StackTraceCollector(sim);
                ICallable op = sim.Get<ICallable, GenericFail1>();
                try
                {
                    QVoid res = op.Apply<QVoid>(QVoid.Instance);
                }
                catch (ExecutionFailException)
                {
                    StackFrame[] stackFrames = sc.CallStack.ToArray();

                    Assert.Equal(3, stackFrames.Length);

                    Assert.Equal(namespacePrefix + "AlwaysFail", stackFrames[0].operation.FullName);
                    Assert.Equal(namespacePrefix + "GenericFail", stackFrames[1].operation.FullName);
                    Assert.Equal(namespacePrefix + "GenericFail1", stackFrames[2].operation.FullName);

                    Assert.Equal(OperationFunctor.Body, stackFrames[0].operation.Variant);
                    Assert.Equal(OperationFunctor.Body, stackFrames[1].operation.Variant);
                    Assert.Equal(OperationFunctor.Body, stackFrames[2].operation.Variant);

                    Assert.Equal(7, stackFrames[0].failedLineNumber);
                    Assert.Equal(25, stackFrames[1].failedLineNumber);
                    Assert.Equal(29, stackFrames[2].failedLineNumber);
                }
            }

            {
                StackTraceCollector sc = new StackTraceCollector(sim);
                ICallable op = sim.Get<ICallable, GenericAdjFail1>();
                try
                {
                    QVoid res = op.Apply<QVoid>(QVoid.Instance);
                }
                catch (ExecutionFailException)
                {
                    StackFrame[] stackFrames = sc.CallStack.ToArray();

                    Assert.Equal(3, stackFrames.Length);

                    Assert.Equal(namespacePrefix + "AlwaysFail", stackFrames[0].operation.FullName);
                    Assert.Equal(namespacePrefix + "GenericFail", stackFrames[1].operation.FullName);
                    Assert.Equal(namespacePrefix + "GenericAdjFail1", stackFrames[2].operation.FullName);

                    Assert.Equal(OperationFunctor.Adjoint, stackFrames[0].operation.Variant);
                    Assert.Equal(OperationFunctor.Adjoint, stackFrames[1].operation.Variant);
                    Assert.Equal(OperationFunctor.Body, stackFrames[2].operation.Variant);

                    Assert.Equal(5, stackFrames[0].failedLineNumber);
                    Assert.Equal(23, stackFrames[1].failedLineNumber);
                    Assert.Equal(52, stackFrames[2].failedLineNumber);
                }
            }

            {
                StackTraceCollector sc = new StackTraceCollector(sim);
                ICallable op = sim.Get<ICallable, GenericCtlFail1>();
                try
                {
                    QVoid res = op.Apply<QVoid>(QVoid.Instance);
                }
                catch (ExecutionFailException)
                {
                    StackFrame[] stackFrames = sc.CallStack.ToArray();

                    Assert.Equal(3, stackFrames.Length);

                    Assert.Equal(namespacePrefix + "AlwaysFail", stackFrames[0].operation.FullName);
                    Assert.Equal(namespacePrefix + "GenericFail", stackFrames[1].operation.FullName);
                    Assert.Equal(namespacePrefix + "GenericCtlFail1", stackFrames[2].operation.FullName);

                    Assert.Equal(OperationFunctor.Controlled, stackFrames[0].operation.Variant);
                    Assert.Equal(OperationFunctor.Controlled, stackFrames[1].operation.Variant);
                    Assert.Equal(OperationFunctor.Body, stackFrames[2].operation.Variant);

                    Assert.Equal(5, stackFrames[0].failedLineNumber);
                    Assert.Equal(23, stackFrames[1].failedLineNumber);
                    Assert.Equal(56, stackFrames[2].failedLineNumber);
                }
            }
        }

        [Fact]
        public void PartialFail1Test()
        {
            ToffoliSimulator sim = new ToffoliSimulator();

            {
                StackTraceCollector sc = new StackTraceCollector(sim);
                ICallable op = sim.Get<ICallable, PartialFail1>();
                try
                {
                    QVoid res = op.Apply<QVoid>(QVoid.Instance);
                }
                catch (ExecutionFailException)
                {
                    StackFrame[] stackFrames = sc.CallStack.ToArray();

                    Assert.Equal(3, stackFrames.Length);

                    Assert.Equal(namespacePrefix + "AlwaysFail", stackFrames[0].operation.FullName);
                    Assert.Equal(namespacePrefix + "PartialFail", stackFrames[1].operation.FullName);
                    Assert.Equal(namespacePrefix + "PartialFail1", stackFrames[2].operation.FullName);

                    Assert.Equal(OperationFunctor.Body, stackFrames[0].operation.Variant);
                    Assert.Equal(OperationFunctor.Body, stackFrames[1].operation.Variant);
                    Assert.Equal(OperationFunctor.Body, stackFrames[2].operation.Variant);

                    Assert.Equal(7, stackFrames[0].failedLineNumber);
                    Assert.Equal(33, stackFrames[1].failedLineNumber);
                    Assert.Equal(38, stackFrames[2].failedLineNumber);
                }
            }

            {
                StackTraceCollector sc = new StackTraceCollector(sim);
                ICallable op = sim.Get<ICallable, PartialAdjFail1>();
                try
                {
                    QVoid res = op.Apply<QVoid>(QVoid.Instance);
                }
                catch (ExecutionFailException)
                {
                    StackFrame[] stackFrames = sc.CallStack.ToArray();

                    Assert.Equal(3, stackFrames.Length);

                    Assert.Equal(namespacePrefix + "AlwaysFail", stackFrames[0].operation.FullName);
                    Assert.Equal(namespacePrefix + "PartialFail", stackFrames[1].operation.FullName);
                    Assert.Equal(namespacePrefix + "PartialAdjFail1", stackFrames[2].operation.FullName);

                    Assert.Equal(OperationFunctor.Adjoint, stackFrames[0].operation.Variant);
                    Assert.Equal(OperationFunctor.Adjoint, stackFrames[1].operation.Variant);
                    Assert.Equal(OperationFunctor.Body, stackFrames[2].operation.Variant);

                    Assert.Equal(5, stackFrames[0].failedLineNumber);
                    Assert.Equal(31, stackFrames[1].failedLineNumber);
                    Assert.Equal(43, stackFrames[2].failedLineNumber);
                }
            }

            {
                StackTraceCollector sc = new StackTraceCollector(sim);
                ICallable op = sim.Get<ICallable, PartialCtlFail1>();
                try
                {
                    QVoid res = op.Apply<QVoid>(QVoid.Instance);
                }
                catch (ExecutionFailException)
                {
                    StackFrame[] stackFrames = sc.CallStack.ToArray();

                    Assert.Equal(3, stackFrames.Length);

                    Assert.Equal(namespacePrefix + "AlwaysFail", stackFrames[0].operation.FullName);
                    Assert.Equal(namespacePrefix + "PartialFail", stackFrames[1].operation.FullName);
                    Assert.Equal(namespacePrefix + "PartialCtlFail1", stackFrames[2].operation.FullName);

                    Assert.Equal(OperationFunctor.Controlled, stackFrames[0].operation.Variant);
                    Assert.Equal(OperationFunctor.Controlled, stackFrames[1].operation.Variant);
                    Assert.Equal(OperationFunctor.Body, stackFrames[2].operation.Variant);

                    Assert.Equal(5, stackFrames[0].failedLineNumber);
                    Assert.Equal(31, stackFrames[1].failedLineNumber);
                    Assert.Equal(48, stackFrames[2].failedLineNumber);
                }
            }
        }
    }
}