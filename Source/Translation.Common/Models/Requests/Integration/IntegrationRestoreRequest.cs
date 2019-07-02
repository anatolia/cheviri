﻿using System;

using Translation.Common.Models.Base;

namespace Translation.Common.Models.Requests.Integration
{
    public class IntegrationRestoreRequest : BaseAuthenticatedRequest
    {
        public Guid IntegrationUid { get; set; }
        public int Revision { get; set; }

        public IntegrationRestoreRequest(long currentUserId, Guid integrationUid, int revision) : base(currentUserId)
        {
            IntegrationUid = integrationUid;
            Revision = revision;
        }
    }
}