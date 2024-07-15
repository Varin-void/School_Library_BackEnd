using Microsoft.EntityFrameworkCore;
using Backend_ASP_Project.Data;
using Backend_ASP_Project.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Backend_ASP_Project.Models.Groups.Get;
using Backend_ASP_Project.Tool;
using Backend_ASP_Project.Models.Groups.Create;
using Backend_ASP_Project.Models.Groups.GetById;
using Backend_ASP_Project.Models.Groups.AddBook;
using Backend_ASP_Project.Models.Groups.AddUser;
using Backend_ASP_Project.Models.Groups.Update;
namespace Backend_ASP_Project.Endpoint;

public static class GroupEndpoints
{
	public static void MapGroupEndpoints(this IEndpointRouteBuilder routes)
	{
		var MapArray = routes.MapGroup("/FT_SD_A_3/Group").WithTags(nameof(Group));
		Damon_Tool damon_Tool = new Damon_Tool();


		MapArray.MapPost("/Get", (GetGroupParams Params, Backend_ASP_ProjectContext db) =>
		{
			GetGroupResponse groups = new GetGroupResponse();
			groups.Status = 0;
			groups.Data = null;
			//if (damon_Tool.CheckValidate(typeof(GetGroupParams), Params) == "")
			//{
			if (Params.apiToken != "" || Params.apiToken != null)
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (Token != null)
				{
					if (db.Users.Where(user => user.Id == Token.UserId).FirstOrDefault(user => user.IsDelete == 0) != null)
					{
						var data = db.Groups.Where(group => group.IsDelete == 0 && group.Name.ToUpper().Contains(Params.Name.ToUpper()))
							.Include(group => group.Books.Where(Book => db.Books.Where(book => book.IsDelete == 0).Select(book => book.Id).Contains(Book.Id)))
							.Include(group => group.Users.Where(User => db.Users.Where(user => user.IsDelete == 0).Select(user => user.Id).Contains(User.Id))).ThenInclude(user => user.Role);
						List<Group> SortedData;
						if (Params.Sort.ToLower() == "asc")
						{
							SortedData = data.OrderBy(data => data.Name).ToList();
						}
						else
						{
							SortedData = data.OrderByDescending(data => data.Name).ToList();
						}
						groups.Data = damon_Tool.MapGroupBody(SortedData);
						groups.Status = 1;
						groups.Message = "Success";
					}
					else
					{
						groups.Message = "Invalid Token!";
					}
				}
				else
				{
					groups.Message = "Invalid Token!";
				}
			}
			else
			{
				groups.Data = null;
				groups.Message = "Params need apiToken!";
				//damon_Tool.CheckValidate(typeof(GetGroupParams), Params);
			}
			return groups;
		})
		.WithName("GetAllGroups")
		.WithOpenApi();

