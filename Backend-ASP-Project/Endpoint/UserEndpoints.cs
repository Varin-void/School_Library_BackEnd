using Microsoft.EntityFrameworkCore;
using Backend_ASP_Project.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Backend_ASP_Project.Models;
using Backend_ASP_Project.Tool;
using Microsoft.Extensions.Primitives;
using static Backend_ASP_Project.Endpoint.UserEndpoints;
using Backend_ASP_Project.Models.Users.Get.By_Id;
using Backend_ASP_Project.Models.Users.Create;
using Backend_ASP_Project.DTO;
using Backend_ASP_Project.Models.Users.Update.Params;
using Backend_ASP_Project.Author;
using Microsoft.AspNetCore.Mvc;
namespace Backend_ASP_Project.Endpoint;

public static class UserEndpoints
{
	public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/FT_SD_A_3/User").WithTags(nameof(User));
		Damon_Tool damon_Tool = new Damon_Tool();

		group.MapPost("/ChangePassword", async Task<Response> (ChangePasswordParams Params, Backend_ASP_ProjectContext db) =>
		{
			Response response = new Response();
			response.Status = 0;
			if (damon_Tool.CheckValidate(typeof(ChangePasswordParams), Params) == "")
			{
				var token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (token != null)
				{
					if (db.Users.FirstOrDefault(user => user.Id == token.UserId && user.IsDelete == 0) != null)
					{
						if (Params.ConfirmNewPassword == Params.NewPassword)
						{
							var user = db.Users.Where(user => user.Id == token.UserId).FirstOrDefault(user => user.IsDelete == 0 && user.Password == Params.OldPassword);
							if (user != null)
							{
								user.Password = Params.NewPassword;
								await db.SaveChangesAsync();
								response.Status = 1;
								response.Message = "Success";
							}
							else
							{
								response.Message = "Old Password is incorrect";
							}
						}
						else
						{
							response.Message = "Confirm New Password and New Password are not match";
						}
					}
					else
					{
						response.Message = "Invalid Token!";
					}
				}
				else
				{
					response.Message = "Invalid Token!";
				}
			}
			else
			{
				response.Message = damon_Tool.CheckValidate(typeof(ChangePasswordParams), Params);
			}
			return response;
		})
		.WithName("ChangePassword")
		.WithOpenApi();


		group.MapPost("/Get/Summary", (TokenParams Params, Backend_ASP_ProjectContext db) =>
		{
			SummaryResponse response = new SummaryResponse();
			response.Status = 0;
			response.Data = null;
			if (damon_Tool.CheckValidate(typeof(TokenParams), Params) == "")
			{
				var token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (token != null)
				{
					if (db.Users.Where(user => user.Id == token.UserId).FirstOrDefault(user => user.IsDelete == 0 && user.Role != null && user.Role.Name == "Librarian") != null)
					{
						response.Status = 1;
						response.Message = "Success";
						response.Data = new Summary();
						response.Data.Teachers = db.Users.Where(user => user.IsDelete == 0 && user.Role != null && user.Role.Name == "Teacher").ToList().Count;
						response.Data.Students = db.Users.Where(user => user.IsDelete == 0 && user.Role != null && user.Role.Name == "Student").ToList().Count;
						response.Data.Books = db.Books.Where(book => book.IsDelete == 0).ToList().Count;
						response.Data.Groups = db.Groups.Where(groups => groups.IsDelete == 0).ToList().Count;
						var book = db.Books.Where(book => book.IsDelete == 0).Select(book => book.DownloadCount).ToList();
						for (int i = 0; i < book.Count; i++)
						{
							response.Data.Downloadeds += book[i];
						}
					}
					else
					{
						response.Message = "Invalid Token";
					}
				}
				else
				{
					response.Message = "Invalid Token";
				}
			}
			else
			{
				response.Message = damon_Tool.CheckValidate(typeof(TokenParams), Params);
			}
			return response;
		})
		.WithName("GetSummary")
		.WithOpenApi();

