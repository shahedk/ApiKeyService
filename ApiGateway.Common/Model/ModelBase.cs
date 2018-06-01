﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Common.Model
{
    public class ModelBase
    {
        public string Id { get; set; }

        [Required]
        [StringLength(32)]
        public string OwnerKeyId { get; set; }


        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}