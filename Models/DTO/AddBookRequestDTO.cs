﻿using System.ComponentModel.DataAnnotations;

namespace WebAPI_simple.Models.DTO
{
    public class AddBookRequestDTO
    {
        [Required]
        [MinLength(10, ErrorMessage = "Tiêu đề phải có ít nhất 10 ký tự")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public bool IsRead { get; set; }

        public DateTime? DateRead { get; set; }

        [Range(0, 5, ErrorMessage = "Điểm đánh giá phải từ 0 đến 5")]
        public int? Rate { get; set; }

        public string? Genre { get; set; }

        public string? CoverUrl { get; set; }

        public DateTime DateAdded { get; set; }

        public int PublisherID { get; set; }

        public List<int> AuthorIds { get; set; } = new();
    }
}