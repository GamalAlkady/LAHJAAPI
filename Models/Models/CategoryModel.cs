﻿using AutoGenerator;
using System.ComponentModel.DataAnnotations;

namespace LAHJAAPI.Models
{
    public class CategoryModel : ITModel
    {
        [Key]
        public string Id { get; set; } = $"catm_{Guid.NewGuid():N}";

        [ToTranslation]
        public required string Name { get; set; }
        [ToTranslation]
        public string? Description { get; set; }


    }

}
