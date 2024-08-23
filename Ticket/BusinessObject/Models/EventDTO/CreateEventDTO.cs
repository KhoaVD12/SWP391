﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace BusinessObject.Models.EventDTO
{
    public class CreateEventDTO
    {
        [Required(ErrorMessage = "Title is required.")]
        [SwaggerSchema(Description = "Title of the event")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [SwaggerSchema(Description = "Start date of the event")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [SwaggerSchema(Description = "End date of the event")]
        public DateTime EndDate { get; set; }

        [SwaggerSchema(Description = "ID of the event organizer")]
        public int OrganizerId { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [SwaggerSchema(Description = "Description of the event")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Venue ID is required.")]
        [SwaggerSchema(Description = "ID of the venue where the event will be held")]
        public int VenueId { get; set; }

        [SwaggerSchema(Description = "URL of the image for the event banner")]
        public string? ImageUrl { get; set; }
    }
}