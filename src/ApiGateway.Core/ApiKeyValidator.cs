﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiGateway.Common.Constants;
using ApiGateway.Common.Exceptions;
using ApiGateway.Common.Extensions;
using ApiGateway.Common.Models;
using ApiGateway.Core.KeyValidators;
using ApiGateway.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace ApiGateway.Core
{
    public class ApiKeyValidator : IApiKeyValidator
    {
        private readonly KeySecretValidator _keySecretValidator;
        private readonly IStringLocalizer<ApiKeyValidator> _localizer;
        private readonly ILogger<ApiKeyValidator> _logger;
        private readonly IApiManager _apiManager;
        private readonly IKeyManager _keyManager;
        private readonly IServiceManager _serviceManager;

        public ApiKeyValidator(KeySecretValidator keySecretValidator, IStringLocalizer<ApiKeyValidator> localizer, ILogger<ApiKeyValidator> logger, IApiManager apiManager, IKeyManager keyManager, IServiceManager serviceManager)
        {
            _keySecretValidator = keySecretValidator;
            _localizer = localizer;
            _logger = logger;
            _apiManager = apiManager;
            _keyManager = keyManager;
            _serviceManager = serviceManager;
        }


        public async Task<KeyValidationResult> IsValid(KeyChallenge keyChallenge, string httpMethod, string serviceName, string apiNameOrUrl)
        {
            // Validate client key
            var clientKeyResult = await IsKeyValid(keyChallenge);
            var publicKey = keyChallenge.Properties[ApiKeyPropertyNames.PublicKey];

            if (!clientKeyResult.IsValid)
            {
                // Client key validation failed
                return new KeyValidationResult
                {
                    InnerValidationResult = clientKeyResult,
                    IsValid = false,
                    Message = _localizer["Client key validation failed"]
                };
            }

            // Key validation passed. Now check if client has the right permission to access the api/url
            
            var result = new KeyValidationResult();

            ServiceModel service = null;
            if (serviceName.ToLower() == AppConstants.SysApiServiceName.ToLower())
            {
                // Its a core service (eg. manage key, service, role, etc. All active clients can use this service) 
                service = await _serviceManager.GetSysService();
            }
            else
            {
                service = await _serviceManager.GetByName(publicKey, serviceName);
            }

            if (service == null)
            {
                result.Message = _localizer["Service not found. Service name or api key is invalid."];
                result.IsValid = false;
                return result;
            }

            ApiModel api;
            if(serviceName.ToLower() == AppConstants.SysApiServiceName.ToLower())
            {
                // System API
                api = await _apiManager.GetByApiName(service.Id, httpMethod, apiNameOrUrl);
            }
            else
            {
                // User API
                api = await _apiManager.GetByApiName(publicKey, service.Id, httpMethod, apiNameOrUrl);
            }

            if (api == null && !string.IsNullOrEmpty(apiNameOrUrl))
            {
                api = await _apiManager.GetByApiUrl(publicKey, service.Id, httpMethod, apiNameOrUrl);
            }
            
            if (api == null)
            {
                api = await _apiManager.GetByApiName(publicKey, service.Id, httpMethod, string.Empty);
            }
            
            if (api == null)
            {
                result.Message = _localizer["Api not found"];
                result.IsValid = false;
                return result;
            }

            var clientKeyWithRoles = await _keyManager.GetByPublicKey(publicKey);
            foreach (var role in api.Roles)
            {
                result.IsValid = clientKeyWithRoles.Roles.SingleOrDefault(x => x.Id == role.Id && !role.IsDisabled) != null;
                if (result.IsValid)
                {
                    break;
                }
            }

            if (result.IsValid == false)
            {
                result.Message = _localizer["Access denied."];
            }

            
            result.ApiId = api.Id;
            result.KeyId = clientKeyResult.KeyId;
            result.ServiceId = api.ServiceId;
            
            return result;

        }

        private async Task<KeyValidationResult> IsKeyValid(KeyChallenge key)
        {
            KeyValidationResult result;
            if (key.Type == ApiKeyTypes.ClientSecret)
            {
                result = await _keySecretValidator.IsValid(key.Properties[ApiKeyPropertyNames.PublicKey], key.Properties[ApiKeyPropertyNames.ClientSecret]);
            }
            else
            {
                var msg = _localizer["Unknown key type."];
                throw new InvalidKeyException(msg, HttpStatusCode.Unauthorized);
            }

            return result;
        }
    }
}