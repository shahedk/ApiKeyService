﻿using System.ComponentModel.DataAnnotations;
using System.IO;
using ApiGateway.Common.Constants;

namespace ApiGateway.Data.EFCore.Entity
{
    public class Key : EntityBase
    {
        [Required]
        [StringLength(50)]
        public string PublicKey { get; set; }
        
        [StringLength(500)]
        public string Tags { get; set; }

        [Required]
        public bool IsDisabled { get; set; }
        
        private string _type = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Type
        {
            get => _type;
            set
            {
                if (ApiKeyTypes.IsValid(value))
                {
                    _type = value;
                }
                else
                {
                    var errorMessage = "Invalid data. Valid types are: " + string.Join(", ", ApiKeyTypes.ToList());
                    throw new InvalidDataException(errorMessage);
                }
            }
        }

        [StringLength(1000)]
        public string Properties { get; set; }
    }
}