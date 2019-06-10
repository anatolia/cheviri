﻿using System;
using NodaTime;
using Translation.Common.Models.Base;

namespace Translation.Common.Models.DataTransferObjects
{
    public class UserDto : BaseDto
    {
        public Guid OrganizationUid { get; set; }
        public string OrganizationName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSuperAdmin { get; set; }
        public Instant? LastLoggedInAt { get; set; }

        public Instant? InvitedAt { get; set; }
        public Guid? InvitedByUserUid { get; set; }
        public string InvitedByUserName { get; set; }

        public int LabelCount { get; set; }
        public int LabelTranslationCount { get; set; }
    }
}