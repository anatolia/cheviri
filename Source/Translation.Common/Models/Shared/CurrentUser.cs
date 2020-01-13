﻿using StandardUtils.Models.Shared;

namespace Translation.Common.Models.Shared
{
    public class CurrentUser : BaseCurrentUser<CurrentOrganization>
    {
        public bool IsAdmin { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsActive { get; set; }
    }
}