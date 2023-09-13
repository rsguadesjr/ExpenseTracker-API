using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.CompilerServices;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly string _ftpUser;
        private readonly string _ftpPassword;
        private string _ftpUrl;
        private readonly FtpWebRequest _ftpWebRequest;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _configuration = configuration;
            _env = environment;
            var baseFolder = "Portals";
            // api app service
            //_ftpUrl = "ftps://waws-prod-blu-423.ftp.azurewebsites.windows.net";
            //_ftpUser = "rsg-expensetracker\\$rsg-expensetracker";
            //_ftpPassword = "bDNdoti5SRwbpMfYMPoYuSkgBkZt7dBFlxtyorfyl7p0phwB3pTv1GNdnJzr";


            // ui app service
            _ftpUrl = "ftps://waws-prod-sg1-085.ftp.azurewebsites.windows.net";
            _ftpUser = "rsg-expensetracker-ui\\$rsg-expensetracker-ui";
            _ftpPassword = "GqJrJegYhMNdRy2g0kfnkquvawkulTg8bd6LuwYJlYsrpLQg6BaTEqwk70jl";

            if (_ftpUrl.StartsWith("ftps://"))
            {
                _ftpUrl = _ftpUrl.Replace("ftps://", "ftp://");
            }

            _ftpWebRequest = (FtpWebRequest)WebRequest.Create(_ftpUrl);
            _ftpWebRequest.EnableSsl = true;
            _ftpWebRequest.Credentials = new NetworkCredential(_ftpUser, _ftpPassword);
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSettings()
        {
            var test = new
            {
                test = _configuration.GetConnectionString("DefaultConnection"),
                WebRootPath = _env.WebRootPath,
                ContentRootPath = _env.ContentRootPath,
            };
            return Ok(test);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                var contentRooth = _env.ContentRootPath;
                var sourcePath = Path.Combine(contentRooth, "TestFiles");
                var sourceRoothPath = Path.GetPathRoot(sourcePath);
                var sourceDir = new DirectoryInfo(sourcePath);

                if (!sourceDir.Exists)
                {
                    Directory.CreateDirectory(sourcePath);
                }


                var filePath = Path.Combine(sourcePath, file.FileName);
                using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(stream);
                }


                return Ok(new
                {
                    contentRooth,
                    sourcePath,
                    sourceDir = sourceDir.FullName,
                    filePath,
                    testPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System)),
                    testPath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)),
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("[action]/{folderName}")]
        public async Task<IActionResult> CloneFiles(string folderName)
        {
            try
            {
                var contentRooth = _env.ContentRootPath;
                var sourcePath = Path.Combine(contentRooth, "TestFiles");
                var sourceRoothPath = Path.GetPathRoot(sourcePath);
                var sourceDir = new DirectoryInfo(sourcePath);


                var targetPath = Path.Combine(contentRooth, folderName);
                var targetRoothPath = Path.GetPathRoot(targetPath);

                var targetDir = new DirectoryInfo(targetPath);
                if (!targetDir.Exists)
                {
                    Directory.CreateDirectory(targetPath);
                }

                var files = sourceDir.GetFiles();
                foreach (var file in files)
                {
                    var fileTargetPath = Path.Combine(targetPath, file.Name);
                    file.CopyTo(fileTargetPath);
                }


                return Ok(new
                {
                    contentRooth,
                    sourcePath,
                    sourceDir = sourceDir.FullName,
                    targetPath,
                    targetDir = targetDir.FullName,
                    testPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System)),
                    testPath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)),

                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> UploadV2(IFormFile file)
        {
            try
            {
                var contentRooth = _env.ContentRootPath;
                var sourcePath = Path.Combine(contentRooth, "TestFiles");
                var sourceRoothPath = Path.GetPathRoot(sourcePath);
                if (sourceRoothPath.EndsWith(":\\"))
                {
                    sourcePath = string.Format(@"\\{0}\{1}\{2}", Environment.MachineName, sourceRoothPath.Replace(":\\", "$"), sourcePath.Replace(sourceRoothPath, ""));
                }

                var sourceDir = new DirectoryInfo(sourcePath);

                if (!sourceDir.Exists)
                {
                    Directory.CreateDirectory(sourcePath);
                }


                var filePath = Path.Combine(sourcePath, file.FileName);
                using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(stream);
                }


                return Ok(new
                {
                    contentRooth,
                    sourcePath,
                    sourceDir = sourceDir.FullName,
                    filePath,
                    testPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System)),
                    testPath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)),
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("[action]/{folderName}")]
        public async Task<IActionResult> CloneFilesV2(string folderName)
        {
            try
            {
                var contentRooth = _env.ContentRootPath;
                var sourcePath = Path.Combine(contentRooth, "TestFiles");
                var sourceRoothPath = Path.GetPathRoot(sourcePath);
                if (sourceRoothPath.EndsWith(":\\"))
                {
                    sourcePath = string.Format(@"\\{0}\{1}\{2}", Environment.MachineName, sourceRoothPath.Replace(":\\", "$"), sourcePath.Replace(sourceRoothPath, ""));
                }
                var sourceDir = new DirectoryInfo(sourcePath);



                var targetPath = Path.Combine(contentRooth, folderName);
                var targetRoothPath = Path.GetPathRoot(targetPath);
                if (targetRoothPath.EndsWith(":\\"))
                {
                    targetPath = string.Format(@"\\{0}\{1}\{2}", Environment.MachineName, targetRoothPath.Replace(":\\", "$"), targetPath.Replace(targetRoothPath, ""));
                }

                var targetDir = new DirectoryInfo(targetPath);
                if (!targetDir.Exists)
                {
                    Directory.CreateDirectory(targetPath);
                }

                var files = sourceDir.GetFiles();
                foreach (var file in files)
                {
                    var fileTargetPath = Path.Combine(targetPath, file.Name);
                    file.CopyTo(fileTargetPath);
                }


                return Ok(new
                {
                    contentRooth,
                    sourcePath,
                    sourceDir = sourceDir.FullName,
                    targetPath,
                    targetDir = targetDir.FullName,
                    testPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System)),
                    testPath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)),
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}