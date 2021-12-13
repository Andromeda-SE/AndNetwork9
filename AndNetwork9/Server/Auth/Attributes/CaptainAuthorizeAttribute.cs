﻿using AndNetwork9.Shared;
using Microsoft.AspNetCore.Authorization;

namespace AndNetwork9.Server.Auth.Attributes;

public class CaptainAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthPass
{
    public bool Pass(Member member) => member.SquadPart is not null
                                       && member.SquadPartNumber == 0
                                       && member.SquadPart.Captain.Id == member.Id;
}