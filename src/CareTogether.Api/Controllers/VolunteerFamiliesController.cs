﻿using CareTogether.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareTogether.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/{organizationId:guid}/{locationId:guid}/[controller]")]
    public class VolunteerFamiliesController : ControllerBase
    {
        private readonly AuthorizationProvider authorizationProvider;

        public VolunteerFamiliesController(AuthorizationProvider authorizationProvider)
        {
            this.authorizationProvider = authorizationProvider;
        }


        public sealed record VolunteerFamily(Family Family /*TODO: Per-person & family-level volunteer records and policy evaluation results*/);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VolunteerFamily>>> Get(Guid organizationId, Guid locationId)
        {
            //TODO: Extract authorization provider logic into an authorization policy & claims provider.
            var authorizedUser = await authorizationProvider.AuthorizeAsync(organizationId, locationId, User);

            throw new NotImplementedException();
        }
    }
}
