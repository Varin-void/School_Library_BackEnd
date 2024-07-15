using Microsoft.EntityFrameworkCore;
using Backend_ASP_Project.Data;
using Backend_ASP_Project.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Backend_ASP_Project.Tool;
using Backend_ASP_Project.Models.Books.Get;
using Backend_ASP_Project.DTO;
using Backend_ASP_Project.Models.Books.Create;
using Backend_ASP_Project.Models.Books.Update;
using Microsoft.AspNetCore.Mvc;
using Backend_ASP_Project.Models.Books.GetById;
using Backend_ASP_Project.Models.Books.Download;
namespace Backend_ASP_Project.Endpoint;

public static class BookEndpoints
{
	public static void MapBookEndpoints(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/FT_SD_A_3/Book").WithTags(nameof(Book));
		Damon_Tool damon_Tool = new Damon_Tool();
		group.MapPost("/Get", (GetBookParams Params, Backend_ASP_ProjectContext db) =>
		{
			GetBookResponse getBookResponse = new GetBookResponse();
			getBookResponse.Status = 0;
			getBookResponse.data = null;
			//if (damon_Tool.CheckValidate(typeof(GetBookParams), Params) == "")
			if (Params.apiToken != "")
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (Token != null)
				{
					if (db.Users.Where(user => user.Id == Token.UserId).FirstOrDefault(user => user.IsDelete == 0) != null)
					{
						var book = db.Books.Include(book => book.Groups).Where(book => book.IsDelete == 0);
						if (Params.Search != "")
						{
							book = book.Where(book => book.Title.ToUpper().Contains(Params.Search.ToUpper()));
						}
						if (Params.Sort.ToLower() == "desc")
						{
							book = book.OrderByDescending(book => book.Title);
						}
						else if (Params.Sort.ToLower() == "asc")
						{
							book = book.OrderBy(book => book.Title);
						}
						List<BookBody> data = damon_Tool.MapBookBody(book.ToList());
						getBookResponse.data = data;
						getBookResponse.Status = 1;
						getBookResponse.Message = "Success!";
					}
					else
					{
						getBookResponse.Message = "Invalid Token!";
					}
				}
				else
				{
					getBookResponse.Message = "Invalid Token!";
				}
			}
			else
			{
				getBookResponse.Message = "Params must have apiToken";
			}
			return getBookResponse;
		})
		.WithName("GetAllBooks")
		.WithOpenApi();

