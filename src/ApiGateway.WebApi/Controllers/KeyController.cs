﻿using System.Collections.Generic;
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
    [Route("sys/Key")]
    public class KeyController : ApiControllerBase
    {
        private readonly IKeyManager _manager;

        public KeyController( IKeyManager manager, IApiRequestHelper apiRequestHelper):base(apiRequestHelper)
        {
            _manager = manager;
        }

        [HttpGet("/sys/key-summary/")]
        public async Task<IList<KeySummaryModel>> GetSummary()
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
        public async Task<IList<KeyModel>> Get()
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
        
        [HttpGet("/sys/key-detail/{id}")]
        public async Task<KeyModel> Get(string id)
        {
            return await _manager.Get(ApiKey, id);
        }
        
        [HttpPost]
        public async Task<KeyModel> Post([FromBody]KeyModelLite liteModel)
        {
            var model = new KeyModel{ Type =  liteModel.Type, PublicKey =  liteModel.PublicKey};
            
            var keyModel = await _manager.Create(ApiKey, model);

            return keyModel;
        }
        
        
        [HttpPost("/sys/regenerate-secret1/{publicKey}")]
        public async Task<KeyModel> ReGenerateSecret1(string publicKey)
        {
            var keyModel = await _manager.ReGenerateSecret1(ApiKey, publicKey);

            return keyModel;
        }
        
        
        [HttpPost("/sys/regenerate-secret2/{publicKey}")]
        public async Task<KeyModel> ReGenerateSecret2(string publicKey)
        {
            var keyModel = await _manager.ReGenerateSecret2(ApiKey, publicKey);

            return keyModel;
        }
        
        [HttpPost("/sys/key/regenerate-secret3/{publicKey}")]
        public async Task<KeyModel> ReGenerateSecret3(string publicKey)
        {
            var keyModel = await _manager.ReGenerateSecret3(ApiKey, publicKey);

            return keyModel;
        }
        
        [HttpPut("{id}")]
        public async Task<KeyModel> Put(string id, [FromBody]KeyModel model)
        {
            model.Id = id;
            var keyModel = await _manager.Update(ApiKey, model);

            return keyModel;
        }
        
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _manager.Delete(ApiKey, id);
        }
    }
}