		MapArray.MapPost("/Remove-User", (AddUserToGroupParams Params, Backend_ASP_ProjectContext db) =>
		{
			Response response = new Response();
			response.Status = 0;
			if (damon_Tool.CheckValidate(typeof(AddUserToGroupParams), Params) == "")
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (Token != null)
				{
					if (db.Users.Where(user => user.Id == Token.UserId).FirstOrDefault(user => user.Role != null && user.Role.Name == "Librarian" && user.IsDelete == 0) != null)
					{
						var Group_Data = db.Groups.Include(group => group.Users).ThenInclude(user => user.Role).FirstOrDefault(group => group.Id.ToString() == Params.GroupId && group.IsDelete == 0);
						//var User_Data = db.Users.Include(user => user.Role).FirstOrDefault(user => user.Id.ToString() == Params.UserId && user.IsDelete == 0);

						if (Group_Data != null)
						{
							var User_Data = Group_Data.Users.FirstOrDefault(user => user.Id.ToString() == Params.UserId);
							if (User_Data != null)
							{
								if (Group_Data.Users.FirstOrDefault(user => user.Id == User_Data.Id) != null)
								{
									db.Groups.First(group => group.Id.ToString() == Params.GroupId && group.IsDelete == 0).Users.Remove(User_Data);
									db.Users.First(user => user.Id.ToString() == Params.UserId && user.IsDelete == 0).Group?.Remove(Group_Data);
									db.SaveChanges();
									response.Status = 1;
									response.Message = "User has been remove";
								}
								else
								{
									if (User_Data.Role != null)
									{
										response.Message = "This " + User_Data.Role.Name + " isn`t in the Group!";
									}
									else
									{
										response.Message = "This user isn`t in the Group!";
									}
								}
							}
							else
							{
								response.Message = "UserId not Found!";
							}
						}
						else
						{
							response.Message = "GroupId not Found!";
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
				response.Message = damon_Tool.CheckValidate(typeof(AddUserToGroupParams), Params);
			}
			return response;
		})
		.WithName("Remove-Student/Teacher")
		.WithOpenApi();


		MapArray.MapPost("/Add-User", (AddUserToGroupParams Params, Backend_ASP_ProjectContext db) =>
			{
				Response response = new Response();
				response.Status = 0;
				if (damon_Tool.CheckValidate(typeof(AddUserToGroupParams), Params) == "")
				{
					var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
					if (Token != null)
					{
						if (db.Users.Where(user => user.Id == Token.UserId).FirstOrDefault(user => user.Role != null && user.Role.Name == "Librarian" && user.IsDelete == 0) != null)
						{
							var CheckStudent = db.Users.Include(user => user.Group).Include(user => user.Role).FirstOrDefault(user => user.Id.ToString() == Params.UserId && user.IsDelete == 0);

							if (CheckStudent != null)
							{
								if (CheckStudent.Role != null && CheckStudent.Role.Name == "Student")
								{
									if (CheckStudent.Group != null)
									{
										var GroupCheck = CheckStudent.Group.Where(group => db.Groups.Where(group => group.IsDelete == 0).Select(group => group.Id).Contains(group.Id)).ToList();
										if (GroupCheck.Count > 0)
										{
											response.Message = "Student can only be in 1 group";
											return response;
										}
									}
								}
							}
							var User_Data = db.Users.Include(user => user.Role).FirstOrDefault(user => user.Id.ToString() == Params.UserId && user.IsDelete == 0);
							var Group_Data = db.Groups.Include(group => group.Users).FirstOrDefault(group => group.Id.ToString() == Params.GroupId && group.IsDelete == 0);
							if (User_Data != null)
							{
								if (Group_Data != null)
								{
									if (Group_Data.Users.FirstOrDefault(user => user.Id == User_Data.Id) == null)
									{
										db.Groups.First(group => group.Id.ToString() == Params.GroupId && group.IsDelete == 0).Users.Add(User_Data);
										db.Users.First(user => user.Id.ToString() == Params.UserId && user.IsDelete == 0).Group?.Add(Group_Data);
										db.SaveChanges();
										response.Status = 1;
										response.Message = "Success";
									}
									else
									{
										if (User_Data.Role != null)
										{
											response.Message = "This " + User_Data.Role.Name + " is already in the Group!";
										}
									}
								}
								else
								{
									response.Message = "GroupId not Found!";
								}
							}
							else
							{
								response.Message = "UserId not Found!";
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
					response.Message = damon_Tool.CheckValidate(typeof(AddUserToGroupParams), Params);
				}
				return response;
			}).WithName("Add-Student/Teacher").WithOpenApi();


		MapArray.MapPost("/Remove-Book", async (AddBookToGroupParams Params, Backend_ASP_ProjectContext db) =>
		{
			Response response = new Response();
			response.Status = 0;
			if (damon_Tool.CheckValidate(typeof(AddBookToGroupParams), Params) == "")
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken);
				if (Token != null)
				{
					if (Token.Expired > DateTime.Now)
					{
						if (db.Users.Where(user => user.Id == Token.UserId && user.Role != null && user.Role.Name == "Teacher" && user.IsDelete == 0) != null)
						{
							var Groups = db.Groups.Include(group => group.Books).FirstOrDefault(group => group.Id.ToString() == Params.GroupId && group.IsDelete == 0);
							if (Groups != null)
							{
								var Books = db.Books.Include(book => book.Groups).FirstOrDefault(book => book.IsDelete == 0 && book.Id.ToString() == Params.BookId);
								if (Books != null)
								{
									var oldBook = Groups.Books.FirstOrDefault(book => book.Id == Books.Id);
									if (oldBook != null)
									{
										Groups.Books.Remove(Books);
										Books.Groups.Remove(Groups);
										await db.SaveChangesAsync();
										response.Message = "Book Removed Successfully";
										response.Status = 1;
									}
									else
									{
										response.Message = "BookID not in Groups!";
									}
								}
								else
								{
									response.Message = "BookID not found!";
								}
							}
							else
							{
								response.Message = "GroupID not found!";
							}
						}
						else
						{
							response.Message = "You dont have access to Remove Book from Group!";
						}
					}
					else
					{
						response.Message = "Token Expired";
					}
				}
				else
				{
					response.Message = "Invalid Token!";
				}
			}
			else
			{
				response.Message = damon_Tool.CheckValidate(typeof(AddBookToGroupParams), Params);
			}
			return response;
		})
		.WithName("RemoveBookFromGroup")
		.WithOpenApi();

		MapArray.MapPost("/Add-Book", async (AddBookToGroupParams Params, Backend_ASP_ProjectContext db) =>
		{
			Response response = new Response();
			response.Status = 0;
			if (damon_Tool.CheckValidate(typeof(AddBookToGroupParams), Params) == "")
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (Token != null)
				{
					if (db.Users.Where(user => user.Id == Token.UserId).FirstOrDefault(user => user.IsDelete == 0 && user.Role != null && (user.Role.Name == "Librarian" || user.Role.Name == "Teacher")) != null)
					{
						if (db.Books.FirstOrDefault(book => book.Id.ToString() == Params.BookId && book.IsDelete == 0) != null)
						{
							if (db.Groups.FirstOrDefault(group => group.Id.ToString() == Params.GroupId && group.IsDelete == 0) != null)
							{
								var Group_Data = db.Groups.Include(group => group.Books.Where(book => db.Books
															.Where(book => book.IsDelete == 0)
															.Select(book => book.Id).Contains(book.Id)))
															.First(group => group.Id.ToString() == Params.GroupId && group.IsDelete == 0);
								var Book_Data = db.Books.First(book => book.Id.ToString() == Params.BookId && book.IsDelete == 0);
								if (Group_Data.Books.FirstOrDefault(book => book.Id == Book_Data.Id) == null)
								{
									Group_Data.Books.Add(Book_Data);
									await db.SaveChangesAsync();
									response.Status = 1;
									response.Message = "Success";
								}
								else
								{
									response.Message = "BookId Already Exist In Group!";
								}
							}
							else
							{
								response.Message = "GroupId Not Found!";
							}
						}
						else
						{
							response.Message = "BookId Not Found!";
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
				response.Message = damon_Tool.CheckValidate(typeof(AddBookToGroupParams), Params);
			}
			return response;
		})
		.WithName("AddBook")
		.WithOpenApi();


		MapArray.MapPost("/Create", async (CreateGroupParams Params, Backend_ASP_ProjectContext db) =>
		{
			CreateGroupResponse response = new CreateGroupResponse();
			response.Status = 0;
			response.data = null;
			if (damon_Tool.CheckValidate(typeof(CreateGroupParams), Params) == "")
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (Token != null)
				{
					if (db.Users.Where(user => user.Id == Token.UserId).FirstOrDefault(user => user.Role != null && user.Role.Name == "Librarian" && user.IsDelete == 0) != null)
					{
						if (db.Groups.FirstOrDefault(group => group.Name == Params.Name && group.IsDelete == 0) == null)
						{
							Group Data = new Group()
							{
								Name = Params.Name,
								IsDelete = 0
							};
							db.Groups.Add(Data);
							await db.SaveChangesAsync();
							response.Status = 1;
							response.data = damon_Tool.MapGroupBody(db.Groups.First(group => group.Name == Params.Name));
							response.Message = "Success";
						}
						else
						{
							response.Message = "Group Name Already Exist";
						}
					}
					else
					{
						response.Message = "Token Invalid";
					}
				}
				else
				{
					response.Message = "Token Invalid";
				}
			}
			else
			{
				response.Message = damon_Tool.CheckValidate(typeof(CreateGroupParams), Params);
			}
			return response;
		})
		.WithName("CreateGroup")
		.WithOpenApi();

		MapArray.MapPost("/Get-Id", (GetGroupByIdParams Params, Backend_ASP_ProjectContext db) =>
		{
			GetGroupByIdResponse response = new GetGroupByIdResponse();
			response.data = null;
			response.Status = 0;
			if (damon_Tool.CheckValidate(typeof(GetGroupByIdParams), Params) == "")
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (Token != null)
				{
					if (db.Users.Where(user => user.Id == Token.UserId).FirstOrDefault(user => user.IsDelete == 0) != null)
					{
						if (db.Groups.FirstOrDefault(group => group.Id.ToString() == Params.Id && group.IsDelete == 0) != null)
						{
							response.data = damon_Tool.MapGroupBody(db.Groups.Include(group => group.Books.Where(Book => db.Books.Where(book => book.IsDelete == 0).Select(book => book.Id).Contains(Book.Id)))
							.Include(group => group.Users.Where(User => db.Users.Where(user => user.IsDelete == 0).Select(user => user.Id).Contains(User.Id))).ThenInclude(user => user.Role).First(group => group.Id.ToString() == Params.Id && group.IsDelete == 0));
							response.Message = "Success";
							response.Status = 1;
						}
						else
						{
							response.Message = "Group Id is not Found!";
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
				response.Message = damon_Tool.CheckValidate(typeof(GetGroupByIdParams), Params);
			}
			return response;
		})
		.WithName("GetGroupById")
		.WithOpenApi();

		MapArray.MapPost("/Update", (GroupUpdateParams Params, Backend_ASP_ProjectContext db) =>
		{
			Response response = new Response();
			response.Status = 0;
			if (damon_Tool.CheckValidate(typeof(GroupUpdateParams), Params) == "")
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (Token != null)
				{
					if (db.Users.Where(user => user.Id == Token.UserId).FirstOrDefault(user => user.IsDelete == 0 && user.Role != null && user.Role.Name == "Librarian") != null)
					{
						if (db.Groups.Where(group => group.Id.ToString() == Params.Id).FirstOrDefault(group => group.IsDelete == 0) != null)
						{
							if (db.Groups.FirstOrDefault(group => group.Id.ToString() != Params.Id && group.Name == Params.Name && group.IsDelete == 0) == null)
							{
								db.Groups.First(group => group.Id.ToString() == Params.Id && group.IsDelete == 0).Name = Params.Name;
								db.SaveChanges();
								response.Status = 1;
								response.Message = "Updated Successful!";
							}
							else
							{
								response.Message = "Group Name Already Exist!";
							}
						}
						else
						{
							response.Message = "Group ID Not Founded!";
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
				response.Message = damon_Tool.CheckValidate(typeof(GroupUpdateParams), Params);
			}
			return response;
		})
		.WithName("UpdateGroup")
		.WithOpenApi();



		MapArray.MapPost("/Delete", (Delete Params, Backend_ASP_ProjectContext db) =>
		{
			Response response = new Response();
			response.Status = 0;
			if (damon_Tool.CheckValidate(typeof(Delete), Params) == "")
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (Token != null)
				{
					if (db.Users.Where(user => user.Id == Token.UserId).FirstOrDefault(user => user.Role != null && user.Role.Name == "Librarian" && user.IsDelete == 0) != null)
					{
						if (db.Groups.FirstOrDefault(group => group.Id.ToString() == Params.Id && group.IsDelete == 0) != null)
						{
							db.Groups.First(group => group.Id.ToString() == Params.Id && group.IsDelete == 0).IsDelete = 1;
							db.SaveChanges();

							response.Status = 1;
							response.Message = "Deleted Successful!";
						}
						else
						{
							response.Message = "GroupId not Found!";
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
				response.Message = damon_Tool.CheckValidate(typeof(Delete), Params);
			}
			return response;
		})
		.WithName("DeleteGroup")
		.WithOpenApi();
	}
}