		group.MapPost("/Download", async Task<DownloadPDFResponse> (DownloadPDFParams Params, Backend_ASP_ProjectContext db) =>
		{
			DownloadPDFResponse response = new DownloadPDFResponse();
			response.Status = 0;
			if (damon_Tool.CheckValidate(typeof(DownloadPDFParams), Params) == "")
			{
				var token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (token != null)
				{
					if (db.Users.FirstOrDefault(user => user.Id == token.UserId && user.IsDelete == 0) != null)
					{
						var book = db.Books.FirstOrDefault(book => book.Id.ToString() == Params.Id);
						if (book != null)
						{
							book.DownloadCount += 1;
							db.SaveChanges();
							response.Status = 1;
							response.Message = "Success";
							var result = await damon_Tool.DownloadFile(book.Pdf);
							response.Pdf = new PdfFile();
							response.Pdf.Filename = result.Item3;
							response.Pdf.ContentType = result.Item2;
							response.Pdf.Data = result.Item1;
						}
						else
						{
							response.Message = "Book id not found";
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
				response.Message = damon_Tool.CheckValidate(typeof(DownloadPDFParams), Params);
			}
			return response;
		})
		.WithName("DownloadPDF")
		.WithOpenApi();

		group.MapPost("/Create", async Task<CreateBookResponse> ([FromForm] CreateBookParams Params, Backend_ASP_ProjectContext db) =>
		{
			CreateBookResponse response = new CreateBookResponse
			{
				Status = 0,
				Data = null
			};
			if (damon_Tool.CheckValidate(typeof(CreateBookParams), Params) == "")
			{
				if (Params.Image != null)
				{
					if (Params.Pdf != null)
					{
						FileInfo file = new FileInfo(Params.Pdf.FileName);
						if (file.Extension == ".pdf" || file.Extension == ".xlsx")
						{
							file = new FileInfo(Params.Image.FileName);
							var exte = file.Extension;
							if (exte.Contains(".png") || exte.Contains(".pjp") || exte.Contains(".jpg") || exte.Contains(".pjpeg") || exte.Contains(".jpeg") || exte.Contains(".jfif"))
							{
								var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
								if (Token != null)
								{

									if (db.Users.Where(user => user.Id == Token.UserId).FirstOrDefault(user => user.IsDelete == 0 && user.Role != null && (user.Role.Name == "Librarian" || user.Role.Name == "Teacher")) != null)
									{
										if (db.Books.Where(book => book.Title == Params.Title).FirstOrDefault(book => book.IsDelete == 0) == null)
										{
											if (Params.GroupId != null)
											{
												if (db.Groups.FirstOrDefault(group => group.Id.ToString() == Params.GroupId && group.IsDelete == 0) == null)
												{
													response.Message = "Group Id not Found";
													return response;
												}
											}

											Book book = new Book()
											{
												Author = Params.Author,
												Pdf = await damon_Tool.UploadFile(Params.Pdf),
												ImagePath = damon_Tool.CopyImage(Params.Image, "Book"),
												Title = Params.Title,
												IsDelete = 0,
												Description = Params.Description,
												DownloadCount = 0
											};
											db.Books.Add(book);
											await db.SaveChangesAsync();
											if (Params.GroupId != null)
											{
												var tempBook = db.Books.First(book => book.Title == Params.Title && book.IsDelete == 0);
												var tempGroup = db.Groups.First(group => group.Id.ToString() == Params.GroupId && group.IsDelete == 0);
												tempGroup.Books.Add(tempBook);
												tempBook.Groups.Add(tempGroup);
												await db.SaveChangesAsync();
											}
											response.Data = damon_Tool.MapBookBody(book);
											response.Status = 1;
											response.Message = "Success";
										}
										else
										{
											response.Message = "Title of the book already in the Database!";
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
								response.Message = "Image file does not accpet " + exte + " type of file";
							}
						}
						else
						{
							response.Message = "PDF file input must be an PDF file";
						}
					}
					else
					{
						response.Message = "Params must have an Elsl and PDF file";
					}
				}
				else
				{
					response.Message = "Params must have an Image";
				}
			}
			else
			{
				response.Message = damon_Tool.CheckValidate(typeof(CreateBookParams), Params);
			}
			return response;
		})
		.DisableAntiforgery()
		.WithName("CreateBook")
		.WithOpenApi();

		group.MapPost("/Get-Id", (GetBookByIdParams Params, Backend_ASP_ProjectContext db) =>
		{
			GetBookByIdResponse getBookResponse = new GetBookByIdResponse();
			getBookResponse.Status = 0;
			getBookResponse.data = null;
			if (damon_Tool.CheckValidate(typeof(GetBookByIdParams), Params) == "")
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (Token != null)
				{
					if (db.Users.Where(user => user.Id == Token.UserId).FirstOrDefault(user => user.IsDelete == 0) != null)
					{
						var book = db.Books.Where(book => book.Id.ToString() == Params.Id).FirstOrDefault(book => book.IsDelete == 0);
						if (book != null)
						{
							BookBody data = damon_Tool.MapBookBody(book);
							getBookResponse.data = data;
							getBookResponse.Status = 1;
							getBookResponse.Message = "Success!";
						}
						else
						{
							getBookResponse.Message = "Book Id not Found!";
						}
					}
					else
					{
						getBookResponse.Message = "Invalid Token!";
					}
				}
				else
				{
					getBookResponse.Message = "Invalid Token!";
				}
			}
			else
			{
				getBookResponse.Message = damon_Tool.CheckValidate(typeof(GetBookByIdParams), Params);
			}
			return getBookResponse;
		})
		.WithName("GetBookById")
		.WithOpenApi();


		//async Task<Results<Ok<UpdateBookResponse>, NotFound<UpdateBookResponse>>>
		group.MapPost("/Update", async ([FromForm] UpdateBookParams Params, Backend_ASP_ProjectContext db) =>
		{
			UpdateBookResponse response = new UpdateBookResponse
			{
				Status = 0,
				data = null
			};
			if (damon_Tool.CheckValidate(typeof(UpdateBookParams), Params) == "")
			{
				var Token = db.Tokens.FirstOrDefault(token => token.Token == Params.apiToken && token.Expired > DateTime.Now);
				if (Token != null)
				{
					if (db.Users.Where(user => user.Id == Token.UserId).FirstOrDefault(user => user.IsDelete == 0 && user.Role != null && (user.Role.Name == "Librarian" || user.Role.Name == "Teacher")) != null)
					{
						if (db.Books.FirstOrDefault(book => book.IsDelete == 0 && book.Title == Params.Title && book.Id != Params.Id) == null)
						{

							if (db.Books.FirstOrDefault(book => book.Id == Params.Id && book.IsDelete == 0) != null)
							{
								var book = db.Books.First(book => book.Id == Params.Id && book.IsDelete == 0);
								book.Title = Params.Title;
								book.Author = Params.Author;
								book.Description = Params.Description;
								if (Params.Pdf != null)
								{
									FileInfo file = new FileInfo(Params.Pdf.FileName);
									if (file.Extension == ".pdf")
									{
										book.Pdf = await damon_Tool.UploadFile(Params.Pdf);
									}
									else
									{
										response.Message = "PDF file input must be an PDF file";
										return response;
									}
								}
								if (Params.Image != null)
								{
									FileInfo file = new FileInfo(Params.Image.FileName);
									var exte = file.Extension;
									if (exte.Contains(".png") || exte.Contains(".pjp") || exte.Contains(".jpg") || exte.Contains(".pjpeg") || exte.Contains(".jpeg") || exte.Contains(".jfif"))
									{
										book.ImagePath = damon_Tool.CopyImage(Params.Image, "Book");
									}
									else
									{
										response.Message = "Image file input must be an Image file";
										return response;
									}
								}
								db.SaveChanges();
								response.data = damon_Tool.MapBookBody(book);
								response.Status = 1;
								response.Message = "Updated Successfully!";

							}
							else
							{
								response.Message = "Id Not Found!";
							}
						}
						else
						{
							response.Message = "Book Title already taken!";
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
				response.Message = damon_Tool.CheckValidate(typeof(UpdateBookParams), Params);
			}
			//if (response.Status == 1)
			//return TypedResults.Ok(response);
			//return TypedResults.NotFound(response);
			return response;
		})
		.DisableAntiforgery()
		.WithName("UpdateBook")
		.WithOpenApi();

		//async Task<Results<Ok<Response>, NotFound<Response>>>
		group.MapPost("/Delete", (Delete Params, Backend_ASP_ProjectContext db) =>
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
						if (db.Books.FirstOrDefault(book => book.Id.ToString() == Params.Id && book.IsDelete == 0) != null)
						{
							var book = db.Books.First(book => book.Id.ToString() == Params.Id && book.IsDelete == 0);
							book.IsDelete = 1;
							db.SaveChanges();
							response.Status = 1;
							response.Message = "Deleted Successful";
						}
						else
						{
							response.Message = "Id Not Found!";
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
		.WithName("DeleteBook")
		.WithOpenApi();
	}


}
