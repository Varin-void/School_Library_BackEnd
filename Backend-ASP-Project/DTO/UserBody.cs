﻿using Backend_ASP_Project.Models;

namespace Backend_ASP_Project.DTO
{
	public class UserBody
	{
		public int Id { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string ImagePath { get; set; } = string.Empty;
		public int IsDelete { get; set; }
		public virtual RoleBody? Role { get; set; } = new RoleBody();
		public virtual List<GroupBodyForUser>? Group { get; set; } = new List<GroupBodyForUser>();
	}
}
