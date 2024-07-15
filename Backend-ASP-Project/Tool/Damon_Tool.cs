
using Backend_ASP_Project.DTO;
using Backend_ASP_Project.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend_ASP_Project.Tool
{
	public class Damon_Tool
	{
		public async Task<string> UploadFile(IFormFile _File)
		{
			string Filepath = "";
			FileInfo fileInfo = new FileInfo(_File.FileName);
			Filepath = "/Asset/PDF/" + _File.FileName;
			var check = Directory.GetCurrentDirectory() + Filepath;

			int i = 1;
			while (Path.Exists(check))
			{
				Filepath = "/Asset/PDF/" + i + "-" + _File.FileName;
				check = Directory.GetCurrentDirectory() + Filepath;
				i++;
			}

			using (var fileStream = new FileStream(check, FileMode.Create))
			{
				await _File.CopyToAsync(fileStream);
			}
			return Filepath;
		}

		public async Task<(byte[],string,string)> DownloadFile(string Filename)
		{
			var FilePath = Directory.GetCurrentDirectory() + Filename;
			var provider = new FileExtensionContentTypeProvider();
			if(!provider.TryGetContentType(FilePath,out var contentType))
			{
				contentType = "application/octet-stream";
			}
			var ReadAllByteAsync = await File.ReadAllBytesAsync(FilePath);
			return (ReadAllByteAsync, contentType, Path.GetFileName(FilePath));
		}

		public JwtToken GenerateNewToken(string userName, string password)
		{
			var secret_token = Encoding.ASCII.GetBytes("DAMON_KEY_U_NOOB_1234567891234567");
			var handlesecurity = new JwtSecurityTokenHandler();
			var expired = DateTime.Now.AddMinutes(60);
			var securityToken = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new List<Claim> {
					new Claim("username",userName),
					new Claim("password",password)
				}),
				Expires = expired,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret_token), SecurityAlgorithms.HmacSha256Signature)
			};
			var security_Token = handlesecurity.CreateToken(securityToken);
			var token = handlesecurity.WriteToken(security_Token);
			return new JwtToken
			{
				Token = token,
				Expired = expired
			};
		}

		public string CopyImage(IFormFile file, string Type)
		{
			var FileName = file.FileName;
			string current = Directory.GetCurrentDirectory() + "\\Asset\\Images\\" + Type + "\\";
			var check = current + FileName;
			int i = 1;
			while (Path.Exists(check))
			{
				FileName = i + "-" + file.FileName;
				check = current + i + "-" + file.FileName;
				i++;
			}
			string folder = current + FileName;
			var stream = new FileStream(folder, FileMode.Create);
			file.CopyToAsync(stream);
			return "/Asset/Images/" + Type + "/" + FileName;
		}
		public string CheckValidate(Type type, object temp)
		{
			string Message = "";
			var propertyInfo = type.GetProperties();

			foreach (var property in propertyInfo)
			{
				var value = property.GetValue(temp);
				if (value is null or (object)"")
				{
					var ParamsName = property.ToString();
					if (ParamsName != null && (ParamsName.Split(' ')[1] != "Image" && ParamsName.Split(' ')[1] != "GroupId" && ParamsName.Split(' ')[1] != "Pdf"))
					{
						if (Message == "")
						{
							Message += "Params must have " + ParamsName.Split(' ')[1];
						}
						else
						{
							Message += ", " + ParamsName.Split(' ')[1];
						}
					}
				}
			}
			return Message;
		}


		public RoleBody MapRoleBody(Role role)
		{
			return new RoleBody() { Name = role.Name, Id = role.Id };
		}

		public List<RoleBody> MapRoleBody(List<Role> roles)
		{
			List<RoleBody> bodies = new List<RoleBody>();
			foreach (var role in roles)
			{
				bodies.Add(MapRoleBody(role));
			}
			return bodies;
		}

		public UserBody MapUserBody(User user)
		{
			UserBody bodies = new UserBody()
			{
				Username = user.Username,
				Password = user.Password,
				Id = user.Id,
				ImagePath = user.ImagePath,
				IsDelete = user.IsDelete,
			};
			if (user.Role != null)
				bodies.Role = MapRoleBody(user.Role);
			else bodies.Role = null;
			if (user.Group != null)
				bodies.Group = MapGroupBodyForUser(user.Group.ToList());
			else bodies.Group = null;
			return bodies;
		}

		private GroupBodyForUser MapGroupBodyForUser(Group group)
		{
			return new GroupBodyForUser { Id = group.Id, Name = group.Name };
		}

		private List<GroupBodyForUser>? MapGroupBodyForUser(List<Group> groups)
		{
			List<GroupBodyForUser> NewData = new List<GroupBodyForUser>();
			foreach (var group in groups)
			{
				NewData.Add(MapGroupBodyForUser(group));
			}
			return NewData;
		}

		public List<UserBody> MapUserBody(List<User> users)
		{
			List<UserBody> bodies = new List<UserBody>();
			foreach (var user in users)
			{
				bodies.Add(MapUserBody(user));
			}
			return bodies;
		}

		public int[] GetAllId(List<Group> groups)
		{
			int[] Groups = new int[groups.Count];
			int i = 0;
			foreach (var group in groups)
			{
				Groups[i] = group.Id;
				i++;
			}
			return Groups;
		}

		public BookBody MapBookBody(Book body)
		{
			return new BookBody()
			{
				Id = body.Id,
				Title = body.Title,
				Author = body.Author,
				Description = body.Description,
				ImagePath = body.ImagePath,
				Pdf = body.Pdf,
				Groups = GetAllId(body.Groups.ToList())
			};

		}

		public List<BookBody> MapBookBody(List<Book> datas)
		{
			List<BookBody> Newdata = new List<BookBody>();
			foreach (var data in datas)
			{
				Newdata.Add(MapBookBody(data));
			}
			return Newdata;
		}

		private UserBodyForGroup MapUserBodyForGroup(User user)
		{
			UserBodyForGroup bodies = new UserBodyForGroup()
			{
				Username = user.Username,
				Password = user.Password,
				Id = user.Id,
				//Token = user.Token2,
				Image = user.ImagePath,
				IsDelete = user.IsDelete,
			};
			if (user.Role != null)
				bodies.Role = MapRoleBody(user.Role);
			else bodies.Role = null;
			return bodies;
		}

		private List<UserBodyForGroup> MapUserBodyForGroup(List<User> datas)
		{
			List<UserBodyForGroup> Newdata = new List<UserBodyForGroup>();
			foreach (var data in datas)
			{
				Newdata.Add(MapUserBodyForGroup(data));
			}
			return Newdata;
		}

		public GroupBody MapGroupBody(Group data)
		{
			GroupBody Newdata = new GroupBody();
			Newdata.Id = data.Id;
			Newdata.Name = data.Name;
			Newdata.Books = MapBookBody(data.Books.ToList());
			Newdata.Teachers = MapUserBodyForGroup(data.Users.Where(user => user.Role != null && user.Role.Name == "Teacher").ToList());
			Newdata.Students = MapUserBodyForGroup(data.Users.Where(user => user.Role != null && user.Role.Name == "Student").ToList());
			return Newdata;
		}

		public List<GroupBody> MapGroupBody(List<Group> datas)
		{
			List<GroupBody> Newdata = new List<GroupBody>();
			foreach (var data in datas)
			{
				Newdata.Add(MapGroupBody(data));
			}
			return Newdata;
		}

	}
}