		group.MapPost("/Get", (TokenParams Params, Backend_ASP_ProjectContext db) =>
		{
			GetUserResponse listUserResponse = new GetUserResponse();
			listUserResponse.Status = 0;
			listUserResponse.data = null;
			if (damon_Tool.CheckValidate(typeof(TokenParams), Params) != "")
			{
				listUserResponse.Message = damon_Tool.CheckValidate(typeof(TokenParams), Params);
				return listUserResponse;
			}

			var expired = DateTime.Now;
			var checkToken = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > expired);
			if (checkToken != null)
			{
				var check = db.Users.FirstOrDefault(item => item.IsDelete == 0 && item.Id == checkToken.UserId);
				if (check != null)
				{
					listUserResponse.Status = 1;
					listUserResponse.Message = "Success";
					UserBody data = damon_Tool.MapUserBody(db.Users.Include(user => user.Role).Include(user => user.Group.Where(group2 => db.Groups.Where(group => group.IsDelete == 0).Select(group => group.Id).Contains(group2.Id))).First(user => user.Id == checkToken.UserId));
					listUserResponse.data = data;
					return listUserResponse;
				}
			}
			listUserResponse.Message = "Invalid Token!";
			return listUserResponse;
		})
		.WithName("GetUsers")
		.WithOpenApi();

		group.MapPost("/GetList", (SearchParamsAllUser Params, Backend_ASP_ProjectContext db) =>
		{
			GetListUserResponse response = new GetListUserResponse();
			response.Status = 0;
			response.data = null;
			if (Params.apiToken != "")
			{
				var check = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (check != null)
				{
					if (db.Users.Where(user => user.Id == check.UserId).FirstOrDefault(user => user.IsDelete == 0 && user.Role != null && user.Role.Name == "Librarian") != null)
					{
						var Datas = db.Users.Include(user => user.Role).Where(user => user.IsDelete == 0 && user.Username.ToUpper().Contains(Params.Username.ToUpper()));
						if (Params.RoleId != null)
						{
							Datas = Datas.Where(data => data.Role != null && data.Role.Id.ToString() == Params.RoleId);
						}
						response.Status = 1;
						response.Message = "Success";
						response.data = damon_Tool.MapUserBody([.. Datas]);
					}
					else
					{
						response.Message = "Only Librarian have access to this Fetch!";
					}
				}
				else
				{
					response.Message = "Invalid Token!";
				}
			}
			else
			{
				response.Message = "Params Must have apiToken";
			}
			return response;
		})
		.WithName("GetUsersList")
		.WithOpenApi();

		group.MapPost("/Get/All", (TokenParams Params, Backend_ASP_ProjectContext db) =>
		{
			GetListUserResponse listUserResponse = new GetListUserResponse();
			listUserResponse.Status = 0;
			listUserResponse.data = null;
			if (damon_Tool.CheckValidate(typeof(TokenParams), Params) != "")
			{
				listUserResponse.Message = damon_Tool.CheckValidate(typeof(TokenParams), Params);
				return listUserResponse;
			}

			var check = db.Users.FirstOrDefault(item => item.IsDelete == 0 && db.Tokens.First(token => token.Token == Params.apiToken).UserId == item.Id && item.Role != null && item.Role.Name == "Librarian");
			if (check != null)
			{
				listUserResponse.Status = 1;
				listUserResponse.Message = "Success";
				List<UserBody> data = damon_Tool.MapUserBody(db.Users.Where(user => user.IsDelete == 0).Include(user => user.Group).Include(User => User.Role).ToList());
				listUserResponse.data = data;
				return listUserResponse;
			}
			listUserResponse.Message = "Invalid Token!";
			return listUserResponse;
		})
		.WithName("GetAllUsers")
		.WithOpenApi();

		group.MapPost("/Get/Teacher", (TokenParams Params, Backend_ASP_ProjectContext db) =>
		{
			GetListUserResponse listUserResponse = new GetListUserResponse();
			listUserResponse.Status = 0;
			listUserResponse.data = null;
			if (damon_Tool.CheckValidate(typeof(TokenParams), Params) != "")
			{
				listUserResponse.Message = damon_Tool.CheckValidate(typeof(TokenParams), Params);
				return listUserResponse;
			}
			var check = db.Users.FirstOrDefault(item => item.IsDelete == 0 && db.Tokens.First(token => token.Token == Params.apiToken).UserId == item.Id && item.Role != null && item.Role.Name == "Librarian");
			if (check != null)
			{
				listUserResponse.Status = 1;
				listUserResponse.Message = "Success";
				List<UserBody> data = damon_Tool.MapUserBody(db.Users.Where(user => user.IsDelete == 0 && user.Role != null && user.Role.Name == "Teacher").Include(User => User.Role).Include(user => user.Group).ToList());
				listUserResponse.data = data;
				return listUserResponse;
			}
			listUserResponse.Message = "Invalid Token!";
			return listUserResponse;
		})
		.WithName("GetAllTeachers")
		.WithOpenApi();

		group.MapPost("/Get/Student", (TokenParams Params, Backend_ASP_ProjectContext db) =>
		{
			GetListUserResponse listUserResponse = new GetListUserResponse();
			listUserResponse.Status = 0;
			listUserResponse.data = null;
			if (damon_Tool.CheckValidate(typeof(TokenParams), Params) != "")
			{
				listUserResponse.Message = damon_Tool.CheckValidate(typeof(TokenParams), Params);
				return listUserResponse;
			}

			var check = db.Users.FirstOrDefault(item => item.IsDelete == 0 && db.Tokens.First(token => token.Token == Params.apiToken).UserId == item.Id && item.Role != null && (item.Role.Name == "Librarian" || item.Role.Name == "Teacher"));
			if (check != null)
			{
				listUserResponse.Status = 1;
				listUserResponse.Message = "Success";
				List<UserBody> data = damon_Tool.MapUserBody(db.Users.Where(user => user.IsDelete == 0 && user.Role != null && user.Role.Name == "Student").Include(User => User.Role).Include(user => user.Group).ToList());
				listUserResponse.data = data;
				return listUserResponse;
			}
			listUserResponse.Message = "Invalid Token!";
			return listUserResponse;
		})
		.WithName("GetAllStudents")
		.WithOpenApi();


		//Completed
		group.MapPost("/Create", ([FromForm] CreateUserParams Params, Backend_ASP_ProjectContext db) =>
		{
			CreateUserResponse createUserResponse = new CreateUserResponse();
			createUserResponse.Status = 0;
			if (damon_Tool.CheckValidate(typeof(CreateUserParams), Params) != "")
			{
				createUserResponse.Message = damon_Tool.CheckValidate(typeof(CreateUserParams), Params);
				return createUserResponse;
			}

			if (Params.Image == null)
			{
				createUserResponse.Message = "Params must have Image";
				return createUserResponse;
			}
			else
			{
				var file = new FileInfo(Params.Image.FileName);
				var exte = file.Extension;
				if (exte.Contains(".png") || exte.Contains(".pjp") || exte.Contains(".jpg") || exte.Contains(".pjpeg") || exte.Contains(".jpeg") || exte.Contains(".jfif"))
				{
					var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
					if (Token != null)
					{
						var check = db.Users.FirstOrDefault(item => item.IsDelete == 0 && Token.UserId == item.Id && item.Role != null && item.Role.Name == "Librarian");
						if (check != null)
						{
							if (Params.Password == Params.ConfirmPassword)
							{
								if (db.Users.FirstOrDefault(user => user.Username == Params.Username && user.IsDelete == 0) == null)
								{
									var CheckRole = db.Roles.FirstOrDefault(item => item.Id.ToString() == Params.RoleId);
									if (CheckRole != null)
									{
										createUserResponse.Status = 1;
										createUserResponse.Message = "Submitted Successfully";
										User New_User = new User()
										{
											Username = Params.Username,
											Password = Params.Password,
											IsDelete = 0,
										};
										New_User.ImagePath = damon_Tool.CopyImage(Params.Image, "User");
										New_User.Role = CheckRole;
										db.Users.Add(New_User);
										db.SaveChanges();
									}
									else
									{
										createUserResponse.Message = "Roleid invalid";
									}
								}
								else
								{
									createUserResponse.Message = "Username already taken";
								}
							}
							else
							{
								createUserResponse.Message = "Confirm Password and Password not match";
							}
						}
						else
						{
							createUserResponse.Message = "User Token Invalid";
						}
					}
					else
					{
						createUserResponse.Message = "User Token Invalid";
					}
				}
				else
				{
					createUserResponse.Message = "Image file does not accpet " + exte + " type of file";
				}
			}
			return createUserResponse;
		})
		.DisableAntiforgery()
		.WithName("CreateUsers")
		.WithOpenApi();


		//Completed
		group.MapPost("/Get/Id", (GetByIdUserParams GetByIdUserParams, Backend_ASP_ProjectContext db) =>
		{
			GetByIdUserResponse getByIdUserResponse = new GetByIdUserResponse();
			getByIdUserResponse.Status = 0;
			getByIdUserResponse.User = null;
			if (damon_Tool.CheckValidate(typeof(GetByIdUserParams), GetByIdUserParams) != "")
			{
				getByIdUserResponse.Message = damon_Tool.CheckValidate(typeof(GetByIdUserParams), getByIdUserResponse);
				return getByIdUserResponse;
			}
			var Token = db.Tokens.FirstOrDefault(token => token.Token == GetByIdUserParams.apiToken && token.Expired > DateTime.Now);
			if (Token != null)
			{
				var check = db.Users.FirstOrDefault(User => User.TokenId == Token.Id && User.IsDelete == 0 && User.Role != null && User.Role.Name == "Librarian");
				if (check != null)
				{
					check = db.Users.FirstOrDefault(User => User.Id == GetByIdUserParams.Id && User.IsDelete == 0);
					if (check != null)
					{
						getByIdUserResponse.Status = 1;
						getByIdUserResponse.Message = "Success";
						getByIdUserResponse.User = damon_Tool.MapUserBody(db.Users.Include(user => user.Role).Include(user => user.Group).First(user => user.Id == GetByIdUserParams.Id && user.IsDelete == 0));
					}
					else
					{
						getByIdUserResponse.Message = "User Id not Found";
					}
				}
				else
				{
					getByIdUserResponse.Message = "Invalid userToken";
				}
			}
			else
			{
				getByIdUserResponse.Message = "Invalid userToken";
			}
			return getByIdUserResponse;
		})
		.WithName("GetUserById")
		.WithOpenApi();

		//Completed
		group.MapPost("/Register", (string Username, Backend_ASP_ProjectContext db) =>
		{
			var checkUsername = db.Users.FirstOrDefault(item => item.Username == Username && item.IsDelete == 0);
			PostRegisterResponse postRegisterResponse = new PostRegisterResponse();
			postRegisterResponse.Status = 0;
			postRegisterResponse.data = null;

			if (Username == "")
			{
				postRegisterResponse.Message = "Params must have Username";
				return postRegisterResponse;
			}
			if (checkUsername == null)
			{
				//if (Username.Password == Username.ConfirmPassword)
				//{
				if (db.Roles.Select(role => role).ToList().Count == 0)
				{
					var role = new Role();
					role.Name = "Librarian";
					db.Roles.Add(role);
					var role2 = new Role();
					role2.Name = "Teacher";
					db.Roles.Add(role2);
					var role3 = new Role();
					role3.Name = "Student";
					db.Roles.Add(role3);
					db.SaveChangesAsync();
				}
				User _user = new()
				{
					Username = Username,
					Password = "123",
					TokenId = null,
					IsDelete = 0,
					ImagePath = "/Asset/Images/User/Damon.jpg"
				};
				postRegisterResponse.Status = 1;
				postRegisterResponse.Message = "Success";
				postRegisterResponse.data = new RegisterTokenUser();
				postRegisterResponse.data.User.Password = _user.Password;
				postRegisterResponse.data.User.Username = _user.Username;

				postRegisterResponse.data.RoleBody = damon_Tool.MapRoleBody(db.Roles.Single(role => role.Name == "Librarian"));

				_user.Role = db.Roles.Single(role => role.Name == "Librarian");
				db.Users.Add(_user);
				db.SaveChanges();
				return postRegisterResponse;
			}
			else
			{
				postRegisterResponse.Message = "Username is already taken!";
				return postRegisterResponse;
			}
		})
		.WithName("RegisterLibrarian")
		.WithOpenApi();

		//Completed
		group.MapPost("/Login", (Login user, Backend_ASP_ProjectContext db) =>
		{
			PostLoginResponse postLoginResponse = new PostLoginResponse();
			postLoginResponse.Status = 0;
			postLoginResponse.data = null;

			if (damon_Tool.CheckValidate(typeof(Login), user) != "")
			{
				postLoginResponse.Message = damon_Tool.CheckValidate(typeof(Login), user);
				return postLoginResponse;
			}
			var check = db.Users.FirstOrDefault(item => item.IsDelete == 0 && item.Username == user.Username);
			if (check != null)
			{
				check = db.Users.Include(user => user.Role).FirstOrDefault(item => item.IsDelete == 0 && item.Username == user.Username && item.Password == user.Password);
				if (check != null)
				{
					var LoginTokenUser = check;
					postLoginResponse.Status = 1;
					postLoginResponse.Message = "Success";
					var Token = damon_Tool.GenerateNewToken(user.Username, user.Password);

					if (check.TokenId == null)
					{
						Token.UserId = check.Id;
						db.Tokens.Add(Token);
						db.SaveChangesAsync();
						var token = db.Tokens.First(Token => Token.UserId == check.Id);
						check.TokenId = token.Id;
						db.SaveChangesAsync();
					}
					else
					{
						var token = db.Tokens.First(Token => Token.Id == check.TokenId);
						token.Token = Token.Token;
						token.Expired = Token.Expired;
						db.SaveChangesAsync();
					}
					postLoginResponse.data = damon_Tool.MapUserBody(check);
					postLoginResponse.Token = Token.Token;
				}
				else
				{
					postLoginResponse.Message = "Incorrect Password!";
				}
			}
			else
			{
				postLoginResponse.Message = "User not Found!";
			}
			return postLoginResponse;
		})
		.WithName("LoginUser")
		.WithOpenApi();

		group.MapPost("/Delete", (Delete Params, Backend_ASP_ProjectContext db) =>
		{
			Response response = new Response();
			response.Status = 0;
			if (damon_Tool.CheckValidate(typeof(Delete), Params) == "")
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (Token != null)
				{
					var Librarian = db.Users.FirstOrDefault(user => user.TokenId == Token.Id && user.IsDelete == 0 && user.Role != null && user.Role.Name == "Librarian");
					if (Librarian != null)
					{
						var User = db.Users.FirstOrDefault(user => user.Id.ToString() == Params.Id && user.IsDelete == 0);
						if (User != null)
						{
							if (User.Id != Librarian.Id)
							{
								User.IsDelete = 1;
								db.SaveChanges();
								response.Message = "Deleted Success";
								response.Status = 1;
							}
							else
							{
								response.Message = "This user can not be deleted";
							}
						}
						else
						{
							response.Message = "Id not found";
						}
					}
					else
					{
						response.Message = "Invalid Token";
					}
				}
				else
				{
					response.Message = "Invalid Token";
				}
			}
			else
			{
				response.Message = damon_Tool.CheckValidate(typeof(Delete), Params);
			}
			return response;
		})
		.WithName("DeleteUser")
		.WithOpenApi();

		group.MapPost("/Update", ([FromForm] UpdateUserParams Params, Backend_ASP_ProjectContext db) =>
		{
			Response response = new Response();
			response.Status = 0;
			if (damon_Tool.CheckValidate(typeof(UpdateUserParams), Params) != "")
			{
				response.Message = damon_Tool.CheckValidate(typeof(UpdateUserParams), Params);
			}
			else
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (Token != null)
				{

					if (db.Users.FirstOrDefault(user => user.TokenId == Token.Id && user.IsDelete == 0) != null)
					{
						if (db.Users.FirstOrDefault(user => user.Id.ToString() != Params.Id && user.Username == Params.Username && user.IsDelete == 0) == null)
						{
							if (Params.Password == Params.ConfirmPassword)
							{
								if (db.Users.FirstOrDefault(user => user.Id.ToString() == Params.Id && user.IsDelete == 0) != null)
								{
									var role = db.Roles.First(role => role.Id.ToString() == Params.RoleId);
									var data = db.Users.Include(user => user.Role).First(user => user.Id.ToString() == Params.Id && user.IsDelete == 0);

									if (data.Role != null && data.Role.Name == "Librarian")
									{
										if (role.Name != "Librarian")
										{
											if (db.Users.Where(user => user.IsDelete == 0 && user.Role != null && user.Role.Name == "Librarian").ToList().Count == 1)
											{
												response.Message = "There must be at least 1 user librarian in Database";
												return response;
											}
										}
									}
									data.Username = Params.Username;
									data.Password = Params.Password;
									data.Role = db.Roles.First(role => role.Id.ToString() == Params.RoleId);
									if (Params.Image != null)
									{
										var file = new FileInfo(Params.Image.FileName);
										var exte = file.Extension;
										if (exte.Contains(".png") || exte.Contains(".pjp") || exte.Contains(".jpg") || exte.Contains(".pjpeg") || exte.Contains(".jpeg") || exte.Contains(".jfif"))
										{
											data.ImagePath = damon_Tool.CopyImage(Params.Image, "User");
										}
										else
										{
											response.Message = "Image file does not accpet " + exte + " type of file";
											return response;
										}
									}
									db.SaveChanges();
									response.Status = 1;
									response.Message = "Updated Successfully";
								}
								else
								{
									response.Message = "Id Not Found!";
								}
							}
							else
							{
								response.Message = "Confirm password and Password not Match!";
							}
						}
						else
						{
							response.Message = "Username already taken!";
						}
					}
					else
					{
						response.Message = "Invalid Token!";
					}
				}
				else
				{
					response.Message = "Invalid Token!";
				}
			}
			return response;
		})
		.DisableAntiforgery()
		.WithName("UpdateUser")
		.WithOpenApi();
	}


	public class TokenParams
	{
		public string apiToken { get; set; } = string.Empty;
	}

	public class SearchParamsAllUser
	{
		public string Username { get; set; } = string.Empty;
		public string RoleId { get; set; } = string.Empty;
		public string apiToken { get; set; } = string.Empty;
	}

	public class RegisterTokenUser
	{
		//public string Token2 { get; set; } = string.Empty;
		//public JwtToken Token { get; set; } = new JwtToken();
		public Login User { get; set; } = new Login();
		public RoleBody RoleBody { get; set; } = new RoleBody();
	}

	public class PostRegisterResponse : Response
	{
		public RegisterTokenUser? data { get; set; } = new RegisterTokenUser();
	}

	//public class PostLoginResponse : Response
	//{
	//	public LoginTokenUser data { get; set; } = new LoginTokenUser();
	//}
	public class PostLoginResponse : Response
	{
		public UserBody? data { get; set; } = new UserBody();
		public string Token { get; set; } = string.Empty;
	}

	public class LoginTokenUser
	{
		public string Token { get; set; } = string.Empty;
		public Login User { get; set; } = new Login();
	}

	public class GetUserResponse : Response
	{
		public UserBody? data { get; set; } = new UserBody();
	}

	public class GetListUserResponse : Response
	{
		public List<UserBody>? data { get; set; } = new List<UserBody>();
	}

	public class ChangePasswordParams
	{
		public string apiToken { get; set; } = string.Empty;
		public string OldPassword { get; set; } = string.Empty;
		public string NewPassword { get; set; } = string.Empty;
		public string ConfirmNewPassword { get; set; } = string.Empty;
	}
}
