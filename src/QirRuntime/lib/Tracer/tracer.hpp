// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include <memory>
#include <unordered_map>
#include <vector>

#include "CoreTypes.hpp"
#include "QuantumApi_I.hpp"

namespace Microsoft
{
namespace Quantum
{
    using OpId = int32_t;
    using Time = int32_t;
    using Duration = int32_t;

    constexpr long INVALID = -1;

    /*==================================================================================================================
        Layer
    ==================================================================================================================*/
    struct Layer
    {
        // Width of the layer on the time axis.
        const Duration duration;

        // Start time of the layer.
        const Time startTime;

        std::unordered_map<OpId, int32_t /*count of the op with this id*/> operations;

        Layer(Duration duration, Time startTime)
            : duration(duration)
            , startTime(startTime)
        {
        }
    };

    /*==================================================================================================================
        QubitState
    ==================================================================================================================*/
    struct QubitState
    {
        // The last layer this qubit was used in, `INVALID` means the qubit haven't been used yet in any
        // operations of non-zero duration.
        int layer = INVALID;

        // For layers with duration greater than one, multiple operations might fit on the same qubit, if the operations
        // are short. `lastUsedTime` is the end time of the last operation, the qubit participated it.
        Time lastUsedTime = 0;

        std::vector<OpId> pendingZeroOps;
    };

    /*==================================================================================================================
        The tracer implements resource estimation. See readme in this folder for details.
    ==================================================================================================================*/
    class CTracer : public ISimulator
    {
        // Start with no reuse of qubits.
        std::vector<QubitState> qubits;

        // The preferred duration of a layer.
        int preferredLayerDuration = INVALID;

        // The index into the vector is treated as implicit id of the layer.
        std::vector<Layer> metricsByLayer;

      public:
        // -------------------------------------------------------------------------------------------------------------
        // ISimulator interface
        // -------------------------------------------------------------------------------------------------------------
        IQuantumGateSet* AsQuantumGateSet() override
        {
            return nullptr;
        }
        IDiagnostics* AsDiagnostics() override
        {
            return nullptr;
        }
        Qubit AllocateQubit() override
        {
            size_t qubit = qubits.size();
            qubits.push_back({});
            return reinterpret_cast<Qubit>(qubit);
        }
        void ReleaseQubit(Qubit qubit) override
        {
            // nothing for now
        }
        std::string QubitToString(Qubit qubit) override
        {
            throw std::logic_error("not_implemented");
        }
        Result M(Qubit target) override
        {
            throw std::logic_error("not_implemented");
        }
        Result Measure(long numBases, PauliId bases[], long numTargets, Qubit targets[]) override
        {
            throw std::logic_error("not_implemented");
        }
        void ReleaseResult(Result result) override
        {
            throw std::logic_error("not_implemented");
        }
        bool AreEqualResults(Result r1, Result r2) override
        {
            throw std::logic_error("not_implemented");
        }
        ResultValue GetResultValue(Result result) override
        {
            throw std::logic_error("not_implemented");
        }
        Result UseZero() override
        {
            return reinterpret_cast<Result>(0);
        }
        Result UseOne() override
        {
            return reinterpret_cast<Result>(1);
        }

        // -------------------------------------------------------------------------------------------------------------
        // Instead of implementing IQuantumGateSet, the tracer provides 'tracing-by-id' methods. The QIR generation
        // should translate all intrinsics to invoke these methods.
        // -------------------------------------------------------------------------------------------------------------
        void TraceSingleQubitOp(OpId id, Duration duration, Qubit target);
        void TraceControlledSingleQubitOp(OpId id, Duration duration, int64_t nCtrls, Qubit* ctls, Qubit target);
        void TraceMultiQubitOp(OpId id, Duration duration, int64_t nTargets, Qubit* targets);
        void TraceControlledMultiQubitOp(
            OpId id,
            Duration duration,
            int64_t nCtrls,
            Qubit* ctls,
            int64_t nTargets,
            Qubit* targets);

        // -------------------------------------------------------------------------------------------------------------
        // Configuring the tracer and getting data back from it.
        // -------------------------------------------------------------------------------------------------------------
        void SetPreferredLayerDuration(int dur)
        {
            this->preferredLayerDuration = dur;
        }

        // Temporary method for initial testing
        // TODO: replace with a safer accessor
        const std::vector<Layer>& UseLayers()
        {
            return this->metricsByLayer;
        }
    };

    std::shared_ptr<CTracer> CreateTracer();

} // namespace Quantum
} // namespace Microsoft