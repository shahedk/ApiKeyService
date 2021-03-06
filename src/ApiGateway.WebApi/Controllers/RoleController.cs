﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ApiGateway.Client;
using ApiGateway.Common.Exceptions;
using ApiGateway.Common.Models;
using ApiGateway.Core;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("sys/Role")]
    public class RoleController : ApiControllerBase
    {
        private readonly IRoleManager _manager;

        public RoleController( IRoleManager manager, IApiRequestHelper apiRequestHelper):base(apiRequestHelper)
        {
            _manager = manager;
        }

        [HttpGet("/sys/role-summary/")]
        public async Task<IList<RoleSummaryModel>> GetSummary()
        {
            try
            {
                return await _manager.GetAllSummary(ApiKey);
            }
            catch (ApiGatewayException e)
            {
                if (e.ErrorCode == HttpStatusCode.NotFound)
                {
                    Response.StatusCode = (int)e.ErrorCode;
                    
                }
            }
            return null;
        }
        
        [HttpGet]
        public async Task<IList<RoleModel>> Get()
        {
            try
            {
                return await _manager.GetAll(ApiKey);
            }
            catch (ApiGatewayException e)
            {
                if (e.ErrorCode == HttpStatusCode.NotFound)
                {
                    Response.StatusCode = (int)e.ErrorCode;
                    
                }
            }
            return null;
        }
        
        // GET: api/Role/5
        [HttpGet("/sys/role-detail/{id}")]
        public async Task<RoleModel> Get(string id)
        {
            return await _manager.Get(ApiKey, id);
        }
        
        // POST: api/Role
        [HttpPost]
        public async Task<RoleModel> Post([FromBody]RoleModel model)
        {
            return await _manager.Create(ApiKey, model);
        }

        [Route("/sys/AddKeyInRole")]
        [HttpPost]
        public async Task AddKeyInRole(string key, string roleId)
        {
            await _manager.AddKeyInRole(ApiKey, roleId, key);
        }

        [Route("/sys/RemoveKeyFromRole")]
        [HttpPost]
        public async Task RemoveKeyFromRole(string key, string roleId)
        {
            await _manager.RemoveKeyFromRole(ApiKey, roleId, key);
        }
        
        [Route("/sys/AddApiInRole")]
        [HttpPost]
        public async Task AddApiInRole(string apiId, string roleId)
        {
            await _manager.AddApiInRole(ApiKey, roleId, apiId);
        }

        [Route("/sys/RemoveApiFromRole")]
        [HttpPost]
        public async Task RemoveApiFromRole(string apiId, string roleId)
        {
            await _manager.RemoveApiFromRole(ApiKey, roleId, apiId);
        }

        // PUT: api/Role/5
        [HttpPut("{id}")]
        public async Task<RoleModel> Put(string id, [FromBody]RoleModel model)
        {
            model.Id = id;
            return await _manager.Update(ApiKey, model);
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _manager.Delete(ApiKey, id);
        }
    }
}
