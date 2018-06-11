﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiGateway.Common.Constants;
using ApiGateway.Common.Exceptions;
using ApiGateway.Common.Extensions;
using ApiGateway.Common.Models;
using ApiGateway.Data.EFCore.Entity;
using ApiGateway.Data.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace ApiGateway.Data.EFCore.DataAccess
{
    public class KeyData: IKeyData
    {
        private readonly ApiGatewayContext _context;
        private readonly IStringLocalizer<KeyData> _localizer;
        private readonly ILogger<KeyData> _logger;

        public KeyData(ApiGatewayContext context, IStringLocalizer<KeyData> localizer, ILogger<KeyData> logger)
        {
            _context = context;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<KeyModel> Create(string ownerPublicKey, KeyModel model)
        {
            var entity = model.ToEntity();

            _context.Keys.Add(entity);
            await _context.SaveChangesAsync();

            return entity.ToModel();
        }

        public async Task<KeyModel> Update(string ownerPublicKey, KeyModel model)
        {
            var existing = await GetEntity(ownerPublicKey, model.Id);

            // Update properties
            existing.PublicKey = model.PublicKey;
            existing.IsDisabled = model.IsDisabled;
            existing.Properties = model.Properties.ToJson();
            existing.Type = model.Type;
            existing.ModifiedDate = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            return await Get(ownerPublicKey, model.Id);
        }

        public async Task Delete(string ownerPublicKey, string id)
        {
            var existing = await GetEntity(ownerPublicKey, id);

            _context.Keys.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<Key> GetEntity(string ownerPublicKey, string keyId)
        {
            var id = int.Parse(keyId);
            var entity = await _context.Keys.SingleOrDefaultAsync(x => x.OwnerKeyId == ownerPublicKey && x.Id == id);

            if (entity == null)
            {
                var msg = _localizer["No key found for the specified owner and Id"];
                throw new ItemNotFoundException(msg);
            }

            return entity;
        }

        public async Task<KeyModel> Get(string ownerPublicKey, string keyId)
        {
            var id = int.Parse(keyId);
            var entity = await _context.Keys.SingleOrDefaultAsync(x => x.OwnerKeyId == ownerPublicKey && x.Id == id);

            if (entity == null)
            {
                var msg = _localizer["No key found for the specified owner and Id"];
                throw new ItemNotFoundException(msg);
            }

            return entity.ToModel();
        }

        public async Task<KeyModel> GetByPublicKey(string publicKey)
        {
            var key = await _context.Keys.SingleOrDefaultAsync(x => x.PublicKey == publicKey);

            if (key == null)
            {
                var message = _localizer["Invalid Key"];
                throw new InvalidKeyException(message, HttpStatusCode.Unauthorized);
            }
            else
            {
                return key.ToModel();
            }
        } 
    }
}