using Microsoft.EntityFrameworkCore;
using Backend_ASP_Project.Data;
using Backend_ASP_Project.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Backend_ASP_Project.Tool;
using Backend_ASP_Project.DTO;
using Microsoft.AspNetCore.Authorization;
namespace Backend_ASP_Project.Endpoint;

public static class RoleEndpoints
{
	public static void MapRoleEndpoints(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/Role").WithTags(nameof(Role));
		Damon_Tool damon_Tool = new Damon_Tool();


		group.MapGet("/", (Backend_ASP_ProjectContext db) =>
		{
			List<RoleBody> body = damon_Tool.MapRoleBody(db.Roles.ToList());
			return body;
		})
		.WithName("GetAllRoles")
		.WithOpenApi();
	}
}
