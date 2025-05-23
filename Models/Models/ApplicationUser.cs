﻿using AutoGenerator;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LAHJAAPI.Models
{

    public class ApplicationUser : IdentityUser, ITModel
    {
        public string? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfileUrl { get; set; }

        [DataType(DataType.ImageUrl)]
        public string? Image { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public string? LastLoginIp { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        //TODO: add model gateway id to user and relation many to many 

        public string? ModelGatewayId { get; set; }
        public ICollection<ModelGateway>? ModelGateways { get; set; } = new List<ModelGateway>();


        public string? SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }

        [JsonIgnore]
        public ICollection<UserModelAi> UserModelAis { get; set; }
        [JsonIgnore]
        public ICollection<UserService> UserServices { get; set; }
        public ICollection<Request> Requests { get; set; } = [];
    }
}
