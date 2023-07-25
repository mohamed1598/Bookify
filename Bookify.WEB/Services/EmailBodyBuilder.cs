using Microsoft.AspNetCore.Hosting;
using System.Text.Encodings.Web;

namespace Bookify.WEB.Services
{
	public class EmailBodyBuilder : IEmailBodyBuilder
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
public EmailBodyBuilder(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}

		public string GetEmailBody(string imageUrl, string header, string body, string url, string linkTitle)
		{
			var filePath = $"{_webHostEnvironment.WebRootPath}/templates/email.html";
			StreamReader str = new(filePath);
			var template = str.ReadToEnd(); 
			str.Close();
			//var url = Url.Action("Index", "Home", null, Request.Scheme);

			return template
				.Replace("[imageUrl]", imageUrl)
				.Replace("[header]", header)
				.Replace("[body]", body)
				.Replace("[url]", url)
				.Replace("[linkTitle]", linkTitle);
		}
	}
}
