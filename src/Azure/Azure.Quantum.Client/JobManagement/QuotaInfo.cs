﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.Quantum
{
    using Microsoft.Azure.Quantum.Client.Models;
    using Microsoft.Azure.Quantum.Utility;

    /// <summary>
    /// Wrapper for Microsoft.Azure.Quantum.Client.Models.Quota.
    /// </summary>
    public class QuotaInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuotaInfo"/> class.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="quota">The job details.</param>
        public QuotaInfo(IWorkspace workspace, Quota quota)
        {
            Ensure.NotNull(workspace, nameof(workspace));
            Ensure.NotNull(quota, nameof(quota));

            Workspace = workspace;
            Quota = quota;
        }

        /// <summary>
        /// Gets the workspace.
        /// </summary>
        public IWorkspace Workspace { get; private set; }

        /// <summary>
        /// Gets the quota information.
        /// </summary>
        public Quota Quota { get; private set; }
    }
}
